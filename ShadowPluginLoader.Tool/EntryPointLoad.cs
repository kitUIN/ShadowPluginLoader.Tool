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
internal static class EntryPointLoad
{
    /// <summary>
    /// 
    /// </summary>
    public static void LoadEntryPoints(Assembly asm, JsonObject jsonObject, string outPath)
    {
        var types = asm.GetExportedTypes()
            .Where(x => x.GetCustomAttributes(typeof(EntryPointAttribute), inherit: false).Any());
        var entryPointTypes = types as Type[] ?? types.ToArray();
        if (entryPointTypes.Length == 0)
        {
            Logger.Log($"No EntryPoint, Skipping...", LoggerLevel.Warning);
            return;
        }
        var entryPoints = new JsonArray(); 
        foreach (var entryPointType in entryPointTypes)
        {
            var entryPoint = entryPointType.GetCustomAttribute<EntryPointAttribute>();
            if (entryPoint is null) continue;
            entryPoints.Add(new JsonObject
            {
                ["Name"] = entryPoint.Name,
                ["Type"] = entryPointType.FullName
            });
            Logger.Log($"Inject {entryPointType.FullName} -> [{entryPoint.Name}]EntryPoints(plugin.json)");
        }

        jsonObject["EntryPoints"] = entryPoints;
        
    }
}