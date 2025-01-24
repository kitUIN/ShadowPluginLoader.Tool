# ShadowPluginLoader.MetaAttribute

## 元数据相关
ShadowPluginLoader的元数据特性
- Meta 元属性项
  - Required 该参数必须
  - Exclude 排除该参数
  - Regex 正则表达式
  - PropertyGroupName 对应的在PropertyGroup中的值,默认为Property名
  - Nullable 是否可空
- ExportMeta 指明类为元数据类
```csharp
[ExportMeta]
public class ExampleMetaData : AbstractPluginMetaData
{
    [Meta(Required = false)]
    public string[] Authors { get; init; }

    [Meta(Required = false)]
    public string Url { get; init; }

    [Meta(Required = false)]
    public double? D { get; init; }

    [Meta(Required = false)]
    public float[]? F { get; init; }
}
```

## AutoPluginMeta

为插件主类添加Meta类

## 设置相关

- ShadowSetting 设置项
- ShadowPluginSettingClass 为插件主类添加Settings类

## 自动装配DI

- Autowired 自动将访问器加载到构造函数中