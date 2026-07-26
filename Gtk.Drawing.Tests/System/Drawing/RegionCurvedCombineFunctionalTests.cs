extern alias gtk;
using Region = gtk::System.Drawing.Region;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing;

/// <summary>
/// Behavioural tests for combining a region whose interior is a curve.
/// <para>
/// A combine works on band lists, and a <see cref="GraphicsPath" /> only ever contributed its *bounding
/// rectangle* to one. So <c>region.Intersect(circle)</c> intersected with the circle's square, and the
/// four corners - well outside the circle - counted as inside the result. The interior of a curve is now
/// obtained the way GDI+ gets it, by scan conversion, so the combine sees the shape.
/// </para>
/// The oracle throughout is a point that lies inside the bounding box and outside the shape: it is the
/// only thing that distinguishes the two behaviours.
/// </summary>
[Collection("NonParallelCollection")]
public class RegionCurvedCombineFunctionalTests
{
    private static GraphicsPath EllipsePath(float x, float y, float width, float height)
    {
        var path = new GraphicsPath();
        path.AddEllipse(x, y, width, height);

        return path;
    }

    /// <summary>A circle of diameter 100 at the origin. Its top-left corner is 15px outside the curve.</summary>
    private static GraphicsPath Circle() => EllipsePath(0, 0, 100, 100);

    private static readonly PointF Centre = new(50, 50);

    private static readonly PointF Corner = new(3, 3);

    [Fact]
    public void Intersect_WithACircle_ExcludesTheCornersOfItsBoundingBox()
    {
        var region = new Region(new RectangleF(0, 0, 100, 100));

        region.Intersect(Circle());

        Assert.True(region.IsVisible(Centre), "the middle of the circle is inside");
        Assert.False(region.IsVisible(Corner), "a corner of the bounding box is outside the circle");
    }

    [Fact]
    public void Intersect_OnACircularRegion_ExcludesTheCorners()
    {
        // The curve on the left-hand side this time: the region itself is the circle.
        var region = new Region(Circle());

        region.Intersect(new RectangleF(0, 0, 100, 100));

        Assert.True(region.IsVisible(Centre));
        Assert.False(region.IsVisible(Corner));
    }

    [Fact]
    public void Intersect_OfTwoOverlappingCircles_IsTheLensNotTheOverlapOfTheirBoxes()
    {
        var region = new Region(Circle());

        region.Intersect(EllipsePath(60, 0, 100, 100));

        // The lens between two circles 60 apart spans x in [60, 100] around y = 50.
        Assert.True(region.IsVisible(new PointF(80, 50)), "the middle of the lens is inside both circles");

        // Inside the overlap of the two *boxes*, but above the lens - inside neither circle there.
        Assert.False(
            region.IsVisible(new PointF(62, 3)),
            "the corner of the box overlap lies outside both circles");
    }

    [Fact]
    public void Exclude_ACircle_LeavesTheCornersOfItsBoundingBox()
    {
        var region = new Region(new RectangleF(0, 0, 100, 100));

        region.Exclude(Circle());

        Assert.False(region.IsVisible(Centre), "the circle was taken away");
        Assert.True(region.IsVisible(Corner), "the corners were never part of the circle");
    }

    [Fact]
    public void Union_WithACircle_DoesNotAddTheCornersOfItsBoundingBox()
    {
        // A square to the left, united with a circle to the right: the circle contributes its curve only.
        var region = new Region(new RectangleF(0, 0, 40, 100));

        region.Union(EllipsePath(100, 0, 100, 100));

        Assert.True(region.IsVisible(new PointF(20, 50)), "the square is still there");
        Assert.True(region.IsVisible(new PointF(150, 50)), "so is the circle");
        Assert.False(region.IsVisible(new PointF(103, 3)), "the circle's bounding-box corner is not");
    }

    [Fact]
    public void Xor_WithACircle_KeepsTheCornersOfTheSquareOnly()
    {
        var region = new Region(new RectangleF(0, 0, 100, 100));

        region.Xor(Circle());

        Assert.False(region.IsVisible(Centre), "the circle is in both operands, so it cancels");
        Assert.True(region.IsVisible(Corner), "the corner is in the square only");
    }

    [Fact]
    public void GetBounds_AfterCombining_IsTheCurvesExtentNotAWiderBox()
    {
        using var bmp = new gtk::System.Drawing.Bitmap(200, 200);
        using var graphics = gtk::System.Drawing.Graphics.FromImage(bmp);
        var region = new Region(new RectangleF(0, 0, 200, 200));

        region.Intersect(EllipsePath(20, 30, 100, 60));

        var bounds = region.GetBounds(graphics);

        // Scan conversion works in whole pixels, so the extent is the ellipse's box rounded outwards -
        // never wider than that, and never the whole 200x200 square.
        Assert.InRange(bounds.Left, 19f, 21f);
        Assert.InRange(bounds.Top, 29f, 31f);
        Assert.InRange(bounds.Right, 119f, 121f);
        Assert.InRange(bounds.Bottom, 89f, 91f);
    }

    [Fact]
    public void CombiningRectangles_KeepsExactFloatingPointEdges()
    {
        // Rectangular paths must not go anywhere near the scan converter: their edges are exact, and
        // rounding them to pixels would be a regression.
        var region = new Region(new RectangleF(0.25f, 0.25f, 10f, 10f));
        var path = new GraphicsPath();
        path.AddRectangle(new RectangleF(0.25f, 0.25f, 5.5f, 10f));

        region.Intersect(path);

        using var bmp = new gtk::System.Drawing.Bitmap(20, 20);
        using var graphics = gtk::System.Drawing.Graphics.FromImage(bmp);
        var bounds = region.GetBounds(graphics);

        Assert.Equal(0.25f, bounds.Left, 3);
        Assert.Equal(5.75f, bounds.Right, 3);
    }

    [Fact]
    public void IsEmpty_AfterCombiningDisjointCurves()
    {
        using var bmp = new gtk::System.Drawing.Bitmap(400, 200);
        using var graphics = gtk::System.Drawing.Graphics.FromImage(bmp);
        var region = new Region(Circle());

        region.Intersect(EllipsePath(200, 0, 100, 100));

        Assert.True(region.IsEmpty(graphics), "two circles 200 apart share nothing");
    }
}
