extern alias gtk;

using System.ComponentModel.Design.Serialization;
using System.Globalization;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class ColorConverterTests
{
    private readonly ColorConverter _sut = new();

    // ----------------------------
    // CanConvertFrom / To
    // ----------------------------

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        var result = _sut.CanConvertFrom(null, typeof(string));

        Assert.True(result);
    }

    [Fact]
    public void CanConvertTo_InstanceDescriptor_ReturnsTrue()
    {
        var result = _sut.CanConvertTo(null, typeof(InstanceDescriptor));

        Assert.True(result);
    }

    [Fact]
    public void ConvertFrom_ArgbCommaSeparated_ReturnsCorrectColor()
    {
        var result = (Color)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "255, 0, 0");
        var color = Color.FromArgb(255, 0, 0);

        Assert.Equal(color.ToArgb(), result.ToArgb());
    }

    // ----------------------------
    // ConvertFrom
    // ----------------------------

    [Fact]
    public void ConvertFrom_EmptyString_ReturnsColorEmpty()
    {
        var result = (Color)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "");

        Assert.Equal(Color.Empty.ToArgb(), result.ToArgb());
    }

    [Fact]
    public void ConvertFrom_HexColor_ReturnsCorrectColor()
    {
        var result = (Color)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "#FF0000");

        Assert.Equal(Color.FromArgb(255, 255, 0, 0).ToArgb(), result.ToArgb());
    }

    [Fact]
    public void ConvertFrom_InvalidColor_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            return _sut.ConvertFrom(null, CultureInfo.InvariantCulture, "not-a-color");
        });
    }

    [Fact]
    public void ConvertFrom_NamedColor_ReturnsCorrectColor()
    {
        var result = (Color)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "Red");

        Assert.Equal(Color.Red.ToArgb(), result.ToArgb());
    }

    [Fact]
    public void ConvertFrom_QuotedColorName_ReturnsCorrectColor()
    {
        var result = (Color)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "'Blue'");

        Assert.Equal(Color.Blue.ToArgb(), result.ToArgb());
    }

    [Fact]
    public void ConvertTo_InstanceDescriptor_ArgbColor_ReturnsDescriptor()
    {
        var color = Color.FromArgb(1, 2, 3);

        var result =
            (InstanceDescriptor)_sut.ConvertTo(null, CultureInfo.InvariantCulture, color, typeof(InstanceDescriptor));

        Assert.NotNull(result);
    }

    // ----------------------------
    // ConvertTo (InstanceDescriptor)
    // ----------------------------

    [Fact]
    public void ConvertTo_InstanceDescriptor_NamedColor_ReturnsDescriptor()
    {
        var result = (InstanceDescriptor)_sut.ConvertTo(null, CultureInfo.InvariantCulture, Color.Red,
            typeof(InstanceDescriptor));

        Assert.NotNull(result);
        Assert.NotNull(result.MemberInfo);
    }

    [Fact]
    public void ConvertTo_String_ArgbColor_ReturnsCommaSeparated()
    {
        var color = Color.FromArgb(10, 20, 30);

        var result = (string)_sut.ConvertTo(null, CultureInfo.InvariantCulture, color, typeof(string));

        Assert.Contains("10", result);
        Assert.Contains("20", result);
        Assert.Contains("30", result);
    }

    // ----------------------------
    // ConvertTo (string)
    // ----------------------------

    [Fact]
    public void ConvertTo_String_EmptyColor_ReturnsEmptyString()
    {
        var result = (string)_sut.ConvertTo(null, CultureInfo.InvariantCulture, Color.Empty, typeof(string));

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConvertTo_String_NamedColor_ReturnsQuotedName()
    {
        var result = (string)_sut.ConvertTo(null, CultureInfo.InvariantCulture, Color.Red, typeof(string));

        Assert.Equal("Red", result);
    }

    // ----------------------------
    // Standard values
    // ----------------------------

    [Fact]
    public void GetStandardValues_ReturnsValues()
    {
        var values = _sut.GetStandardValues(null);

        Assert.NotNull(values);
        Assert.NotEmpty(values);
    }

    [Fact]
    public void GetStandardValuesSupported_ReturnsTrue()
    {
        var result = _sut.GetStandardValuesSupported(null);

        Assert.True(result);
    }
}