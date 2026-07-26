extern alias gtk;
using System.Globalization;
using ColorConverter = gtk::System.Drawing.ColorConverter;

namespace System.Drawing;

/// <summary>Conversion behaviour of <see cref="ColorConverter"/>: named colours, hex and comma-separated
/// component strings must round-trip to the colours they denote.</summary>
[Collection("NonParallelCollection")]
public class ColorConverterFunctionalTests
{
    private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

    private static Color Convert(string text) =>
        (Color)new ColorConverter().ConvertFrom(null!, Ci, text)!;

    [Fact]
    public void ConvertFrom_NamedColour_ReturnsThatColour()
    {
        Assert.Equal(Color.Red.ToArgb(), Convert("Red").ToArgb());
        Assert.Equal(Color.CornflowerBlue.ToArgb(), Convert("CornflowerBlue").ToArgb());
    }

    [Fact]
    public void ConvertFrom_TrimsSurroundingWhitespace()
    {
        Assert.Equal(Color.Green.ToArgb(), Convert("  Green  ").ToArgb());
    }

    [Fact]
    public void ConvertFrom_HexString_ReturnsOpaqueColour()
    {
        var c = Convert("#FF8040");

        Assert.Equal(255, c.R);
        Assert.Equal(128, c.G);
        Assert.Equal(64, c.B);
        Assert.Equal(255, c.A);
    }

    [Fact]
    public void ConvertFrom_ThreeComponents_AreRedGreenBlue()
    {
        var c = Convert("128,64,32");

        Assert.Equal(128, c.R);
        Assert.Equal(64, c.G);
        Assert.Equal(32, c.B);
    }

    [Fact]
    public void ConvertFrom_FourComponents_AreAlphaRedGreenBlue()
    {
        var c = Convert("200,128,64,32");

        Assert.Equal(200, c.A);
        Assert.Equal(128, c.R);
        Assert.Equal(64, c.G);
        Assert.Equal(32, c.B);
    }

    [Fact]
    public void ConvertFrom_EmptyString_IsColorEmpty()
    {
        var c = Convert(string.Empty);
        Assert.True(c.IsEmpty, $"an empty string should convert to Color.Empty but gave {c}");
    }

    [Fact]
    public void ConvertTo_NamedColour_ProducesItsName()
    {
        var text = (string)new ColorConverter().ConvertTo(null!, Ci, Color.Red, typeof(string))!;
        Assert.Contains("Red", text);
    }

    [Fact]
    public void ConvertTo_UnnamedOpaqueColour_ProducesItsComponents()
    {
        var text = (string)new ColorConverter().ConvertTo(null!, Ci, Color.FromArgb(1, 2, 3), typeof(string))!;

        Assert.Contains("1", text);
        Assert.Contains("2", text);
        Assert.Contains("3", text);
    }

    [Fact]
    public void CanConvert_ReportsStringSupport()
    {
        var cc = new ColorConverter();

        Assert.True(cc.CanConvertFrom(null!, typeof(string)));
        Assert.True(cc.CanConvertTo(null!, typeof(string)));
    }

    [Fact]
    public void GetStandardValues_ExposesTheKnownColours()
    {
        var cc = new ColorConverter();

        Assert.True(cc.GetStandardValuesSupported(null!));
        var values = cc.GetStandardValues(null!);
        Assert.NotNull(values);
        Assert.True(values!.Count > 0, "the converter should expose the known colours as standard values");
    }
}
