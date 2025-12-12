using Newtonsoft.Json.Linq;
using NJsonSchema;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;

namespace ShadowPluginLoader.Tool;

internal static class ReadMetaMethod
{
    private static async Task<JsonSchema> GetDefineJson(string projectPath)
    {
        var file = Path.Combine(projectPath, "plugin.d.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {file}");
        return await JsonSchema.FromFileAsync(file);
    }

    private static string GetPluginJson(string projectPath)
    {
        var file = Path.Combine(projectPath, "plugin.json");
        if (!File.Exists(file))
            throw new Exception($"Missing {file}");
        return File.ReadAllText(file);
    }

    private static void WarnDependencies()
    {
        Logger.Log("Dependency not specified, ignored.", LoggerLevel.Warning);
    }

    private static JsonArray LoadDependencies(XmlNode root)
    {
        var itemGroup = root.SelectSingleNode("ItemGroup[@Label='Dependencies']");
        var arrays = new JsonArray();
        if (itemGroup is null)
        {
            WarnDependencies();
            return arrays;
        }

        var dependencies = itemGroup.SelectNodes("PackageReference");
        if (dependencies is null)
        {
            WarnDependencies();
            return arrays;
        }

        foreach (XmlNode dependency in dependencies)
        {
            var id = dependency.Attributes!["Include"]?.Value;
            var label = dependency.Attributes?["Need"]?.Value;

            if (string.IsNullOrWhiteSpace(id))
                continue;
            arrays.Add(new JsonObject
            {
                ["Id"] = id,
                ["Need"] = label
            });
        }

        return arrays;
    }

    private static Dictionary<string, object> LoadTemplateDict(XmlNode root)
    {
        var dict = new Dictionary<string, object>();
        var propertyGroupList = root.SelectNodes("PropertyGroup");
        if (propertyGroupList == null) return dict;
        foreach (XmlNode child in propertyGroupList)
        {
            XmlNodeToDict(child, dict);
        }

        return dict;
    }

    private static Dictionary<string, object> XmlNodeToDict(XmlNode node, Dictionary<string, object> dict)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.HasChildNodes)
            {
                // 判断是否是数组：同名节点出现多次
                var sameNameNodes = node.SelectNodes(child.Name);
                if (sameNameNodes == null) continue;
                if (sameNameNodes.Count > 1)
                {
                    var list = (from XmlNode n in sameNameNodes
                        select XmlNodeToDict(n, new Dictionary<string, object>())).Cast<object>().ToList();
                    dict[child.Name] = list;
                }
                else if (child.ChildNodes.Count == 1 && child.FirstChild is { NodeType: XmlNodeType.Text })
                {
                    // 单一文本节点
                    dict[child.Name] = child.InnerText;
                }
                else
                {
                    // 嵌套对象
                    dict[child.Name] = XmlNodeToDict(child, new Dictionary<string, object>());
                }
            }
            else
            {
                dict[child.Name] = child.InnerText;
            }
        }

        return dict;
    }

    private static string ValidateJson(string projectPath, string json)
    {
        var schema = GetDefineJson(projectPath).GetAwaiter().GetResult();
        schema.AllowAdditionalProperties = true;
        var errors = schema.Validate(json);

        if (errors.Count == 0)
        {
            Console.WriteLine("JSON 校验通过");
            return ApplyDefaults(JObject.Parse(json), schema);
        }

        Console.WriteLine("JSON 校验失败");

        foreach (var error in errors)
        {
            Console.WriteLine($"路径: {error.Path}");
            Console.WriteLine($"类型: {error.Kind}");
            Console.WriteLine($"属性: {error.Property}");
            Console.WriteLine($"消息: {error}");
            Console.WriteLine("----------------------");
        }

        throw new Exception("JSON 校验失败");
    }

    private static string ApplyDefaults(JObject json, JsonSchema schema)
    {
        foreach (var (key, propSchema) in schema.Properties)
        {
            if (!json.ContainsKey(key) && propSchema.Default != null)
            {
                json[key] = JToken.FromObject(propSchema.Default);
            }
        }

        return json.ToString();
    }

    public static JsonObject CheckJsonRequired(string projectPath, XmlNode xmlDoc, string dllFilePath)
    {
        var pluginJsonValue = GetPluginJson(projectPath);
        var dllName = Path.GetFileNameWithoutExtension(dllFilePath);
        var dict = LoadTemplateDict(xmlDoc);
        var template = Template.Parse(pluginJsonValue);
        var result = template.Render(dict);
        var res = (JsonObject)JsonNode.Parse(ValidateJson(projectPath, result))!;
        res["DllName"] = dllName;
        if (!res.ContainsKey("Dependencies")) res["Dependencies"] = new JsonArray();
        var depArray = (JsonArray)res["Dependencies"]!;
        foreach (var dep in LoadDependencies(xmlDoc))
        {
            depArray.Add(dep!.DeepClone());
        }

        return res;
    }
}