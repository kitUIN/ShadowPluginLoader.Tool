namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// This attribute is intended to provide settings for configuration and management
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ShadowSettingAttribute : Attribute
{
    /// <summary>
    /// Setting Type
    /// </summary>
    public Type SettingType { get; }

    /// <summary>
    /// Setting Default
    /// </summary>
    public string? Default { get; }

    /// <summary>
    /// Setting Comment
    /// </summary>
    public string? Comment { get; }

    /// <summary>
    /// Represents an attribute used to configure and manage specific field settings.
    /// </summary>
    public ShadowSettingAttribute(Type settingType, string? defaultVal = null, string? comment = null)
    {
        SettingType = settingType;
        Default = defaultVal;
        Comment = comment;
    }
}