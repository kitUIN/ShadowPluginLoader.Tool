namespace ShadowPluginLoader.Attributes;
/// <summary>
/// Plugin Main Class
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MainPluginAttribute : EntryPointAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public override string Name => "MainPlugin";
}