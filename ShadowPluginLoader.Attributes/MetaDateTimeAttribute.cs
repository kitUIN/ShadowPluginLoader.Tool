namespace ShadowPluginLoader.Attributes;
/// <summary>
/// Used to add datetime settings (type of property was <see cref="DateTime"/>/<see cref="DateTimeOffset"/>)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MetaDateTimeAttribute: Attribute
{
    /// <summary>
    /// DateTime Format
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// Use <see cref="System.Globalization.CultureInfo.InvariantCulture"/> if true; Else <see cref="System.Globalization.CultureInfo.CurrentCulture"/>
    /// </summary>
    public bool InvariantCulture { get; init; } = true;
}