﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ShadowPluginLoader.Tool;

internal class Program
{
    // public static string DirPath = Environment.CurrentDirectory;
    private static readonly string[] ArgNames0 = ["Method", "ExportMetaFile", "OutputPath"];
    private static readonly string[] ArgNames1 = ["Method", "ProjectPath", "CsprojPath", "PluginMeta", "DllName"];
    private static readonly string[] ArgNames3 = ["Method", "ProjectPath", "PluginFile"];

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
                    ReadMetaMethod.Read(projectPath, csproj, pluginMeta, dllName);
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
                    PackageMethod.Exclude(outputPath, excludesFile,
                        zipPath, zipName, zipExt, configuration,
                        defaultExclude);
                    break;
                }
                case "3":
                {
                    ShowArgs(args, ArgNames3);
                    var projectPath = args[1]; // projectPath
                    var pluginFile = args[2]; // pluginFile
                    var pluginsJsonFile = Path.Combine(projectPath, "Assets", "plugin.json");

                    var outPath = Path.Combine(Path.GetDirectoryName(pluginFile)!,
                        Path.GetFileNameWithoutExtension(pluginFile), "Assets", "plugin.json");
                    EntryPointLoad.LoadEntryPoints(Assembly.LoadFrom(pluginFile), pluginsJsonFile, outPath);
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