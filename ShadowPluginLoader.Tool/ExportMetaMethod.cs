using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

public static class ExportMetaMethod
{
    /// <summary>
    /// 导出 JSON 结构文件，每个成员对应一个 JSON 对象
    /// </summary>
    /// <param name="type">要导出的类型</param>
    /// <returns>JSON 结构字符串</returns>
    public static string ExportMetaToJson(Type type)
    {
        var root = new JsonObject
        {
            ["Type"] = type.FullName,
            ["Properties"] = new JsonObject()
        };

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            // 忽略TypeId,Object
            if (property.Name == "TypeId" && property.PropertyType == typeof(object)) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m is null or { Exclude: true }) continue;

            var propertyType = PropertyTypeInfo.Analyze(property);

            var isArray = propertyType is { IsArray: true, ItemType: not null };
            var element = new JsonObject
            {
                // 添加属性信息
                ["Type"] = propertyType.TypeName,
                ["Required"] = m?.Required ?? true,
                ["Nullable"] = propertyType.IsNullable
            };

            if (!string.IsNullOrEmpty(m?.PropertyGroupName))
            {
                element["PropertyGroupName"] = m.PropertyGroupName;
            }

            if (!string.IsNullOrEmpty(m?.Regex))
            {
                element["Regex"] = m.Regex;
            }

            // 处理数组类型
            if (isArray)
            {
                element["IsArray"] = true;

                // ItemType 作为对象，包含完整的类型信息
                var itemTypeInfo = new JsonObject
                {
                    ["Type"] = propertyType.ItemType!.TypeName,
                    ["Required"] = true, // 数组项默认必需
                    ["Nullable"] = propertyType.ItemType!.IsNullable,
                    ["DefaultValue"] = GetDefaultValue(propertyType.ItemType!.TypeName)
                };

                // 如果数组项是自定义类型，添加 Properties
                if (!ReadMetaMethod.SupportType.Contains(propertyType.ItemType!.TypeName))
                {
                    if (HasMetaAttributes(propertyType.ItemType!.RawType))
                    {
                        itemTypeInfo["Properties"] = AddPropertiesToJson(propertyType.ItemType!.RawType);
                    }
                }

                element["ItemType"] = itemTypeInfo;
                element["DefaultValue"] = "[]";
            }
            else if (!ReadMetaMethod.SupportType.Contains(propertyType.TypeName))
            {
                // 自定义类型，递归处理属性
                element["Properties"] = AddPropertiesToJson(propertyType.RawType);
            }
            else
            {
                // 基本类型，添加示例值
                element["DefaultValue"] = GetDefaultValue(propertyType.TypeName);
            }

            root["Properties"]![property.Name] = element;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
#if NET7_0_OR_GREATER
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        return root.ToJsonString(options);
    }

    /// <summary>
    /// 递归添加属性到 JSON 对象
    /// </summary>
    private static JsonObject AddPropertiesToJson(Type type)
    {
        var result = new JsonObject();
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "TypeId" && property.PropertyType == typeof(object)) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m is null or { Exclude: true }) continue;

            var propertyType = PropertyTypeInfo.Analyze(property);

            var isArray = propertyType is { IsArray: true, ItemType: not null };
            var element = new JsonObject
            {
                ["Type"] = propertyType.TypeName,
                ["Required"] = m?.Required ?? true,
                ["Nullable"] = propertyType.IsNullable
            };

            if (!string.IsNullOrEmpty(m?.PropertyGroupName))
            {
                element["PropertyGroupName"] = m.PropertyGroupName;
            }

            if (!string.IsNullOrEmpty(m?.Regex))
            {
                element["Regex"] = m.Regex;
            }

            if (isArray)
            {
                element["IsArray"] = true;

                // ItemType 作为对象，包含完整的类型信息
                var itemTypeInfo = new JsonObject
                {
                    ["Type"] = propertyType.ItemType!.TypeName,
                    ["Required"] = true, // 数组项默认必需
                    ["Nullable"] = propertyType.ItemType!.IsNullable,
                    ["DefaultValue"] = GetDefaultValue(propertyType.ItemType!.TypeName)
                };

                // 如果数组项是自定义类型，添加 Properties
                if (!ReadMetaMethod.SupportType.Contains(propertyType.ItemType!.TypeName))
                {
                    if (HasMetaAttributes(propertyType.ItemType!.RawType))
                    {
                        itemTypeInfo["Properties"] = AddPropertiesToJson(propertyType.ItemType!.RawType);
                    }
                }

                element["ItemType"] = itemTypeInfo;
                element["DefaultValue"] = "";
            }
            else if (!ReadMetaMethod.SupportType.Contains(propertyType.TypeName))
            {
                element["Properties"] = AddPropertiesToJson(propertyType.RawType);
            }
            else
            {
                element["DefaultValue"] = GetDefaultValue(propertyType.TypeName);
            }

            result[property.Name] = element;
        }

        return result;
    }

    /// <summary>
    /// 检查类型是否有任何属性带有 MetaAttribute
    /// </summary>
    private static bool HasMetaAttributes(Type type)
    {
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "TypeId" && property.PropertyType == typeof(object)) continue;
            var m = property.GetCustomAttribute<MetaAttribute>();
            if (m != null && !m.Exclude)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取类型的默认值
    /// </summary>
    private static string GetDefaultValue(string typeName)
    {
        return typeName switch
        {
            "System.String" => "",
            "System.Int32" => "0",
            "System.UInt32" => "0",
            "System.Int16" => "0",
            "System.UInt16" => "0",
            "System.Int64" => "0",
            "System.UInt64" => "0",
            "System.Boolean" => "false",
            "System.Single" => "0.0",
            "System.Double" => "0.0",
            "System.Decimal" => "0.0",
            "System.Char" => "",
            "System.SByte" => "0",
            "System.Byte" => "0",
            "System.DateTime" => DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            "System.DateTimeOffset" => DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssK"),
            "System.TimeSpan" => "00:00:00",
            "System.Guid" => Guid.NewGuid().ToString(),
            "System.Version" => "1.0.0.0",
            _ => ""
        };
    }
}