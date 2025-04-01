using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.Tool;

namespace TestProject1;

[ExportMeta]
public class TestMetaData1
{
    public string TestString { get; init; }
    public string[] TestStringArray { get; init; }
    public string? TestNullableString { get; init; }
    public int TestInt32 { get; init; }
    public int[] TestInt32Array { get; init; }
    public int? TestNullableInt32 { get; init; }
    public bool TestBoolean { get; init; }
    public bool? TestNullableBoolean { get; init; }
    public double TestDouble { get; init; }
    public double[] TestDoubleArray { get; init; }
    public double? TestNullableDouble { get; init; }
    public float TestFloat { get; init; }
    public float[] TestFloatArray { get; init; }
    public float? TestNullableFloat { get; init; }
    public long TestInt64 { get; init; }
    public long[] TestInt64Array { get; init; }
    public long? TestNullableInt64 { get; init; }
}

[ExportMeta]
public class TestMetaData2
{
    public short TestInt16 { get; init; }
    public short[] TestInt16Array { get; init; }
    public short? TestNullableInt16 { get; init; }
    public byte TestByte { get; init; }
    public byte[] TestByteArray { get; init; }
    public byte? TestNullableByte { get; init; }
    public char TestChar { get; init; }
    public char[] TestCharArray { get; init; }
    public char? TestNullableChar { get; init; }
    public decimal TestDecimal { get; init; }
    public decimal[] TestDecimalArray { get; init; }
    public decimal? TestNullableDecimal { get; init; }
    public sbyte TestSByte { get; init; }
    public sbyte[] TestSByteArray { get; init; }
    public sbyte? TestNullableSByte { get; init; }
    public ushort TestUInt16 { get; init; }
    public ushort[] TestUInt16Array { get; init; }
    public ushort? TestNullableUInt16 { get; init; }
    public uint TestUInt32 { get; init; }
    public uint[] TestUInt32Array { get; init; }
    public uint? TestNullableUInt32 { get; init; }
    public ulong TestUInt64 { get; init; }
    public ulong[] TestUInt64Array { get; init; }
    public ulong? TestNullableUInt64 { get; init; }
}

[ExportMeta]
public class TestMetaData3
{
    public Type TestType { get; init; }
    public Type[] TestTypeArray { get; init; }
    public Type? TestNullableType { get; init; }

    public DateTime TestDateTime { get; init; }
    public DateTime[] TestDateTimeArray { get; init; }
    public DateTime? TestNullableDateTime { get; init; }

    public DateTimeOffset TestDateTimeOffset { get; init; }
    public DateTimeOffset[] TestDateTimeOffsetArray { get; init; }
    public DateTimeOffset? TestNullableDateTimeOffset { get; init; }

    public TimeSpan TestTimeSpan { get; init; }
    public TimeSpan[] TestTimeSpanArray { get; init; }
    public TimeSpan? TestNullableTimeSpan { get; init; }

    public Guid TestGuid { get; init; }
    public Guid[] TestGuidArray { get; init; }
    public Guid? TestNullableGuid { get; init; }
}

[ExportMeta]
public class TestMetaData4
{
    [MetaDateTime(Format = "yyyy-MM-dd HH:mm:ss", InvariantCulture = false)]
    public DateTime TestDateTime { get; init; }

    [MetaDateTime(Format = "yyyy-MM-dd HH:mm:ss")]
    public DateTimeOffset TestDateTimeOffset { get; init; }

    [MetaDateTime] public DateTimeOffset TestDateTimeOffset2 { get; init; }
}

[ExportMeta]
public class TestMetaData5
{
    public List<string> TestListString { get; init; } = [];
}
[ExportMeta]
public class TestMetaData6
{
  public TestGe<string,int> TestListString { get; init; }
}

