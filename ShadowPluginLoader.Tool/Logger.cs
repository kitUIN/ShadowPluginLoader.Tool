using System;
using System.Windows;

namespace ShadowPluginLoader.Tool;

public enum LoggerLevel
{
    Success,
    Debug,
    Info,
    Warning,
    Error
}

public static class Logger
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