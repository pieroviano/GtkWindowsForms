extern alias gtk;
using Font = gtk::System.Drawing.Font;
using FontFamily = gtk::System.Drawing.FontFamily;
using FontStyle = gtk::System.Drawing.FontStyle;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="Font"/>: constructor values and style flags round-trip.</summary>
[Collection("NonParallelCollection")]
public class FontFunctionalTests
{
    [Fact]
    public void Constructor_NameAndSize_RoundTrip()
    {
        using var font = new Font("Sans", 12f);

        Assert.Equal("Sans", font.Name);
        Assert.Equal(12f, font.Size, 3);
        Assert.Equal(FontStyle.Regular, font.Style);
    }

    [Fact]
    public void StyleFlags_AreReportedIndividually()
    {
        using var font = new Font("Sans", 10f, FontStyle.Bold | FontStyle.Italic);

        Assert.True(font.Bold, "Bold flag should be set");
        Assert.True(font.Italic, "Italic flag should be set");
        Assert.False(font.Underline, "Underline was not requested");
        Assert.False(font.Strikeout, "Strikeout was not requested");
    }

    [Fact]
    public void UnderlineAndStrikeout_AreReportedIndividually()
    {
        using var font = new Font("Sans", 10f, FontStyle.Underline | FontStyle.Strikeout);

        Assert.True(font.Underline);
        Assert.True(font.Strikeout);
        Assert.False(font.Bold);
        Assert.False(font.Italic);
    }

    [Fact]
    public void PrototypeConstructor_KeepsFamilyAndSize_ButAppliesNewStyle()
    {
        using var original = new Font("Sans", 14f, FontStyle.Regular);
        using var bold = new Font(original, FontStyle.Bold);

        Assert.Equal(original.Name, bold.Name);
        Assert.Equal(original.Size, bold.Size, 3);
        Assert.True(bold.Bold);
    }

    [Fact]
    public void Unit_RoundTrips_FromConstructor()
    {
        using var font = new Font("Sans", 10f, FontStyle.Regular, GraphicsUnit.Pixel);
        Assert.Equal(GraphicsUnit.Pixel, font.Unit);
    }

    [Fact]
    public void Height_IsPositive_ForARealFont()
    {
        using var font = new Font(new FontFamily("Sans"), 12f);
        Assert.True(font.Height > 0, $"font height should be positive but was {font.Height}");
    }
}
