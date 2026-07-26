extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing;

/// <summary>Every <see cref="Region"/> combine operation is offered in Rectangle, RectangleF, Region and
/// GraphicsPath flavours. Equivalent inputs expressed through different overloads must produce the same
/// region, and the query overloads must agree with the geometry they report.</summary>
[Collection("NonParallelCollection")]
public class RegionOverloadFunctionalTests
{
    private static Graphics NewGraphics(Bitmap bmp) => Graphics.FromImage(bmp);

    private static GraphicsPath PathOf(RectangleF r)
    {
        var p = new GraphicsPath();
        p.AddRectangle(r);
        return p;
    }

    private static void AssertSameBounds(Region expected, Region actual, Graphics g, string what)
    {
        var e = expected.GetBounds(g);
        var a = actual.GetBounds(g);
        Assert.True(Math.Abs(e.X - a.X) < 0.01 && Math.Abs(e.Y - a.Y) < 0.01
                    && Math.Abs(e.Width - a.Width) < 0.01 && Math.Abs(e.Height - a.Height) < 0.01,
            $"{what}: expected {e} but got {a}");
    }

    [Theory]
    [InlineData("union")]
    [InlineData("intersect")]
    [InlineData("exclude")]
    [InlineData("xor")]
    [InlineData("complement")]
    public void RectangleF_And_GraphicsPath_Overloads_Agree(string op)
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        var operand = new RectangleF(20, 20, 40, 40);

        using var viaRect = new Region(new RectangleF(0, 0, 40, 40));
        using var viaPath = new Region(new RectangleF(0, 0, 40, 40));
        using var path = PathOf(operand);

        switch (op)
        {
            case "union": viaRect.Union(operand); viaPath.Union(path); break;
            case "intersect": viaRect.Intersect(operand); viaPath.Intersect(path); break;
            case "exclude": viaRect.Exclude(operand); viaPath.Exclude(path); break;
            case "xor": viaRect.Xor(operand); viaPath.Xor(path); break;
            default: viaRect.Complement(operand); viaPath.Complement(path); break;
        }

