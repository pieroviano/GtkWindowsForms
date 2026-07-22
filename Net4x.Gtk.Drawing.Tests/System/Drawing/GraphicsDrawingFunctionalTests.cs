extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Region = gtk::System.Drawing.Region;
using Font = gtk::System.Drawing.Font;

namespace System.Drawing;

/// <summary>
/// Verifies each drawing primitive actually paints where it should, by rendering onto a bitmap and
/// inspecting the resulting pixels. Anti-aliasing means colours are compared with a tolerance.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsDrawingFunctionalTests
{
    private const int Size = 40;

    /// <summary>Renders onto a white 40x40 bitmap and returns it for pixel inspection.</summary>
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

    /// <summary>True when any pixel in the bitmap is clearly not the white background.</summary>
    private static bool HasNonBackgroundPixel(Bitmap bmp)
    {
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                if (!IsWhitish(bmp.GetPixel(x, y)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Fact]
    public void FillEllipse_PaintsCentre_ButNotTheCorner()
    {
        using var bmp = Render(g => g.FillEllipse(new SolidBrush(Color.Red), 5, 5, 30, 30));

        Assert.True(IsReddish(bmp.GetPixel(20, 20)), $"ellipse centre was {bmp.GetPixel(20, 20)}");
        Assert.True(IsWhitish(bmp.GetPixel(0, 0)), $"corner outside the ellipse was {bmp.GetPixel(0, 0)}");
    }

    [Fact]
    public void FillPolygon_PaintsInsideTheTriangle()
    {
        var triangle = new[] { new Point(20, 2), new Point(38, 38), new Point(2, 38) };
        using var bmp = Render(g => g.FillPolygon(new SolidBrush(Color.Red), triangle));

        Assert.True(IsReddish(bmp.GetPixel(20, 30)), $"inside the triangle was {bmp.GetPixel(20, 30)}");
        Assert.True(IsWhitish(bmp.GetPixel(2, 2)), $"outside the triangle was {bmp.GetPixel(2, 2)}");
    }

    [Fact]
    public void FillPath_PaintsTheEnclosedArea()
    {
        using var path = new GraphicsPath();
        path.AddRectangle(new Rectangle(10, 10, 20, 20));
        using var bmp = Render(g => g.FillPath(new SolidBrush(Color.Red), path));

        Assert.True(IsReddish(bmp.GetPixel(20, 20)), $"inside the path was {bmp.GetPixel(20, 20)}");
        Assert.True(IsWhitish(bmp.GetPixel(2, 2)), $"outside the path was {bmp.GetPixel(2, 2)}");
    }

    [Fact]
    public void FillRegion_PaintsTheRegionArea()
    {
        using var region = new Region(new Rectangle(10, 10, 20, 20));
        using var bmp = Render(g => g.FillRegion(new SolidBrush(Color.Red), region));

        Assert.True(IsReddish(bmp.GetPixel(20, 20)), $"inside the region was {bmp.GetPixel(20, 20)}");
        Assert.True(IsWhitish(bmp.GetPixel(2, 2)), $"outside the region was {bmp.GetPixel(2, 2)}");
    }

    [Fact]
    public void DrawLine_PaintsAlongTheLine_NotElsewhere()
    {
        using var bmp = Render(g => g.DrawLine(new Pen(Color.Red, 3f), 0, 20, 39, 20));

        Assert.True(IsReddish(bmp.GetPixel(20, 20)), $"on the line was {bmp.GetPixel(20, 20)}");
        Assert.True(IsWhitish(bmp.GetPixel(20, 2)), $"far from the line was {bmp.GetPixel(20, 2)}");
    }

    [Fact]
    public void DrawRectangle_PaintsTheOutline_LeavingTheInteriorUnpainted()
    {
        using var bmp = Render(g => g.DrawRectangle(new Pen(Color.Red, 2f), 8, 8, 24, 24));

        Assert.True(IsReddish(bmp.GetPixel(20, 8)), $"top edge was {bmp.GetPixel(20, 8)}");
        Assert.True(IsWhitish(bmp.GetPixel(20, 20)), $"interior of an outlined rectangle was {bmp.GetPixel(20, 20)}");
    }

    [Fact]
    public void DrawImage_CopiesSourcePixelsOntoTheTarget()
    {
        using var source = new Bitmap(8, 8);
        for (var x = 0; x < 8; x++)
        {
            for (var y = 0; y < 8; y++)
            {
                source.SetPixel(x, y, Color.Red);
            }
        }

        using var bmp = Render(g => g.DrawImage(source, 10, 10));

        Assert.True(IsReddish(bmp.GetPixel(13, 13)), $"pixel under the drawn image was {bmp.GetPixel(13, 13)}");
        Assert.True(IsWhitish(bmp.GetPixel(35, 35)), $"pixel away from the drawn image was {bmp.GetPixel(35, 35)}");
    }

    [Fact]
    public void DrawString_PutsInkOnTheSurface()
    {
        using var bmp = Render(g =>
            g.DrawString("Hello", new Font("Sans", 14f), new SolidBrush(Color.Black), new PointF(2, 2)));

        Assert.True(HasNonBackgroundPixel(bmp), "drawing text should mark at least one pixel");
    }

    [Fact]
    public void TranslateTransform_ShiftsWhereSubsequentDrawingLands()
    {
        using var untranslated = Render(g => g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 10, 10));
        using var translated = Render(g =>
        {
            g.TranslateTransform(20, 20);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 10, 10);
        });

        Assert.True(IsReddish(untranslated.GetPixel(5, 5)), "un-translated fill should land at the origin");
        Assert.True(IsWhitish(untranslated.GetPixel(25, 25)), "un-translated fill should not reach (25,25)");

        Assert.True(IsReddish(translated.GetPixel(25, 25)),
            $"after TranslateTransform(20,20) the fill should land at (25,25) but was {translated.GetPixel(25, 25)}");
    }
}
