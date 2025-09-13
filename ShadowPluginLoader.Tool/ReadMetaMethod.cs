using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;

namespace ShadowPluginLoader.Tool;

public static class ReadMetaMethod
{
    public static readonly string[] SupportType =
    [
        "System.Int32", "System.Int32[]",
        "System.UInt32", "System.UInt32[]",
        "System.Int16", "System.Int16[]",
        "System.UInt16", "System.UInt16[]",
        "System.Int64", "System.Int64[]",
        "System.UInt64", "System.UInt64[]",
        "System.Boolean", "System.Boolean[]",
        "System.Single", "System.Single[]",
        "System.Double", "System.Double[]",
        "System.Decimal", "System.Decimal[]",
        "System.String", "System.String[]",
        "System.Char", "System.Char[]",
        // "System.SByte", "System.SByte[]",
        // "System.Type", "System.Type[]",
        // "System.DateTime", "System.DateTime[]",
        // "System.DateTimeOffset", "System.DateTimeOffset[]",
        // "System.TimeSpan", "System.TimeSpan[]",
        // "System.Guid", "System.Guid[]",
        // "System.Byte", "System.Byte[]",
        // "System.Version", "System.Version[]",
    ];

    public static JsonObject GetDefineJson(string projectPath)
    {
        var file = Path.Combine(projectPath, "plugin.d.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {projectPath}plugin.d.json");
        return (JsonObject) JsonNode.Parse(File.ReadAllText(file))!;
    }

    private static void WarnDependencies()
    {
        Logger.Log("Dependency not specified, ignored.", LoggerLevel.Warning);
    }

    private static JsonArray LoadDependencies(XmlNode root)
    {
        var itemGroup = root.SelectSingleNode("ItemGroup[@Label='Dependencies']");
        var arrays = new JsonArray();
        if (itemGroup is null)
        {
            WarnDependencies();
            return arrays;
        }

        var dependencies = itemGroup.SelectNodes("PackageReference");
        if (dependencies is null)
        {
            WarnDependencies();
            return arrays;
        }

        foreach (XmlNode dependency in dependencies)
        {
            var name = dependency.Attributes!["Include"]?.Value;
            var label = dependency.Attributes?["Need"]?.Value;

            if (string.IsNullOrWhiteSpace(name))
                continue;
            arrays.Add(new JsonObject
            {
                ["Name"] = name,
                ["Need"] = label
            });
        }

        return arrays;
    }


    public static JsonObject CheckJsonRequired(JsonObject structureJson, XmlNode xmlDoc,
        XmlNode propertyGroup, string dllFilePath)
    {
        var dllName = Path.GetFileNameWithoutExtension(dllFilePath);
        var structure = structureJson["Properties"]!.AsObject();
        var res = (JsonObject)ConvertXmlToJsonWithValidation(propertyGroup, structure);
        res["DllName"] = dllName;
        if (!res.ContainsKey("Dependencies")) res["Dependencies"] = new JsonArray();

        foreach (var dep in LoadDependencies(xmlDoc))
        {
            ((JsonArray)res["Dependencies"]!).Add(dep!.GetValue<string>());
        }

        return res;
    }


    /// <summary>
    /// 从 JSON 数据文件根据结构文件转换为类结构并保存为 JSON
    /// </summary>
    /// <param name="dataFilePath">数据文件路径（JSON 格式）</param>
    /// <param name="structureFilePath">结构文件路径（JSON 格式）</param>
    /// <param name="outputJsonPath">输出 JSON 文件路径</param>
    public static void JsonToJson(string dataFilePath, string structureFilePath, string outputJsonPath)
    {
        // 读取结构文件
        var structureJson = JsonNode.Parse(System.IO.File.ReadAllText(structureFilePath))!;
        var structure = structureJson["Properties"]!.AsObject();

        // 读取数据文件
        var dataJson = JsonNode.Parse(System.IO.File.ReadAllText(dataFilePath))!;

        // 转换 JSON 到 JSON（根据结构验证和转换）
        var result = ConvertJsonToJson(dataJson, structure);

        // 保存为 JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
#if NET7_0_OR_GREATER
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        System.IO.File.WriteAllText(outputJsonPath, result.ToJsonString(options));
    }

