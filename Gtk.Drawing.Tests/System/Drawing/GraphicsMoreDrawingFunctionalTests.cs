extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using LinearGradientBrush = gtk::System.Drawing.Drawing2D.LinearGradientBrush;
using HatchBrush = gtk::System.Drawing.Drawing2D.HatchBrush;
using HatchStyle = gtk::System.Drawing.Drawing2D.HatchStyle;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;

namespace System.Drawing;

/// <summary>
/// Pixel-verified behaviour for the remaining <see cref="Graphics"/> primitives and brush kinds.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsMoreDrawingFunctionalTests
{
    private const int Size = 40;

    private static Bitmap Render(Action<Graphics> draw, int size = Size)
    {
        var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        return bmp;
    }

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    private static int CountNonBackground(Bitmap bmp, int size = Size)
    {
        var n = 0;
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                if (!IsWhitish(bmp.GetPixel(x, y)))
                {
                    n++;
                }
            }
        }

        return n;
    }

    [Fact]
    public void DrawLines_PaintsEverySegmentOfThePolyline()
    {
        var pts = new[] { new Point(2, 2), new Point(36, 2), new Point(36, 36) };
        using var bmp = Render(g => g.DrawLines(new Pen(Color.Red, 3f), pts));

        Assert.False(IsWhitish(bmp.GetPixel(20, 2)), "the horizontal segment should be painted");
        Assert.False(IsWhitish(bmp.GetPixel(36, 20)), "the vertical segment should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(5, 30)), "an area away from both segments should stay clear");
    }

    [Fact]
    public void DrawPolygon_PaintsTheOutlineButNotTheInterior()
    {
        var triangle = new[] { new Point(20, 3), new Point(36, 36), new Point(4, 36) };
        using var bmp = Render(g => g.DrawPolygon(new Pen(Color.Red, 2f), triangle));

        Assert.False(IsWhitish(bmp.GetPixel(20, 36)), "the bottom edge should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(20, 25)), "the interior of an outlined polygon must stay clear");
    }

    [Fact]
    public void FillRectangles_FillsEveryRectangleGiven()
    {
        var rects = new[] { new Rectangle(2, 2, 8, 8), new Rectangle(28, 28, 8, 8) };
        using var bmp = Render(g => g.FillRectangles(new SolidBrush(Color.Red), rects));

        Assert.False(IsWhitish(bmp.GetPixel(5, 5)), "first rectangle should be filled");
        Assert.False(IsWhitish(bmp.GetPixel(31, 31)), "second rectangle should be filled");
        Assert.True(IsWhitish(bmp.GetPixel(20, 20)), "the gap between them should stay clear");
    }

    [Fact]
    public void ThickerPen_PaintsMorePixelsThanAThinPen()
    {
        using var thin = Render(g => g.DrawLine(new Pen(Color.Red, 1f), 0, 20, 39, 20));
        using var thick = Render(g => g.DrawLine(new Pen(Color.Red, 6f), 0, 20, 39, 20));

        var thinCount = CountNonBackground(thin);
        var thickCount = CountNonBackground(thick);

        Assert.True(thinCount > 0, "a 1px line should paint something");
        Assert.True(thickCount > thinCount,
            $"a 6px pen should paint more than a 1px pen (thin={thinCount}, thick={thickCount})");
    }

    [Fact]
    public void LinearGradientBrush_ProducesDifferentColoursAcrossTheFill()
    {
        var rect = new Rectangle(0, 0, Size, Size);
        using var bmp = Render(g => g.FillRectangle(new LinearGradientBrush(rect, Color.Red, Color.Blue, 0f), rect));

        var left = bmp.GetPixel(2, 20);
        var right = bmp.GetPixel(37, 20);

        Assert.True(left.R > left.B, $"the left end should lean red but was {left}");
        Assert.True(right.B > right.R, $"the right end should lean blue but was {right}");
    }

    [Fact]
    public void HatchBrush_FillsUsingItsForegroundColour()
    {
        // NOTE: this shim renders a hatch as a solid fill of the foreground colour rather than drawing the
        // hatch pattern, so only the colour used is asserted here (the pattern itself is not reproduced).
        var rect = new Rectangle(0, 0, Size, Size);
        using var bmp = Render(g =>
            g.FillRectangle(new HatchBrush(HatchStyle.Horizontal, Color.Blue, Color.Red), rect));

        var c = bmp.GetPixel(20, 20);
        Assert.True(c.B > c.R,
            $"the hatch fill should use its blue foreground colour but the pixel was {c}");
    }

    [Fact]
    public void DrawImage_WithSourceRectangle_CopiesOnlyThatPortion()
    {
        using var source = new Bitmap(20, 20);
        for (var x = 0; x < 20; x++)
        {
            for (var y = 0; y < 20; y++)
            {
                // Left half red, right half green.
                source.SetPixel(x, y, x < 10 ? Color.Red : Color.Lime);
            }
        }

        // Copy only the RIGHT (green) half into the destination.
        using var bmp = Render(g =>
            g.DrawImage(source, new Rectangle(0, 0, 20, 20), new Rectangle(10, 0, 10, 20), GraphicsUnit.Pixel));

        var c = bmp.GetPixel(10, 10);
        Assert.True(c.G > c.R, $"only the green half should have been copied but the pixel was {c}");
    }

    [Fact]
    public void DrawImageUnscaled_KeepsTheSourceSize()
    {
        using var source = new Bitmap(6, 6);
        for (var x = 0; x < 6; x++)
        {
            for (var y = 0; y < 6; y++)
            {
                source.SetPixel(x, y, Color.Red);
            }
        }

        using var bmp = Render(g => g.DrawImageUnscaled(source, 5, 5));

        Assert.False(IsWhitish(bmp.GetPixel(7, 7)), "inside the unscaled copy should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(15, 15)),
            "an unscaled 6x6 copy at (5,5) must not reach (15,15)");
    }

    [Fact]
    public void MeasureString_EmptyText_IsNarrowerThanRealText()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        var font = new gtk::System.Drawing.Font("Sans", 12f);

        var empty = g.MeasureString(string.Empty, font);
        var text = g.MeasureString("something", font);

        Assert.True(text.Width > empty.Width,
            $"real text should measure wider than empty text (empty={empty.Width}, text={text.Width})");
    }
}
