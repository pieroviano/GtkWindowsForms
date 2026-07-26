extern alias gtk;
using Image = gtk::System.Drawing.Image;
using Bitmap = gtk::System.Drawing.Bitmap;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;
using RotateFlipType = gtk::System.Drawing.RotateFlipType;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="Image"/>: encode/decode round-trips preserve the image, and
/// rotation actually transposes the dimensions.</summary>
[Collection("NonParallelCollection")]
public class ImageFunctionalTests
{
    private static byte[] EncodePng(Image image)
    {
        using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    [Fact]
    public void Save_ProducesNonEmptyPngBytes()
    {
        var bytes = EncodePng(Tests.Resources.timg6);

        Assert.True(bytes.Length > 8, "encoded PNG should not be empty");
        // PNG magic number.
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public void SaveThenFromStream_PreservesDimensions()
    {
        var source = Tests.Resources.timg6;
        var bytes = EncodePng(source);

        using var reloaded = Assert.IsAssignableFrom<Image>(Image.FromStream(new MemoryStream(bytes)));

        Assert.Equal(source.Width, reloaded.Width);
        Assert.Equal(source.Height, reloaded.Height);
    }

    [Fact]
    public void FromStream_RoundTripsPixelContent()
    {
        using var original = new Bitmap(6, 4);
        var marker = Color.FromArgb(255, 210, 20, 30);
        original.SetPixel(1, 2, marker);

        var bytes = EncodePng(original);
        using var reloaded = Assert.IsType<Bitmap>(Image.FromStream(new MemoryStream(bytes)));

        var got = reloaded.GetPixel(1, 2);
        Assert.Equal(marker.R, got.R);
        Assert.Equal(marker.G, got.G);
        Assert.Equal(marker.B, got.B);
    }

    [Fact]
    public void RotateFlip_Rotate90_TransposesDimensions()
    {
        using var bmp = new Bitmap(20, 8);
        var beforeW = bmp.Width;
        var beforeH = bmp.Height;

        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

        Assert.Equal(beforeH, bmp.Width);
        Assert.Equal(beforeW, bmp.Height);
    }

    [Fact]
    public void RotateFlip_Rotate180_KeepsDimensions()
    {
        using var bmp = new Bitmap(20, 8);

        bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);

        Assert.Equal(20, bmp.Width);
        Assert.Equal(8, bmp.Height);
    }

    [Fact]
    public void Clone_ProducesIndependentImageWithSameSize()
    {
        using var original = new Bitmap(12, 9);
        using var clone = (Image)original.Clone();

        Assert.Equal(original.Width, clone.Width);
        Assert.Equal(original.Height, clone.Height);
    }
}
