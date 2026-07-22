extern alias gtk;
using Bitmap = gtk::System.Drawing.Bitmap;
using Image = gtk::System.Drawing.Image;
using PixelFormat = gtk::System.Drawing.Imaging.PixelFormat;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="Bitmap"/>: pixel round-trips, dimensions, resizing and cloning.</summary>
[Collection("NonParallelCollection")]
public class BitmapFunctionalTests
{
    [Fact]
    public void SetPixel_Then_GetPixel_ReturnsSameColor()
    {
        using var bmp = new Bitmap(8, 8);
        var red = Color.FromArgb(255, 200, 10, 20);
        var green = Color.FromArgb(255, 0, 180, 0);

        bmp.SetPixel(2, 3, red);
        bmp.SetPixel(7, 7, green);

        var got = bmp.GetPixel(2, 3);
        Assert.Equal(red.R, got.R);
        Assert.Equal(red.G, got.G);
        Assert.Equal(red.B, got.B);
        Assert.Equal(red.A, got.A);

        var got2 = bmp.GetPixel(7, 7);
        Assert.Equal(green.G, got2.G);
    }

    [Fact]
    public void Constructor_SetsDimensions()
    {
        using var bmp = new Bitmap(20, 12);
        Assert.Equal(20, bmp.Width);
        Assert.Equal(12, bmp.Height);
        Assert.Equal(new Size(20, 12), bmp.Size);
    }

    [Fact]
    public void Resizing_Constructor_ProducesRequestedSize()
    {
        using var source = (Bitmap)Tests.Resources.timg6;
        using var resized = new Bitmap(source, new Size(16, 9));

        Assert.Equal(16, resized.Width);
        Assert.Equal(9, resized.Height);
    }

    [Fact]
    public void CopyConstructor_PreservesDimensions()
    {
        using var source = (Bitmap)Tests.Resources.timg6;
        using var copy = new Bitmap(source);

        Assert.Equal(source.Width, copy.Width);
        Assert.Equal(source.Height, copy.Height);
    }

    [Fact]
    public void SetPixel_DoesNotAffectNeighbours()
    {
        using var bmp = new Bitmap(4, 4);
        var mark = Color.FromArgb(255, 123, 45, 67);
        bmp.SetPixel(1, 1, mark);

        // A different, untouched pixel must not have taken the mark colour.
        var neighbour = bmp.GetPixel(3, 3);
        Assert.False(neighbour.R == mark.R && neighbour.G == mark.G && neighbour.B == mark.B,
            "An unrelated pixel unexpectedly matched the one that was set.");
    }
}
