extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace System.Drawing;

/// <summary>Remaining <see cref="Region"/> combine operations, observed through hit-testing.</summary>
[Collection("NonParallelCollection")]
public class RegionOpsFunctionalTests
{
    private sealed class Surface : IDisposable
    {
        private readonly Bitmap _bmp = new(10, 10);
        public Graphics G { get; }
        public Surface() => G = Graphics.FromImage(_bmp);
        public void Dispose() { try { G.Dispose(); } catch { } _bmp.Dispose(); }
    }

    // NOTE: this Region models geometry as a bounding rectangle plus a path, so a partial overlap Xor is a
    // documented approximation. The two cases below are the ones the model represents exactly, and they
    // match GDI+ semantics.
    [Fact]
    public void Xor_WithDisjointRectangle_BehavesLikeUnion()
    {
        using var r = new Region(new Rectangle(0, 0, 20, 20));
        r.Xor(new Rectangle(60, 60, 20, 20));

        Assert.True(r.IsVisible(new Point(10, 10)), "the original area stays");
        Assert.True(r.IsVisible(new Point(70, 70)), "the disjoint area is added");
    }

    [Fact]
    public void Xor_WithTheIdenticalRectangle_EmptiesTheRegion()
    {
        using var r = new Region(new Rectangle(0, 0, 20, 20));
        r.Xor(new Rectangle(0, 0, 20, 20));

        Assert.False(r.IsVisible(new Point(10, 10)),
            "xor-ing a region with itself leaves nothing behind");
    }

    [Fact]
    public void Intersect_WithRegion_KeepsOnlyTheCommonArea()
    {
        using var a = new Region(new Rectangle(0, 0, 40, 40));
        using var b = new Region(new Rectangle(20, 20, 40, 40));

        a.Intersect(b);

        Assert.True(a.IsVisible(new Point(30, 30)), "the shared area stays visible");
        Assert.False(a.IsVisible(new Point(5, 5)), "area outside the intersection is dropped");
    }

    [Fact]
    public void Exclude_WithRegion_RemovesThatArea()
    {
        using var a = new Region(new Rectangle(0, 0, 40, 40));
        using var b = new Region(new Rectangle(0, 0, 20, 40));

        a.Exclude(b);

        Assert.False(a.IsVisible(new Point(5, 20)), "the excluded half is gone");
        Assert.True(a.IsVisible(new Point(30, 20)), "the remaining half stays");
    }

    [Fact]
    public void Complement_OfAnInfiniteRegion_IsEmpty()
    {
        // Complement keeps the portion of the argument that does NOT intersect this region. The infinite
        // region covers everything, so nothing of the argument survives. This test previously asserted the
        // opposite, matching an implementation that simply replaced the region with the argument.
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using var r = new Region();

        r.Complement(new Rectangle(10, 10, 20, 20));

        Assert.True(r.IsEmpty(g), "nothing lies outside the infinite region");
        Assert.False(r.IsVisible(new Point(15, 15)), "the supplied area does not become the region");
    }

    [Fact]
    public void Complement_OfAFiniteRegion_KeepsThePartOfTheArgumentOutsideIt()
    {
        using var r = new Region(new Rectangle(0, 0, 20, 20));

        r.Complement(new Rectangle(10, 0, 20, 20));

        Assert.False(r.IsVisible(new Point(5, 10)), "the original region is not part of the result");
        Assert.False(r.IsVisible(new Point(15, 10)), "the overlap is not part of the result");
        Assert.True(r.IsVisible(new Point(25, 10)), "the part of the argument outside the region survives");
    }

    [Fact]
    public void IsInfinite_TrueForADefaultRegion()
    {
        using var s = new Surface();
        using var r = new Region();

        Assert.True(r.IsInfinite(s.G), "a default Region covers everything");
    }

    [Fact]
    public void Union_WithPath_AddsThePathArea()
    {
        using var r = new Region(new Rectangle(0, 0, 10, 10));
        using var path = new GraphicsPath();
        path.AddRectangle(new Rectangle(50, 50, 20, 20));

        r.Union(path);

        Assert.True(r.IsVisible(new Point(5, 5)), "the original area stays");
        Assert.True(r.IsVisible(new Point(60, 60)), "the path's area is added");
    }

    [Fact]
    public void GetRegionData_ReturnsSomethingForARealRegion()
    {
        using var r = new Region(new Rectangle(0, 0, 10, 10));
        var data = r.GetRegionData();

        Assert.NotNull(data);
    }
}
