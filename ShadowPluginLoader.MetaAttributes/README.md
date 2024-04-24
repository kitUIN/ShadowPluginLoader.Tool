# ShadowPluginLoader.MetaAttribute

ShadowPluginLoader的元数据特性

- Required 该参数必须
- Exclude 排除该参数
- Regex 正则表达式
- PropertyGroupName 对应的在PropertyGroup中的值,默认为Property名
- Nullable 是否可空
```csharp
/// <summary>
/// Default PluginMetaData
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DefaultPluginMetaData: Attribute, IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true)]
    public string Id { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true)]
    public string Name { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true, Exclued = true)] // Exclude this
    public string Version { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Requires"/>
    /// </summary>
    [Meta(Required = true)]
    public string[] Requires { get; init; }
}
```