using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;

namespace TestProject1;

public abstract class AbstractPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true)]
    public string Id { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.DllName"/>
    /// </summary>
    [Meta(Required = false)]
    public string DllName { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="System.Version"/>
    /// </summary>
    [Meta(Required = true)]
    public string Version { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="System.Version"/>
    /// </summary>
    [Meta(Required = false)]
    public int Priority { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Dependencies"/>
    /// </summary>
    [Meta(Required = true)]
    public string[] Dependencies { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.EntryPoints"/>
    /// </summary>
    [Meta(Exclude = true)]
    public JsonNode? EntryPoints { get; init; } = null;
}