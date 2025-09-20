namespace ShadowPluginLoader.Attributes;

/// <summary>
/// Used to identify this class as a configuration class
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : Attribute
{
    /// <summary>
    /// Configuration file name
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// Configuration file directory
    /// </summary>
    public string? DirPath { get; init; } = "config";

    /// <summary>
    /// Configuration description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Configuration version
    /// </summary>
    public string? Version { get; init; }
}