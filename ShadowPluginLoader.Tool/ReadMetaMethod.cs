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
        "System.SByte", "System.SByte[]",
        "System.Type", "System.Type[]",
        "System.DateTime", "System.DateTime[]",
        "System.DateTimeOffset", "System.DateTimeOffset[]",
        "System.TimeSpan", "System.TimeSpan[]",
        "System.Guid", "System.Guid[]",
        "System.Byte", "System.Byte[]",
    ];

    public static JsonNode GetDefineJson(string projectPath)
    {
        var file = Path.Combine(projectPath, "plugin.d.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {projectPath}plugin.d.json");
        return JsonNode.Parse(File.ReadAllText(file))!;
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
            var name = dependency.Attributes!["Include"]!.Value;
            var version = dependency.Attributes!["Version"]!.Value;
            arrays.Add($"{name}={version}");
        }

        return arrays;
    }

    private static JsonValue GetVale(string type, string value)
    {
        return type switch
        {
            "System.Int32" => JsonValue.Create(Convert.ToInt32(value)),
            "System.UInt32" => JsonValue.Create(Convert.ToUInt32(value)),
            "System.Int16" => JsonValue.Create(Convert.ToInt16(value)),
            "System.UInt16" => JsonValue.Create(Convert.ToUInt16(value)),
            "System.Int64" => JsonValue.Create(Convert.ToInt64(value)),
            "System.UInt64" => JsonValue.Create(Convert.ToUInt64(value)),
            "System.Boolean" => JsonValue.Create(Convert.ToBoolean(value)),
            "System.Single" => JsonValue.Create(Convert.ToSingle(value)),
            "System.Double" => JsonValue.Create(Convert.ToDouble(value)),
            "System.Decimal" => JsonValue.Create(Convert.ToDecimal(value)),
            _ => JsonValue.Create(value)!,
        };
    }

    public static string CheckJsonRequired(JsonObject json, XmlNode root, XmlNode propertyGroup, string dllName)
    {
        var res = new JsonObject
        {
            ["DllName"] = dllName
        };
        foreach (var node in json["Properties"]!.AsObject())
        {
            if (res.ContainsKey(node.Key)) continue;
            var j = LoadProperty(propertyGroup, (JsonObject)node.Value!);
            if (j is JsonObject { Count: 0 } or JsonArray { Count: 0 }) continue;
            res[node.Key] = j;
        }

        if (!res.ContainsKey("Dependencies")) res["Dependencies"] = new JsonArray();

        foreach (var dep in LoadDependencies(root))
        {
            ((JsonArray)res["Dependencies"]!).Add(dep);
        }

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
#if NET7_0
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        return res.ToJsonString(options);
    }

    private static JsonNode LoadProperty(XmlNode propertyGroup, JsonObject current)
    {
        var isArray = current.ContainsKey("Item");
        var type = current["Type"]!.GetValue<string>();
        JsonNode res = isArray ? new JsonArray() : new JsonObject();
        var propertyGroupName = current["PropertyGroupName"]!.GetValue<string>();
        var property = propertyGroup.SelectSingleNode(propertyGroupName);
        if (property is null)
        {
            if (current["Required"]!.GetValue<bool>())
                throw new Exception($"Missing Required Property {propertyGroupName} In <{propertyGroup.Name}>");
            return res;
        }

        if (!isArray)
        {
            if (current.ContainsKey("Properties") &&
                current["Properties"] is JsonObject jsonProperties &&
                jsonProperties.Count > 0)
            {
                // 自定义类
                foreach (var prop in jsonProperties)
                {
                    var propValue = ScanProperty(property, prop.Value!, propertyGroupName);
                    if (propValue is null) continue;
                    ((JsonObject)res)[prop.Key] = propValue;
                }
            }
            else
            {
                // 值
                return GetVale(type, property.InnerText!);
            }
        }
        else
        {
            // 列表
            var item = current["Item"]!.AsObject();
            var arrayType = item["Type"]!.GetValue<string>();
            if (property.InnerText.Contains(';'))
            {
                foreach (var v in property.InnerText.Split(";"))
                {
                    ((JsonArray)res).Add(GetVale(arrayType, v));
                }
            }
            else
            {
                foreach (XmlNode child in property.ChildNodes)
                {
                    var propValue = ScanProperty(child, item, propertyGroupName);
                    if (propValue is null) continue;
                    ((JsonArray)res).Add(propValue);
                }
            }
        }

        return res;
    }

    private static JsonNode? ScanProperty(XmlNode property, JsonNode current, string name)
    {
        var nullable = current["Nullable"]!.GetValue<bool>();
        var type = current["Type"]!.GetValue<string>();
        if (!property.HasChildNodes)
        {
            var value = property.InnerText;
            if (current["Regex"] is not null)
            {
                var regex = current["Regex"]!.GetValue<string>();
                if (!Regex.IsMatch(value, regex))
                {
                    throw new Exception($"{name}: {value} Does Not Match Regex: {regex}");
                }
            }
            if (nullable && value == "null") return null;
            if (SupportType.Contains(type)) return GetVale(type, value);
        }
        

        var propertyJson = new JsonObject();
        var properties = current["Properties"];
        if (properties == null) return null;
        foreach (var node in properties.AsObject())
        {
            propertyJson[node.Key] = LoadProperty(property, (JsonObject)node.Value!);
        }

        return propertyJson;
    }
}