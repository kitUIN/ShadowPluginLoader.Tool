using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShadowPluginLoader.MetaAttributes;

namespace ShadowPluginLoader.Tool;

public static class ExportMetaMethod
{
    private static string _outputPath = "";

    private static readonly List<string> Types = new()
    {
        "String",
        "Int32",
        "Boolean",
        "String[]",
        "Int32[]",
        "Boolean[]",
    };

    private static bool CheckExportPropertyType(PropertyInfo property)
    {
        if (property.Name == "TypeId")
        {
            Logger.Log("Not Support Property Name: TypeId", LoggerLevel.Warning);
            return false;
        }

        var f = Types.Any(x => x == property.PropertyType.Name);
        if (!f) Logger.Log($"{property.Name}: Not Support {property.PropertyType.Name} Type", LoggerLevel.Warning);
        return f;
    }

    private static void WriteDefineFile(Type type)
    {
        JsonNode root = new JsonObject()
        {
            ["Namespace"] = type.Namespace,
            ["Type"] = type.Name,
        };
        var properties = type.GetProperties();
        var props = new JsonObject();
        var required = new JsonArray();

        foreach (var property in properties)
        {
            if (!CheckExportPropertyType(property)) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m is { Exclude: true }) continue;
            var prop = new JsonObject
            {
                ["Type"] = property.PropertyType.Name,
                ["PropertyGroupName"] = string.IsNullOrEmpty(m?.PropertyGroupName)
                    ? property.Name
                    : m.PropertyGroupName
            };
            if (m is not null)
            {
                if (m.Nullable)
                {
                    prop["Nullable"] = m.Nullable;
                }

                if (m.Required) required.Add(property.Name);
                if (!string.IsNullOrEmpty(m?.Regex))
                {
                    prop["Regex"] = m.Regex;
                }
            }

            Logger.Log($"{property.Name}: {property.PropertyType.Name} -> plugin.d.json");
            props[property.Name] = prop;
        }

        root["Properties"] = props;
        root["Required"] = required;

        File.WriteAllText(
            Path.Combine(_outputPath, "plugin.d.json"),
            root.ToJsonString(
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            )
        );
    }

    public static void ExportMeta(Assembly asm, string outputPath)
    {
        _outputPath = outputPath;
        var type = asm.GetExportedTypes()
            .FirstOrDefault(
                x => x.GetCustomAttributes()
                    .Any(y => y is ExportMetaAttribute));
        if (type is null) throw new Exception("Not Found ExportMetaAttribute In Any Class");
        WriteDefineFile(type);
        Logger.Log($"plugin.d.json -> {Path.Combine(_outputPath, "plugin.d.json")}", LoggerLevel.Success);
    }
}