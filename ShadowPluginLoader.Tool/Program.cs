using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace ShadowPluginLoader.Tool
{
    internal class Program
    {
        public static string DirPath = Environment.CurrentDirectory;
        public static string OutputPath = "";
        private static string _projectPath = "";
        public static string TypeIPluginMetaData = "";
        public static bool IsCn;

        private static readonly string[] ArgNames0 = { "Method", "ExportMetaFile", "TypeIPluginMetaData", "OutputPath", "ProjectPath", "Forces" };

        private static void ShowArgs(string[] args, string[] name)
        {
            Console.WriteLine("[ExportMeta] Start! Args:");
            for (var i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"[ExportMeta] {name[i]}: {args[i]}");
            }
        }
        private static readonly List<string> Types = new()
        {
            "String",
            "Int32",
            "Boolean",
            "String[]",
            "Int32[]",
            "Boolean[]",
        };
        public static bool CheckExportPropertyType(string type)
        {
            return Types.Any(x => x == type);
        }
        private static bool IsChinese()
        {
            var lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;
            return lcid is 0x804 or 0xc04 or 0x1404 or 0x1004 or 0x404;
        }

        #region Cache
        private static bool CheckCache()
        {
            return File.Exists(Path.Combine(_projectPath, "ToolTarget.cache"));
        }
        private static string LoadCache()
        {
            return File.ReadAllText(Path.Combine(_projectPath, "ToolTarget.cache"));
        }
        public static void BuildCache(string type)
        {
            File.WriteAllText(Path.Combine(_projectPath, "ToolTarget.cache"), type);
        }
        
        #endregion
        
        private static void Main(string[] args)
        {
            var method = args[0];
            if (method == "0")
            {
                ShowArgs(args, ArgNames0);
                var exportMetaFile = args[1];
                TypeIPluginMetaData = args[2];
                OutputPath = args[3];
                _projectPath = args[4];
                var forces = args[5];
                if (forces == "false" && CheckCache())
                {
                    var asm = Assembly.LoadFrom(exportMetaFile);
                    ExportMetaWindow.ExportMeta(asm, LoadCache());
                }
                else
                {
                    var thread = new Thread(() =>
                    {
                        var window = new ExportMetaWindow(exportMetaFile)
                        {
                            IsChinese = IsChinese()
                        };

                        window.Closed += (sender2, e2) =>
                            window.Dispatcher.InvokeShutdown();

                        window.Show();
                        System.Windows.Threading.Dispatcher.Run();
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                
            }
            else if (method == "1")
            {
                _projectPath = args[1];
                var csproj = args[2];
                ReadMetaWindow.Read(_projectPath,csproj);
            }
            
        }
    }
}
