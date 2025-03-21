using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

public static class ExportMetaMethod
{
    private static string _outputPath = "";


    private static void WriteDefineFile(Type type)
    {
        JsonNode root = new JsonObject
        {
            ["MetaDataType"] = type.FullName,
        };
        var props = Properties2JsonObject(type);

        root["Properties"] = props;
        var options = new JsonSerializerOptions { WriteIndented = true };
#if NET7_0
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        File.WriteAllText(
            Path.Combine(_outputPath, "plugin.d.json"),
            root.ToJsonString(options)
        );
    }

    private static JsonObject Properties2JsonObject(Type type, string prefix = "")
    {
        var properties = type.GetProperties();
        var props = new JsonObject();
        foreach (var property in properties)
        {
            var typeName = property.PropertyType.FullName;
            // 忽略TypeId
            if (property.Name == "TypeId" && typeName == "System.Object") continue;
            if (typeName is null) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            var isNullable = property.CustomAttributes
                .Any(attr => attr.AttributeType.Name == "NullableAttribute");
            if (m is { Exclude: true }) continue;
            var prop = new JsonObject
            {
                ["Type"] = typeName,
                ["Nullable"] = isNullable,
                ["Required"] = m?.Required ?? true,
                ["PropertyGroupName"] = string.IsNullOrEmpty(m?.PropertyGroupName)
                    ? property.Name
                    : m.PropertyGroupName
            };
            if (!ReadMetaMethod.SupportType.Contains(typeName))
            {
                var t = typeName.EndsWith("[]") ? property.PropertyType.GetElementType() : property.PropertyType;
                if (t != null)
                    prop["Properties"] = Properties2JsonObject(t, prefix + prop["PropertyGroupName"] + ".");
            }

            if (m is not null && !string.IsNullOrEmpty(m.Regex))
            {
                prop["Regex"] = m.Regex;
            }

            Logger.Log($"{prefix}{property.Name}: {typeName}" + (isNullable ? "?" : "") + " -> plugin.d.json");
            props[property.Name] = prop;
        }

        return props;
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