using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

public static class ExportMetaMethod
{
    private static Dictionary<string, string> Regexs { get; } = new()
    {
        ["System.TimeSpan"] = @"^(\d\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\.\d{1,7})?$",
        ["System.Guid"] = @"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$",
    };


    private static JsonObject Properties2JsonObject(Type type, string prefix = "")
    {
        var properties = type.GetProperties();
        var props = new JsonObject();
        foreach (var property in properties)
        {
            var typeName = property.PropertyType.FullName;
            // 忽略TypeId
            if (property.Name == "TypeId" && typeName == "System.Object") continue;
            var genericType = "";
            var isNullable = property.CustomAttributes
                .Any(attr => attr.AttributeType.Name == "NullableAttribute");
            var nullType = Nullable.GetUnderlyingType(property.PropertyType);
            if (nullType != null)
            {
                typeName = nullType.FullName;
                isNullable = true;
            }
            else
            {
                // Check List
                if (property.PropertyType.IsGenericType)
                {
                    genericType = property.PropertyType.GetGenericTypeDefinition().FullName;
                    if(genericType != null && genericType.Contains('\u0060')) 
                        genericType = genericType.Split('\u0060')[0];
                    if (genericType == "System.Collections.Generic.List")
                    {
                        var genericArguments = property.PropertyType.GetGenericArguments();
                        if (genericArguments.Length > 0) typeName = genericArguments[0].FullName;
                    }
                }
            }

            if (typeName is null) continue;


            var m = property.GetCustomAttribute<MetaAttribute>();
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
            if (genericType != "")
            {
                prop["GenericType"] = genericType;
            }
            if (!ReadMetaMethod.SupportType.Contains(typeName))
            {
                var t = typeName.EndsWith("[]") ? property.PropertyType.GetElementType() : property.PropertyType;
                if (t != null)
                    prop["Properties"] = Properties2JsonObject(t, prefix + prop["PropertyGroupName"] + ".");
            }

            if (typeName is "System.DateTimeOffset" or "System.DateTime" &&
                property.GetCustomAttribute<MetaDateTimeAttribute>() is { } dateTimeAttr &&
                (dateTimeAttr.Format != null || !dateTimeAttr.InvariantCulture))
            {
                prop["DateTime"] = new JsonObject()
                {
                    ["Format"] = dateTimeAttr.Format,
                    ["InvariantCulture"] = dateTimeAttr.InvariantCulture
                };
            }

            if (m is not null)
            {
                if (!string.IsNullOrEmpty(m.Regex)) prop["Regex"] = m.Regex;
                if (!string.IsNullOrEmpty(m.EntryPointName)) prop["EntryPointName"] = m.EntryPointName;
            }

            if (!prop.ContainsKey("Regex") && Regexs.TryGetValue(typeName.TrimEnd(']', '['), out var value))
                prop["Regex"] = value;
            Logger.Log($"{prefix}{property.Name}: {typeName}" + (isNullable ? "?" : "") + " -> plugin.d.json");
            props[property.Name] = prop;
        }

        return props;
    }

    public static string ExportMeta(Type type)
    {
        JsonNode root = new JsonObject
        {
            ["Type"] = type.FullName,
        };
        var props = Properties2JsonObject(type);
        root["Properties"] = props;
        var options = new JsonSerializerOptions { WriteIndented = true };
#if NET7_0_OR_GREATER
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        return root.ToJsonString(options);
    }
}