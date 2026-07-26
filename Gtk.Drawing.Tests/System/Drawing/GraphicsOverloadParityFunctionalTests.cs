extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

/// <summary>
/// Overloads that describe the same geometry (int / float / Point / PointF / Rectangle / RectangleF) must
/// draw the same thing. Comparing the rendered pixels of each pair catches an overload that drops or
/// mis-orders its arguments.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsOverloadParityFunctionalTests
{
    private const int Size = 40;

    private static bool[,] Render(Action<Graphics> draw)
    {
        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        var ink = new bool[Size, Size];
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                var c = bmp.GetPixel(x, y);
                ink[x, y] = c.R < 200 || c.G < 200 || c.B < 200;
            }
        }

        return ink;
    }

    private static int Count(bool[,] ink)
    {
        var n = 0;
        foreach (var b in ink)
        {
            if (b) n++;
        }

        return n;
    }

    /// <summary>Asserts both draw actions produce identical, non-empty output.</summary>
    private static void AssertSameRendering(string what, Action<Graphics> a, Action<Graphics> b)
    {
        var first = Render(a);
        var second = Render(b);

        Assert.True(Count(first) > 0, $"{what}: the first overload drew nothing");
        Assert.True(Count(second) > 0, $"{what}: the second overload drew nothing");

        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                Assert.True(first[x, y] == second[x, y],
                    $"{what}: overloads disagree at ({x},{y})");
            }
        }
    }

    private static Pen NewPen() => new(Color.Red, 3f);

    private static SolidBrush NewBrush() => new(Color.Red);

    [Fact]
    public void DrawLine_IntAndPointOverloads_Match() =>
        AssertSameRendering("DrawLine",
            g => g.DrawLine(NewPen(), 4, 6, 34, 28),
            g => g.DrawLine(NewPen(), new Point(4, 6), new Point(34, 28)));

    [Fact]
    public void DrawLine_FloatAndPointFOverloads_Match() =>
        AssertSameRendering("DrawLine(float)",
            g => g.DrawLine(NewPen(), 4f, 6f, 34f, 28f),
            g => g.DrawLine(NewPen(), new PointF(4f, 6f), new PointF(34f, 28f)));

    [Fact]
    public void DrawRectangle_IntAndRectangleOverloads_Match() =>
        AssertSameRendering("DrawRectangle",
            g => g.DrawRectangle(NewPen(), 6, 8, 22, 18),
            g => g.DrawRectangle(NewPen(), new Rectangle(6, 8, 22, 18)));

    [Fact]
    public void FillRectangle_IntRectangleAndRectangleFOverloads_Match()
    {
        AssertSameRendering("FillRectangle(int/Rectangle)",
            g => g.FillRectangle(NewBrush(), 6, 8, 22, 18),
            g => g.FillRectangle(NewBrush(), new Rectangle(6, 8, 22, 18)));

        AssertSameRendering("FillRectangle(Rectangle/RectangleF)",
            g => g.FillRectangle(NewBrush(), new Rectangle(6, 8, 22, 18)),
            g => g.FillRectangle(NewBrush(), new RectangleF(6f, 8f, 22f, 18f)));
    }

    [Fact]
    public void DrawEllipse_IntRectangleAndRectangleFOverloads_Match()
    {
        AssertSameRendering("DrawEllipse(int/Rectangle)",
            g => g.DrawEllipse(NewPen(), 5, 5, 28, 22),
            g => g.DrawEllipse(NewPen(), new Rectangle(5, 5, 28, 22)));

        AssertSameRendering("DrawEllipse(Rectangle/RectangleF)",
            g => g.DrawEllipse(NewPen(), new Rectangle(5, 5, 28, 22)),
            g => g.DrawEllipse(NewPen(), new RectangleF(5f, 5f, 28f, 22f)));
    }

    [Fact]
    public void FillEllipse_IntAndRectangleOverloads_Match() =>
        AssertSameRendering("FillEllipse",
            g => g.FillEllipse(NewBrush(), 5, 5, 28, 22),
            g => g.FillEllipse(NewBrush(), new Rectangle(5, 5, 28, 22)));

    [Fact]
    public void DrawArc_IntAndRectangleOverloads_Match() =>
        AssertSameRendering("DrawArc",
            g => g.DrawArc(NewPen(), 4, 4, 30, 30, 0f, 120f),
            g => g.DrawArc(NewPen(), new Rectangle(4, 4, 30, 30), 0f, 120f));

    [Fact]
    public void DrawPie_And_FillPie_IntAndRectangleOverloads_Match()
    {
        AssertSameRendering("DrawPie",
            g => g.DrawPie(NewPen(), 4, 4, 30, 30, 0f, 120f),
            g => g.DrawPie(NewPen(), new Rectangle(4, 4, 30, 30), 0f, 120f));

        AssertSameRendering("FillPie",
            g => g.FillPie(NewBrush(), 4, 4, 30, 30, 0, 120),
            g => g.FillPie(NewBrush(), new Rectangle(4, 4, 30, 30), 0f, 120f));
    }

    [Fact]
    public void DrawPolygon_PointAndPointFOverloads_Match()
    {
        var pts = new[] { new Point(20, 4), new Point(34, 32), new Point(6, 32) };
        var ptsF = Array.ConvertAll(pts, p => new PointF(p.X, p.Y));

        AssertSameRendering("DrawPolygon", g => g.DrawPolygon(NewPen(), pts), g => g.DrawPolygon(NewPen(), ptsF));
        AssertSameRendering("FillPolygon", g => g.FillPolygon(NewBrush(), pts), g => g.FillPolygon(NewBrush(), ptsF));
    }

    [Fact]
    public void DrawLines_PointAndPointFOverloads_Match()
    {
        var pts = new[] { new Point(4, 4), new Point(34, 4), new Point(34, 34) };
        var ptsF = Array.ConvertAll(pts, p => new PointF(p.X, p.Y));

        AssertSameRendering("DrawLines", g => g.DrawLines(NewPen(), pts), g => g.DrawLines(NewPen(), ptsF));
    }

    [Fact]
    public void DrawCurve_PointAndPointFOverloads_Match()
    {
        var pts = new[] { new Point(4, 20), new Point(20, 6), new Point(34, 22) };
        var ptsF = Array.ConvertAll(pts, p => new PointF(p.X, p.Y));

        AssertSameRendering("DrawCurve", g => g.DrawCurve(NewPen(), pts), g => g.DrawCurve(NewPen(), ptsF));
        AssertSameRendering("DrawClosedCurve", g => g.DrawClosedCurve(NewPen(), pts), g => g.DrawClosedCurve(NewPen(), ptsF));
        AssertSameRendering("FillClosedCurve", g => g.FillClosedCurve(NewBrush(), pts), g => g.FillClosedCurve(NewBrush(), ptsF));
    }

    [Fact]
    public void DrawBezier_PointAndFloatOverloads_Match() =>
        AssertSameRendering("DrawBezier",
            g => g.DrawBezier(NewPen(), new Point(4, 30), new Point(14, 6), new Point(26, 6), new Point(36, 30)),
            g => g.DrawBezier(NewPen(), 4f, 30f, 14f, 6f, 26f, 6f, 36f, 30f));

    [Fact]
    public void DrawRectangles_And_FillRectangles_RectangleAndRectangleFOverloads_Match()
    {
        var rects = new[] { new Rectangle(4, 4, 12, 12), new Rectangle(22, 22, 12, 12) };
        var rectsF = Array.ConvertAll(rects, r => new RectangleF(r.X, r.Y, r.Width, r.Height));

        AssertSameRendering("DrawRectangles", g => g.DrawRectangles(NewPen(), rects), g => g.DrawRectangles(NewPen(), rectsF));
        AssertSameRendering("FillRectangles", g => g.FillRectangles(NewBrush(), rects), g => g.FillRectangles(NewBrush(), rectsF));
    }
}
