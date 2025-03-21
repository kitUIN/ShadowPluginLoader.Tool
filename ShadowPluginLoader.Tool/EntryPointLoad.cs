using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

/// <summary>
/// 
/// </summary>
public static class EntryPointLoad
{
    /// <summary>
    /// 
    /// </summary>
    public static void LoadEntryPoints(Assembly asm, string pluginJsonFile, string outPath)
    {
        if (!File.Exists(pluginJsonFile))
        {
            Logger.Log($"{pluginJsonFile} not exists, Skipping...", LoggerLevel.Warning);
            return;
        }

        var types = asm.GetExportedTypes()
            .Where(x => x.GetCustomAttributes(typeof(EntryPointAttribute), inherit: false).Any());
        var entryPointTypes = types as Type[] ?? types.ToArray();
        if (entryPointTypes.Length == 0)
        {
            Logger.Log($"No EntryPoint, Skipping...", LoggerLevel.Warning);
            return;
        }

        var jsonString = File.ReadAllText(pluginJsonFile);
        if (JsonNode.Parse(jsonString) is not JsonObject jsonObject) return;
        var entryPoints = new JsonObject();
        var dict = new Dictionary<string, List<string>>();
        foreach (var entryPointType in entryPointTypes)
        {
            var entryPoint = entryPointType.GetCustomAttribute<EntryPointAttribute>();
            if (entryPoint is null) continue;
            if (dict.ContainsKey(entryPoint.Name))
            {
                dict[entryPoint.Name].Add(entryPointType.FullName!);
            }
            else
            {
                dict.Add(entryPoint.Name, [entryPointType.FullName!]);
            }
        }

        foreach (var d in dict)
        {
            entryPoints[d.Key] = d.Value.Count > 1
                ? new JsonArray(d.Value.Select(entry => JsonValue.Create(entry)!).ToArray<JsonNode?>())
                : d.Value[0];

            Logger.Log($"Inject {d.Key}: [{string.Join(',', d.Value)}] -> EntryPoints(plugin.json)");
        }

        jsonObject["EntryPoints"] = entryPoints;
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
#if NET7_0
        options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
#endif
        File.WriteAllText(outPath,
            jsonObject.ToJsonString(options)
        );
        Logger.Log($"Patch plugin.json -> {outPath}", LoggerLevel.Success);
    }
}