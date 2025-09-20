namespace ShadowPluginLoader.Attributes;

/// <summary>
/// Used to identify this field as a configuration field
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConfigFieldAttribute : Attribute
{
    /// <summary>
    /// Field name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Field description
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Field alias for serialization
    /// </summary>
    public string? Alias { get; init; }
    
    /// <summary>
    /// Apply naming conventions
    /// </summary>
    public bool ApplyNamingConventions { get; init; } = false;
}
