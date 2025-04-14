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
    /// EntryPoint Name
    /// </summary>
    public string? EntryPointName { get; init; }

    /// <summary>
    /// Type in plugin.json
    /// </summary>
    public Type? JsonType { get; init; }  
    
    /// <summary>
    /// Construction Template
    /// </summary>
    public string? ConstructionTemplate { get; init; }
}