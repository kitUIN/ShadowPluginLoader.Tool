namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// Entry Point
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EntryPointAttribute : Attribute
{
    /// <summary>
    /// Entry Point Name
    /// </summary>
    public virtual string Name { get; init; } = null!;
}