    /// <summary>
    /// 将 XML 节点转换为 JSON 对象（带验证）
    /// </summary>
    private static JsonNode ConvertXmlToJsonWithValidation(XmlNode xmlNode, JsonObject structure)
    {
        var result = new JsonObject();

        foreach (var prop in structure)
        {
            var propertyName = prop.Key;
            var propertyInfo = prop.Value!.AsObject();
            var propertyGroupName = propertyInfo.ContainsKey("PropertyGroupName")
                ? propertyInfo["PropertyGroupName"]!.GetValue<string>()
                : propertyName;

            var xmlElement = xmlNode.SelectSingleNode(propertyGroupName);
            if (xmlElement == null)
            {
                if (propertyInfo["Required"]!.GetValue<bool>())
                {
                    throw new Exception($"缺少必需的属性: {propertyGroupName}");
                }

                // 使用默认值
                result[propertyName] = GetDefaultValueFromStructure(propertyInfo);
                continue;
            }

            var isArray = propertyInfo.ContainsKey("IsArray") && propertyInfo["IsArray"]!.GetValue<bool>();

            if (isArray)
            {
                result[propertyName] = ConvertXmlArrayToJson(xmlElement, propertyInfo);
            }
            else if (propertyInfo.ContainsKey("Properties"))
            {
                // 复杂对象
                var subStructure = propertyInfo["Properties"]!.AsObject();
                result[propertyName] = ConvertXmlToJsonWithValidation(xmlElement, subStructure);
            }
            else
            {
                // 简单值，使用结构文件中的类型信息进行转换
                result[propertyName] = ConvertXmlValueToJson(xmlElement.InnerText, propertyInfo);
            }
        }

        return result;
    }


    /// <summary>
    /// 将 JSON 数据转换为根据结构验证的 JSON 对象
    /// </summary>
    private static JsonNode ConvertJsonToJson(JsonNode dataNode, JsonObject structure)
    {
        var result = new JsonObject();

        foreach (var prop in structure)
        {
            var propertyName = prop.Key;
            var propertyInfo = prop.Value!.AsObject();
            var propertyGroupName = propertyInfo.ContainsKey("PropertyGroupName")
                ? propertyInfo["PropertyGroupName"]!.GetValue<string>()
                : propertyName;

            if (!dataNode.AsObject().ContainsKey(propertyGroupName))
            {
                if (propertyInfo["Required"]!.GetValue<bool>())
                {
                    throw new Exception($"缺少必需的属性: {propertyGroupName}");
                }

                continue;
            }

            var dataValue = dataNode[propertyGroupName];
            var isArray = propertyInfo.ContainsKey("IsArray") && propertyInfo["IsArray"]!.GetValue<bool>();

            if (isArray)
            {
                if (dataValue is JsonArray dataArray)
                {
                    var array = new JsonArray();
                    var itemType = propertyInfo["ItemType"]!.GetValue<string>();

                    foreach (var item in dataArray)
                    {
                        if (propertyInfo.ContainsKey("ItemProperties"))
                        {
                            // 复杂对象数组
                            var itemStructure = propertyInfo["ItemProperties"]!.AsObject();
                            array.Add(ConvertJsonToJson(item!, itemStructure));
                        }
                        else
                        {
                            // 简单数组
                            array.Add(ConvertValue(item!.GetValue<string>(), itemType));
                        }
                    }

                    result[propertyName] = array;
                }
                else
                {
                    throw new Exception($"属性 {propertyGroupName} 应该是数组类型");
                }
            }
            else if (propertyInfo.ContainsKey("Properties"))
            {
                // 复杂对象
                var subStructure = propertyInfo["Properties"]!.AsObject();
                result[propertyName] = ConvertJsonToJson(dataValue!, subStructure);
            }
            else
            {
                // 简单值
                var type = propertyInfo["Type"]!.GetValue<string>();
                result[propertyName] = ConvertValue(dataValue!.GetValue<string>(), type);
            }
        }

        return result;
    }

