namespace ShadowPluginLoader.Attributes;

/// <summary>
/// 
/// </summary>
/// <example>
/// <code>
/// [ShadowSettingClass(Container = "ShadowExample.Plugin.Emoji",
///                     ClassName = "EmojiSetting"
///                     PluginAccessorName = "Setting")]
/// public enum BikaConfigKey
/// {
///     [ShadowSetting(typeof(int), "1", "Api分流")]
///     ApiShunt,
///
///     [ShadowSetting(typeof(bool), "true", "登陆后记住我")]
///     RememberMe,
///
///     [ShadowSetting(typeof(string), "tt",comment: "测试名称")]
///     TestName,
///
///     [ShadowSetting(typeof(string), "Temp",  "测试名称", true)]
///     TempPath,
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Enum)]
public class ShadowSettingClassAttribute : Attribute
{
    /// <summary>
    /// Class Name
    /// </summary>
    public string? ClassName { get; init; }

    /// <summary>
    /// Setting Container
    /// </summary>
    public string? Container { get; init; }

    /// <summary>
    /// Settings In Plugin Member
    /// </summary>
    public string? PluginAccessorName { get; init; }
}