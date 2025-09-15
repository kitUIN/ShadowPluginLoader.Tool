using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShadowPluginLoader.Tool;

using System.IO.Compression;

internal static class PackageMethod
{
    private static readonly List<string> DefaultExclude =
    [
        "ShadowPluginLoader.WinUI.dll",
        "ShadowPluginLoader.WinUI.xml",
        "ShadowPluginLoader.WinUI.pri",
        "ShadowPluginLoader.WinUI.pdb",
        "Microsoft.Build.Utilities.dll"
    ];

    private static void Zip(string startPath, string zipPath)
    {
        var dir = Path.GetDirectoryName(zipPath);
        if (dir is not null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ZipFile.CreateFromDirectory(startPath, zipPath);
        Logger.Log($"Pack Plugin: {zipPath}", LoggerLevel.Success);
    }

    public static void Exclude(string startPath, string excludePath, 
        string zipDir, string zipName,string zipExt, string configuration,
        bool defaultExclude,bool needMsix=false)
    {
        var zipPath = Path.Combine(zipDir, $"{zipName}{(configuration == "Debug" ? "-Debug" : "")}{zipExt}");
        if (File.Exists(zipPath)) File.Delete(zipPath);
        if (!defaultExclude) DefaultExclude.Clear();
        if (File.Exists(excludePath))
        {
            DefaultExclude.AddRange(File.ReadLines(excludePath));
        }
        else
        {
            Logger.Log($"Exclude Path: {excludePath} Not Found, Skip",LoggerLevel.Warning);            
        }
        var directoryInfo = new DirectoryInfo(startPath);
        foreach (var exclude in DefaultExclude.Where(exclude => !string.IsNullOrEmpty(exclude)))
        {
            try
            {
                directoryInfo.GetFiles(exclude, SearchOption.AllDirectories)
                    .ToList().ForEach(file =>
                    {
                        file.Delete();
                        Logger.Log($"Exclude File: {file.FullName}");
                    });
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.Log(e.Message, LoggerLevel.Warning);
            }
        }

        CheckFolderEmpty(startPath);
        Zip(startPath, zipPath);
    }
    private static void CheckFolderEmpty(string path)
    {
        foreach (var dir in Directory.EnumerateDirectories(path))
        {
            if (!IsFolderEmpty(dir)) continue;
            Logger.Log($"{dir}为空,删除");
            Directory.Delete(dir,true);
        }
    }
    private static bool IsFolderEmpty(string path)
    {
        if (Directory.EnumerateDirectories(path).Any(dir => !IsFolderEmpty(dir))) return false;
        return !Directory.EnumerateFiles(path).Any();
    }
}