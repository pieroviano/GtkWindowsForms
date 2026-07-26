extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing.Drawing2D;

/// <summary>
/// Overloads of the Add* methods that describe the same geometry (int / float / Point / PointF /
/// Rectangle / RectangleF) must build the same path. Comparing the resulting bounds catches an overload
/// that drops or mis-orders its arguments.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsPathOverloadParityFunctionalTests
{
    private static RectangleF BoundsOf(Action<GraphicsPath> build)
    {
        using var p = new GraphicsPath();
        build(p);
        return p.GetBounds();
    }

    private static void AssertSameBounds(string what, Action<GraphicsPath> a, Action<GraphicsPath> b)
    {
        var first = BoundsOf(a);
        var second = BoundsOf(b);

        Assert.True(first.Width > 0 || first.Height > 0, $"{what}: the first overload produced empty bounds");
        Assert.Equal(first.X, second.X, 2);
        Assert.Equal(first.Y, second.Y, 2);
        Assert.Equal(first.Width, second.Width, 2);
        Assert.Equal(first.Height, second.Height, 2);
    }

    private static readonly Point[] Pts = { new(4, 20), new(20, 6), new(34, 24) };
    private static readonly PointF[] PtsF = { new(4f, 20f), new(20f, 6f), new(34f, 24f) };

    [Fact]
    public void AddLine_Overloads_Match()
    {
        AssertSameBounds("AddLine(int) vs AddLine(Point)",
            p => p.AddLine(4, 6, 30, 26),
            p => p.AddLine(new Point(4, 6), new Point(30, 26)));

        AssertSameBounds("AddLine(float) vs AddLine(PointF)",
            p => p.AddLine(4f, 6f, 30f, 26f),
            p => p.AddLine(new PointF(4f, 6f), new PointF(30f, 26f)));
    }

    [Fact]
    public void AddLines_PointAndPointFOverloads_Match() =>
        AssertSameBounds("AddLines", p => p.AddLines(Pts), p => p.AddLines(PtsF));

    [Fact]
    public void AddArc_Overloads_Match()
    {
        AssertSameBounds("AddArc(int) vs AddArc(Rectangle)",
            p => p.AddArc(4, 6, 30, 26, 0f, 120f),
            p => p.AddArc(new Rectangle(4, 6, 30, 26), 0f, 120f));

        AssertSameBounds("AddArc(float) vs AddArc(RectangleF)",
            p => p.AddArc(4f, 6f, 30f, 26f, 0f, 120f),
            p => p.AddArc(new RectangleF(4f, 6f, 30f, 26f), 0f, 120f));
    }

    [Fact]
    public void AddBezier_Overloads_Match()
    {
        AssertSameBounds("AddBezier(int) vs AddBezier(Point)",
            p => p.AddBezier(4, 30, 14, 6, 26, 6, 36, 30),
            p => p.AddBezier(new Point(4, 30), new Point(14, 6), new Point(26, 6), new Point(36, 30)));

        AssertSameBounds("AddBezier(float) vs AddBezier(PointF)",
            p => p.AddBezier(4f, 30f, 14f, 6f, 26f, 6f, 36f, 30f),
            p => p.AddBezier(new PointF(4f, 30f), new PointF(14f, 6f), new PointF(26f, 6f), new PointF(36f, 30f)));
    }

    [Fact]
    public void AddBeziers_PointAndPointFOverloads_Match()
    {
        var pts = new[] { new Point(4, 30), new Point(14, 6), new Point(26, 6), new Point(36, 30) };
        var ptsF = Array.ConvertAll(pts, q => new PointF(q.X, q.Y));

        AssertSameBounds("AddBeziers", p => p.AddBeziers(pts), p => p.AddBeziers(ptsF));
    }

    [Fact]
    public void AddCurve_And_AddClosedCurve_Overloads_Match()
    {
        AssertSameBounds("AddCurve", p => p.AddCurve(Pts), p => p.AddCurve(PtsF));
        AssertSameBounds("AddClosedCurve", p => p.AddClosedCurve(Pts), p => p.AddClosedCurve(PtsF));
    }

    [Fact]
    public void AddEllipse_Overloads_Match()
    {
        AssertSameBounds("AddEllipse(int) vs AddEllipse(Rectangle)",
            p => p.AddEllipse(4, 6, 30, 26),
            p => p.AddEllipse(new Rectangle(4, 6, 30, 26)));

        AssertSameBounds("AddEllipse(float) vs AddEllipse(RectangleF)",
            p => p.AddEllipse(4f, 6f, 30f, 26f),
            p => p.AddEllipse(new RectangleF(4f, 6f, 30f, 26f)));
    }

    [Fact]
    public void AddPie_Overloads_Match() =>
        AssertSameBounds("AddPie(int) vs AddPie(Rectangle)",
            p => p.AddPie(4, 6, 30, 26, 0f, 120f),
            p => p.AddPie(new Rectangle(4, 6, 30, 26), 0f, 120f));

    [Fact]
    public void AddPolygon_PointAndPointFOverloads_Match() =>
        AssertSameBounds("AddPolygon", p => p.AddPolygon(Pts), p => p.AddPolygon(PtsF));

    [Fact]
    public void AddRectangle_And_AddRectangles_Overloads_Match()
    {
        AssertSameBounds("AddRectangle",
            p => p.AddRectangle(new Rectangle(4, 6, 30, 26)),
            p => p.AddRectangle(new RectangleF(4f, 6f, 30f, 26f)));

        var rects = new[] { new Rectangle(4, 4, 10, 10), new Rectangle(24, 22, 10, 10) };
        var rectsF = Array.ConvertAll(rects, r => new RectangleF(r.X, r.Y, r.Width, r.Height));
        AssertSameBounds("AddRectangles", p => p.AddRectangles(rects), p => p.AddRectangles(rectsF));
    }
}
