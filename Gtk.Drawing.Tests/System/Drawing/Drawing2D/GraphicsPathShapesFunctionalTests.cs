extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing.Drawing2D;

/// <summary>Each curve/arc primitive must contribute its geometry to the path's bounds.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathShapesFunctionalTests
{
    private static void AssertWithin(RectangleF bounds, RectangleF outer, string what)
    {
        Assert.True(bounds.Left >= outer.Left - 0.5f && bounds.Top >= outer.Top - 0.5f
                    && bounds.Right <= outer.Right + 0.5f && bounds.Bottom <= outer.Bottom + 0.5f,
            $"{what} bounds {bounds} should stay within {outer}");
    }

    [Fact]
    public void AddArc_BoundsStayInsideTheArcBoundingBox()
    {
        using var p = new GraphicsPath();
        p.AddArc(0, 0, 40, 40, 0, 90);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"an arc should produce non-empty bounds but got {b}");
        AssertWithin(b, new RectangleF(0, 0, 40, 40), "arc");
    }

    [Fact]
    public void AddPie_BoundsStayInsideThePieBoundingBox()
    {
        using var p = new GraphicsPath();
        p.AddPie(5, 5, 30, 30, 0, 120);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"a pie should produce non-empty bounds but got {b}");
        AssertWithin(b, new RectangleF(5, 5, 30, 30), "pie");
    }

    [Fact]
    public void AddBezier_BoundsSpanTheEndPoints()
    {
        using var p = new GraphicsPath();
        p.AddBezier(0, 0, 10, 30, 30, 30, 40, 0);

        var b = p.GetBounds();
        Assert.True(b.Left <= 0.5f, $"bezier bounds should start at the first point but were {b}");
        Assert.True(b.Right >= 39.5f, $"bezier bounds should reach the last point but were {b}");
    }

    [Fact]
    public void AddCurve_BoundsSpanAllControlPoints()
    {
        using var p = new GraphicsPath();
        p.AddCurve(new[] { new Point(0, 10), new Point(20, 0), new Point(40, 30) });

        var b = p.GetBounds();
        Assert.True(b.Left <= 0.5f, $"curve should span x=0 but bounds were {b}");
        Assert.True(b.Right >= 39.5f, $"curve should span x=40 but bounds were {b}");
        Assert.True(b.Bottom >= 29.5f, $"curve should span y=30 but bounds were {b}");
    }

    [Fact]
    public void AddClosedCurve_ProducesNonEmptyBounds()
    {
        using var p = new GraphicsPath();
        p.AddClosedCurve(new[] { new Point(5, 5), new Point(25, 8), new Point(15, 30) });

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"closed curve bounds were {b}");
    }

    [Fact]
    public void StartAndCloseFigure_DoNotDiscardTheGeometry()
    {
        using var p = new GraphicsPath();
        p.StartFigure();
        p.AddLine(0, 0, 30, 0);
        p.AddLine(30, 0, 30, 20);
        p.CloseFigure();

        var b = p.GetBounds();
        Assert.Equal(0, b.X, 3);
        Assert.Equal(0, b.Y, 3);
        Assert.Equal(30, b.Width, 3);
        Assert.Equal(20, b.Height, 3);
    }

    [Fact]
    public void IsVisible_UsesTheEllipseShape_NotJustItsBoundingBox()
    {
        using var p = new GraphicsPath();
        p.AddEllipse(0, 0, 100, 100);

        Assert.True(p.IsVisible(new Point(50, 50)), "the centre of the ellipse is inside it");
        Assert.False(p.IsVisible(new Point(400, 400)), "a point well outside must not be reported inside");
    }
}