    /// <summary>
    /// 从结构文件获取默认值
    /// </summary>
    private static JsonNode GetDefaultValueFromStructure(JsonObject propertyInfo)
    {
        if (propertyInfo.ContainsKey("DefaultValue"))
        {
            var defaultValue = propertyInfo["DefaultValue"]!.GetValue<string>();
            if (propertyInfo.ContainsKey("IsArray") && propertyInfo["IsArray"]!.GetValue<bool>())
            {
                return new JsonArray(); // 数组默认值为空数组
            }

            return JsonValue.Create(defaultValue) ?? JsonValue.Create("");
        }

        if (propertyInfo.ContainsKey("IsArray") && propertyInfo["IsArray"]!.GetValue<bool>())
        {
            return new JsonArray();
        }

        return JsonValue.Create("");
    }

    /// <summary>
    /// 转换 XML 数组到 JSON
    /// </summary>
    private static JsonNode ConvertXmlArrayToJson(XmlNode xmlElement, JsonObject propertyInfo)
    {
        var array = new JsonArray();
        var itemTypeInfo = propertyInfo["ItemType"]!.AsObject();
        var itemType = itemTypeInfo["Type"]!.GetValue<string>();

        if (xmlElement.InnerText.Contains(';'))
        {
            // 分号分隔的简单数组
            foreach (var value in xmlElement.InnerText.Split(';'))
            {
                array.Add(ConvertValue(value.Trim(), itemType));
            }
        }
        else
        {
            // 复杂对象数组
            foreach (XmlNode child in xmlElement.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    if (itemTypeInfo.ContainsKey("Properties"))
                    {
                        var itemStructure = itemTypeInfo["Properties"]!.AsObject();
                        array.Add(ConvertXmlToJsonWithValidation(child, itemStructure));
                    }
                    else
                    {
                        array.Add(ConvertValue(child.InnerText, itemType));
                    }
                }
            }
        }

        return array;
    }

    /// <summary>
    /// 转换 XML 值到 JSON（使用结构文件信息）
    /// </summary>
    private static JsonNode ConvertXmlValueToJson(string value, JsonObject propertyInfo)
    {
        var type = propertyInfo["Type"]!.GetValue<string>();

        // 检查是否有正则表达式验证
        if (propertyInfo.ContainsKey("Regex"))
        {
            var regex = propertyInfo["Regex"]!.GetValue<string>();
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, regex))
            {
                throw new Exception($"值 '{value}' 不匹配正则表达式: {regex}");
            }
        }

        return ConvertValue(value, type);
    }

    /// <summary>
    /// 转换字符串值为指定类型的 JSON 值
    /// </summary>
    private static JsonValue ConvertValue(string value, string type)
    {
        return type switch
        {
            "System.Int32" => JsonValue.Create(int.Parse(value)),
            "System.UInt32" => JsonValue.Create(uint.Parse(value)),
            "System.Int16" => JsonValue.Create(short.Parse(value)),
            "System.UInt16" => JsonValue.Create(ushort.Parse(value)),
            "System.Int64" => JsonValue.Create(long.Parse(value)),
            "System.UInt64" => JsonValue.Create(ulong.Parse(value)),
            "System.Boolean" => JsonValue.Create(bool.Parse(value)),
            "System.Single" => JsonValue.Create(float.Parse(value)),
            "System.Double" => JsonValue.Create(double.Parse(value)),
            "System.Decimal" => JsonValue.Create(decimal.Parse(value)),
            "System.Char" => JsonValue.Create(char.Parse(value)),
            "System.SByte" => JsonValue.Create(sbyte.Parse(value)),
            "System.Byte" => JsonValue.Create(byte.Parse(value)),
            "System.DateTime" => JsonValue.Create(DateTime.Parse(value)),
            "System.DateTimeOffset" => JsonValue.Create(DateTimeOffset.Parse(value)),
            "System.TimeSpan" => JsonValue.Create(TimeSpan.Parse(value)),
            "System.Guid" => JsonValue.Create(Guid.Parse(value)),
            "System.Version" => JsonValue.Create(Version.Parse(value)),
            _ => JsonValue.Create(value)!
        };
    }
}