namespace ShadowPluginLoader.Attributes;

/// <summary>
/// Used to mark a property as a meta attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MetaAttribute : Attribute
{
    /// <summary>
    /// Is Required
    /// </summary>
    public bool Required { get; init; } = true;

    /// <summary>
    /// Is Excluded
    /// </summary>
    public bool Exclude { get; init; } = false;

    /// <summary>
    /// Mapping Project PropertyGroup Value
    /// </summary>
    public string? PropertyGroupName { get; init; }

    /// <summary>
    /// Using Regex To Validate
    /// </summary>
    public string? Regex { get; init; }
    
    /// <summary>
    /// Custom Converter Type.
    /// </summary>
    public Type? Converter { get; init; }

    /// <summary>
    /// Json Type As String
    /// </summary>
    public bool AsString { get; init; } = false;
}