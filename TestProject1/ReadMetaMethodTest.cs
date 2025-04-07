using System.Text.Json.Nodes;
using System.Xml;
using ShadowPluginLoader.Tool;

namespace TestProject1;

public class ReadMetaMethodTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var define = JsonNode.Parse("""
                                    {
                                      "Type": "TestProject1.AbstractPluginMetaData",
                                      "Properties": {
                                        "Id": {
                                          "Type": "System.String",
                                          "Required": true,
                                          "PropertyGroupName": "Id",
                                          "Nullable": false
                                        },
                                        "Name": {
                                          "Type": "System.String",
                                          "Required": true,
                                          "PropertyGroupName": "Name",
                                          "Nullable": false
                                        },
                                        "DllName": {
                                          "Type": "System.String",
                                          "Required": false,
                                          "PropertyGroupName": "DllName",
                                          "Nullable": false
                                        },
                                        "Version": {
                                          "Type": "System.String",
                                          "Required": true,
                                          "PropertyGroupName": "Version",
                                          "Nullable": false
                                        },
                                        "Priority": {
                                          "Type": "System.Int32",
                                          "Required": false,
                                          "PropertyGroupName": "Priority",
                                          "Nullable": false
                                        },
                                        "Dependencies": {
                                          "Type": "System.Array",
                                          "Required": false,
                                          "PropertyGroupName": "Dependencies",
                                          "Nullable": false,
                                          "Item": {
                                            "Type": "System.String",
                                            "Nullable": false
                                          }
                                        }
                                      }
                                    }

                                    """);
        var pluginMeta = """
                         	<Id>Local</Id>
                         	<Name>本地阅读器</Name>
                         	<Version>1.0.0</Version>
                         	<Logo>fluent://regular/ResizeImage</Logo>
                         	<PluginLang>zh-CN</PluginLang>
                         	<WebUri>$(RepositoryUrl)</WebUri>
                         	<Description>$(Description)</Description>
                         	<Authors>$(Authors)</Authors>
                         	<PluginManage>
                         		<CanOpenFolder>false</CanOpenFolder>
                         		<CanDelete>false</CanDelete>
                         		<CanSwitch>false</CanSwitch>
                         	</PluginManage>
                         	<AffiliationTag>
                         		<Name>Local</Name>
                         		<PluginId>Local</PluginId>
                         		<Icon>fluent://regular/ResizeImage</Icon>
                         		<ForegroundHex>#000000</ForegroundHex>
                         		<BackgroundHex>#ffd657</BackgroundHex>
                         	</AffiliationTag>
                         """;

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("""
                       <Project Sdk="Microsoft.NET.Sdk">
                       <PropertyGroupName></PropertyGroupName>
                       </Project>
                       """);
        var pluginMetaRoot = new XmlDocument();
        pluginMetaRoot.LoadXml("<PluginMeta>" + pluginMeta + "</PluginMeta>");
        var root = xmlDoc.DocumentElement;
        var pluginMetaDoc = pluginMetaRoot.DocumentElement;
        var content =
            ReadMetaMethod.CheckJsonRequired((JsonObject)define!, root!, pluginMetaDoc!, "Test.dll");
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "DllName": "Test.dll",
                                          "Id": "Local",
                                          "Name": "本地阅读器",
                                          "Version": "1.0.0",
                                          "Dependencies": []
                                        }
                                        """));
    }

    [Test]
    public void Test2()
    {
        var define = JsonNode.Parse("""
                                    {
                                      "Type": "ShadowViewer.Core.Plugins.PluginMetaData",
                                      "Properties": {
                                        "Description": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Description"
                                        },
                                        "Authors": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Authors"
                                        },
                                        "WebUri": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "WebUri"
                                        },
                                        "Logo": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Logo"
                                        },
                                        "PluginManage": {
                                          "Type": "ShadowViewer.Core.Plugins.PluginManage",
                                          "Nullable": false,
                                          "Required": false,
                                          "PropertyGroupName": "PluginManage",
                                          "Properties": {
                                            "CanSwitch": {
                                              "Type": "System.Boolean",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "CanSwitch"
                                            },
                                            "CanDelete": {
                                              "Type": "System.Boolean",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "CanDelete"
                                            },
                                            "CanOpenFolder": {
                                              "Type": "System.Boolean",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "CanOpenFolder"
                                            },
                                            "SettingsPage": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "SettingsPage",
                                              "EntryPointName": "SettingsPage"
                                            }
                                          }
                                        },
                                        "PluginResponder": {
                                          "Type": "ShadowViewer.Core.Plugins.PluginResponder",
                                          "Nullable": false,
                                          "Required": false,
                                          "PropertyGroupName": "PluginResponder",
                                          "Properties": {
                                            "HistoryResponder": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "HistoryResponder",
                                              "EntryPointName": "HistoryResponder"
                                            },
                                            "NavigationResponder": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "NavigationResponder",
                                              "EntryPointName": "NavigationResponder"
                                            },
                                            "PicViewResponder": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "PicViewResponder",
                                              "EntryPointName": "PicViewResponder"
                                            },
                                            "SearchSuggestionResponder": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": false,
                                              "PropertyGroupName": "SearchSuggestionResponder",
                                              "EntryPointName": "SearchSuggestionResponder"
                                            },
                                            "SettingFolders": {
                                              "Type": "System.Type[]",
                                              "Nullable": true,
                                              "Required": false,
                                              "PropertyGroupName": "SettingFolders",
                                              "EntryPointName": "SettingFolders"
                                            }
                                          }
                                        },
                                        "AffiliationTag": {
                                          "Type": "ShadowViewer.Core.Models.ShadowTag",
                                          "Nullable": true,
                                          "Required": false,
                                          "PropertyGroupName": "AffiliationTag",
                                          "Properties": {
                                            "Name": {
                                              "Type": "System.String",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "Name"
                                            },
                                            "BackgroundHex": {
                                              "Type": "System.String",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "BackgroundHex"
                                            },
                                            "ForegroundHex": {
                                              "Type": "System.String",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "ForegroundHex"
                                            },
                                            "Icon": {
                                              "Type": "System.String",
                                              "Nullable": true,
                                              "Required": false,
                                              "PropertyGroupName": "Icon"
                                            },
                                            "PluginId": {
                                              "Type": "System.String",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "PluginId"
                                            }
                                          }
                                        },
                                        "CoreVersion": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "CoreVersion"
                                        },
                                        "Id": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Id"
                                        },
                                        "Name": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Name"
                                        },
                                        "DllName": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": false,
                                          "PropertyGroupName": "DllName"
                                        },
                                        "Version": {
                                          "Type": "System.String",
                                          "Nullable": false,
                                          "Required": true,
                                          "PropertyGroupName": "Version"
                                        },
                                        "Priority": {
                                          "Type": "System.Int32",
                                          "Nullable": false,
                                          "Required": false,
                                          "PropertyGroupName": "Priority"
                                        },
                                        "Dependencies": {
                                          "Type": "System.String[]",
                                          "Nullable": false,
                                          "Required": false,
                                          "PropertyGroupName": "Dependencies"
                                        }
                                      }
                                    }
                                    """);
        var pluginMeta = """
                           <Id>Local</Id>
                         <Name>本地阅读器</Name>
                         <Version>$(Version)</Version>
                         <Logo>fluent://regular/ResizeImage</Logo>
                         <PluginLang>zh-CN</PluginLang>
                         <WebUri>$(RepositoryUrl)</WebUri>
                         <Description>$(Description)</Description>
                         <Authors>$(Authors)</Authors>
                         <CoreVersion>2025.4.7.8</CoreVersion>
                         <PluginManage>
                         	<CanOpenFolder>false</CanOpenFolder>
                         	<CanSwitch>false</CanSwitch>
                         </PluginManage>
                         <AffiliationTag>
                         	<Name>Local</Name>
                         	<PluginId>Local</PluginId>
                         	<Icon>fluent://regular/ResizeImage</Icon>
                         	<ForegroundHex>#000000</ForegroundHex>
                         	<BackgroundHex>#ffd657</BackgroundHex>
                         </AffiliationTag>
                         """;

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("""
                       <Project Sdk="Microsoft.NET.Sdk">
                       <PropertyGroupName></PropertyGroupName>
                       </Project>
                       """);
        var pluginMetaRoot = new XmlDocument();
        pluginMetaRoot.LoadXml("<PluginMeta>" + pluginMeta + "</PluginMeta>");
        var root = xmlDoc.DocumentElement;
        var pluginMetaDoc = pluginMetaRoot.DocumentElement;
        var content =
            ReadMetaMethod.CheckJsonRequired((JsonObject)define!, root!, pluginMetaDoc!, "Test.dll");
        Console.WriteLine(content);
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "DllName": "Test.dll",
                                          "Description": "$(Description)",
                                          "Authors": "$(Authors)",
                                          "WebUri": "$(RepositoryUrl)",
                                          "Logo": "fluent://regular/ResizeImage",
                                          "PluginManage": {
                                            "CanSwitch": false,
                                            "CanOpenFolder": false
                                          },
                                          "AffiliationTag": {
                                            "Name": "Local",
                                            "BackgroundHex": "#ffd657",
                                            "ForegroundHex": "#000000",
                                            "Icon": "fluent://regular/ResizeImage",
                                            "PluginId": "Local"
                                          },
                                          "CoreVersion": "2025.4.7.8",
                                          "Id": "Local",
                                          "Name": "本地阅读器",
                                          "Version": "$(Version)",
                                          "Dependencies": []
                                        }
                                        """));
    }
}