        AssertSameBounds(viaRect, viaPath, g, $"{op} via RectangleF vs GraphicsPath");
    }

    [Fact]
    public void Intersect_WithARectangleF_ProducesTheOverlap()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(0, 0, 40, 40));

        region.Intersect(new RectangleF(20, 20, 40, 40));

        var b = region.GetBounds(g);
        Assert.Equal(20f, b.X, 2);
        Assert.Equal(20f, b.Y, 2);
        Assert.Equal(20f, b.Width, 2);
        Assert.Equal(20f, b.Height, 2);
    }

    [Fact]
    public void Union_WithARectangleF_GrowsToCoverBoth()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(0, 0, 10, 10));

        region.Union(new RectangleF(30, 30, 10, 10));

        var b = region.GetBounds(g);
        Assert.Equal(0f, b.X, 2);
        Assert.Equal(40f, b.Width, 2);
        Assert.Equal(40f, b.Height, 2);
    }

    [Fact]
    public void Union_WithAnotherRegion_MatchesTheRectangleOverload()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        var operand = new RectangleF(30, 30, 10, 10);

        using var viaRect = new Region(new RectangleF(0, 0, 10, 10));
        viaRect.Union(operand);

        using var viaRegion = new Region(new RectangleF(0, 0, 10, 10));
        using var other = new Region(operand);
        viaRegion.Union(other);

        AssertSameBounds(viaRect, viaRegion, g, "Union via RectangleF vs Region");
    }

    [Fact]
    public void Intersect_WithAnotherRegion_MatchesTheRectangleOverload()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        var operand = new RectangleF(20, 20, 40, 40);

        using var viaRect = new Region(new RectangleF(0, 0, 40, 40));
        viaRect.Intersect(operand);

        using var viaRegion = new Region(new RectangleF(0, 0, 40, 40));
        using var other = new Region(operand);
        viaRegion.Intersect(other);

        AssertSameBounds(viaRect, viaRegion, g, "Intersect via RectangleF vs Region");
    }

    [Fact]
    public void IsVisible_WithAGraphics_AgreesWithTheGraphicsLessOverload()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(10, 10, 20, 20));

        Assert.Equal(region.IsVisible(new PointF(15, 15)), region.IsVisible(new PointF(15, 15), g));
        Assert.Equal(region.IsVisible(new Point(15, 15)), region.IsVisible(new Point(15, 15), g));
        Assert.Equal(region.IsVisible(15f, 15f), region.IsVisible(15f, 15f, g));
        Assert.Equal(region.IsVisible(new RectangleF(12, 12, 4, 4)), region.IsVisible(new RectangleF(12, 12, 4, 4), g));
        Assert.Equal(region.IsVisible(new Rectangle(12, 12, 4, 4)), region.IsVisible(new Rectangle(12, 12, 4, 4), g));
        Assert.Equal(region.IsVisible(12f, 12f, 4f, 4f), region.IsVisible(12f, 12f, 4f, 4f, g));
    }

    [Fact]
    public void IsVisible_ReportsInsideAndOutside()
    {
        using var region = new Region(new RectangleF(10, 10, 20, 20));

        Assert.True(region.IsVisible(new PointF(15, 15)), "a point inside the region should be visible");
        Assert.False(region.IsVisible(new PointF(100, 100)), "a point far outside the region should not be visible");
        Assert.True(region.IsVisible(new RectangleF(12, 12, 4, 4)), "a rectangle inside the region should be visible");
        Assert.False(region.IsVisible(new RectangleF(100, 100, 4, 4)), "a rectangle far outside should not be visible");
    }

    [Fact]
    public void Equals_IsTrueForTheSameRectangleAndFalseForADifferentOne()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var a = new Region(new RectangleF(1, 2, 3, 4));
        using var same = new Region(new RectangleF(1, 2, 3, 4));
        using var different = new Region(new RectangleF(9, 9, 3, 4));

        Assert.True(a.Equals(same, g), "identical regions should compare equal");
        Assert.False(a.Equals(different, g), "regions at different positions should not compare equal");
    }

    [Fact]
    public void Translate_WithFloatOffsets_MovesTheBoundsWithoutResizing()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(5, 6, 20, 10));

        region.Translate(3.5f, -2.5f);

        var b = region.GetBounds(g);
        Assert.Equal(8.5f, b.X, 2);
        Assert.Equal(3.5f, b.Y, 2);
        Assert.Equal(20f, b.Width, 2);
        Assert.Equal(10f, b.Height, 2);
    }

    [Fact]
    public void GetRegionScans_ReportsTheRegionBounds()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(5, 6, 20, 10));
        using var matrix = new gtk::System.Drawing.Drawing2D.Matrix();

        var scans = region.GetRegionScans(matrix);

        Assert.NotEmpty(scans);
        var union = scans[0];
        foreach (var s in scans) union = RectangleF.Union(union, s);
        var bounds = region.GetBounds(g);
        Assert.Equal(bounds.Width, union.Width, 2);
        Assert.Equal(bounds.Height, union.Height, 2);
    }

    [Fact]
    public void RegionData_RoundTripsThroughTheConstructor()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var original = new Region(new RectangleF(4, 5, 16, 12));

        using var restored = new Region(original.GetRegionData());

        AssertSameBounds(original, restored, g, "Region round-tripped through RegionData");
    }

    [Fact]
    public void MakeEmpty_And_MakeInfinite_FlipTheQueryFlags()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = NewGraphics(bmp);
        using var region = new Region(new RectangleF(1, 1, 5, 5));

        region.MakeEmpty();
        Assert.True(region.IsEmpty(g), "MakeEmpty should make the region empty");
        Assert.False(region.IsInfinite(g), "an empty region is not infinite");

        region.MakeInfinite();
        Assert.True(region.IsInfinite(g), "MakeInfinite should make the region infinite");
        Assert.False(region.IsEmpty(g), "an infinite region is not empty");
    }
}
