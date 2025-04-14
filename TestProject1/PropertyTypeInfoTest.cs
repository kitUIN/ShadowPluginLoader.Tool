using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ShadowPluginLoader.Tool;

namespace TestProject1;

[TestFixture]
public class PropertyTypeInfoTests
{
    // 测试普通类型
    [Test]
    public void Test_NormalType()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(string));

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.String"));
            Assert.That(typeInfo.IsNullable, Is.False);
            Assert.That(typeInfo.IsArray, Is.False);
            Assert.That(typeInfo.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType, Is.Null);
            Assert.That(typeInfo.GenericArguments, Is.Empty);
        });
    }

    // 测试 Nullable 类型
    [Test]
    public void Test_NullableType()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(int?));

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.Int32"));
            Assert.That(typeInfo.IsNullable, Is.True);
            Assert.That(typeInfo.IsArray, Is.False);
            Assert.That(typeInfo.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType, Is.Null);
            Assert.That(typeInfo.GenericArguments, Is.Empty);
        });
    }

    // 测试数组类型
    [Test]
    public void Test_ArrayType()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(int[]));
        var itemType = PropertyTypeInfo.Analyze(typeof(int));

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.Array"));
            Assert.That(typeInfo.IsNullable, Is.False);
            Assert.That(typeInfo.IsArray, Is.True);
            Assert.That(typeInfo.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType!.TypeName, Is.EqualTo("System.Int32"));
            Assert.That(typeInfo.ItemType!.IsNullable, Is.False);
            Assert.That(typeInfo.ItemType!.IsArray, Is.False);
            Assert.That(typeInfo.ItemType!.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType!.GenericArguments, Is.Empty);
            Assert.That(typeInfo.GenericArguments, Is.Empty);
        });
    }

    // 测试 List<T> 类型
    [Test]
    public void Test_GenericTypeList()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(List<int>));

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.Collections.Generic.List"));
            Assert.That(typeInfo.IsNullable, Is.False);
            Assert.That(typeInfo.IsArray, Is.True);
            Assert.That(typeInfo.IsGeneric);
            // Assert.That(typeInfo.ItemType, Is.EqualTo(typeof(int)));
            Assert.That(typeInfo.GenericArguments.Count, Is.EqualTo(1));
        });
    }

    // 测试 Dictionary<TKey, TValue> 类型
    [Test]
    public void Test_GenericTypeDictionary()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(Dictionary<string, int>));

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.Collections.Generic.Dictionary"));
            Assert.That(typeInfo.IsNullable, Is.False);
            Assert.That(typeInfo.IsArray, Is.False);
            Assert.That(typeInfo.IsGeneric, Is.True);
            Assert.That(typeInfo.ItemType, Is.Null);
            Assert.That(typeInfo.GenericArguments.Count, Is.EqualTo(2));
        });
        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.GenericArguments[0].TypeName, Is.EqualTo("System.String"));
            Assert.That(typeInfo.GenericArguments[1].TypeName, Is.EqualTo("System.Int32"));
        });
    }

    // 测试带有 NullableAttribute 的属性
    [Test]
    public void Test_NullableAttribute()
    {
        var property = typeof(TestClass).GetProperty("NullableString");
        var typeInfo = PropertyTypeInfo.Analyze(property);

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.String"));
            Assert.That(typeInfo.IsNullable, Is.True);
            Assert.That(typeInfo.IsArray, Is.False);
            Assert.That(typeInfo.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType, Is.Null);
            Assert.That(typeInfo.GenericArguments, Is.Empty);
        });
    }

    // 测试没有 NullableAttribute 的普通属性
    [Test]
    public void Test_NonNullableAttribute()
    {
        var property = typeof(TestClass).GetProperty("NonNullableString");
        var typeInfo = PropertyTypeInfo.Analyze(property);

        Assert.Multiple(() =>
        {
            Assert.That(typeInfo.TypeName, Is.EqualTo("System.String"));
            Assert.That(typeInfo.IsNullable, Is.False);
            Assert.That(typeInfo.IsArray, Is.False);
            Assert.That(typeInfo.IsGeneric, Is.False);
            Assert.That(typeInfo.ItemType, Is.Null);
            Assert.That(typeInfo.GenericArguments, Is.Empty);
        });
    }

    // 测试嵌套属性
    [Test]
    public void Test_NestedGenericType()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(Dictionary<string, List<int>>));

        Assert.AreEqual("System.Collections.Generic.Dictionary", typeInfo.TypeName);
        Assert.IsFalse(typeInfo.IsNullable);
        Assert.IsFalse(typeInfo.IsArray);
        Assert.IsTrue(typeInfo.IsGeneric);
        Assert.IsNull(typeInfo.ItemType);
        Assert.AreEqual(2, typeInfo.GenericArguments.Count);
        Assert.AreEqual("System.String", typeInfo.GenericArguments[0].TypeName);
        Assert.AreEqual("System.Collections.Generic.List", typeInfo.GenericArguments[1].TypeName);
    }

    // 测试多层泛型
    [Test]
    public void Test_MultiLevelGenericType()
    {
        var typeInfo = PropertyTypeInfo.Analyze(typeof(Dictionary<string, List<Dictionary<int, string>>>));

        Assert.AreEqual("System.Collections.Generic.Dictionary", typeInfo.TypeName);
        Assert.IsFalse(typeInfo.IsNullable);
        Assert.IsFalse(typeInfo.IsArray);
        Assert.IsTrue(typeInfo.IsGeneric);
        Assert.IsNull(typeInfo.ItemType);
        Assert.AreEqual(2, typeInfo.GenericArguments.Count);
        Assert.AreEqual("System.String", typeInfo.GenericArguments[0].TypeName);
        Assert.AreEqual("System.Collections.Generic.List", typeInfo.GenericArguments[1].TypeName);

        var nestedGenericInfo = typeInfo.GenericArguments[1].GenericArguments[0];
        Assert.AreEqual("System.Collections.Generic.Dictionary", nestedGenericInfo.TypeName);
        Assert.AreEqual("System.Int32", nestedGenericInfo.GenericArguments[0].TypeName);
        Assert.AreEqual("System.String", nestedGenericInfo.GenericArguments[1].TypeName);
    }
}

// 测试用的类
public class TestClass
{
    public string NonNullableString { get; set; }
    public string? NullableString { get; set; }
}