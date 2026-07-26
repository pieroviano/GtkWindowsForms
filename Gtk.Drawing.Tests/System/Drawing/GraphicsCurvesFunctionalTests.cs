extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

/// <summary>Pixel-verified behaviour of the curve/arc drawing primitives.</summary>
[Collection("NonParallelCollection")]
public class GraphicsCurvesFunctionalTests
{
    private const int Size = 40;

    private static Bitmap Render(Action<Graphics> draw)
    {
        var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        return bmp;
    }

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    /// <summary>True when any pixel within <paramref name="radius"/> of (cx,cy) has been painted.</summary>
    private static bool PaintedNear(Bitmap bmp, int cx, int cy, int radius = 3)
    {
        for (var x = Math.Max(0, cx - radius); x <= Math.Min(Size - 1, cx + radius); x++)
        {
            for (var y = Math.Max(0, cy - radius); y <= Math.Min(Size - 1, cy + radius); y++)
            {
                if (!IsWhitish(bmp.GetPixel(x, y)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Fact]
    public void DrawArc_PaintsAlongTheArc_NotAtTheCentre()
    {
        // Box (0,0,40,40): centre (20,20), radius 20. A 0..90 degree sweep runs from (40,20) to (20,40),
        // passing through roughly (34,34) at 45 degrees.
        using var bmp = Render(g => g.DrawArc(new Pen(Color.Red, 3f), 0, 0, 40, 40, 0, 90));

        Assert.True(PaintedNear(bmp, 34, 34), "the 45-degree point of the arc should be painted");
        Assert.False(PaintedNear(bmp, 20, 20, 2), "the centre of the arc's circle must stay unpainted");
    }

    [Fact]
    public void DrawPie_PaintsTheBoundingRadii_MeetingAtTheCentre()
    {
        using var bmp = Render(g => g.DrawPie(new Pen(Color.Red, 3f), 0, 0, 40, 40, 0, 90));

        // A pie outline includes the two radii, so the centre region is on the outline.
        Assert.True(PaintedNear(bmp, 20, 20), "the pie's radii meet at the centre and should be painted");
    }

    [Fact]
    public void FillClosedCurve_FillsTheEnclosedRegion()
    {
        var pts = new[] { new Point(20, 4), new Point(36, 20), new Point(20, 36), new Point(4, 20) };
        using var bmp = Render(g => g.FillClosedCurve(new SolidBrush(Color.Red), pts));

        Assert.False(IsWhitish(bmp.GetPixel(20, 20)), "the middle of a filled closed curve should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(1, 1)), "a far corner should stay unpainted");
    }

    [Fact]
    public void DrawBezier_PaintsBetweenItsEndPoints()
    {
        using var bmp = Render(g =>
            g.DrawBezier(new Pen(Color.Red, 3f), new Point(2, 30), new Point(14, 4), new Point(26, 4), new Point(38, 30)));

        Assert.True(PaintedNear(bmp, 2, 30), "the curve should start at its first end point");
        Assert.True(PaintedNear(bmp, 38, 30), "the curve should end at its last end point");
        Assert.True(PaintedNear(bmp, 20, 12, 6), "the curve should arc up through the middle");
    }

    [Fact]
    public void DrawCurve_PassesThroughItsControlPoints()
    {
        var pts = new[] { new Point(4, 20), new Point(20, 6), new Point(36, 20) };
        using var bmp = Render(g => g.DrawCurve(new Pen(Color.Red, 3f), pts));

        Assert.True(PaintedNear(bmp, 4, 20), "curve should pass through the first point");
        Assert.True(PaintedNear(bmp, 20, 6), "curve should pass through the middle point");
        Assert.True(PaintedNear(bmp, 36, 20), "curve should pass through the last point");
    }

    [Fact]
    public void DrawClosedCurve_ClosesBackToTheStart()
    {
        var pts = new[] { new Point(10, 10), new Point(30, 10), new Point(30, 30), new Point(10, 30) };
        using var bmp = Render(g => g.DrawClosedCurve(new Pen(Color.Red, 3f), pts));

        // Every side of the closed loop, including the closing one, should carry ink.
        Assert.True(PaintedNear(bmp, 20, 10), "top side");
        Assert.True(PaintedNear(bmp, 30, 20), "right side");
        Assert.True(PaintedNear(bmp, 20, 30), "bottom side");
        Assert.True(PaintedNear(bmp, 10, 20), "closing (left) side");
    }
}
