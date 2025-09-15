using System;
using System.Windows;

namespace ShadowPluginLoader.Tool;

internal enum LoggerLevel
{
    Success,
    Debug,
    Info,
    Warning,
    Error
}

internal static class Logger
{
    public static void Log(string message,LoggerLevel level = LoggerLevel.Info)
    {
        Console.WriteLine($"[Build] | {level} | {message}");
    }
    public static void Error(string message)
    {
        Console.Error.WriteLine($"[Build] | Error | {message}");
    }
}