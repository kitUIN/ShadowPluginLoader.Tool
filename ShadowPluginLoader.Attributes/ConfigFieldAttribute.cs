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
    /// Whether this field is required
    /// </summary>
    public bool Required { get; init; } = true;

    /// <summary>
    /// Default value
    /// </summary>
    public object? DefaultValue { get; init; }

}
