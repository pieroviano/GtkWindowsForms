extern alias gtk;
using Image = gtk::System.Drawing.Image;
using Bitmap = gtk::System.Drawing.Bitmap;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;
using PixelFormat = gtk::System.Drawing.Imaging.PixelFormat;

namespace System.Drawing;

/// <summary>Encoding/decoding behaviour across the supported image formats.</summary>
[Collection("NonParallelCollection")]
public class ImageFormatsFunctionalTests
{
    private static Bitmap Sample(int w = 12, int h = 8)
    {
        var bmp = new Bitmap(w, h);
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                bmp.SetPixel(x, y, x < w / 2 ? Color.FromArgb(255, 200, 30, 30) : Color.FromArgb(255, 30, 30, 200));
            }
        }

        return bmp;
    }

    private static byte[] Encode(Image image, ImageFormat format)
    {
        using var ms = new MemoryStream();
        image.Save(ms, format);
        return ms.ToArray();
    }

    [Theory]
    [InlineData("Png")]
    [InlineData("Jpeg")]
    [InlineData("Bmp")]
    public void SaveThenReload_PreservesDimensions(string formatName)
    {
        var format = (ImageFormat)typeof(ImageFormat)
            .GetProperty(formatName, Reflection.BindingFlags.Public | Reflection.BindingFlags.Static)!
            .GetValue(null)!;

        using var source = Sample();
        var bytes = Encode(source, format);
        Assert.True(bytes.Length > 0, $"{formatName} encoding produced no bytes");

        using var reloaded = Assert.IsAssignableFrom<Image>(Image.FromStream(new MemoryStream(bytes)));
        Assert.Equal(source.Width, reloaded.Width);
        Assert.Equal(source.Height, reloaded.Height);
    }

    [Fact]
    public void PngEncoding_StartsWithThePngSignature()
    {
        using var source = Sample();
        var bytes = Encode(source, ImageFormat.Png);

        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public void JpegEncoding_StartsWithTheJpegSignature()
    {
        using var source = Sample();
        var bytes = Encode(source, ImageFormat.Jpeg);

        Assert.Equal(0xFF, bytes[0]);
        Assert.Equal(0xD8, bytes[1]);
    }

    [Fact]
    public void PngRoundTrip_PreservesDistinctHalves()
    {
        using var source = Sample(16, 8);
        var bytes = Encode(source, ImageFormat.Png);
        using var reloaded = Assert.IsType<Bitmap>(Image.FromStream(new MemoryStream(bytes)));

        var left = reloaded.GetPixel(2, 4);
        var right = reloaded.GetPixel(13, 4);
        Assert.True(left.R > left.B, $"left half should stay red but was {left}");
        Assert.True(right.B > right.R, $"right half should stay blue but was {right}");
    }

    [Fact]
    public void GetThumbnailImage_ReturnsTheRequestedSize()
    {
        using var source = Sample(40, 20);
        using var thumb = source.GetThumbnailImage(10, 5, () => false, IntPtr.Zero);

        Assert.NotNull(thumb);
        Assert.Equal(10, thumb.Width);
        Assert.Equal(5, thumb.Height);
    }

    [Fact]
    public void NewBitmap_ReportsAnArgbPixelFormat()
    {
        using var bmp = new Bitmap(4, 4);
        Assert.Equal(PixelFormat.Format32bppArgb, bmp.PixelFormat);
    }
}