public class ExportMetaTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData1));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData1",
                                          "Properties": {
                                            "TestString": {
                                              "Type": "System.String",
                                              "Required": true,
                                              "PropertyGroupName": "TestString",
                                              "Nullable": false
                                            },
                                            "TestStringArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestStringArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.String",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableString": {
                                              "Type": "System.String",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableString",
                                              "Nullable": true
                                            },
                                            "TestInt32": {
                                              "Type": "System.Int32",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt32",
                                              "Nullable": false
                                            },
                                            "TestInt32Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt32Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Int32",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableInt32": {
                                              "Type": "System.Int32",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt32",
                                              "Nullable": true
                                            },
                                            "TestBoolean": {
                                              "Type": "System.Boolean",
                                              "Required": true,
                                              "PropertyGroupName": "TestBoolean",
                                              "Nullable": false
                                            },
                                            "TestNullableBoolean": {
                                              "Type": "System.Boolean",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableBoolean",
                                              "Nullable": true
                                            },
                                            "TestDouble": {
                                              "Type": "System.Double",
                                              "Required": true,
                                              "PropertyGroupName": "TestDouble",
                                              "Nullable": false
                                            },
                                            "TestDoubleArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestDoubleArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Double",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableDouble": {
                                              "Type": "System.Double",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDouble",
                                              "Nullable": true
                                            },
                                            "TestFloat": {
                                              "Type": "System.Single",
                                              "Required": true,
                                              "PropertyGroupName": "TestFloat",
                                              "Nullable": false
                                            },
                                            "TestFloatArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestFloatArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Single",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableFloat": {
                                              "Type": "System.Single",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableFloat",
                                              "Nullable": true
                                            },
                                            "TestInt64": {
                                              "Type": "System.Int64",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt64",
                                              "Nullable": false
                                            },
                                            "TestInt64Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt64Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Int64",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableInt64": {
                                              "Type": "System.Int64",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt64",
                                              "Nullable": true
                                            }
                                          }
                                        }
                                        """));
    }

    [Test]
    public void Test2()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData2));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData2",
                                          "Properties": {
                                            "TestInt16": {
                                              "Type": "System.Int16",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt16",
                                              "Nullable": false
                                            },
                                            "TestInt16Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestInt16Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Int16",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableInt16": {
                                              "Type": "System.Int16",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt16",
                                              "Nullable": true
                                            },
                                            "TestByte": {
                                              "Type": "System.Byte",
                                              "Required": true,
                                              "PropertyGroupName": "TestByte",
                                              "Nullable": false
                                            },
                                            "TestByteArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestByteArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Byte",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableByte": {
                                              "Type": "System.Byte",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableByte",
                                              "Nullable": true
                                            },
                                            "TestChar": {
                                              "Type": "System.Char",
                                              "Required": true,
                                              "PropertyGroupName": "TestChar",
                                              "Nullable": false
                                            },
                                            "TestCharArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestCharArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Char",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableChar": {
                                              "Type": "System.Char",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableChar",
                                              "Nullable": true
                                            },
                                            "TestDecimal": {
                                              "Type": "System.Decimal",
                                              "Required": true,
                                              "PropertyGroupName": "TestDecimal",
                                              "Nullable": false
                                            },
                                            "TestDecimalArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestDecimalArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Decimal",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableDecimal": {
                                              "Type": "System.Decimal",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDecimal",
                                              "Nullable": true
                                            },
                                            "TestSByte": {
                                              "Type": "System.SByte",
                                              "Required": true,
                                              "PropertyGroupName": "TestSByte",
                                              "Nullable": false
                                            },
                                            "TestSByteArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestSByteArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.SByte",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableSByte": {
                                              "Type": "System.SByte",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableSByte",
                                              "Nullable": true
                                            },
                                            "TestUInt16": {
                                              "Type": "System.UInt16",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt16",
                                              "Nullable": false
                                            },
                                            "TestUInt16Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt16Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.UInt16",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableUInt16": {
                                              "Type": "System.UInt16",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt16",
                                              "Nullable": true
                                            },
                                            "TestUInt32": {
                                              "Type": "System.UInt32",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt32",
                                              "Nullable": false
                                            },
                                            "TestUInt32Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt32Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.UInt32",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableUInt32": {
                                              "Type": "System.UInt32",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt32",
                                              "Nullable": true
                                            },
                                            "TestUInt64": {
                                              "Type": "System.UInt64",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt64",
                                              "Nullable": false
                                            },
                                            "TestUInt64Array": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt64Array",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.UInt64",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableUInt64": {
                                              "Type": "System.UInt64",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt64",
                                              "Nullable": true
                                            }
                                          }
                                        }
                                        """));
    }

    [Test]
    public void Test3()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData3));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData3",
                                          "Properties": {
                                            "TestType": {
                                              "Type": "System.Type",
                                              "Required": true,
                                              "PropertyGroupName": "TestType",
                                              "Nullable": false
                                            },
                                            "TestTypeArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestTypeArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Type",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableType": {
                                              "Type": "System.Type",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableType",
                                              "Nullable": true
                                            },
                                            "TestDateTime": {
                                              "Type": "System.DateTime",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTime",
                                              "Nullable": false
                                            },
                                            "TestDateTimeArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.DateTime",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableDateTime": {
                                              "Type": "System.DateTime",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDateTime",
                                              "Nullable": true
                                            },
                                            "TestDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset",
                                              "Nullable": false
                                            },
                                            "TestDateTimeOffsetArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffsetArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.DateTimeOffset",
                                                "Nullable": false
                                              }
                                            },
                                            "TestNullableDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDateTimeOffset",
                                              "Nullable": true
                                            },
                                            "TestTimeSpan": {
                                              "Type": "System.TimeSpan",
                                              "Required": true,
                                              "PropertyGroupName": "TestTimeSpan",
                                              "Nullable": false,
                                              "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                            },
                                            "TestTimeSpanArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestTimeSpanArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.TimeSpan",
                                                "Nullable": false,
                                                "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                              }
                                            },
                                            "TestNullableTimeSpan": {
                                              "Type": "System.TimeSpan",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableTimeSpan",
                                              "Nullable": true,
                                              "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                            },
                                            "TestGuid": {
                                              "Type": "System.Guid",
                                              "Required": true,
                                              "PropertyGroupName": "TestGuid",
                                              "Nullable": false,
                                              "Regex": "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"
                                            },
                                            "TestGuidArray": {
                                              "Type": "System.Array",
                                              "Required": true,
                                              "PropertyGroupName": "TestGuidArray",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.Guid",
                                                "Nullable": false,
                                                "Regex": "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"
                                              }
                                            },
                                            "TestNullableGuid": {
                                              "Type": "System.Guid",
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableGuid",
                                              "Nullable": true,
                                              "Regex": "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"
                                            }
                                          }
                                        }
                                        """));
    }

    [Test]
    public void Test4()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData4));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData4",
                                          "Properties": {
                                            "TestDateTime": {
                                              "Type": "System.DateTime",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTime",
                                              "Nullable": false,
                                              "DateTime": {
                                                "Format": "yyyy-MM-dd HH:mm:ss",
                                                "InvariantCulture": false
                                              }
                                            },
                                            "TestDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset",
                                              "Nullable": false,
                                              "DateTime": {
                                                "Format": "yyyy-MM-dd HH:mm:ss",
                                                "InvariantCulture": true
                                              }
                                            },
                                            "TestDateTimeOffset2": {
                                              "Type": "System.DateTimeOffset",
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset2",
                                              "Nullable": false
                                            }
                                          }
                                        }
                                        """));
    }

    [Test]
    public void Test5()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData5));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData5",
                                          "Properties": {
                                            "TestListString": {
                                              "Type": "System.Collections.Generic.List",
                                              "Required": true,
                                              "PropertyGroupName": "TestListString",
                                              "Nullable": false,
                                              "Item": {
                                                "Type": "System.String",
                                                "Nullable": false
                                              }
                                            }
                                          }
                                        }
                                        """));
    }
    [Test]
    public void Test6()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData6));
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData6",
                                          "Properties": {
                                            "TestListString": {
                                              "Type": "TestProject1.TestGe",
                                              "Required": true,
                                              "PropertyGroupName": "TestListString",
                                              "Nullable": false,
                                              "GenericType": [
                                                "System.String",
                                                "System.Int32"
                                              ],
                                              "Properties": {}
                                            }
                                          }
                                        }
                                        """));
    }
}