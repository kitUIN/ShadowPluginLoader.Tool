namespace ShadowPluginLoader.MetaAttributes;

/// <summary>
/// Check DI Constructor
/// <example>
/// <code>
/// class A {
///     [Autowired]
///     ILogger Logger { get; set; }
/// }
/// [CheckAutowired]
/// class B: A { }
/// </code>
/// </example>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CheckAutowiredAttribute: Attribute
{
    
}