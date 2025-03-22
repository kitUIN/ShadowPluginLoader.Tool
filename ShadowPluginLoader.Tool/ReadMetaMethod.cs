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
    private static string _projectPath = "";

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
    ];

    private static JsonNode GetDefineJson()
    {
        var file = Path.Combine(_projectPath, "plugin.d.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {_projectPath}plugin.d.json");
        return JsonNode.Parse(File.ReadAllText(file))!;
    }

    private static void SavePluginJson(JsonObject jsonObject)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
#if NET7_0
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        var path = Path.Combine(_projectPath, "plugin.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path,
            jsonObject.ToJsonString(options)
        );
        Logger.Log($"plugin.json -> {path}", LoggerLevel.Success);
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

    private static JsonValue? GetVale(string type, string value)
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
            "System.String" => JsonValue.Create(value),
            _ => throw new Exception($"Not Support Type: {type}")
        };
    }

    private static void CheckJsonRequired(JsonNode json, XmlNode root, XmlNode propertyGroup, string dllName)
    {
        var res = new JsonObject
        {
            ["DllName"] = dllName
        };
        var properties = json["Properties"]!.AsObject();
        foreach (var node in properties)
        {
            var name = node!.Key;
            if (name == "Dependencies")
            {
                res[name] = LoadDependencies(root);
                continue;
            }

            var current = node.Value!;
            ReadProperty(propertyGroup, current, name, res);
        }

        SavePluginJson(res);
    }

    private static void ReadProperty(XmlNode propertyGroup, JsonNode current, string name, JsonObject res)
    {
        var nullable = current["Nullable"]!.GetValue<bool>();
        var propertyGroupName = current["PropertyGroupName"]!.GetValue<string>();
        var property = propertyGroup.SelectSingleNode(propertyGroupName);
        if (property is null)
        {
            if (current["Required"]!.GetValue<bool>())
                throw new Exception($"Missing Required Property {propertyGroupName} In <{propertyGroup.Name}>");
            return;
        }

        var type = current["Type"]!.GetValue<string>();
        var value = property.InnerText;
        if (current["Regex"] is not null)
        {
            var regex = current["Regex"]!.GetValue<string>();
            if (!Regex.IsMatch(value, regex))
            {
                throw new Exception($"{name}: {value} Does Not Match Regex: {regex}");
            }
        }

        if (nullable && value == "null")
        {
            res[name] = null;
        }
        else
        {
            if (!SupportType.Contains(type))
            {
                var propertyJson = new JsonObject();
                var properties = current["Properties"];
                if (properties == null) return;
                foreach (var node in properties.AsObject())
                {
                    ReadProperty(property, node.Value!, node!.Key, propertyJson);
                }

                res[name] = propertyJson;
            }
            else if (type.EndsWith("[]"))
            {
                var arrayType = type[..^2];
                var arrays = new JsonArray();
                if (property.InnerText.Contains(';'))
                {
                    foreach (var v in property.InnerText.Split(";"))
                    {
                        arrays.Add(GetVale(arrayType, v));
                    }
                }
                else
                {
                    var items = property.SelectNodes("Item");
                    if (items is null) return;
                    foreach (XmlNode item in items)
                    {
                        arrays.Add(GetVale(arrayType, item.InnerText));
                    }
                }

                res[name] = arrays;
            }
            else
            {
                res[name] = GetVale(type, value);
            }
        }
    }

    public static void Read(string projectPath, string csproj, string meta, string dllName)
    {
        _projectPath = projectPath;
        var json = GetDefineJson();
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(Path.Combine(_projectPath, csproj));
        var pluginMeta = new XmlDocument();
        pluginMeta.LoadXml("<PluginMeta>" + meta + "</PluginMeta>");
        var root = xmlDoc.DocumentElement;
        var pluginMetaRoot = pluginMeta.DocumentElement;
        if (root is null) return;
        if (pluginMetaRoot is null) return;
        CheckJsonRequired(json, root, pluginMetaRoot, dllName);
    }
}