using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShadowPluginLoader.Tool;

using System.IO.Compression;

internal static class PackageMethod
{
    private static readonly HashSet<string> DefaultExclude = [];

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

    private static string WildcardToRegex(string pattern)
    {
        return "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
    }
    
    private static void DeleteExcludeFile(string startPath, IList<string> excludePatterns)
    {
        var directoryInfo = new DirectoryInfo(startPath);

        foreach (var regex in excludePatterns.Where(pattern => !string.IsNullOrWhiteSpace(pattern))
                     .Select(pattern => new Regex(WildcardToRegex(pattern), RegexOptions.IgnoreCase)))
        {
            // 匹配目录
            foreach (var dir in directoryInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (!regex.IsMatch(dir.Name)) continue;
                dir.Delete(true);
                Logger.Log($"Exclude Directory: {dir.FullName}");
            }

            // 匹配文件
            foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                if (!regex.IsMatch(file.Name)) continue;
                file.Delete();
                Logger.Log($"Exclude File: {file.FullName}");
            }
        }
    }

    public static void ExcludeAndZip(string startPath, string excludesFiles,
        string zipDir, string zipName, string zipExt, string configuration)
    {
        var zipPath = Path.Combine(zipDir, $"{zipName}{(configuration == "Debug" ? "-Debug" : "")}{zipExt}");
        if (File.Exists(zipPath)) File.Delete(zipPath);
        foreach (var excludePath in excludesFiles.Split(";"))
        {
            if (File.Exists(excludePath))
            {
                foreach (var readLine in File.ReadLines(excludePath))
                {
                    DefaultExclude.Add(readLine);
                }
            }
            else
            {
                Logger.Log($"Exclude Path: {excludePath} Not Found, Skip", LoggerLevel.Warning);
            }
        }

        DeleteExcludeFile(startPath, DefaultExclude.ToArray());

        CheckFolderEmpty(startPath);
        Zip(startPath, zipPath);
    }

    private static void CheckFolderEmpty(string path)
    {
        foreach (var dir in Directory.EnumerateDirectories(path))
        {
            if (!IsFolderEmpty(dir)) continue;
            Logger.Log($"{dir}为空,删除");
            Directory.Delete(dir, true);
        }
    }

    private static bool IsFolderEmpty(string path)
    {
        if (Directory.EnumerateDirectories(path).Any(dir => !IsFolderEmpty(dir))) return false;
        return !Directory.EnumerateFiles(path).Any();
    }
}