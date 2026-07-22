extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="Region"/>: set operations observed through hit-testing and bounds.</summary>
[Collection("NonParallelCollection")]
public class RegionFunctionalTests
{
    private static Graphics NewGraphics(Bitmap bmp) => Graphics.FromImage(bmp);

    [Fact]
    public void Region_FromRectangle_ContainsInsidePoints_NotOutside()
    {
        using var r = new Region(new Rectangle(10, 10, 40, 40));

        Assert.True(r.IsVisible(new Point(20, 20)), "a point inside the rectangle should be visible");
        Assert.False(r.IsVisible(new Point(500, 500)), "a far point should not be visible");
    }

    [Fact]
    public void Union_AddsTheSecondRectangleToTheRegion()
    {
        using var r = new Region(new Rectangle(0, 0, 10, 10));
        Assert.False(r.IsVisible(new Point(55, 55)), "point should start outside the region");

        r.Union(new Rectangle(50, 50, 20, 20));

        Assert.True(r.IsVisible(new Point(55, 55)), "after Union the point in the added rectangle should be visible");
        Assert.True(r.IsVisible(new Point(5, 5)), "the original area should still be visible");
    }

    [Fact]
    public void Exclude_RemovesTheExcludedArea()
    {
        using var r = new Region(new Rectangle(0, 0, 100, 100));
        Assert.True(r.IsVisible(new Point(10, 10)));

        r.Exclude(new Rectangle(0, 0, 50, 50));

        Assert.False(r.IsVisible(new Point(10, 10)), "a point inside the excluded rectangle should no longer be visible");
        Assert.True(r.IsVisible(new Point(80, 80)), "a point outside the excluded rectangle should remain visible");
    }

    [Fact]
    public void Intersect_KeepsOnlyTheOverlap()
    {
        using var r = new Region(new Rectangle(0, 0, 100, 100));
        r.Intersect(new Rectangle(50, 50, 100, 100));

        Assert.True(r.IsVisible(new Point(60, 60)), "a point in the overlap should be visible");
        Assert.False(r.IsVisible(new Point(10, 10)), "a point outside the overlap should not be visible");
    }

    [Fact]
    public void MakeEmpty_And_MakeInfinite_ReportCorrectly()
    {
        using var bmp = new Bitmap(10, 10);
        using var g = NewGraphics(bmp);
        using var r = new Region(new Rectangle(0, 0, 10, 10));

        r.MakeEmpty();
        Assert.True(r.IsEmpty(g), "region should report empty after MakeEmpty");

        r.MakeInfinite();
        Assert.True(r.IsInfinite(g), "region should report infinite after MakeInfinite");
    }

    [Fact]
    public void GetBounds_ReflectsTheRectangleTheRegionWasBuiltFrom()
    {
        using var bmp = new Bitmap(10, 10);
        using var g = NewGraphics(bmp);
        using var r = new Region(new Rectangle(5, 7, 30, 40));

        var bounds = r.GetBounds(g);
        Assert.Equal(5, bounds.X, 3);
        Assert.Equal(7, bounds.Y, 3);
        Assert.Equal(30, bounds.Width, 3);
        Assert.Equal(40, bounds.Height, 3);
    }

    [Fact]
    public void Region_FromPath_ContainsPointInsideThePath()
    {
        using var path = new GraphicsPath();
        path.AddRectangle(new Rectangle(0, 0, 40, 40));
        using var r = new Region(path);

        Assert.True(r.IsVisible(new Point(20, 20)), "a point inside the path should be visible in the region");
    }

    [Fact]
    public void Clone_IsIndependentOfTheOriginal()
    {
        using var r = new Region(new Rectangle(0, 0, 50, 50));
        using var clone = r.Clone();

        r.MakeEmpty();

        Assert.True(clone.IsVisible(new Point(10, 10)),
            "emptying the original region must not affect the clone");
    }
}
