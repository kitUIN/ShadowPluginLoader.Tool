namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// Auto DI
/// </summary>
/// <example>
/// <code>
/// class A {
///     [Autowired]
///     ILogger Logger { get; set; }
/// }
/// </code>
/// equal
/// <code>
/// class A {
///     ILogger Logger { get; set; }
///     public A(ILogger logger) {
///         Logger = logger;
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public class AutowiredAttribute: Attribute
{
    
}