extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Region = gtk::System.Drawing.Region;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using CombineMode = gtk::System.Drawing.Drawing2D.CombineMode;

namespace System.Drawing;

/// <summary>
/// Behavioural tests for <see cref="Graphics" />' clipping.
/// <para>
/// <c>Clip</c> was an auto-property assigned once in the constructor: no clip mutator ever wrote to it, so
/// the getter reported the full surface however much had been clipped, and the setter stored a region that
/// was never pushed down to Cairo - assigning a clip did nothing. Separately, only Replace and Intersect
/// were honoured; Union, Exclude and Xor silently behaved like Intersect.
/// </para>
/// The oracle for "did it actually clip" is the pixels, not the reported state.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsClipFunctionalTests
{
    private const int Size = 40;

    /// <summary>
    /// Runs <paramref name="paint" /> against a white <see cref="Size" />-square bitmap through a single
    /// <see cref="Graphics" />, then hands the finished bitmap back for pixel inspection. The white ground
    /// is laid down inside the same Graphics, before any clip is set, so the clip under test is the only
    /// thing restricting the paint.
    /// </summary>
    private static Bitmap Paint(Action<Graphics> paint)
    {
        var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            paint(g);
        }

        return bmp;
    }

    private static void FillEverything(Graphics g) =>
        g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);

    private static bool IsRed(Color c) => c.R > 200 && c.G < 80 && c.B < 80;

    private static bool IsWhite(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    [Fact]
    public void SetClip_RestrictsWhatIsPainted()
    {
        using var bmp = Paint(g =>
        {
            g.SetClip(new Rectangle(0, 0, 20, 20));
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(10, 10)), "inside the clip the fill must land");
        Assert.True(IsWhite(bmp.GetPixel(30, 30)), "outside the clip the fill must be suppressed");
    }

    [Fact]
    public void Clip_Getter_ReflectsWhatWasClipped()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);

        g.SetClip(new Rectangle(5, 5, 10, 10));

        var bounds = g.Clip.GetBounds(g);
        Assert.Equal(5f, bounds.X, 3);
        Assert.Equal(5f, bounds.Y, 3);
        Assert.Equal(10f, bounds.Width, 3);
        Assert.Equal(10f, bounds.Height, 3);
    }

    [Fact]
    public void Clip_Setter_ActuallyClips()
    {
        using var bmp = Paint(g =>
        {
            // The property assignment is the idiom under test: it used to store the region and clip nothing.
            g.Clip = new Region(new Rectangle(0, 0, 20, 20));
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(10, 10)));
        Assert.True(IsWhite(bmp.GetPixel(30, 30)), "assigning Graphics.Clip must restrict painting");
    }

    [Fact]
    public void IntersectClip_NarrowsAnExistingClip()
    {
        using var bmp = Paint(g =>
        {
            g.SetClip(new Rectangle(0, 0, 30, 30));
            g.IntersectClip(new Rectangle(0, 0, 15, 15));
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(7, 7)), "inside both rectangles");
        Assert.True(IsWhite(bmp.GetPixel(22, 22)), "inside the first clip only - the intersection must win");
    }

    [Fact]
    public void ResetClip_RestoresTheFullSurface()
    {
        using var bmp = Paint(g =>
        {
            g.SetClip(new Rectangle(0, 0, 10, 10));
            g.ResetClip();
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(35, 35)), "after ResetClip the whole surface must paint again");
    }

    [Fact]
    public void ResetClip_RestoresTheReportedClip()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);

        g.SetClip(new Rectangle(0, 0, 10, 10));
        g.ResetClip();

        Assert.Equal(Size, g.Clip.GetBounds(g).Width, 3);
        Assert.Equal(Size, g.ClipBounds.Width, 3);
    }

    [Fact]
    public void SetClip_Exclude_CutsAHoleInsteadOfIntersecting()
    {
        using var bmp = Paint(g =>
        {
            // Exclude used to be treated as Intersect, which would have kept only the hole.
            g.SetClip(new Region(new Rectangle(10, 10, 20, 20)), CombineMode.Exclude);
            FillEverything(g);
        });

        Assert.True(IsWhite(bmp.GetPixel(20, 20)), "the excluded rectangle must not paint");
        Assert.True(IsRed(bmp.GetPixel(2, 2)), "everything outside the excluded rectangle must still paint");
    }

    [Fact]
    public void SetClip_Union_WidensTheClip()
    {
        using var bmp = Paint(g =>
        {
            g.SetClip(new Rectangle(0, 0, 10, 10));
            g.SetClip(new Region(new Rectangle(30, 30, 10, 10)), CombineMode.Union);
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(5, 5)), "the original clip must still paint");
        Assert.True(IsRed(bmp.GetPixel(35, 35)), "the unioned rectangle must paint too");
        Assert.True(IsWhite(bmp.GetPixel(20, 20)), "the gap between them must stay unpainted");
    }

    [Fact]
    public void Clip_SurvivesAsRegionState_AcrossCombines()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);

        g.SetClip(new Rectangle(0, 0, 10, 10));
        g.SetClip(new Region(new Rectangle(30, 30, 10, 10)), CombineMode.Union);

        var bounds = g.Clip.GetBounds(g);
        Assert.Equal(0f, bounds.X, 3);
        Assert.Equal(40f, bounds.Width, 3);
        Assert.False(g.Clip.IsEmpty(g));
    }

    [Fact]
    public void SetClip_FromAnotherGraphics_CopiesTheClip()
    {
        using var source = new Bitmap(Size, Size);
        using var sourceGraphics = Graphics.FromImage(source);
        sourceGraphics.SetClip(new Rectangle(0, 0, 12, 12));

        using var target = Paint(g =>
        {
            g.SetClip(sourceGraphics);
            FillEverything(g);
        });

        Assert.True(IsRed(target.GetPixel(6, 6)));
        Assert.True(IsWhite(target.GetPixel(20, 20)), "the copied clip must restrict the target too");
    }

    [Fact]
    public void ClipBounds_TracksTheClip()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);

        g.SetClip(new Rectangle(4, 6, 8, 10));

        Assert.Equal(4f, g.ClipBounds.X, 3);
        Assert.Equal(6f, g.ClipBounds.Y, 3);
        Assert.Equal(8f, g.ClipBounds.Width, 3);
        Assert.Equal(10f, g.ClipBounds.Height, 3);
    }
}
