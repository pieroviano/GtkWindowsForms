extern alias gtk;
using Bitmap = gtk::System.Drawing.Bitmap;
using PixelFormat = gtk::System.Drawing.Imaging.PixelFormat;
using ImageLockMode = gtk::System.Drawing.Imaging.ImageLockMode;

namespace System.Drawing;

/// <summary>Behavioural tests for the richer <see cref="Bitmap"/> operations.</summary>
[Collection("NonParallelCollection")]
public class BitmapAdvancedFunctionalTests
{
    private static Bitmap Striped(int w, int h)
    {
        var bmp = new Bitmap(w, h);
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                // Left half red, right half green - lets a sub-region be identified unambiguously.
                bmp.SetPixel(x, y, x < w / 2 ? Color.FromArgb(255, 220, 10, 10) : Color.FromArgb(255, 10, 200, 10));
            }
        }

        return bmp;
    }

    [Fact]
    public void Clone_WithRectangle_CopiesThatRegionsPixels()
    {
        using var source = Striped(20, 10);

        // Clone the right (green) half.
        using var clone = source.Clone(new Rectangle(10, 0, 10, 10), PixelFormat.Format32bppArgb);

        Assert.Equal(10, clone.Width);
        Assert.Equal(10, clone.Height);

        var c = clone.GetPixel(2, 5);
        Assert.True(c.G > c.R, $"the cloned region should come from the green half but was {c}");
    }

    [Fact]
    public void MakeTransparent_MakesTheGivenColourFullyTransparent()
    {
        using var bmp = new Bitmap(4, 4);
        var target = Color.FromArgb(255, 10, 20, 30);
        bmp.SetPixel(1, 1, target);
        bmp.SetPixel(2, 2, Color.FromArgb(255, 200, 200, 200));

        bmp.MakeTransparent(target);

        Assert.Equal(0, bmp.GetPixel(1, 1).A);
        Assert.True(bmp.GetPixel(2, 2).A > 0, "a different colour must keep its opacity");
    }

    [Fact]
    public void LockBits_ReportsConsistentGeometry_AndUnlocksCleanly()
    {
        using var bmp = Striped(8, 8);
        var rect = new Rectangle(0, 0, 8, 8);

        var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            Assert.Equal(8, data.Width);
            Assert.Equal(8, data.Height);
            Assert.True(data.Stride >= data.Width, $"stride ({data.Stride}) must cover a row of {data.Width} pixels");
            Assert.NotEqual(IntPtr.Zero, data.Scan0);
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        // The bitmap must still be usable after unlocking.
        var c = bmp.GetPixel(1, 1);
        Assert.True(c.R > c.G, $"pixel content should survive a lock/unlock cycle but was {c}");
    }

    [Fact]
    public void SetPixel_OutOfRange_Throws()
    {
        using var bmp = new Bitmap(4, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() => bmp.SetPixel(4, 0, Color.Red));
        Assert.Throws<ArgumentOutOfRangeException>(() => bmp.GetPixel(0, -1));
    }

    [Fact]
    public void ResizingConstructor_ResamplesContent_KeepingTheLeftHalfRed()
    {
        using var source = Striped(20, 10);
        using var resized = new Bitmap(source, 10, 5);

        Assert.Equal(10, resized.Width);
        Assert.Equal(5, resized.Height);

        var left = resized.GetPixel(1, 2);
        var right = resized.GetPixel(8, 2);
        Assert.True(left.R > left.G, $"left of the resized image should stay red but was {left}");
        Assert.True(right.G > right.R, $"right of the resized image should stay green but was {right}");
    }
}
