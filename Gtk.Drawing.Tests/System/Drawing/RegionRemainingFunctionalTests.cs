extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;

namespace System.Drawing;

/// <summary>The remaining <see cref="Region"/> API: path-based combines, transforms and hit-test overloads.</summary>
[Collection("NonParallelCollection")]
public class RegionRemainingFunctionalTests
{
    private sealed class Surface : IDisposable
    {
        private readonly Bitmap _bmp = new(10, 10);
        public Graphics G { get; }
        public Surface() => G = Graphics.FromImage(_bmp);
        public void Dispose() { try { G.Dispose(); } catch { } _bmp.Dispose(); }
    }

    private static GraphicsPath PathOf(Rectangle rect)
    {
        var p = new GraphicsPath();
        p.AddRectangle(rect);
        return p;
    }

    [Fact]
    public void Intersect_WithPath_KeepsOnlyTheOverlap()
    {
        using var r = new Region(new Rectangle(0, 0, 40, 40));
        using var path = PathOf(new Rectangle(20, 20, 40, 40));

        r.Intersect(path);

        Assert.True(r.IsVisible(new Point(30, 30)), "the overlap stays visible");
        Assert.False(r.IsVisible(new Point(5, 5)), "area outside the overlap is dropped");
    }

    [Fact]
    public void Exclude_WithPath_RemovesThatArea()
    {
        using var r = new Region(new Rectangle(0, 0, 40, 40));
        using var path = PathOf(new Rectangle(0, 0, 20, 40));

        r.Exclude(path);

        Assert.False(r.IsVisible(new Point(5, 20)), "the excluded strip is gone");
        Assert.True(r.IsVisible(new Point(30, 20)), "the rest stays");
    }

    [Fact]
    public void Transform_WithTranslation_MovesTheRegion()
    {
        using var s = new Surface();
        using var r = new Region(new Rectangle(0, 0, 20, 20));
        using var translate = new Matrix(1, 0, 0, 1, 50, 60);

        r.Transform(translate);

        var b = r.GetBounds(s.G);
        Assert.Equal(50, b.X, 3);
        Assert.Equal(60, b.Y, 3);
        Assert.Equal(20, b.Width, 3);
        Assert.Equal(20, b.Height, 3);
    }

    [Fact]
    public void IsVisible_FloatOverloads_AgreeWithThePointOverload()
    {
        using var r = new Region(new Rectangle(10, 10, 20, 20));

        Assert.True(r.IsVisible(15f, 15f));
        Assert.True(r.IsVisible(new PointF(15f, 15f)));
        Assert.False(r.IsVisible(1f, 1f));
        Assert.False(r.IsVisible(new PointF(1f, 1f)));
    }

    [Fact]
    public void IsVisible_RectangleOverloads_ReportOverlap()
    {
        using var r = new Region(new Rectangle(0, 0, 40, 40));

        Assert.True(r.IsVisible(5, 5, 4, 4));
        Assert.True(r.IsVisible(new RectangleF(5f, 5f, 4f, 4f)));
        Assert.False(r.IsVisible(500, 500, 4, 4));
    }

    [Fact]
    public void IsEmpty_IsTrueOnlyAfterEmptying()
    {
        using var s = new Surface();
        using var r = new Region(new Rectangle(0, 0, 10, 10));

        Assert.False(r.IsEmpty(s.G), "a real rectangle region is not empty");

        r.MakeEmpty();
        Assert.True(r.IsEmpty(s.G), "after MakeEmpty the region is empty");
    }

    [Fact]
    public void Union_OfTwoRegions_IsVisibleInBoth()
    {
        using var a = new Region(new Rectangle(0, 0, 10, 10));
        using var b = new Region(new Rectangle(80, 80, 10, 10));

        a.Union(b);

        Assert.True(a.IsVisible(new Point(5, 5)));
        Assert.True(a.IsVisible(new Point(85, 85)));
    }
}
