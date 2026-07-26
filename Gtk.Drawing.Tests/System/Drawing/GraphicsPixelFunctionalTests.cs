extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using Font = gtk::System.Drawing.Font;

namespace System.Drawing;

/// <summary>
/// Verifies <see cref="Graphics"/> actually paints by inspecting the resulting pixels of the backing
/// <see cref="Bitmap"/>, and that text measurement is sensible.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsPixelFunctionalTests
{
    [Fact]
    public void FillRectangle_PaintsThePixelsItCovers()
    {
        using var bmp = new Bitmap(20, 20);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.FillRectangle(new SolidBrush(Color.Red), 5, 5, 10, 10);
        }

        var inside = bmp.GetPixel(10, 10);
        Assert.True(inside.R > 200 && inside.G < 80 && inside.B < 80,
            $"pixel inside the filled red rectangle was {inside}");

        var outside = bmp.GetPixel(0, 0);
        Assert.True(outside.R > 200 && outside.G > 200 && outside.B > 200,
            $"pixel outside the rectangle should still be the white background but was {outside}");
    }

    [Fact]
    public void Clear_FillsEntireSurface()
    {
        using var bmp = new Bitmap(10, 10);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.FromArgb(255, 10, 120, 240));
        }

        var corner = bmp.GetPixel(9, 9);
        Assert.True(corner.B > 200 && corner.G > 80,
            $"a corner pixel should carry the clear colour but was {corner}");
    }

    [Fact]
    public void MeasureString_LongerText_IsWider()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        var font = new Font("Sans", 12);

        var shortSize = g.MeasureString("i", font);
        var longSize = g.MeasureString("wwwwwwwwww", font);

        Assert.True(shortSize.Width > 0, "measured width of non-empty text should be positive");
        Assert.True(longSize.Width > shortSize.Width,
            $"a longer string should measure wider ('i'={shortSize.Width}, 'wwww...'={longSize.Width})");
    }
}
