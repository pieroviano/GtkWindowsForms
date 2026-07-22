extern alias gtk;
using Region = gtk::System.Drawing.Region;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;

namespace System.Drawing;

/// <summary>Covers EMULATION-GAPS.md §2.7. The region was backed by a single <see cref="RectangleF"/>, so
/// every combine operation collapsed to rectangle arithmetic and a non-rectangular region was not
/// representable. These tests assert the cases that arithmetic gets wrong — the ones earlier tests in this
/// suite had to be written around and documented as "approximations".</summary>
[Collection("NonParallelCollection")]
public class RegionBandsFunctionalTests
{
    // Declaration order matters: the Graphics must be disposed before the Bitmap it flushes into.
    // ---- the L-shape: the case a bounding box cannot express ---------------

    /// <summary>A 40x40 square with its bottom-right 20x20 quadrant removed.</summary>
    private static Region LShape()
    {
        var region = new Region(new Rectangle(0, 0, 40, 40));
        region.Exclude(new Rectangle(20, 20, 20, 20));

        return region;
    }

    [Fact]
    public void ExcludingAQuadrantLeavesAnLShapeWithAGenuineHole()
    {
        using var region = LShape();

        Assert.True(region.IsVisible(new Point(10, 10)), "the top-left quadrant survives");
        Assert.True(region.IsVisible(new Point(30, 10)), "the top-right quadrant survives");
        Assert.True(region.IsVisible(new Point(10, 30)), "the bottom-left quadrant survives");
        Assert.False(region.IsVisible(new Point(30, 30)),
            "the excluded quadrant must not be visible - a bounding-box region reports it as inside");
    }

