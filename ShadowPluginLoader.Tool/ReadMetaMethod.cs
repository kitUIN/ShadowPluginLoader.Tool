using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;

namespace ShadowPluginLoader.Tool;

public static class ReadMetaMethod
{
    private static string _projectPath = "";

    private static JsonNode GetDefineJson()
    {
        var file = Path.Combine(_projectPath, "plugin.d.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {_projectPath}plugin.d.json");
        return JsonNode.Parse(File.ReadAllText(file))!;
    }

    private static void SavePluginJson(JsonObject jsonObject)
    {
        File.WriteAllText(
            Path.Combine(_projectPath, "plugin.json"),
            jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true })
        );
        Logger.Log($"plugin.json -> {_projectPath}plugin.json", LoggerLevel.Success);
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

    private static void CheckJsonRequired(JsonNode json, XmlNode root)
    {
        var propertyGroup = root.SelectSingleNode("PropertyGroup");
        if (propertyGroup is null) return;
        var res = new JsonObject();
        var required = json["Required"]!.AsArray();
        var properties = json["Properties"]!;
        foreach (var node in required)
        {
            var name = node!.GetValue<string>();
            if (name == "Dependencies")
            {
                res[name] = LoadDependencies(root);
                continue;
            }

            var current = properties[name]!;
            var propertyGroupName = current["PropertyGroupName"]!.GetValue<string>();
            var property = propertyGroup.SelectSingleNode(propertyGroupName);
            if (property is null)
                throw new Exception($"Missing property {propertyGroupName} In PropertyGroup");
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

            switch (type)
            {
                case "String":
                    res[name] = value;
                    break;
                case "Int32":
                    res[name] = Convert.ToInt32(value);
                    break;
                case "Boolean":
                    res[name] = Convert.ToBoolean(value);
                    break;
                default:
                {
                    if (type.EndsWith("[]"))
                    {
                        var items = property.SelectNodes("Item");
                        if (items is null) continue;
                        var arrays = new JsonArray();
                        Action<string> action = type switch
                        {
                            "Int32[]" => v => { arrays.Add(Convert.ToInt32(v)); },
                            "Boolean[]" => v => { arrays.Add(Convert.ToBoolean(v)); },
                            _ => v => { arrays.Add(v); }
                        };
                        foreach (XmlNode item in items)
                        {
                            action(item.InnerText);
                        }

                        res[name] = arrays;
                    }
                    else
                    {
                        res[name] = null;
                    }

                    break;
                }
            }
        }

        SavePluginJson(res);
    }

    public static void Read(string projectPath, string csproj)
    {
        _projectPath = projectPath;
        var json = GetDefineJson();
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(Path.Combine(_projectPath, csproj));
        var root = xmlDoc.DocumentElement;
        if (root is null) return;
        CheckJsonRequired(json, root);
    }
}