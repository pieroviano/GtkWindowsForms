#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

extern alias gtk;
using System.ComponentModel;
using System.Globalization;
using Bitmap = gtk::System.Drawing.Bitmap;
using GtkImageConverter = gtk::System.Drawing.GtkImageConverter;

namespace System.Drawing.Drawing2D;

[Collection("NonParallelCollection")]
public class GtkImageConverterTests
{
    private readonly gtk::System.Drawing.GtkImageConverter _converter = new GtkImageConverter();

    // ----------------------------
    // CanConvertFrom
    // ----------------------------

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(byte[]))]
    [InlineData(typeof(int))]
    [InlineData(typeof(object))]
    public void CanConvertFrom_ShouldAlwaysReturnTrue(Type sourceType)
    {
        var result = _converter.CanConvertFrom(null, sourceType);

        Assert.True(result);
    }

    // ----------------------------
    // CanConvertTo
    // ----------------------------

    [Fact]
    public void CanConvertTo_ByteArray_ShouldReturnTrue()
    {
        var result = _converter.CanConvertTo(null, typeof(byte[]));

        Assert.True(result);
    }

    [Fact]
    public void CanConvertTo_String_ShouldReturnTrue()
    {
        var result = _converter.CanConvertTo(null, typeof(string));

        Assert.True(result);
    }

    [Fact]
    public void CanConvertTo_OtherType_ShouldReturnFalse()
    {
        var result = _converter.CanConvertTo(null, typeof(int));

        Assert.False(result);
    }

    // ----------------------------
    // ConvertFrom
    // ----------------------------

    [Fact]
    public void ConvertFrom_ShouldCallBase_AndThrowForUnsupported()
    {
        var input = new object();

        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input));
    }

    // ----------------------------
    // ConvertTo
    // ----------------------------

    [Theory]
    [InlineData("test")]
    [InlineData(123)]
    public void ConvertTo_ShouldReturnSameValue(object input)
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));

        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertTo_NullValue_ShouldReturnNull()
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string));

        Assert.Null(result);
    }

    // ----------------------------
    // GetPropertiesSupported
    // ----------------------------

    [Fact]
    public void GetPropertiesSupported_ShouldReturnTrue()
    {
        var result = _converter.GetPropertiesSupported(null);

        Assert.True(result);
    }

    // ----------------------------
    // GetProperties
    // ----------------------------

    [Fact]
    public void GetProperties_ShouldReturnImageProperties()
    {
        var props = _converter.GetProperties(null, new Bitmap(10, 10), null);

        Assert.NotNull(props);
        Assert.NotEmpty(props);

        // sanity check: Image properties should include Width/Height
        Assert.NotNull(props["Width"]);
        Assert.NotNull(props["Height"]);
    }

    [Fact]
    public void GetProperties_WithAttributes_ShouldRespectAttributes()
    {
        var attributes = new Attribute[] { new BrowsableAttribute(true) };

        var props = _converter.GetProperties(null, new Bitmap(10, 10), attributes);

        Assert.NotNull(props);
    }
}