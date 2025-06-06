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
        "System.Version", "System.Version[]",
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
            var name = dependency.Attributes!["Include"]?.Value;
            var version = dependency.Attributes!["Version"]?.Value;
            var label = dependency.Attributes?["Label"]?.Value;

            if (string.IsNullOrWhiteSpace(name))
                continue;

            string depString;

            if (!string.IsNullOrWhiteSpace(label))
            {
                // 校验 Label 格式，只允许 >= 或 <= 开头
                if (label.StartsWith(">=") || label.StartsWith("<=")|| label.StartsWith("="))
                {
                    depString = $"{name}{label}";
                }
                else
                {
                    throw new Exception($"Invalid Label format for dependency '{name}': '{label}' (must start with '>=' or '<=' or '=').");
                }
            }
            else if (!string.IsNullOrWhiteSpace(version))
            {
                depString = $"{name}>={version}";
            }
            else
            {
                continue; // 如果既没有 label 也没有 version，就跳过
            }

            arrays.Add(depString);
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
            ((JsonArray)res["Dependencies"]!).Add(dep!.GetValue<string>());
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
                    var propValue = LoadProperty(property, (JsonObject)prop.Value!);
                    if (propValue is JsonObject { Count: 0 }) continue;
                    ((JsonObject)res)[prop.Key] = propValue;
                }
            }
            else
            {
                var value = property.InnerText;
                // 值
                if (current["Regex"] is null) return GetVale(type, value);
                var regex = current["Regex"]!.GetValue<string>();
                if (!Regex.IsMatch(value, regex))
                {
                    throw new Exception($"{propertyGroupName}: {value} Does Not Match Regex: {regex}");
                }
                return GetVale(type, value);
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
                    var propValue = LoadProperty(child, item);
                    if (propValue is JsonObject { Count: 0 }) continue;
                    ((JsonArray)res).Add(propValue);
                }
            }
        }

        return res;
    }
}