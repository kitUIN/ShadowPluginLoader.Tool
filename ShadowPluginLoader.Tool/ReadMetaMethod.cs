﻿using System;
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
    
    private static JsonValue? GetVale(string type,string value)
    {
        return type switch
        {
            "int" => JsonValue.Create(Convert.ToInt32(value)),
            "uint" => JsonValue.Create(Convert.ToUInt32(value)),
            "short" => JsonValue.Create(Convert.ToInt16(value)),
            "ushort" => JsonValue.Create(Convert.ToUInt16(value)),
            "long" => JsonValue.Create(Convert.ToInt64(value)),
            "ulong" => JsonValue.Create(Convert.ToUInt64(value)),
            "bool" => JsonValue.Create(Convert.ToBoolean(value)),
            "float" => JsonValue.Create(Convert.ToSingle(value)),
            "double" => JsonValue.Create(Convert.ToDouble(value)),
            "decimal" => JsonValue.Create(Convert.ToDecimal(value)),
            "string" => JsonValue.Create(value) ,
            _ => throw new Exception($"Not Support Type: {type}")
        };
        /*switch (type)
        {
            case "String":
                res[name] = value;
                break;
            case "Int32":
                res[name] = Convert.ToInt32(value);
                break;
            case "Boolean":
                res[name] = Convert.ToBoolean(value);
                res[name] = JsonValue.Create()
                break;
            default:
            {
                if (type.EndsWith("[]"))
                {
                    var items = property.SelectNodes("Item");
                    if (items is null) return;
                    var arrays = new JsonArray();
                    Action<string> action = type switch
                    {
                        "int[]" => v => { arrays.Add(Convert.ToInt32(v)); },
                        "uint[]" => v => { arrays.Add(Convert.ToUInt32(v)); },
                        "short[]" => v => { arrays.Add(Convert.ToInt16(v)); },
                        "ushort[]" => v => { arrays.Add(Convert.ToUInt16(v)); },
                        "long[]" => v => { arrays.Add(Convert.ToInt16(v)); },
                        "ulong[]" => v => { arrays.Add(Convert.ToUInt16(v)); },
                        "bool[]" => v => { arrays.Add(Convert.ToBoolean(v)); },
                        _ => v => { arrays.Add(v); }
                    };
                    foreach (XmlNode item in items)
                    {
                        action(item.InnerText);
                    }

                    res[name] = arrays;
                }
                else if(nullable)
                {
                    res[name] = null;
                }
                else
                {
                    throw new Exception($"Missing Value Of {name}, It Is Not Be Nullable");
                }

                break;
            }
        }*/
    }
    private static void CheckJsonRequired(JsonNode json, XmlNode root, XmlNode propertyGroup)
    {
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

            var nullable = false;
            var current = properties[name]!;
            var propertyGroupName = current["PropertyGroupName"]!.GetValue<string>();
            var property = propertyGroup.SelectSingleNode(propertyGroupName);
            if (property is null)
                throw new Exception($"Missing property <{propertyGroupName}> In <PluginMeta>");
            var type = current["Type"]!.GetValue<string>();
            if (type.EndsWith("?"))
            {
                type = type[..^1];
            }
            var value = property.InnerText;
            if (current["Regex"] is not null)
            {
                var regex = current["Regex"]!.GetValue<string>();
                if (!Regex.IsMatch(value, regex))
                {
                    throw new Exception($"{name}: {value} Does Not Match Regex: {regex}");
                }
            }
            if (current["Nullable"] is not null && current["Nullable"]!.GetValue<bool>())
            {
                nullable = true;
            }

            if (nullable && value == "null")
            {
                res[name] = null;
            }
            else
            {
                if (type.EndsWith("[]"))
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
                        if (items is null) continue;
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

        SavePluginJson(res);
    }

    public static void Read(string projectPath, string csproj,string meta)
    {
        _projectPath = projectPath;
        var json = GetDefineJson();
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(Path.Combine(_projectPath, csproj));
        var pluginMeta = new XmlDocument();
        pluginMeta.LoadXml("<PluginMeta>"+meta+"</PluginMeta>");
        var root = xmlDoc.DocumentElement;
        var pluginMetaRoot = pluginMeta.DocumentElement;
        if (root is null) return;
        if (pluginMetaRoot is null) return;
        CheckJsonRequired(json, root, pluginMetaRoot);
    }
}