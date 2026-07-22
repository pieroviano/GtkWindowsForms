extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;

namespace System.Drawing;

/// <summary>Further <see cref="Region"/> behaviour: translation, scans and rectangle hit-testing.</summary>
[Collection("NonParallelCollection")]
public class RegionMoreFunctionalTests
{
    private static Graphics NewGraphics(Bitmap bmp) => Graphics.FromImage(bmp);

    [Fact]
    public void Translate_MovesTheRegionBounds()
    {
        using var bmp = new Bitmap(10, 10);
        using var g = NewGraphics(bmp);
        using var r = new Region(new Rectangle(0, 0, 20, 20));

        r.Translate(15, 25);

        var b = r.GetBounds(g);
        Assert.Equal(15, b.X, 3);
        Assert.Equal(25, b.Y, 3);
        Assert.Equal(20, b.Width, 3);
        Assert.Equal(20, b.Height, 3);
    }

    [Fact]
    public void Translate_MovesWhichPointsAreVisible()
    {
        using var r = new Region(new Rectangle(0, 0, 10, 10));
        Assert.True(r.IsVisible(new Point(5, 5)));

        r.Translate(100, 0);

        Assert.False(r.IsVisible(new Point(5, 5)), "the original point should no longer be inside");
        Assert.True(r.IsVisible(new Point(105, 5)), "the translated location should now be inside");
    }

    [Fact]
    public void RectangleConstructor_FromRectangleF_IsHitTestedTheSame()
    {
        using var r = new Region(new RectangleF(10f, 10f, 20f, 20f));

        Assert.True(r.IsVisible(new PointF(15f, 15f)));
        Assert.False(r.IsVisible(new PointF(5f, 5f)));
    }

    [Fact]
    public void IsVisible_Rectangle_ReportsOverlap()
    {
        using var r = new Region(new Rectangle(0, 0, 50, 50));

        Assert.True(r.IsVisible(new Rectangle(10, 10, 5, 5)), "a fully contained rectangle overlaps");
        Assert.False(r.IsVisible(new Rectangle(500, 500, 5, 5)), "a far rectangle does not overlap");
    }

    [Fact]
    public void GetRegionScans_ReturnsTheRegionExtent()
    {
        using var r = new Region(new Rectangle(5, 6, 30, 40));
        using var identity = new Matrix();

        var scans = r.GetRegionScans(identity);

        Assert.NotNull(scans);
        Assert.NotEmpty(scans);
        Assert.Equal(5, scans[0].X, 3);
        Assert.Equal(6, scans[0].Y, 3);
        Assert.Equal(30, scans[0].Width, 3);
        Assert.Equal(40, scans[0].Height, 3);
    }

    [Fact]
    public void MakeEmpty_MakesNoPointVisible()
    {
        using var r = new Region(new Rectangle(0, 0, 40, 40));
        Assert.True(r.IsVisible(new Point(10, 10)));

        r.MakeEmpty();

        Assert.False(r.IsVisible(new Point(10, 10)), "an emptied region should contain nothing");
    }

    [Fact]
    public void Union_WithAnotherRegion_CombinesBothAreas()
    {
        using var a = new Region(new Rectangle(0, 0, 10, 10));
        using var b = new Region(new Rectangle(90, 90, 10, 10));

        a.Union(b);

        Assert.True(a.IsVisible(new Point(5, 5)), "the original area stays visible");
        Assert.True(a.IsVisible(new Point(95, 95)), "the unioned area becomes visible");
    }
}
