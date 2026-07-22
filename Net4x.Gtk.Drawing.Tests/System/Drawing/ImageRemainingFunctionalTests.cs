extern alias gtk;
using Image = gtk::System.Drawing.Image;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;
using FrameDimension = gtk::System.Drawing.Imaging.FrameDimension;

namespace System.Drawing;

/// <summary>The remaining <see cref="Image"/> surface: resolution, physical bounds, frames and metadata.</summary>
[Collection("NonParallelCollection")]
public class ImageRemainingFunctionalTests
{
    private static Bitmap Sample(int w = 20, int h = 10)
    {
        var bmp = new Bitmap(w, h);
        bmp.SetPixel(0, 0, Color.Red);
        return bmp;
    }

    [Fact]
    public void SetResolution_RoundTrips()
    {
        using var bmp = Sample();

        bmp.SetResolution(120f, 144f);

        Assert.Equal(120f, bmp.HorizontalResolution, 2);
        Assert.Equal(144f, bmp.VerticalResolution, 2);
    }

    [Fact]
    public void PhysicalDimension_MatchesPixelSize()
    {
        using var bmp = Sample(24, 18);

        Assert.Equal(24f, bmp.PhysicalDimension.Width, 2);
        Assert.Equal(18f, bmp.PhysicalDimension.Height, 2);
    }

    [Fact]
    public void GetBounds_ReportsTheImageRectangle()
    {
        using var bmp = Sample(24, 18);
        var unit = GraphicsUnit.Pixel;

        var bounds = bmp.GetBounds(ref unit);

        Assert.Equal(0f, bounds.X, 2);
        Assert.Equal(0f, bounds.Y, 2);
        Assert.Equal(24f, bounds.Width, 2);
        Assert.Equal(18f, bounds.Height, 2);
    }

    [Fact]
    public void Tag_RoundTrips()
    {
        using var bmp = Sample();

        bmp.Tag = "payload";

        Assert.Equal("payload", bmp.Tag);
    }

    [Fact]
    public void Size_AgreesWithWidthAndHeight()
    {
        using var bmp = Sample(31, 17);

        Assert.Equal(31, bmp.Size.Width);
        Assert.Equal(17, bmp.Size.Height);
    }

    [Fact]
    public void GetFrameCount_OfAStillImage_IsOne()
    {
        using var bmp = Sample();

        var frames = bmp.GetFrameCount(FrameDimension.Page);

        Assert.Equal(1, frames);
    }

    [Fact]
    public void Clone_IsIndependent_EditingTheCloneLeavesTheOriginalAlone()
    {
        using var original = Sample(8, 8);
        original.SetPixel(4, 4, Color.FromArgb(255, 10, 200, 10));

        using var clone = (Bitmap)original.Clone();
        clone.SetPixel(4, 4, Color.FromArgb(255, 200, 10, 10));

        var originalPixel = original.GetPixel(4, 4);
        Assert.True(originalPixel.G > originalPixel.R,
            $"editing the clone must not change the original, but it became {originalPixel}");
    }

    [Fact]
    public void IsAlphaPixelFormat_AgreesWithTheFormat()
    {
        Assert.True(Image.IsAlphaPixelFormat(gtk::System.Drawing.Imaging.PixelFormat.Format32bppArgb));
        Assert.False(Image.IsAlphaPixelFormat(gtk::System.Drawing.Imaging.PixelFormat.Format24bppRgb));
    }

    [Fact]
    public void GetPixelFormatSize_ReportsBitsPerPixel()
    {
        Assert.Equal(32, Image.GetPixelFormatSize(gtk::System.Drawing.Imaging.PixelFormat.Format32bppArgb));
        Assert.Equal(24, Image.GetPixelFormatSize(gtk::System.Drawing.Imaging.PixelFormat.Format24bppRgb));
    }
}
