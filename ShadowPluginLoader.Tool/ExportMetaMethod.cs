using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

public record UnKnownType(bool Nullable, string? TypeName, Type? PropertyType)
{
    public static UnKnownType Check(PropertyInfo property)
    {
        string? typeName;
        bool isNullable;
        Type type;
        var nullType = System.Nullable.GetUnderlyingType(property.PropertyType);
        if (nullType != null)
        {
            typeName = nullType.FullName;
            isNullable = true;
            type = nullType;
        }
        else
        {
            isNullable = property.CustomAttributes
                .Any(attr => attr.AttributeType.Name == "NullableAttribute");
            typeName = property.PropertyType.FullName;
            type = property.PropertyType;
        }

        return new UnKnownType(isNullable, typeName, type);
    }

    public static UnKnownType Check(Type type)
    {
        var nullType = System.Nullable.GetUnderlyingType(type);
        return nullType == null
            ? new UnKnownType(false, type.FullName, type)
            : new UnKnownType(true, nullType.FullName, nullType);
    }
}

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
            // 忽略TypeId,Object
            if (property.Name == "TypeId" && property.PropertyType == typeof(object)) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m is { Exclude: true }) continue;
            var propertyType = PropertyTypeInfo.Analyze(m?.JsonType ?? property.PropertyType);
            var isArray = propertyType is { IsArray: true, ItemType: not null };
            var prop = new JsonObject()
            {
                ["Type"] = propertyType.TypeName,
                ["Required"] = m?.Required ?? true,
                ["PropertyGroupName"] = string.IsNullOrEmpty(m?.PropertyGroupName)
                    ? property.Name
                    : m.PropertyGroupName,
                ["Nullable"] = propertyType.IsNullable,
            };
            if (isArray)
            {
                prop["Item"] = new JsonObject()
                {
                    ["Type"] = propertyType.ItemType!.TypeName,
                    ["Nullable"] = propertyType.ItemType!.IsNullable,
                };
                if (!ReadMetaMethod.SupportType.Contains(propertyType.ItemType!.TypeName))
                {
                    prop["Item"]!["Properties"] = Properties2JsonObject(propertyType.ItemType!.RawType,
                        prefix + prop["PropertyGroupName"] + ".");
                }
            }else if (!ReadMetaMethod.SupportType.Contains(propertyType.TypeName))
                prop["Properties"] = Properties2JsonObject(propertyType.RawType,
                    prefix + prop["PropertyGroupName"] + ".");

            if (propertyType.GenericArguments.Count > 0)
            {
                // prop["GenericType"] = genericTypes;
            }

            if (propertyType.TypeName is "System.DateTimeOffset" or "System.DateTime" &&
                property.GetCustomAttribute<MetaDateTimeAttribute>() is { } dateTimeAttr &&
                (dateTimeAttr.Format != null || !dateTimeAttr.InvariantCulture))
            {
                if (isArray)
                    prop["Item"]!["DateTime"] = new JsonObject()
                    {
                        ["Format"] = dateTimeAttr.Format,
                        ["InvariantCulture"] = dateTimeAttr.InvariantCulture
                    };
                else
                    prop["DateTime"] = new JsonObject()
                    {
                        ["Format"] = dateTimeAttr.Format,
                        ["InvariantCulture"] = dateTimeAttr.InvariantCulture
                    };
            }

            if (m is not null)
            {
                if (!string.IsNullOrEmpty(m.Regex))
                {
                    if (isArray)
                        prop["Item"]!["Regex"] = m.Regex;
                    else
                        prop["Regex"] = m.Regex;
                }
                if (!string.IsNullOrEmpty(m.ConstructionTemplate))
                {
                    if (isArray)
                        prop["Item"]!["ConstructionTemplate"] = m.ConstructionTemplate;
                    else
                        prop["ConstructionTemplate"] = m.ConstructionTemplate;
                }
                if (!string.IsNullOrEmpty(m.EntryPointName))
                {
                    prop["EntryPointName"] = m.EntryPointName;
                }
            }

            if (!prop.ContainsKey("Regex") &&
                Regexs.TryGetValue(isArray ? propertyType.ItemType!.TypeName! : propertyType.TypeName, out var value))
                if (propertyType is { IsArray: true, ItemType: not null })
                    prop["Item"]!["Regex"] = value;
                else
                    prop["Regex"] = value;
            props[property.Name] = prop;
            var name = isArray
                ? $"{propertyType.TypeName}<{propertyType.ItemType!.TypeName}" + (propertyType.ItemType!.IsNullable ? "?" : "") + ">"
                : propertyType.TypeName + (propertyType.IsNullable ? "?" : "");
            Logger.Log($"{prefix}{property.Name}: {name} -> plugin.d.json");
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