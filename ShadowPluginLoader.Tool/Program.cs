using NJsonSchema;
using NJsonSchema.Generation;
using ShadowPluginLoader.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;

namespace ShadowPluginLoader.Tool;

internal class Program
{
    // public static string DirPath = Environment.CurrentDirectory;
    private static readonly string[] ArgNames0 = ["Method", "ExportMetaFile", "OutputPath"];
    private static readonly string[] ArgNames1 = ["Method", "ProjectPath", "CsprojPath", "DllFilePath"];

    private static readonly string[] ArgNames2 =
    [
        "Method", "OutputPath", "ExcludesFile",
        "zipPath", "zipName", "zipExt", "Configuration", "defaultExclude"
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
                    var assembly = Assembly.LoadFrom(exportMetaFile);
                    FieldAttributeSchemaProcessor.SdkVersion = assembly?.GetName().Version?.ToString();
                    var type = assembly?.GetExportedTypes()
                        .FirstOrDefault(x => x.GetCustomAttributes()
                            .Any(y => y is ExportMetaAttribute));
                    if (type is null) throw new Exception("Not Found ExportMetaAttribute In Any Class");
                    var settings = new SystemTextJsonSchemaGeneratorSettings
                    {
                        FlattenInheritanceHierarchy = true,
                        // AllowReferencesWithProperties = true,
                    };
                    settings.SchemaProcessors.Add(new FieldAttributeSchemaProcessor());
                    var schema = JsonSchema.FromType(type, settings);
                    JsonSchemaDefinitionCleaner.RemoveUnusedDefinitions(schema);
                    var path = Path.Combine(outputPath, "plugin.d.json");
                    File.WriteAllText(path, schema.ToJson());
                    Logger.Log($"plugin.d.json -> {path}", LoggerLevel.Success);
                    break;
                }
                case "1":
                {
                    ShowArgs(args, ArgNames1);
                    var projectPath = args[1]; // projectPath
                    var csprojPath = args[2]; // csprojPath
                    var dllFilePath = args[3]; // DllFilePath
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(csprojPath);
                    var root = xmlDoc.DocumentElement;
                    if (root is null) break;
                    var content = ReadMetaMethod.CheckJsonRequired(projectPath, root, dllFilePath);
                    var dllName = Path.GetFileNameWithoutExtension(dllFilePath);
                    var outPath = Path.Combine(Path.GetDirectoryName(csprojPath)!, dllName, "plugin.json");
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
                    PackageMethod.Exclude(outputPath, excludesFile,
                        zipPath, zipName, zipExt, configuration);
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