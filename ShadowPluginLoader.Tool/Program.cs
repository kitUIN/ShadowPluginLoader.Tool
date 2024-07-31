using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShadowPluginLoader.Tool;

internal class Program
{
    // public static string DirPath = Environment.CurrentDirectory;
    private static readonly string[] ArgNames0 = ["Method", "ExportMetaFile", "OutputPath"];
    private static readonly string[] ArgNames1 = ["Method", "ProjectPath", "CsprojPath" ,"PluginMeta", "DllName"];
    private static readonly string[] ArgNames2 =
    [
        "Method", "OutputPath", "ExcludesFile",
        "zipPath","zipName","zipExt","Configuration", "defaultExclude"
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
                    ExportMetaMethod.ExportMeta(Assembly.LoadFrom(exportMetaFile), outputPath);
                    break;
                }
                case "1":
                {
                    ShowArgs(args, ArgNames1);
                    var projectPath = args[1]; // projectPath
                    var csproj = args[2]; // csprojPath
                    var pluginMeta = args[3]; // PluginMeta
                    if (string.IsNullOrEmpty(pluginMeta))
                    {
                        throw new Exception("Missing <PluginMeta> in <PropertyGroup>(.csproj)");
                    }
                    var dllName = args[4]; // DllName
                    ReadMetaMethod.Read(projectPath, csproj, pluginMeta,dllName);
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
                    PackageMethod.Exclude(outputPath,excludesFile, 
                        zipPath, zipName,zipExt,configuration,
                        defaultExclude);
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Log($"{exception.GetType().Name}: {exception.Message}", LoggerLevel.Error);
            return -1;
        }

        return 0;
    }

    
}