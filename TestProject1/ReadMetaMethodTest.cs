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
}