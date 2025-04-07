# ShadowPluginLoader.Tool

ShadowPluginLoader.Tool 是一个用于 [ShadowPluginLoader.WinUI](https://github.com/kitUIN/ShadowPluginLoader.WinUI) 的工具类。

[![nuget](https://img.shields.io/nuget/v/ShadowPluginLoader.Tool?style=flat-square)](https://www.nuget.org/packages/ShadowPluginLoader.Tool)

提供构建方法:
- 为自定义的`PluginLoader`导出`MetaData`为`plugin.d.json`
- 为自定义的`PluginLoader`与`Plugin`项目自动生成`props`文件
- 自动复制`plugin.d.json`到插件开发者的插件目录
- 自动生成插件开发者的插件的`plugin.json`
- 自动修补`plugin.json`
- 自动打包插件开发者的插件
