﻿namespace ShadowPluginLoader.MetaAttributes;
/// <summary>
/// Used to mark a property as a meta attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MetaAttribute: Attribute
{
    /// <summary>
    /// Is Required
    /// </summary>
    public bool Required { get; init; }
    
    /// <summary>
    /// Is Excluded
    /// </summary>
    public bool Exclude { get; init; }
    
    /// <summary>
    /// Mapping Project PropertyGroup Value
    /// </summary>
    public string PropertyGroupName { get; init; }
    
    /// <summary>
    /// Using Regex To Validate
    /// </summary>
    public string? Regex { get; init; }
}