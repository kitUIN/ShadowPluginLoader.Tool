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
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestString"
                                            },
                                            "TestStringArray": {
                                              "Type": "System.String[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestStringArray"
                                            },
                                            "TestNullableString": {
                                              "Type": "System.String",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableString"
                                            },
                                            "TestInt32": {
                                              "Type": "System.Int32",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt32"
                                            },
                                            "TestInt32Array": {
                                              "Type": "System.Int32[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt32Array"
                                            },
                                            "TestNullableInt32": {
                                              "Type": "System.Int32",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt32"
                                            },
                                            "TestBoolean": {
                                              "Type": "System.Boolean",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestBoolean"
                                            },
                                            "TestNullableBoolean": {
                                              "Type": "System.Boolean",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableBoolean"
                                            },
                                            "TestDouble": {
                                              "Type": "System.Double",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDouble"
                                            },
                                            "TestDoubleArray": {
                                              "Type": "System.Double[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDoubleArray"
                                            },
                                            "TestNullableDouble": {
                                              "Type": "System.Double",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDouble"
                                            },
                                            "TestFloat": {
                                              "Type": "System.Single",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestFloat"
                                            },
                                            "TestFloatArray": {
                                              "Type": "System.Single[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestFloatArray"
                                            },
                                            "TestNullableFloat": {
                                              "Type": "System.Single",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableFloat"
                                            },
                                            "TestInt64": {
                                              "Type": "System.Int64",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt64"
                                            },
                                            "TestInt64Array": {
                                              "Type": "System.Int64[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt64Array"
                                            },
                                            "TestNullableInt64": {
                                              "Type": "System.Int64",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt64"
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
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt16"
                                            },
                                            "TestInt16Array": {
                                              "Type": "System.Int16[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestInt16Array"
                                            },
                                            "TestNullableInt16": {
                                              "Type": "System.Int16",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableInt16"
                                            },
                                            "TestByte": {
                                              "Type": "System.Byte",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestByte"
                                            },
                                            "TestByteArray": {
                                              "Type": "System.Byte[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestByteArray"
                                            },
                                            "TestNullableByte": {
                                              "Type": "System.Byte",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableByte"
                                            },
                                            "TestChar": {
                                              "Type": "System.Char",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestChar"
                                            },
                                            "TestCharArray": {
                                              "Type": "System.Char[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestCharArray"
                                            },
                                            "TestNullableChar": {
                                              "Type": "System.Char",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableChar"
                                            },
                                            "TestDecimal": {
                                              "Type": "System.Decimal",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDecimal"
                                            },
                                            "TestDecimalArray": {
                                              "Type": "System.Decimal[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDecimalArray"
                                            },
                                            "TestNullableDecimal": {
                                              "Type": "System.Decimal",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDecimal"
                                            },
                                            "TestSByte": {
                                              "Type": "System.SByte",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestSByte"
                                            },
                                            "TestSByteArray": {
                                              "Type": "System.SByte[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestSByteArray"
                                            },
                                            "TestNullableSByte": {
                                              "Type": "System.SByte",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableSByte"
                                            },
                                            "TestUInt16": {
                                              "Type": "System.UInt16",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt16"
                                            },
                                            "TestUInt16Array": {
                                              "Type": "System.UInt16[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt16Array"
                                            },
                                            "TestNullableUInt16": {
                                              "Type": "System.UInt16",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt16"
                                            },
                                            "TestUInt32": {
                                              "Type": "System.UInt32",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt32"
                                            },
                                            "TestUInt32Array": {
                                              "Type": "System.UInt32[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt32Array"
                                            },
                                            "TestNullableUInt32": {
                                              "Type": "System.UInt32",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt32"
                                            },
                                            "TestUInt64": {
                                              "Type": "System.UInt64",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt64"
                                            },
                                            "TestUInt64Array": {
                                              "Type": "System.UInt64[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestUInt64Array"
                                            },
                                            "TestNullableUInt64": {
                                              "Type": "System.UInt64",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableUInt64"
                                            }
                                          }
                                        }
                                        """));
    }

    [Test]
    public void Test3()
    {
        var content = ExportMetaMethod.ExportMeta(typeof(TestMetaData3));
        Console.WriteLine(content);
        Assert.That(content, Is.EqualTo("""
                                        {
                                          "Type": "TestProject1.TestMetaData3",
                                          "Properties": {
                                            "TestType": {
                                              "Type": "System.Type",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestType"
                                            },
                                            "TestTypeArray": {
                                              "Type": "System.Type[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestTypeArray"
                                            },
                                            "TestNullableType": {
                                              "Type": "System.Type",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableType"
                                            },
                                            "TestDateTime": {
                                              "Type": "System.DateTime",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTime"
                                            },
                                            "TestDateTimeArray": {
                                              "Type": "System.DateTime[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeArray"
                                            },
                                            "TestNullableDateTime": {
                                              "Type": "System.DateTime",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDateTime"
                                            },
                                            "TestDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset"
                                            },
                                            "TestDateTimeOffsetArray": {
                                              "Type": "System.DateTimeOffset[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffsetArray"
                                            },
                                            "TestNullableDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableDateTimeOffset"
                                            },
                                            "TestTimeSpan": {
                                              "Type": "System.TimeSpan",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestTimeSpan",
                                              "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                            },
                                            "TestTimeSpanArray": {
                                              "Type": "System.TimeSpan[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestTimeSpanArray",
                                              "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                            },
                                            "TestNullableTimeSpan": {
                                              "Type": "System.TimeSpan",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableTimeSpan",
                                              "Regex": "^(\\d\\.)?(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9](\\.\\d{1,7})?$"
                                            },
                                            "TestGuid": {
                                              "Type": "System.Guid",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestGuid",
                                              "Regex": "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"
                                            },
                                            "TestGuidArray": {
                                              "Type": "System.Guid[]",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestGuidArray",
                                              "Regex": "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"
                                            },
                                            "TestNullableGuid": {
                                              "Type": "System.Guid",
                                              "Nullable": true,
                                              "Required": true,
                                              "PropertyGroupName": "TestNullableGuid",
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
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTime",
                                              "DateTime": {
                                                "Format": "yyyy-MM-dd HH:mm:ss",
                                                "InvariantCulture": false
                                              }
                                            },
                                            "TestDateTimeOffset": {
                                              "Type": "System.DateTimeOffset",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset",
                                              "DateTime": {
                                                "Format": "yyyy-MM-dd HH:mm:ss",
                                                "InvariantCulture": true
                                              }
                                            },
                                            "TestDateTimeOffset2": {
                                              "Type": "System.DateTimeOffset",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestDateTimeOffset2"
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
                                              "Type": "System.String",
                                              "Nullable": false,
                                              "Required": true,
                                              "PropertyGroupName": "TestListString",
                                              "GenericType": "System.Collections.Generic.List"
                                            }
                                          }
                                        }
                                        """));
    }
}