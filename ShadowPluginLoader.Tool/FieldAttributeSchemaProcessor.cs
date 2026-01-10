using System;
using NJsonSchema;
using NJsonSchema.Generation;
using ShadowPluginLoader.Attributes;
using System.Linq;

namespace ShadowPluginLoader.Tool;

internal class FieldAttributeSchemaProcessor : ISchemaProcessor
{
    private static string? _sdkVersion;

    public static void SetSdkVersion(Version? version)
    {
        if (version == null) throw new ArgumentNullException(nameof(version));
        var major = version.Major;
        var minor = version.Minor;
        var min = $"{major}.{minor}";
        var max = $"{major}.{minor + 1}";
        _sdkVersion = $"[{min}, {max})";
    }

    public void Process(SchemaProcessorContext context)
    {
        foreach (var propInfo in context.ContextualType.Properties)
        {
            // 用 JSON 名称，而不是 C# 名称
            var jsonName = propInfo.Name;

            if (!context.Schema.Properties.TryGetValue(jsonName, out var propSchema))
                continue;

            if (propInfo.GetCustomAttributes(typeof(MetaAttribute), true)
                    .FirstOrDefault() is not MetaAttribute baseAttr)
                continue;

            // 忽略字段
            if (baseAttr.Exclude)
            {
                context.Schema.Properties.Remove(jsonName);
                continue;
            }


            if (baseAttr.Required)
            {
                if (!context.Schema.RequiredProperties.Contains(jsonName))
                    context.Schema.RequiredProperties.Add(jsonName);
            }

            if (!string.IsNullOrEmpty(baseAttr.Regex))
            {
                propSchema.Pattern = baseAttr.Regex;
            }

            if (baseAttr is { AsString: true })
            {
                propSchema.Type = JsonObjectType.String;
                propSchema.Reference = null;
                propSchema.Properties.Clear();
            }

            // 如果字段是 SdkVersion 且没有默认值，则设置为 DLL 的版本号
            if (jsonName == "SdkVersion" && propSchema.Default == null && _sdkVersion != null)
            {
                propSchema.Default = _sdkVersion;
            }
        }
    }
}