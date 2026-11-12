extern alias gtk;

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
// Bind the types under test through the `gtk` alias (the test project references Gtk.Drawing with
// Aliases="gtk"), so these resolve to Gtk.Drawing's own types rather than any ambient System.Drawing.
using Font = gtk::System.Drawing.Font;
using FontConverter = gtk::System.Drawing.FontConverter;
using FontStyle = gtk::System.Drawing.FontStyle;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Drawing.Tests.SystemDrawing;

[Collection("NonParallelCollection")]
public class FontConverterTests
{
    private readonly FontConverter _sut = new();

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        Assert.True(_sut.CanConvertFrom(null, typeof(string)));
    }

    [Fact]
    public void CanConvertTo_StringAndInstanceDescriptor_ReturnsTrue()
    {
        Assert.True(_sut.CanConvertTo(null, typeof(string)));
        Assert.True(_sut.CanConvertTo(null, typeof(InstanceDescriptor)));
    }

    // The regression: this exact resource string used to throw
    // "TypeConverter cannot convert from System.String." because Gtk.Drawing's Font had no FontConverter.
    [Fact]
    public void ConvertFrom_ResxFontString_ParsesNameSizeUnitAndStyle()
    {
        var font = (Font)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "Microsoft YaHei UI, 8.142858pt, style=Italic");

        Assert.Equal("Microsoft YaHei UI", font.Name);
        Assert.Equal(8.142858f, font.Size, 3);
        Assert.Equal(GraphicsUnit.Point, font.Unit);
        Assert.Equal(FontStyle.Italic, font.Style);
    }

    [Fact]
    public void ConvertFrom_PixelUnitAndMultipleStyles_Parsed()
    {
        var font = (Font)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "Arial, 12px, style=Bold, Italic");

        Assert.Equal("Arial", font.Name);
        Assert.Equal(12f, font.Size, 3);
        Assert.Equal(GraphicsUnit.Pixel, font.Unit);
        Assert.Equal(FontStyle.Bold | FontStyle.Italic, font.Style);
    }

    [Fact]
    public void ConvertFrom_NameAndSizeOnly_DefaultsToRegular()
    {
        var font = (Font)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, "Arial, 9pt");

        Assert.Equal("Arial", font.Name);
        Assert.Equal(9f, font.Size, 3);
        Assert.Equal(FontStyle.Regular, font.Style);
    }

    [Fact]
    public void ConvertTo_String_ProducesRoundTrippableForm()
    {
        var original = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Point);

        var text = (string)_sut.ConvertTo(null, CultureInfo.InvariantCulture, original, typeof(string));

        Assert.Contains("Arial", text);
        Assert.Contains("pt", text);
        Assert.Contains("style=Bold", text);

        var roundTripped = (Font)_sut.ConvertFrom(null, CultureInfo.InvariantCulture, text);
        Assert.Equal(original.Name, roundTripped.Name);
        Assert.Equal(original.Size, roundTripped.Size, 3);
        Assert.Equal(original.Style, roundTripped.Style);
        Assert.Equal(original.Unit, roundTripped.Unit);
    }

    [Fact]
    public void ConvertTo_InstanceDescriptor_ReconstructsFont()
    {
        var original = new Font("Arial", 10f, FontStyle.Italic, GraphicsUnit.Point);

        var descriptor = (InstanceDescriptor)_sut.ConvertTo(null, CultureInfo.InvariantCulture, original, typeof(InstanceDescriptor));
        var rebuilt = (Font)descriptor.Invoke();

        Assert.Equal(original.Name, rebuilt.Name);
        Assert.Equal(original.Size, rebuilt.Size, 3);
        Assert.Equal(original.Style, rebuilt.Style);
        Assert.Equal(original.Unit, rebuilt.Unit);
    }

    // The TypeConverter resolves through the [TypeConverter] attribute on Font, which is the path the
    // resource deserializer uses (TypeDescriptor.GetConverter(type).ConvertFromInvariantString(...)).
    [Fact]
    public void TypeDescriptor_GetConverter_OnFont_IsFontConverter_AndConvertsFromString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Font));

        Assert.IsType<FontConverter>(converter);
        Assert.True(converter.CanConvertFrom(typeof(string)));

        var font = (Font)converter.ConvertFromInvariantString("Arial, 9pt, style=Bold");
        Assert.Equal("Arial", font.Name);
    }
}
