using System.Globalization;

namespace System.Windows.Forms.CollectionTests;

public class FormatterTests
{
    [Fact]
    public void FormatObject_AssignableType_ReturnsSameValue()
    {
        var value = "test";

        var result = Formatter.FormatObject(
            value,
            typeof(string),
            null,
            null,
            "",
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Same(value, result);
    }

    [Fact]
    public void FormatObject_BoolToCheckState_ConvertsCorrectly()
    {
        var resultTrue = Formatter.FormatObject(
            true,
            typeof(CheckState),
            null,
            null,
            "",
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        var resultFalse = Formatter.FormatObject(
            false,
            typeof(CheckState),
            null,
            null,
            "",
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Equal(CheckState.Checked, resultTrue);
        Assert.Equal(CheckState.Unchecked, resultFalse);
    }

    [Fact]
    public void FormatObject_IFormattable_UsesFormatString()
    {
        var result = Formatter.FormatObject(
            123.456,
            typeof(string),
            null,
            null,
            "F1",
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Equal("123.5", result);
    }

    [Fact]
    public void FormatObject_InvalidConversion_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            Formatter.FormatObject(
                new object(),
                typeof(int),
                null,
                null,
                "",
                CultureInfo.InvariantCulture,
                null,
                DBNull.Value));
    }

    [Fact]
    public void FormatObject_NullToString_ReturnsEmptyString_WhenNoFormattedNull()
    {
        var result = Formatter.FormatObject(
            null,
            typeof(string),
            null,
            null,
            "",
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatObject_NullValue_ReturnsFormattedNullValue()
    {
        var result = Formatter.FormatObject(
            null,
            typeof(string),
            null,
            null,
            "",
            CultureInfo.InvariantCulture,
            "NULL",
            DBNull.Value);

        Assert.Equal("NULL", result);
    }

    [Fact]
    public void GetDefaultDataSourceNullValue_ReferenceType_ReturnsNull()
    {
        var result = Formatter.GetDefaultDataSourceNullValue(typeof(string));
        Assert.Null(result);
    }

    [Fact]
    public void GetDefaultDataSourceNullValue_ValueType_ReturnsDBNull()
    {
        var result = Formatter.GetDefaultDataSourceNullValue(typeof(int));
        Assert.Equal(DBNull.Value, result);
    }

    [Fact]
    public void InvokeStringParseMethod_InvalidParse_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            Formatter.InvokeStringParseMethod("abc", typeof(int), CultureInfo.InvariantCulture));
    }

    [Fact]
    public void InvokeStringParseMethod_ValidInt_ReturnsParsedValue()
    {
        var result = Formatter.InvokeStringParseMethod("42", typeof(int), CultureInfo.InvariantCulture);
        Assert.Equal(42, result);
    }

    [Fact]
    public void IsNullData_CustomNullValue_ReturnsTrue()
    {
        Assert.True(Formatter.IsNullData(0, 0));
    }

    [Fact]
    public void IsNullData_DBNull_ReturnsTrue()
    {
        Assert.True(Formatter.IsNullData(DBNull.Value, DBNull.Value));
    }

    [Fact]
    public void IsNullData_Null_ReturnsTrue()
    {
        Assert.True(Formatter.IsNullData(null, DBNull.Value));
    }

    [Fact]
    public void ParseObject_CheckStateIndeterminate_ReturnsDBNull()
    {
        var result = Formatter.ParseObject(
            CheckState.Indeterminate,
            typeof(bool),
            typeof(CheckState),
            null,
            null,
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Equal(DBNull.Value, result);
    }

    [Fact]
    public void ParseObject_CheckStateToBool_ConvertsCorrectly()
    {
        var result = Formatter.ParseObject(
            CheckState.Checked,
            typeof(bool),
            typeof(CheckState),
            null,
            null,
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

#pragma warning disable CS8605 // Unboxing a possibly null value.
        Assert.True((bool)result);
#pragma warning restore CS8605 // Unboxing a possibly null value.
    }

    [Fact]
    public void ParseObject_FormattedNullValue_ReturnsDBNull()
    {
        var result = Formatter.ParseObject(
            "NULL",
            typeof(int),
            typeof(string),
            null,
            null,
            CultureInfo.InvariantCulture,
            "NULL",
            DBNull.Value);

        Assert.Equal(DBNull.Value, result);
    }

    [Fact]
    public void ParseObject_StringToInt_UsesParse()
    {
        var result = Formatter.ParseObject(
            "123",
            typeof(int),
            typeof(string),
            null,
            null,
            CultureInfo.InvariantCulture,
            null,
            DBNull.Value);

        Assert.Equal(123, result);
    }
}