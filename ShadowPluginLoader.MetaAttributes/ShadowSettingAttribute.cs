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
    public string SettingType { get; init; }

    /// <summary>
    /// Setting Default
    /// </summary>
    public string? Default { get; init; }

    /// <summary>
    /// Setting Comment
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    /// Represents an attribute used to configure and manage specific field settings.
    /// </summary>
    public ShadowSettingAttribute(string settingType, string? defaultVal = null, string? comment = null)
    {
        SettingType = settingType;
        Default = defaultVal;
        Comment = comment;
    }
}