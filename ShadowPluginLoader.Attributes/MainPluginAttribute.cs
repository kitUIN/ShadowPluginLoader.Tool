namespace ShadowPluginLoader.Attributes;
/// <summary>
/// Plugin Main Class
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MainPluginAttribute : EntryPointAttribute
{
    /// <summary>
    /// Name
    /// </summary>
    public override string Name => "MainPlugin";
    
    /// <summary>
    /// Is BuiltIn Plugin
    /// </summary>
    public bool BuiltIn { get; init; }
}