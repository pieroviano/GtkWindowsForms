extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using Font = gtk::System.Drawing.Font;
using StringFormat = gtk::System.Drawing.StringFormat;
using Region = gtk::System.Drawing.Region;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Icon = gtk::System.Drawing.Icon;

namespace System.Drawing;

/// <summary>Text-drawing overloads must all put ink on the surface, and every clip overload must restrict
/// where painting lands.</summary>
[Collection("NonParallelCollection")]
public class GraphicsTextClipFunctionalTests
{
    private const int Size = 48;

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

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    private static int Ink(Action<Graphics> draw)
    {
        using var bmp = Render(draw);
        var n = 0;
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                if (!IsWhitish(bmp.GetPixel(x, y))) n++;
            }
        }

        return n;
    }

    private static Font TestFont() => new("Sans", 14f);

    private static SolidBrush Black() => new(Color.Black);

    [Fact]
    public void DrawString_FloatPositionOverload_PaintsText() =>
        Assert.True(Ink(g => g.DrawString("Ag", TestFont(), Black(), 2f, 2f)) > 0, "DrawString(x,y) drew nothing");

    [Fact]
    public void DrawString_PointFOverload_PaintsText() =>
        Assert.True(Ink(g => g.DrawString("Ag", TestFont(), Black(), new PointF(2, 2))) > 0, "DrawString(PointF) drew nothing");

    [Fact]
    public void DrawString_RectangleOverload_PaintsText() =>
        Assert.True(Ink(g => g.DrawString("Ag", TestFont(), Black(), new RectangleF(2, 2, 40, 20))) > 0, "DrawString(RectangleF) drew nothing");

    [Fact]
    public void DrawString_WithStringFormatOverloads_PaintText()
    {
        using var fmt = new StringFormat();
        Assert.True(Ink(g => g.DrawString("Ag", TestFont(), Black(), new RectangleF(2, 2, 40, 20), fmt)) > 0,
            "DrawString(RectangleF, StringFormat) drew nothing");
        Assert.True(Ink(g => g.DrawString("Ag", TestFont(), Black(), new PointF(2, 2), fmt)) > 0,
            "DrawString(PointF, StringFormat) drew nothing");
    }

    [Fact]
    public void SetClip_WithRegion_RestrictsPainting()
    {
        using var bmp = Render(g =>
        {
            using var region = new Region(new Rectangle(0, 0, 16, 16));
            g.SetClip(region);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.False(IsWhitish(bmp.GetPixel(8, 8)), "inside the region clip should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(40, 40)), "outside the region clip should stay clear");
    }

    [Fact]
    public void SetClip_WithGraphicsPath_RestrictsPainting()
    {
        using var bmp = Render(g =>
        {
            using var path = new GraphicsPath();
            path.AddRectangle(new Rectangle(0, 0, 16, 16));
            g.SetClip(path);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.False(IsWhitish(bmp.GetPixel(8, 8)), "inside the path clip should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(40, 40)), "outside the path clip should stay clear");
    }

    [Fact]
    public void ExcludeClip_PreventsPaintingInTheExcludedArea()
    {
        using var bmp = Render(g =>
        {
            g.ExcludeClip(new Rectangle(0, 0, 24, 24));
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        });

        Assert.True(IsWhitish(bmp.GetPixel(8, 8)), "the excluded area must stay unpainted");
        Assert.False(IsWhitish(bmp.GetPixel(40, 40)), "everything else should still paint");
    }

    [Fact]
    public void DrawIcon_PaintsTheIcon()
    {
        Assert.True(Ink(g =>
        {
            using var icon = new Icon("test.ico");
            g.DrawIcon(icon, 4, 4);
        }) > 0, "DrawIcon drew nothing");
    }

    [Fact]
    public void DrawIconUnstretched_PaintsTheIcon()
    {
        Assert.True(Ink(g =>
        {
            using var icon = new Icon("test.ico");
            g.DrawIconUnstretched(icon, new Rectangle(4, 4, 16, 16));
        }) > 0, "DrawIconUnstretched drew nothing");
    }
}
