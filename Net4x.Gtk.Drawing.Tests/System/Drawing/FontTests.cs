extern alias gtk;

using Font = gtk::System.Drawing.Font;
using FontFamily = gtk::System.Drawing.FontFamily;
using FontStyle = gtk::System.Drawing.FontStyle;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class FontTests
{
    [Fact]
    public void Clone_ReturnsObject()
    {
        var font = new Font("Arial", 12f);
        var clone = font.Clone();
        Assert.NotNull(clone);
    }

    [Fact]
    public void Constructor_WithFamilyAndSize_SetsProperties()
    {
        var family = new FontFamily("Arial");
        var font = new Font(family, 12f);
        Assert.Equal(family, font.FontFamily);
        Assert.Equal(12f, font.Size);
        Assert.Equal(FontStyle.Regular, font.Style);
    }

    [Fact]
    public void Constructor_WithFamilyAndSizeAndStyle_SetsProperties()
    {
        var family = new FontFamily("Arial");
        var font = new Font(family, 10f, FontStyle.Bold);
        Assert.Equal(family, font.FontFamily);
        Assert.Equal(10f, font.Size);
        Assert.Equal(FontStyle.Bold, font.Style);
        Assert.True(font.Bold);
    }

    [Fact]
    public void Constructor_WithFamilyNameAndSize_SetsProperties()
    {
        var font = new Font("Arial", 14f);
        Assert.Equal("Arial", font.FontFamily.Name);
        Assert.Equal(14f, font.Size);
        Assert.Equal(FontStyle.Regular, font.Style);
    }

    [Fact]
    public void Constructor_WithFamilyNameSizeStyle_SetsProperties()
    {
        var font = new Font("Arial", 16f, FontStyle.Italic);
        Assert.Equal("Arial", font.FontFamily.Name);
        Assert.Equal(16f, font.Size);
        Assert.Equal(FontStyle.Italic, font.Style);
        Assert.True(font.Italic);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var font = new Font("Arial", 12f);
        font.Dispose();
    }

    [Fact]
    public void Equals_DifferentProperties_ReturnsFalse()
    {
        var font1 = new Font("Arial", 12f);
        var font2 = new Font("Arial", 14f);
        Assert.False(font1.Equals(font2));
    }

    [Fact]
    public void Equals_SameProperties_ReturnsTrue()
    {
        var font1 = new Font("Arial", 12f);
        var font2 = new Font("Arial", 12f);
        Assert.True(font1.Equals(font2));
    }

    [Fact]
    public void FromHdc_ReturnsFont()
    {
        var font = Font.FromHdc(IntPtr.Zero);
        Assert.NotNull(font);
    }

    [Fact]
    public void FromHfont_ReturnsFont()
    {
        var font = Font.FromHfont(IntPtr.Zero);
        Assert.NotNull(font);
    }

    [Fact]
    public void FromLogFont_ReturnsFont()
    {
        var font = Font.FromLogFont(new object());
        Assert.NotNull(font);
    }

    [Fact]
    public void FromLogFont_WithHdc_ReturnsFont()
    {
        var font = Font.FromLogFont(new object(), IntPtr.Zero);
        Assert.NotNull(font);
    }

    [Fact]
    public void GetHashCode_ReturnsInt()
    {
        var font = new Font("Arial", 12f);
        var hash = font.GetHashCode();
        Assert.IsType<int>(hash);
    }

    [Fact]
    public void ToString_ReturnsString()
    {
        var font = new Font("Arial", 12f);
        var str = font.ToString();
        Assert.IsType<string>(str);
    }
}