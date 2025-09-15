
using NJsonSchema;
using NJsonSchema.Generation;
using ShadowPluginLoader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShadowPluginLoader.Tool;

internal class FieldAttributeSchemaProcessor : ISchemaProcessor
{
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
        }

    }
    private static JsonSchema GetRootSchema(JsonSchema schema)
    {
        var current = schema;
        while (current.ParentSchema != null)
            current = current.ParentSchema;
        return current;
    }

    
}
