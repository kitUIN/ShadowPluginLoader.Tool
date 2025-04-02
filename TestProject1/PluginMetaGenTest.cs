using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace TestProject1;

public class PluginMetaGenTest
{
    private string _pluginId;

    [SetUp]
    public void Setup()
    {
    }

    private Dictionary<string, List<string>> EntryPoints { get; } = new()
    {
        ["SettingsPage"] = ["SettingsPage"],
        ["HistoryResponder"] = ["HistoryResponder"],
    };

    private string GetValue(JObject dNode, JToken? pluginNode, bool pluginTokenIsObject, int depth = 0)
    {
        if (pluginNode == null) return "null";
        if (!pluginTokenIsObject)
            return pluginNode.Type switch
            {
                JTokenType.Boolean => pluginNode.Value<bool>().ToString().ToLower(),
                JTokenType.String => $"\"{pluginNode.Value<string>()}\"",
                JTokenType.Array => "[" + string.Join(",",
                    pluginNode.Values().Select(x => GetValue(dNode, x, pluginTokenIsObject)).ToList()) + "]",
                _ => $"{pluginNode}",
            };
        var attrs = new List<string>();
        foreach (var item in dNode.Value<JObject>("Properties")!)
        {
            var dObj = item.Value!.Value<JObject>()!;

            var name = dObj.Value<string>("PropertyGroupName");
            var entryPointName = dObj.Value<string>("EntryPointName");
            if (entryPointName != null && EntryPoints.ContainsKey(entryPointName) &&
                EntryPoints[entryPointName].Count > 0)
            {
                if (dNode.ContainsKey("Item"))
                    attrs.Add($"{name} = [" +
                              string.Join(",", EntryPoints[entryPointName].Select(x => $"typeof({x})")) + "]");
                else attrs.Add($"{name} = typeof({EntryPoints[entryPointName][0]})");
                continue;
            }

            var pluginObj = pluginNode.Value<JObject>()!;
            if (name == null) continue;
            var pluginValue = pluginObj.ContainsKey(name) ? pluginObj.GetValue(name) : new JObject();
            var token = GetValue(dObj, pluginValue, dObj.ContainsKey("Properties"), depth + 1);
            if (token == "{}") continue;
            attrs.Add($"{name} = {token}");
            if (name == "Id" && _pluginId == "") _pluginId = token;
        }

        var indent = "\n\t\t\t" + new string('\t', depth + 1);
        var resultIndent = "\n\t\t\t" + new string('\t', depth);
        var result = string.Join(",", attrs.Select(attr => indent + attr));
        var newType = dNode.Value<string>("Type")!;
        return $"new {newType}{resultIndent}{{{result}{resultIndent}}}";
    }

    
    [Test]
    public void Test1()
    {
        var define = JToken.Parse("""
                                  {
                                    "Type": "ShadowViewer.Core.Plugins.PluginMetaData",
                                    "Properties": {
                                      "Description": {
                                        "Type": "System.String",
                                        "Required": true,
                                        "PropertyGroupName": "Description",
                                        "Nullable": false
                                      },
                                      "Authors": {
                                        "Type": "System.String",
                                        "Required": true,
                                        "PropertyGroupName": "Authors",
                                        "Nullable": false
                                      },
                                      "WebUri": {
                                        "Type": "System.String",
                                        "Required": true,
                                        "PropertyGroupName": "WebUri",
                                        "Nullable": false
                                      },
                                      "Logo": {
                                        "Type": "System.String",
                                        "Required": true,
                                        "PropertyGroupName": "Logo",
                                        "Nullable": false
                                      },
                                      "PluginManage": {
                                        "Type": "ShadowViewer.Core.Plugins.PluginManage",
                                        "Required": false,
                                        "PropertyGroupName": "PluginManage",
                                        "Nullable": false,
                                        "Properties": {
                                          "CanSwitch": {
                                            "Type": "System.Boolean",
                                            "Required": false,
                                            "PropertyGroupName": "CanSwitch",
                                            "Nullable": false
                                          },
                                          "CanDelete": {
                                            "Type": "System.Boolean",
                                            "Required": false,
                                            "PropertyGroupName": "CanDelete",
                                            "Nullable": false
                                          },
                                          "CanOpenFolder": {
                                            "Type": "System.Boolean",
                                            "Required": false,
                                            "PropertyGroupName": "CanOpenFolder",
                                            "Nullable": false
                                          },
                                          "SettingsPage": {
                                            "Type": "System.Type",
                                            "Required": false,
                                            "PropertyGroupName": "SettingsPage",
                                            "Nullable": false,
                                            "EntryPointName": "SettingsPage"
                                          }
                                        }
                                      },
                                      "PluginResponder": {
                                        "Type": "ShadowViewer.Core.Plugins.PluginResponder",
                                        "Required": false,
                                        "PropertyGroupName": "PluginResponder",
                                        "Nullable": false,
                                        "Properties": {
                                          "HistoryResponder": {
                                            "Type": "System.Type",
                                            "Required": false,
                                            "PropertyGroupName": "HistoryResponder",
                                            "Nullable": false,
                                            "EntryPointName": "HistoryResponder"
                                          },
                                          "NavigationResponder": {
                                            "Type": "System.Type",
                                            "Required": false,
                                            "PropertyGroupName": "NavigationResponder",
                                            "Nullable": false,
                                            "EntryPointName": "NavigationResponder"
                                          },
                                          "PicViewResponder": {
                                            "Type": "System.Type",
                                            "Required": false,
                                            "PropertyGroupName": "PicViewResponder",
                                            "Nullable": false,
                                            "EntryPointName": "PicViewResponder"
                                          },
                                          "SearchSuggestionResponder": {
                                            "Type": "System.Type",
                                            "Required": false,
                                            "PropertyGroupName": "SearchSuggestionResponder",
                                            "Nullable": false,
                                            "EntryPointName": "SearchSuggestionResponder"
                                          },
                                          "SettingFolders": {
                                            "Type": "System.Array",
                                            "Required": false,
                                            "PropertyGroupName": "SettingFolders",
                                            "Nullable": true,
                                            "Item": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "EntryPointName": "SettingFolders"
                                            }
                                          }
                                        }
                                      },
                                      "AffiliationTag": {
                                        "Type": "ShadowViewer.Core.Models.ShadowTag",
                                        "Required": false,
                                        "PropertyGroupName": "AffiliationTag",
                                        "Nullable": true,
                                        "Properties": {
                                          "Name": {
                                            "Type": "System.String",
                                            "Required": true,
                                            "PropertyGroupName": "Name",
                                            "Nullable": false
                                          },
                                          "BackgroundHex": {
                                            "Type": "System.String",
                                            "Required": true,
                                            "PropertyGroupName": "BackgroundHex",
                                            "Nullable": false
                                          },
                                          "ForegroundHex": {
                                            "Type": "System.String",
                                            "Required": true,
                                            "PropertyGroupName": "ForegroundHex",
                                            "Nullable": false
                                          },
                                          "Icon": {
                                            "Type": "System.String",
                                            "Required": false,
                                            "PropertyGroupName": "Icon",
                                            "Nullable": true
                                          },
                                          "PluginId": {
                                            "Type": "System.String",
                                            "Required": true,
                                            "PropertyGroupName": "PluginId",
                                            "Nullable": false
                                          }
                                        }
                                      },
                                      "CoreVersion": {
                                        "Type": "System.String",
                                        "Required": true,
                                        "PropertyGroupName": "CoreVersion",
                                        "Nullable": false
                                      },
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
        var plugin = JToken.Parse("""
                                  {
                                    "DllName": "ShadowViewer.Plugin.Local",
                                    "Description": "ShadowViewer本地阅读插件",
                                    "Authors": "kitUIN",
                                    "WebUri": "https://github.com/kitUIN/ShadowViewer.Plugin.Local",
                                    "Logo": "fluent://regular/ResizeImage",
                                    "PluginManage": {
                                      "CanSwitch": false,
                                      "CanDelete": false,
                                      "CanOpenFolder": false
                                    },
                                    "AffiliationTag": {
                                      "Name": "Local",
                                      "BackgroundHex": "#ffd657",
                                      "ForegroundHex": "#000000",
                                      "Icon": "fluent://regular/ResizeImage",
                                      "PluginId": "Local"
                                    },
                                    "CoreVersion": "2025.4.1.3",
                                    "Id": "Local",
                                    "Name": "本地阅读器",
                                    "Version": "1.3.15",
                                    "Dependencies": []
                                  }
                                  """);
        var content = GetValue((JObject)define, plugin, true);
        Console.WriteLine(content);
    }
}