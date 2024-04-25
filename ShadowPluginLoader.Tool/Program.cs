﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Windows;
using ShadowPluginLoader.MetaAttributes;

namespace ShadowPluginLoader.Tool
{
    internal class Program
    {
        // public static string DirPath = Environment.CurrentDirectory;
        private static readonly string[] ArgNames0 = { "Method", "ExportMetaFile", "OutputPath" };
        private static readonly string[] ArgNames1 = { "Method", "ProjectPath", "CsprojPath" };
        private static readonly string[] ArgNames2 = { "Method", "OutputPath", "ExcludesFile","zipPath", "defaultExclude"};

        private static void ShowArgs(IReadOnlyList<string> args, IReadOnlyList<string> name)
        {
            Logger.Log("Start! Args:", LoggerLevel.Success);
            for (var i = 0; i < args.Count; i++)
            {
                Logger.Log($"Arg{i} {name[i]}: {args[i]}");
            }
        }

        private static int Main(string[] args)
        {
            try
            {
                var method = args[0]; // Method
                if (method == "0")
                {
                    ShowArgs(args, ArgNames0);
                    var exportMetaFile = args[1]; // ExportMetaFile
                    var outputPath = args[2]; // OutputPath
                    ExportMetaMethod.ExportMeta(Assembly.LoadFrom(exportMetaFile), outputPath);
                }
                else if (method == "1")
                {
                    ShowArgs(args, ArgNames1);
                    var projectPath = args[1]; // projectPath
                    var csproj = args[2]; // csprojPath
                    ReadMetaMethod.Read(projectPath, csproj);
                }
                else if (method == "2")
                {
                    ShowArgs(args, ArgNames2);
                    var outputPath = args[1]; // OutputPath
                    var excludesFile = args[2]; // ExcludesFile
                    var zipPath = args[3];
                    var defaultExclude = Convert.ToBoolean(args[4]);
                    PackageMethod.Exclude(outputPath, zipPath, excludesFile, defaultExclude);
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
}