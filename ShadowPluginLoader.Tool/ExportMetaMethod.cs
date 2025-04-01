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
            JsonArray genericTypes = [];
            Type? itemType = null;
            // 先判断是不是Nullable<T>
            var unknownType = UnKnownType.Check(property);
            if (unknownType.TypeName == null) continue;
            var typeName = unknownType.TypeName;
            // 判断是不是数组
            var unknownGenericType = unknownType.PropertyType;
            if (typeName.EndsWith("[]"))
            {
                typeName = "System.Array";
                itemType = property.PropertyType.GetElementType();
                unknownGenericType = itemType;
            }

            // 检查泛型

            if (unknownGenericType.IsGenericType)
            {
                var genericType = unknownGenericType.GetGenericTypeDefinition().FullName;
                if (genericType != null && genericType.Contains('\u0060'))
                    genericType = genericType.Split('\u0060')[0];
                else continue;
                typeName = genericType;
                if (genericType == "System.Collections.Generic.List")
                {
                    itemType = unknownGenericType.GetGenericArguments()[0];
                }
                else
                {
                    foreach (var arg in unknownGenericType.GetGenericArguments())
                    {
                        genericTypes.Add(arg.FullName!);
                    }
                }
            }

            var itemUnknownType = itemType != null ? UnKnownType.Check(itemType) : null;

            // 忽略TypeId,Object
            if (property.Name == "TypeId" && typeName == "System.Object") continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m is { Exclude: true }) continue;
            var prop = new JsonObject()
            {
                ["Type"] = typeName,
                ["Required"] = m?.Required ?? true,
                ["PropertyGroupName"] = string.IsNullOrEmpty(m?.PropertyGroupName)
                    ? property.Name
                    : m.PropertyGroupName,
                ["Nullable"] = unknownType.Nullable,
            };
            if (itemType != null)
            {
                prop["Item"] = new JsonObject()
                {
                    ["Type"] = itemUnknownType!.TypeName,
                    ["Nullable"] = itemUnknownType!.Nullable,
                };
            }

            if (genericTypes.Count > 0)
            {
                prop["GenericType"] = genericTypes;
            }

            if (itemUnknownType != null)
            {
                if (!ReadMetaMethod.SupportType.Contains(itemUnknownType.TypeName))
                {
                    prop["Item"]!["Properties"] = Properties2JsonObject(itemUnknownType.PropertyType!,
                        prefix + prop["PropertyGroupName"] + ".");
                }
            }
            else if (!ReadMetaMethod.SupportType.Contains(typeName))
                prop["Properties"] = Properties2JsonObject(unknownType.PropertyType!,
                    prefix + prop["PropertyGroupName"] + ".");

            if (typeName is "System.DateTimeOffset" or "System.DateTime" &&
                property.GetCustomAttribute<MetaDateTimeAttribute>() is { } dateTimeAttr &&
                (dateTimeAttr.Format != null || !dateTimeAttr.InvariantCulture))
            {
                if (itemUnknownType != null)
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
                    if (itemUnknownType != null)
                        prop["Item"]!["Regex"] = m.Regex;
                    else
                        prop["Regex"] = m.Regex;
                }

                if (!string.IsNullOrEmpty(m.EntryPointName))
                {
                    if (itemUnknownType != null)
                        prop["Item"]!["EntryPointName"] = m.EntryPointName;
                    else
                        prop["EntryPointName"] = m.EntryPointName;
                }
            }

            if (!prop.ContainsKey("Regex") &&
                Regexs.TryGetValue(itemUnknownType != null ? itemUnknownType.TypeName! : typeName, out var value))
                if (itemUnknownType != null)
                    prop["Item"]!["Regex"] = value;
                else
                    prop["Regex"] = value;
            props[property.Name] = prop;
            var name = itemUnknownType != null
                ? $"{typeName}<{itemUnknownType.TypeName}" + (itemUnknownType.Nullable ? "?" : "") + ">"
                : typeName + (unknownType.Nullable ? "?" : "");
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