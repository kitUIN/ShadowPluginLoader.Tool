namespace ShadowPluginLoader.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EntryPointMetaAttribute : MetaAttribute
{
    /// <summary>
    /// EntryPoint Name
    /// </summary>
    public string EntryPointName { get; init; }


    /// <summary>
    /// Is Required
    /// </summary>
    public new bool Required => false;

    /// <summary>
    /// Is Excluded
    /// </summary>
    public new bool Exclude => true;

    /// <summary>
    /// Json Type As String
    /// </summary>
    public new bool AsString => true;

    /// <summary>
    /// 
    /// </summary>
    public EntryPointMetaAttribute(string entryPointName)
    {
        EntryPointName = entryPointName;
    }
}