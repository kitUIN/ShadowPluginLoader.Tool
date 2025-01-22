namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// 
/// </summary>
public enum SettingsBaseFolder
{
    /// <summary>
    /// Gets the folder in the local app data store where you can save files that are not included in backup and restore.
    /// </summary>
    LocalCacheFolder,

    /// <summary>
    /// Gets the root folder in the local app data store. This folder is backed up to the cloud.
    /// </summary>
    LocalFolder,


    /// <summary>
    /// Gets the root folder in the roaming app data store.
    /// </summary>
    RoamingFolder,


    /// <summary>
    /// Gets the root folder in the shared app data store.
    /// </summary>
    SharedLocalFolder,

    /// <summary>
    /// Gets the root folder in the temporary app data store.
    /// </summary>
    TemporaryFolder,
}