    [Fact]
    public void TheLShapeStillReportsTheFullBoundingBox()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using (var region = LShape())
        {
            var bounds = region.GetBounds(g);

            Assert.Equal(0f, bounds.X, 2);
            Assert.Equal(40f, bounds.Width, 2);
            Assert.Equal(40f, bounds.Height, 2);
        }
    }

    [Fact]
    public void TheLShapeIsMadeOfSeveralScans()
    {
        using var region = LShape();
        using var matrix = new Matrix();

        var scans = region.GetRegionScans(matrix);

        Assert.True(scans.Length > 1,
            $"an L-shaped region needs more than one scan rectangle but reported {scans.Length}");
    }

    [Fact]
    public void TheScansCoverTheRegionAndNotTheHole()
    {
        using var region = LShape();
        using var matrix = new Matrix();

        var scans = region.GetRegionScans(matrix);

        Assert.Contains(scans, s => s.Contains(new PointF(10f, 10f)));
        Assert.DoesNotContain(scans, s => s.Contains(new PointF(30f, 30f)));
    }

    // ---- Xor: previously returned the union ------------------------------

    [Fact]
    public void Xor_OfPartiallyOverlappingRectangles_RemovesTheOverlap()
    {
        // The old implementation returned the union of the two bounding boxes here, so the overlap - the
        // one part that must be excluded - was reported as inside.
        using var region = new Region(new Rectangle(0, 0, 20, 20));

        region.Xor(new Rectangle(10, 0, 20, 20));

        Assert.True(region.IsVisible(new Point(5, 10)), "the part only in the first rectangle survives");
        Assert.True(region.IsVisible(new Point(25, 10)), "the part only in the second rectangle survives");
        Assert.False(region.IsVisible(new Point(15, 10)), "the overlap must be excluded by an exclusive-or");
    }

    [Fact]
    public void Xor_OfIdenticalRectangles_IsEmpty()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using (var region = new Region(new Rectangle(0, 0, 20, 20)))
        {
            region.Xor(new Rectangle(0, 0, 20, 20));

            Assert.True(region.IsEmpty(g), "a region xored with itself is empty");
        }
    }

    [Fact]
    public void Xor_OfDisjointRectangles_KeepsBoth()
    {
        using var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Xor(new Rectangle(50, 50, 10, 10));

        Assert.True(region.IsVisible(new Point(5, 5)), "the first rectangle survives");
        Assert.True(region.IsVisible(new Point(55, 55)), "the second rectangle survives");
        Assert.False(region.IsVisible(new Point(30, 30)), "the gap between them is not part of the region");
    }

    [Fact]
    public void Xor_IsItsOwnInverse()
    {
        using var region = new Region(new Rectangle(0, 0, 20, 20));

        region.Xor(new Rectangle(10, 0, 20, 20));
        region.Xor(new Rectangle(10, 0, 20, 20));

        Assert.True(region.IsVisible(new Point(5, 10)), "the original area is restored");
        Assert.True(region.IsVisible(new Point(15, 10)), "the overlap comes back");
        Assert.False(region.IsVisible(new Point(25, 10)), "nothing outside the original survives");
    }

    // ---- Exclude: previously shrank the bounding box ----------------------

    [Fact]
    public void Exclude_TakingABiteOutOfTheMiddleLeavesAHole()
    {
        using var region = new Region(new Rectangle(0, 0, 30, 30));

        region.Exclude(new Rectangle(10, 10, 10, 10));

        Assert.False(region.IsVisible(new Point(15, 15)), "the bite is gone");
        Assert.True(region.IsVisible(new Point(5, 15)), "the left of the bite survives");
        Assert.True(region.IsVisible(new Point(25, 15)), "the right of the bite survives");
        Assert.True(region.IsVisible(new Point(15, 5)), "above the bite survives");
        Assert.True(region.IsVisible(new Point(15, 25)), "below the bite survives");
    }

    [Fact]
    public void Exclude_OfADisjointRectangleChangesNothing()
    {
        using var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Exclude(new Rectangle(50, 50, 10, 10));

        Assert.True(region.IsVisible(new Point(5, 5)), "an unrelated exclusion leaves the region alone");
    }

    [Fact]
    public void Exclude_OfTheWholeRegionEmptiesIt()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using (var region = new Region(new Rectangle(0, 0, 10, 10)))
        {
            region.Exclude(new Rectangle(0, 0, 10, 10));

            Assert.True(region.IsEmpty(g), "excluding the whole region empties it");
        }
    }

    // ---- Union: previously merged bounding boxes -------------------------

    [Fact]
    public void Union_OfDisjointRectanglesDoesNotFillTheGap()
    {
        // A bounding-box union would report the gap between them as inside the region.
        using var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Union(new Rectangle(50, 50, 10, 10));

        Assert.True(region.IsVisible(new Point(5, 5)), "the first rectangle is inside");
        Assert.True(region.IsVisible(new Point(55, 55)), "the second rectangle is inside");
        Assert.False(region.IsVisible(new Point(30, 30)), "the gap between them must not be inside");
    }

    [Fact]
    public void Union_OfOverlappingRectanglesCountsTheOverlapOnce()
    {
        using var region = new Region(new Rectangle(0, 0, 20, 20));
        region.Union(new Rectangle(10, 0, 20, 20));

        using var matrix = new Matrix();
        var scans = region.GetRegionScans(matrix);

        var area = 0f;
        foreach (var scan in scans)
        {
            area += scan.Width * scan.Height;
        }

        // 20x20 + 20x20 sharing a 10x20 overlap = 600, not 800.
        Assert.Equal(600f, area, 1);
    }

    // ---- Intersect --------------------------------------------------------

    [Fact]
    public void Intersect_KeepsOnlyTheSharedArea()
    {
        using var region = new Region(new Rectangle(0, 0, 20, 20));

        region.Intersect(new Rectangle(10, 10, 20, 20));

        Assert.True(region.IsVisible(new Point(15, 15)), "the shared area survives");
        Assert.False(region.IsVisible(new Point(5, 5)), "the part only in the first rectangle is gone");
        Assert.False(region.IsVisible(new Point(25, 25)), "the part only in the second rectangle is gone");
    }

    [Fact]
    public void Intersect_WithAnLShapeRespectsTheHole()
    {
        using var region = LShape();

        region.Intersect(new Rectangle(20, 20, 20, 20));

        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);

        Assert.True(region.IsEmpty(g), "intersecting an L-shape with its own hole must be empty");
    }

    // ---- region-vs-region combines are exact ------------------------------

    [Fact]
    public void RegionVsRegion_XorMatchesTheRectangleOverload()
    {
        using var viaRect = new Region(new Rectangle(0, 0, 20, 20));
        viaRect.Xor(new Rectangle(10, 0, 20, 20));

        using var viaRegion = new Region(new Rectangle(0, 0, 20, 20));
        using var operand = new Region(new Rectangle(10, 0, 20, 20));
        viaRegion.Xor(operand);

        foreach (var probe in new[] { new Point(5, 10), new Point(15, 10), new Point(25, 10) })
        {
            Assert.Equal(viaRect.IsVisible(probe), viaRegion.IsVisible(probe));
        }
    }

    [Fact]
    public void RegionVsRegion_ExcludingAnLShapeIsExact()
    {
        using var region = new Region(new Rectangle(0, 0, 40, 40));
        using var hole = LShape();

        region.Exclude(hole);

        Assert.False(region.IsVisible(new Point(10, 10)), "the L-shape is removed");
        Assert.True(region.IsVisible(new Point(30, 30)),
            "the L-shape's own hole is not part of it, so it survives the exclusion");
    }

    [Fact]
    public void RegionVsRegion_UnionOfTwoDisjointRegionsKeepsTheGapOut()
    {
        using var region = new Region(new Rectangle(0, 0, 10, 10));
        using var other = new Region(new Rectangle(50, 50, 10, 10));

        region.Union(other);

        Assert.True(region.IsVisible(new Point(5, 5)), "the first region is inside");
        Assert.True(region.IsVisible(new Point(55, 55)), "the second region is inside");
        Assert.False(region.IsVisible(new Point(30, 30)), "the gap is not inside");
    }

    // ---- translation and state -------------------------------------------

    [Fact]
    public void Translate_MovesEveryBandOfANonRectangularRegion()
    {
        using var region = LShape();

        region.Translate(100f, 0f);

        Assert.True(region.IsVisible(new Point(110, 10)), "the shape moved with the translation");
        Assert.False(region.IsVisible(new Point(130, 30)), "the hole moved too");
        Assert.False(region.IsVisible(new Point(10, 10)), "nothing is left at the old position");
    }

    [Fact]
    public void AnLShapeIsNeitherEmptyNorInfinite()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using var region = LShape();

        Assert.False(region.IsEmpty(g), "an L-shaped region is not empty");
        Assert.False(region.IsInfinite(g), "an L-shaped region is not infinite");
    }

    [Fact]
    public void MakeEmptyThenUnionRebuildsARealRegion()
    {
        using var bmp = new Bitmap(4, 4);
        using var g = Graphics.FromImage(bmp);
        using (var region = LShape())
        {
            region.MakeEmpty();
            Assert.True(region.IsEmpty(g), "the region is empty after MakeEmpty");

            region.Union(new Rectangle(0, 0, 10, 10));

            Assert.False(region.IsEmpty(g), "the region is no longer empty");
            Assert.True(region.IsVisible(new Point(5, 5)), "the unioned rectangle is inside");
        }
    }

    // ---- clipping uses the real shape -------------------------------------

    [Fact]
    public void SetClip_WithAnLShapedRegion_LeavesTheHoleUnpainted()
    {
        // The clip used to be reduced to the region's bounding rectangle, so the hole was painted anyway.
        using var bmp = new Bitmap(48, 48);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            using var region = LShape();
            g.SetClip(region);
            g.FillRectangle(new gtk::System.Drawing.SolidBrush(Color.Red), 0, 0, 48, 48);
        }

        static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

        Assert.False(IsWhitish(bmp.GetPixel(10, 10)), "the top-left quadrant is inside the clip and gets painted");
        Assert.False(IsWhitish(bmp.GetPixel(30, 10)), "the top-right quadrant gets painted");
        Assert.False(IsWhitish(bmp.GetPixel(10, 30)), "the bottom-left quadrant gets painted");
        Assert.True(IsWhitish(bmp.GetPixel(30, 30)),
            "the hole in the L-shape must stay unpainted; a bounding-box clip would have filled it");
    }

    [Fact]
    public void SetClip_WithARectangularRegion_StillClipsToThatRectangle()
    {
        using var bmp = new Bitmap(48, 48);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            using var region = new Region(new Rectangle(0, 0, 16, 16));
            g.SetClip(region);
            g.FillRectangle(new gtk::System.Drawing.SolidBrush(Color.Red), 0, 0, 48, 48);
        }

        static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

        Assert.False(IsWhitish(bmp.GetPixel(8, 8)), "inside the region is painted");
        Assert.True(IsWhitish(bmp.GetPixel(40, 40)), "outside the region stays clear");
    }

    [Fact]
    public void CloningAnLShapeCopiesTheHole()
    {
        using var original = LShape();
        using var clone = original.Clone();

        original.MakeEmpty();

        Assert.True(clone.IsVisible(new Point(10, 10)), "the clone keeps its area");
        Assert.False(clone.IsVisible(new Point(30, 30)), "the clone keeps its hole");
    }
}
