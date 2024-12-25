namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// An attribute used to designate a class as a plugin setting class in the Shadow Plugin Loader framework.
/// This attribute is intended for usage on classes that define configuration or settings for a plugin.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class ShadowPluginSettingClassAttribute : Attribute
{
    /// <summary>
    /// Plugin Type
    /// </summary>
    public Type PluginType { get; }

    /// <summary>
    /// Accessor Alias Name
    /// </summary>
    public string AccessorAlias { get; }

    /// <summary>
    /// An attribute used to mark a class as a plugin settings class for the Shadow Plugin Loader framework.
    /// This attribute is applied to specify that the class contains configuration or settings related to a plugin.
    /// </summary>
    /// <remarks>
    /// The designated class using this attribute should be intended to manage settings specific
    /// to a plugin defined by the associated <see cref="PluginType"/>.
    /// </remarks>
    public ShadowPluginSettingClassAttribute(Type pluginType, string accessorAlias = "Settings")
    {
        PluginType = pluginType;
        AccessorAlias = accessorAlias;
    }
}