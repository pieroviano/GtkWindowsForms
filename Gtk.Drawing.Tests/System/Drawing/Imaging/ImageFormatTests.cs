extern alias gtk;

using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;

namespace System.Drawing.Imaging;

public class ImageFormatTests
{
    [Fact]
    public void Ctor_SetsGuidCorrectly()
    {
        var guid = Guid.NewGuid();

        var format = new ImageFormat(guid);

        Assert.Equal(guid, format.Guid);
    }

    [Fact]
    public void StaticProperties_ReturnSameInstances()
    {
        Assert.Same(ImageFormat.Bmp, ImageFormat.Bmp);
        Assert.Same(ImageFormat.Jpeg, ImageFormat.Jpeg);
        Assert.Same(ImageFormat.Png, ImageFormat.Png);
    }

    [Fact]
    public void StaticFormats_HaveExpectedGuids()
    {
        Assert.Equal(new Guid("{b96b3cab-0728-11d3-9d7b-0000f81ef32e}"), ImageFormat.Bmp.Guid);
        Assert.Equal(new Guid("{b96b3cae-0728-11d3-9d7b-0000f81ef32e}"), ImageFormat.Jpeg.Guid);
        Assert.Equal(new Guid("{b96b3caf-0728-11d3-9d7b-0000f81ef32e}"), ImageFormat.Png.Guid);
    }

    [Fact]
    public void Equals_ReturnsTrue_ForSameGuid()
    {
        var guid = Guid.NewGuid();

        var f1 = new ImageFormat(guid);
        var f2 = new ImageFormat(guid);

        Assert.True(f1.Equals(f2));
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentGuid()
    {
        var f1 = new ImageFormat(Guid.NewGuid());
        var f2 = new ImageFormat(Guid.NewGuid());

        Assert.False(f1.Equals(f2));
    }

    [Fact]
    public void Equals_ReturnsFalse_ForNull()
    {
        var f = new ImageFormat(Guid.NewGuid());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.False(f.Equals(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void GetHashCode_MatchesGuid()
    {
        var guid = Guid.NewGuid();
        var f = new ImageFormat(guid);

        Assert.Equal(guid.GetHashCode(), f.GetHashCode());
    }

    // -------------------------
    // ToString() tests
    // -------------------------

    [Theory]
    [InlineData("Bmp")]
    [InlineData("Jpeg")]
    [InlineData("Png")]
    [InlineData("Gif")]
    [InlineData("Tiff")]
    public void ToString_ReturnsExpectedNames(string expected)
    {
        ImageFormat format = expected switch
        {
            "Bmp" => ImageFormat.Bmp,
            "Jpeg" => ImageFormat.Jpeg,
            "Png" => ImageFormat.Png,
            "Gif" => ImageFormat.Gif,
            "Tiff" => ImageFormat.Tiff,
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expected, format.ToString());
    }

    [Fact]
    public void ToString_ReturnsMemoryBmp()
    {
        Assert.Equal("MemoryBMP", ImageFormat.MemoryBmp.ToString());
    }

    [Fact]
    public void ToString_ReturnsCustomFormat_WhenUnknown()
    {
        var guid = Guid.NewGuid();
        var format = new ImageFormat(guid);

        var result = format.ToString();

        Assert.Contains(guid.ToString(), result);
        Assert.StartsWith("[ImageFormat:", result);
    }

    // -------------------------
    // FindEncoder / MIME tests
    // -------------------------

    [Theory]
    [InlineData("Bmp", ".bmp")]
    [InlineData("Jpeg", ".jpeg")]
    [InlineData("Png", ".png")]
    [InlineData("Gif", ".gif")]
    public void FindEncoder_ReturnsExpectedMimeType(string formatName, string expectedMime)
    {
        ImageFormat format = formatName switch
        {
            "Bmp" => ImageFormat.Bmp,
            "Jpeg" => ImageFormat.Jpeg,
            "Png" => ImageFormat.Png,
            "Gif" => ImageFormat.Gif,
            _ => throw new InvalidOperationException()
        };

        var encoder = format.FindEncoder();

        Assert.NotNull(encoder);
        Assert.Equal(format.Guid, encoder.FormatID);
        Assert.Equal(expectedMime, encoder.MimeType);
    }

    [Fact]
    public void FindEncoder_UnknownFormat_DefaultsToBmpMime()
    {
        var format = new ImageFormat(Guid.NewGuid());

        var encoder = format.FindEncoder();

        Assert.Equal(".bmp", encoder.MimeType);
    }
}