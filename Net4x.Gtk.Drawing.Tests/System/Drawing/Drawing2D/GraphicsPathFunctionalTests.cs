extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using FillMode = gtk::System.Drawing.Drawing2D.FillMode;

namespace System.Drawing.Drawing2D;

/// <summary>Behavioural tests for <see cref="GraphicsPath"/> geometry: point counts, bounds and hit-testing.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathFunctionalTests
{
    [Fact]
    public void AddRectangle_BoundsMatchRectangle()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(10, 20, 30, 40));

        var bounds = p.GetBounds();
        Assert.Equal(10, bounds.X, 3);
        Assert.Equal(20, bounds.Y, 3);
        Assert.Equal(30, bounds.Width, 3);
        Assert.Equal(40, bounds.Height, 3);
    }

    [Fact]
    public void Constructor_Points_ReportedByPointCountAndPathPoints()
    {
        // PathPoints/PointCount track the points supplied at construction time.
        var pts = new[] { new PointF(1, 2), new PointF(3, 4), new PointF(5, 6) };
        using var p = new GraphicsPath(pts, new byte[] { 0, 1, 1 });

        Assert.Equal(3, p.PointCount);
        Assert.NotNull(p.PathPoints);
        Assert.Equal(3, p.PathPoints!.Length);
        Assert.Equal(3f, p.PathPoints[1].X, 3);
    }

    [Fact]
    public void Reset_ClearsGeometry_SoBoundsBecomeEmpty()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(10, 10, 50, 50));
        Assert.True(p.GetBounds().Width > 0, "bounds should be non-empty after adding a rectangle");

        p.Reset();

        var bounds = p.GetBounds();
        Assert.Equal(0, bounds.Width, 3);
        Assert.Equal(0, bounds.Height, 3);
    }

    [Fact]
    public void IsVisible_TrueInside_FalseOutside_ForFilledRectangle()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 100, 100));

        Assert.True(p.IsVisible(new Point(50, 50)), "centre of the rectangle should be inside the path");
        Assert.False(p.IsVisible(new Point(500, 500)), "a far point should be outside the path");
    }

    [Fact]
    public void FillMode_RoundTrips()
    {
        using var p = new GraphicsPath(FillMode.Winding);
        Assert.Equal(FillMode.Winding, p.FillMode);

        p.FillMode = FillMode.Alternate;
        Assert.Equal(FillMode.Alternate, p.FillMode);
    }

    [Fact]
    public void Clone_ProducesIndependentPathWithSamePointCount()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 10, 10));
        var original = p.PointCount;

        using var clone = Assert.IsType<GraphicsPath>(p.Clone());
        Assert.Equal(original, clone.PointCount);

        p.Reset();
        Assert.Equal(original, clone.PointCount); // clone unaffected by resetting the source
    }
}
