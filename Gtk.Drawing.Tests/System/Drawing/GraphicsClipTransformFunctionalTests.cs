extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

/// <summary>Verifies clipping and the coordinate transforms actually affect where drawing lands.</summary>
[Collection("NonParallelCollection")]
public class GraphicsClipTransformFunctionalTests
{
    private const int Size = 40;

    private static Bitmap Render(Action<Graphics> draw)
    {
        var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        return bmp;
    }

    private static bool IsReddish(Color c) => c.R > 140 && c.G < 120 && c.B < 120;

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    [Fact]
    public void SetClip_RestrictsPaintingToTheClipRectangle()
    {
        using var bmp = Render(g =>
        {
            g.SetClip(new Rectangle(0, 0, 20, 20));
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.True(IsReddish(bmp.GetPixel(10, 10)), $"inside the clip was {bmp.GetPixel(10, 10)}");
        Assert.True(IsWhitish(bmp.GetPixel(30, 30)),
            $"outside the clip should stay unpainted but was {bmp.GetPixel(30, 30)}");
    }

    [Fact]
    public void ResetClip_RestoresPaintingToTheWholeSurface()
    {
        using var bmp = Render(g =>
        {
            g.SetClip(new Rectangle(0, 0, 10, 10));
            g.ResetClip();
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.True(IsReddish(bmp.GetPixel(30, 30)),
            $"after ResetClip the whole surface should paint but (30,30) was {bmp.GetPixel(30, 30)}");
    }

    [Fact]
    public void ScaleTransform_EnlargesSubsequentDrawing()
    {
        using var bmp = Render(g =>
        {
            g.ScaleTransform(3f, 3f);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 5, 5);
        });

        // 5x5 scaled by 3 covers 15x15, so (12,12) must be painted.
        Assert.True(IsReddish(bmp.GetPixel(12, 12)),
            $"scaled fill should cover (12,12) but it was {bmp.GetPixel(12, 12)}");
        Assert.True(IsWhitish(bmp.GetPixel(30, 30)), "the scaled fill should not reach (30,30)");
    }

    [Fact]
    public void Save_And_Restore_UndoesTheTransform()
    {
        using var bmp = Render(g =>
        {
            var state = g.Save();
            g.TranslateTransform(20, 20);
            g.Restore(state);
            // After Restore the translation is gone, so this lands at the origin again.
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 8, 8);
        });

        Assert.True(IsReddish(bmp.GetPixel(4, 4)),
            $"after Restore the fill should land at the origin but (4,4) was {bmp.GetPixel(4, 4)}");
        Assert.True(IsWhitish(bmp.GetPixel(24, 24)),
            "the translation should have been undone, so (24,24) must stay unpainted");
    }

    [Fact]
    public void IntersectClip_NarrowsAnExistingClip()
    {
        using var bmp = Render(g =>
        {
            g.SetClip(new Rectangle(0, 0, 30, 30));
            g.IntersectClip(new Rectangle(0, 0, 12, 12));
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.True(IsReddish(bmp.GetPixel(5, 5)), "inside the intersected clip should paint");
        Assert.True(IsWhitish(bmp.GetPixel(20, 20)),
            $"outside the intersected clip should stay unpainted but was {bmp.GetPixel(20, 20)}");
    }

    [Fact]
    public void DrawEllipse_And_DrawPie_MarkTheSurface()
    {
        using var ellipse = Render(g => g.DrawEllipse(new Pen(Color.Red, 2f), 5, 5, 30, 30));
        using var pie = Render(g => g.FillPie(new SolidBrush(Color.Red), 0, 0, 40, 40, 0, 90));

        // The ellipse outline crosses the horizontal centre line at the left edge of the bounding box.
        Assert.True(IsReddish(ellipse.GetPixel(5, 20)) || IsReddish(ellipse.GetPixel(6, 20)),
            "the ellipse outline should cross the left of its bounding box");

        // A 0..90 degree pie from the centre covers the right-down quadrant near the centre.
        Assert.True(IsReddish(pie.GetPixel(25, 25)), $"inside the filled pie was {pie.GetPixel(25, 25)}");
    }
}
