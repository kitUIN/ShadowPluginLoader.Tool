using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.Tool;

internal class Program
{
    // public static string DirPath = Environment.CurrentDirectory;
    private static readonly string[] ArgNames0 = ["Method", "ExportMetaFile", "OutputPath"];
    private static readonly string[] ArgNames1 = ["Method", "ProjectPath", "CsprojPath", "PluginMeta", "DllFilePath"];

    private static readonly string[] ArgNames2 =
    [
        "Method", "OutputPath", "ExcludesFile",
        "zipPath", "zipName", "zipExt", "Configuration", "defaultExclude", "needMsix"
    ];

    private static void ShowArgs(IReadOnlyList<string> args, IReadOnlyList<string> name)
    {
        Logger.Log("Start! Args:", LoggerLevel.Success);
        for (var i = 0; i < args.Count; i++)
        {
            Logger.Log($"Arg{i} {name[i]}: {args[i]}");
        }
    }

    public static int Main(string[] args)
    {
        try
        {
            var method = args[0]; // Method
            switch (method)
            {
                case "0":
                {
                    ShowArgs(args, ArgNames0);
                    var exportMetaFile = args[1]; // ExportMetaFile
                    var outputPath = args[2]; // OutputPath
                    var type = Assembly.LoadFrom(exportMetaFile).GetExportedTypes()
                        .FirstOrDefault(
                            x => x.GetCustomAttributes()
                                .Any(y => y is ExportMetaAttribute));
                    if (type is null) throw new Exception("Not Found ExportMetaAttribute In Any Class");
                    var content = ExportMetaMethod.ExportMetaToJson(type);
                    var path = Path.Combine(outputPath, "plugin.d.json");
                    File.WriteAllText(path, content);
                    Logger.Log($"plugin.d.json -> {path}", LoggerLevel.Success);
                    break;
                }
                case "1":
                {
                    ShowArgs(args, ArgNames1);
                    var projectPath = args[1]; // projectPath
                    var csprojPath = args[2]; // csprojPath
                    var pluginMeta = args[3]; // PluginMeta
                    if (string.IsNullOrEmpty(pluginMeta))
                    {
                        throw new Exception("Missing <PluginMeta> in <PropertyGroup>(.csproj)");
                    }
                    var dllFilePath = args[4]; // DllFilePath
                    var defineJson = ReadMetaMethod.GetDefineJson(projectPath);
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(csprojPath);
                    var pluginMetaRoot = new XmlDocument();
                    pluginMetaRoot.LoadXml("<PluginMeta>" + pluginMeta + "</PluginMeta>");
                    var root = xmlDoc.DocumentElement;
                    var pluginMetaDoc = pluginMetaRoot.DocumentElement;
                    if (root is null) break;
                    if (pluginMetaDoc is null) break;
                    var content = ReadMetaMethod.CheckJsonRequired(defineJson, root, pluginMetaDoc, dllFilePath);
                    var dllDir = Path.GetDirectoryName(dllFilePath)!;
                    var outPath = Path.Combine(dllDir, "Assets", "plugin.json");
                    EntryPointLoad.LoadEntryPoints(Assembly.LoadFrom(dllFilePath), content, outPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    };
            #if NET7_0
                    options.TypeInfoResolver = JsonSerializerOptions.Default.TypeInfoResolver;
            #endif
                    File.WriteAllText(outPath, content.ToJsonString(options));
                    Logger.Log($"Export plugin.json -> {outPath}", LoggerLevel.Success);
                    break;
                }
                case "2":
                {
                    ShowArgs(args, ArgNames2);
                    var outputPath = args[1]; // OutputPath
                    var excludesFile = args[2]; // ExcludesFile
                    var zipPath = args[3]; // zipPath
                    var zipName = args[4]; // zipName
                    var zipExt = args[5]; // zipExt
                    var configuration = args[6]; // Configuration
                    var defaultExclude = Convert.ToBoolean(args[7]);
                    var needMsix = Convert.ToBoolean(args[8]);
                    PackageMethod.Exclude(outputPath, excludesFile,
                        zipPath, zipName, zipExt, configuration,
                        defaultExclude, needMsix);
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Error($"{exception.GetType().Name}: {exception.Message}, Stack Trace:\n{exception.StackTrace}");
            return -1;
        }

        return 0;
    }
}