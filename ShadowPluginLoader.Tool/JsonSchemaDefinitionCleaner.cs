namespace ShadowPluginLoader.Tool;

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;

public static class JsonSchemaDefinitionCleaner
{
    public static void RemoveUnusedDefinitions(JsonSchema schema)
    {
        var used = new HashSet<JsonSchema>();
        CollectUsedSchemas(schema, used);

        var allDefs = schema.Definitions.ToList();
        foreach (var kv in allDefs)
        {
            if (!used.Contains(kv.Value))
            {
                schema.Definitions.Remove(kv.Key);
            }
        }
    }

    private static void CollectUsedSchemas(JsonSchema schema, HashSet<JsonSchema> used)
    {
        if (schema == null || !used.Add(schema))
            return;

        // 引用
        if (schema.Reference != null)
            CollectUsedSchemas(schema.Reference, used);

        // 属性
        foreach (var prop in schema.ActualProperties.Values)
            CollectUsedSchemas(prop, used);

        // Items (数组)
        if (schema.Item != null)
            CollectUsedSchemas(schema.Item, used);

        foreach (var i in schema.Items)
            CollectUsedSchemas(i, used);

        // allOf / anyOf / oneOf
        foreach (var s in schema.AllOf) CollectUsedSchemas(s, used);
        foreach (var s in schema.AnyOf) CollectUsedSchemas(s, used);
        foreach (var s in schema.OneOf) CollectUsedSchemas(s, used);
    }
}
