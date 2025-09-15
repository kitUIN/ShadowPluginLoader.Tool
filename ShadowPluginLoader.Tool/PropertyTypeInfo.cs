using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;

namespace ShadowPluginLoader.Tool;

/// <summary>
/// 
/// </summary>
/// <param name="TypeName"></param>
/// <param name="RawType">原始类型</param>
/// <param name="IsNullable">是否Nullable或可为空引用类型</param>
/// <param name="IsArray">是否数组</param>
/// <param name="IsGeneric">是否泛型</param>
/// <param name="ItemType">如果是集合</param>
/// <param name="GenericArguments">泛型参数的类型信息（递归结构）</param>
internal record PropertyTypeInfo(
    string TypeName,
    Type RawType,
    bool IsNullable,
    bool IsArray,
    bool IsGeneric,
    PropertyTypeInfo? ItemType,
    List<PropertyTypeInfo> GenericArguments
)
{
    public static PropertyTypeInfo Analyze(Type type)
    {
        var genericArgs = new List<PropertyTypeInfo>();
        bool isNullable = false;
        bool isArray = type.IsArray;
        bool isGeneric = type.IsGenericType;
        PropertyTypeInfo? itemType = null;
        var fullName = type.FullName;
        // Nullable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            isNullable = true;
            type = Nullable.GetUnderlyingType(type)!;
            fullName = type.FullName;
            isGeneric = false;
        }
        
        if (isArray && type.GetElementType() is { } elementType)
        {
            itemType = Analyze(elementType);
            fullName = "System.Array";
        }

        if (isGeneric)
        {
            
            var genericDef = type.GetGenericTypeDefinition();
            genericArgs.AddRange(type.GetGenericArguments().Select(Analyze));
            var genericDefName = genericDef.FullName;
            if (genericDefName != null && genericDefName.Contains('\u0060'))
                fullName = genericDefName.Split('\u0060')[0];
            if (genericDef == typeof(List<>))
            {
                isArray = true;
                itemType = genericArgs[0];
            } 
        }

        return new PropertyTypeInfo(
            TypeName: fullName,
            RawType: type,
            IsNullable: isNullable,
            IsArray: isArray,
            IsGeneric: isGeneric,
            ItemType: itemType,
            GenericArguments: genericArgs
        );
    }
    public static PropertyTypeInfo Analyze(PropertyInfo property)
    {
        var info = Analyze(property.PropertyType);

        // 可选增强：检测引用类型是否允许 null（非 Nullable<T>）
        var hasNullableAttribute = property
            .CustomAttributes.Any(a => a.AttributeType.Name == "NullableAttribute");

        return info with { IsNullable = info.IsNullable || hasNullableAttribute };
    }
    
    public JsonObject ToJsonObject()
    {
        var jsonObject = new JsonObject
        {
            [nameof(TypeName)] = TypeName,
            [nameof(IsNullable)] = IsNullable,
            [nameof(IsArray)] = IsArray,
            [nameof(IsGeneric)] = IsGeneric
        };

        // 如果存在 ItemType（对于集合类型），则递归处理
        if (ItemType != null)
        {
            jsonObject[nameof(ItemType)] = ItemType.ToJsonObject();
        }

        // 如果存在泛型参数，递归地添加它们
        if (GenericArguments.Count > 0)
        {
            var genericArgumentsArray = new JsonArray();
            foreach (var genericArgument in GenericArguments)
            {
                genericArgumentsArray.Add(genericArgument.ToJsonObject());  // 递归
            }
            jsonObject[nameof(GenericArguments)] = genericArgumentsArray;
        }

        return jsonObject;
    }

}