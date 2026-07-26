extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using Pen = gtk::System.Drawing.Pen;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using WarpMode = gtk::System.Drawing.Drawing2D.WarpMode;

namespace System.Drawing.Drawing2D;

/// <summary>The curve/pie/warp/widen families each ship several overloads. Overloads that describe the same
/// geometry must produce the same bounds, and the transforming ones must move the geometry as asked.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathCurveOverloadFunctionalTests
{
    private static readonly PointF[] CurveF =
    [
        new(10f, 10f), new(30f, 40f), new(60f, 20f), new(90f, 50f)
    ];

    private static readonly Point[] CurveI =
    [
        new(10, 10), new(30, 40), new(60, 20), new(90, 50)
    ];

    private static void AssertSameBounds(GraphicsPath a, GraphicsPath b, string what)
    {
        var x = a.GetBounds();
        var y = b.GetBounds();
        Assert.True(Math.Abs(x.X - y.X) < 0.01 && Math.Abs(x.Y - y.Y) < 0.01
                    && Math.Abs(x.Width - y.Width) < 0.01 && Math.Abs(x.Height - y.Height) < 0.01,
            $"{what}: expected {x} but got {y}");
    }

    [Fact]
    public void AddCurve_PointAndPointFOverloads_DescribeTheSameCurve()
    {
        using var a = new GraphicsPath();
        using var b = new GraphicsPath();

        a.AddCurve(CurveF);
        b.AddCurve(CurveI);

        AssertSameBounds(a, b, "AddCurve PointF[] vs Point[]");
    }

    [Fact]
    public void AddCurve_WithExplicitDefaultTension_MatchesTheTensionLessOverload()
    {
        using var a = new GraphicsPath();
        using var b = new GraphicsPath();

        a.AddCurve(CurveF);
        b.AddCurve(CurveF, 0.5f);

        AssertSameBounds(a, b, "AddCurve with the default tension");
    }

    [Fact]
    public void AddCurve_WithASegmentRange_UsesOnlyThoseSegments()
    {
        using var full = new GraphicsPath();
        using var partial = new GraphicsPath();

        full.AddCurve(CurveF, 0.5f);
        partial.AddCurve(CurveF, 0, 1, 0.5f);

        var f = full.GetBounds();
        var p = partial.GetBounds();
        Assert.True(p.Width <= f.Width + 0.01f && p.Height <= f.Height + 0.01f,
            $"a one-segment curve must not exceed the full curve, but {p} vs {f}");
        Assert.True(p.Width > 0f, "the partial curve should still have geometry");
    }

    [Fact]
    public void AddCurve_PointOverloadWithRange_MatchesThePointFOverload()
    {
        using var a = new GraphicsPath();
        using var b = new GraphicsPath();

        a.AddCurve(CurveF, 0, 2, 0.5f);
        b.AddCurve(CurveI, 0, 2, 0.5f);

        AssertSameBounds(a, b, "AddCurve range PointF[] vs Point[]");
    }

    [Fact]
    public void AddClosedCurve_OverloadsAgree()
    {
        using var a = new GraphicsPath();
        using var b = new GraphicsPath();
        using var c = new GraphicsPath();

        a.AddClosedCurve(CurveF);
        b.AddClosedCurve(CurveF, 0.5f);
        c.AddClosedCurve(CurveI, 0.5f);

        AssertSameBounds(a, b, "AddClosedCurve with the default tension");
        AssertSameBounds(a, c, "AddClosedCurve PointF[] vs Point[]");
    }

    [Fact]
    public void AddPie_FloatAndIntOverloads_DescribeTheSamePie()
    {
        using var a = new GraphicsPath();
        using var b = new GraphicsPath();

        a.AddPie(10f, 20f, 40f, 30f, 0f, 90f);
        b.AddPie(10, 20, 40, 30, 0f, 90f);

        AssertSameBounds(a, b, "AddPie float vs int");
    }

    [Fact]
    public void AddPie_StaysWithinTheBoundingEllipse()
    {
        using var p = new GraphicsPath();

        p.AddPie(10f, 20f, 40f, 30f, 0f, 90f);

        var b = p.GetBounds();
        Assert.True(b.Left >= 9.99f && b.Top >= 19.99f && b.Right <= 50.01f && b.Bottom <= 50.01f,
            $"the pie must stay inside its bounding ellipse but bounds were {b}");
    }

    [Fact]
    public void GetBounds_WithAMatrix_ReportsTheTransformedBounds()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(0, 0, 10, 10));
        using var m = new Matrix();
        m.Translate(25f, 40f);

        var b = p.GetBounds(m);

        Assert.Equal(25f, b.X, 1);
        Assert.Equal(40f, b.Y, 1);
        Assert.Equal(10f, b.Width, 1);
        Assert.Equal(10f, b.Height, 1);
    }

    [Fact]
    public void GetBounds_WithAMatrixAndPen_IsAtLeastTheMatrixOnlyBounds()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(0, 0, 10, 10));
        using var m = new Matrix();
        using var pen = new Pen(Color.Black, 4f);

        var plain = p.GetBounds(m);
        var widened = p.GetBounds(m, pen);

        Assert.True(widened.Width >= plain.Width - 0.01f && widened.Height >= plain.Height - 0.01f,
            $"pen-aware bounds {widened} should not be smaller than {plain}");
    }

    [Fact]
    public void Clone_IsIndependent_TransformingTheCloneLeavesTheOriginalAlone()
    {
        using var original = new GraphicsPath();
        original.AddRectangle(new RectangleF(0, 0, 10, 10));

        using var clone = Assert.IsType<GraphicsPath>(original.Clone());
        using var m = new Matrix();
        m.Translate(100f, 100f);
        clone.Transform(m);

        var b = original.GetBounds();
        Assert.Equal(0f, b.X, 1);
        Assert.Equal(0f, b.Y, 1);
        Assert.Equal(100f, clone.GetBounds().X, 1);
    }

    [Fact]
    public void Widen_WithAMatrix_KeepsGeometry()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(10, 10, 20, 20));
        using var pen = new Pen(Color.Black, 3f);
        using var m = new Matrix();

        p.Widen(pen, m);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"a widened path must still have geometry but bounds were {b}");
    }

    [Fact]
    public void Widen_WithFlatness_KeepsGeometry()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(10, 10, 20, 20));
        using var pen = new Pen(Color.Black, 3f);
        using var m = new Matrix();

        p.Widen(pen, m, 0.25f);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"a widened path must still have geometry but bounds were {b}");
    }

    [Fact]
    public void Warp_WithAMatrixAndWarpMode_KeepsGeometry()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(0, 0, 10, 10));
        using var m = new Matrix();
        var destination = new[]
        {
            new PointF(0f, 0f), new PointF(20f, 0f), new PointF(20f, 20f), new PointF(0f, 20f)
        };

        p.Warp(destination, new RectangleF(0, 0, 10, 10), m);
        p.Warp(destination, new RectangleF(0, 0, 10, 10), m, WarpMode.Perspective);
        p.Warp(destination, new RectangleF(0, 0, 10, 10), m, WarpMode.Bilinear, 0.5f);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"a warped path must still have geometry but bounds were {b}");
    }

    [Fact]
    public void IsVisible_WithAGraphics_AgreesWithTheGraphicsLessOverload()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(10, 10, 20, 20));

        Assert.Equal(p.IsVisible(new PointF(15, 15)), p.IsVisible(new PointF(15, 15), g));
        Assert.Equal(p.IsVisible(new Point(15, 15)), p.IsVisible(new Point(15, 15), g));
        Assert.Equal(p.IsVisible(15f, 15f), p.IsVisible(15f, 15f, g));
    }

    [Fact]
    public void IsVisible_DistinguishesInsideFromOutside()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(10, 10, 20, 20));

        Assert.True(p.IsVisible(new PointF(15, 15)), "a point inside the rectangle should be visible");
        Assert.False(p.IsVisible(new PointF(100, 100)), "a point far outside should not be visible");
    }

    [Fact]
    public void IsOutlineVisible_IsTrueOnTheEdgeAndFalseDeepInside()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using var p = new GraphicsPath();
        p.AddRectangle(new RectangleF(10, 10, 40, 40));
        using var pen = new Pen(Color.Black, 4f);

        Assert.True(p.IsOutlineVisible(new PointF(10, 20), pen, g),
            "a point on the rectangle edge should be on the outline");
        Assert.False(p.IsOutlineVisible(new PointF(30, 30), pen, g),
            "a point in the middle of the rectangle is not on the outline");
    }
}
