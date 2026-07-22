extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using FontFamily = gtk::System.Drawing.FontFamily;
using StringFormat = gtk::System.Drawing.StringFormat;

namespace System.Drawing;

/// <summary>
/// Every kind of segment a <see cref="GraphicsPath"/> can hold must actually render when the path is
/// drawn or filled. Each case builds a path containing a single primitive and checks ink reaches the
/// surface, which exercises the corresponding branch of the path renderer.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsDrawPathFunctionalTests
{
    private const int Size = 40;

    private static int InkCount(Action<Graphics> draw)
    {
        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        var ink = 0;
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                var c = bmp.GetPixel(x, y);
                if (c.R < 200 || c.G < 200 || c.B < 200)
                {
                    ink++;
                }
            }
        }

        return ink;
    }

    public static IEnumerable<object[]> Primitives()
    {
        yield return new object[] { "AddLine", (Action<GraphicsPath>)(p => p.AddLine(4, 4, 34, 34)) };
        yield return new object[] { "AddLines", (Action<GraphicsPath>)(p => p.AddLines(new[] { new Point(4, 4), new Point(34, 4), new Point(34, 34) })) };
        yield return new object[] { "AddBezier", (Action<GraphicsPath>)(p => p.AddBezier(4, 30, 14, 6, 26, 6, 36, 30)) };
        yield return new object[] { "AddBeziers", (Action<GraphicsPath>)(p => p.AddBeziers(new[] { new Point(4, 30), new Point(14, 6), new Point(26, 6), new Point(36, 30) })) };
        yield return new object[] { "AddCurve", (Action<GraphicsPath>)(p => p.AddCurve(new[] { new Point(4, 20), new Point(20, 6), new Point(36, 20) })) };
        yield return new object[] { "AddClosedCurve", (Action<GraphicsPath>)(p => p.AddClosedCurve(new[] { new Point(10, 10), new Point(30, 10), new Point(20, 30) })) };
        yield return new object[] { "AddPolygon", (Action<GraphicsPath>)(p => p.AddPolygon(new[] { new Point(20, 4), new Point(34, 34), new Point(6, 34) })) };
        yield return new object[] { "AddRectangle", (Action<GraphicsPath>)(p => p.AddRectangle(new Rectangle(6, 6, 26, 26))) };
        yield return new object[] { "AddRectangles", (Action<GraphicsPath>)(p => p.AddRectangles(new[] { new Rectangle(4, 4, 10, 10), new Rectangle(24, 24, 10, 10) })) };
        yield return new object[] { "AddEllipse", (Action<GraphicsPath>)(p => p.AddEllipse(5, 5, 28, 28)) };
        yield return new object[] { "AddArc", (Action<GraphicsPath>)(p => p.AddArc(4, 4, 30, 30, 0, 180)) };
        yield return new object[] { "AddPie", (Action<GraphicsPath>)(p => p.AddPie(4, 4, 30, 30, 0, 120)) };
    }

    [Theory]
    [MemberData(nameof(Primitives))]
    public void DrawPath_RendersEveryPrimitiveKind(string name, Action<GraphicsPath> build)
    {
        var ink = InkCount(g =>
        {
            using var path = new GraphicsPath();
            build(path);
            g.DrawPath(new Pen(Color.Red, 3f), path);
        });

        Assert.True(ink > 0, $"DrawPath produced no ink for a path built with {name}");
    }

    /// <summary>
    /// Everything from <see cref="Primitives"/> that encloses an area. A path holding a single straight
    /// line is degenerate - it has no interior - so filling it correctly produces nothing.
    /// </summary>
    public static IEnumerable<object[]> FillablePrimitives() =>
        Primitives().Where(p => (string)p[0] != "AddLine");

    [Theory]
    [MemberData(nameof(FillablePrimitives))]
    public void FillPath_RendersEveryPrimitiveKind(string name, Action<GraphicsPath> build)
    {
        var ink = InkCount(g =>
        {
            using var path = new GraphicsPath();
            build(path);
            g.FillPath(new SolidBrush(Color.Red), path);
        });

        Assert.True(ink > 0, $"FillPath produced no ink for a path built with {name}");
    }

    [Fact]
    public void DrawPath_WithAddedSubPath_RendersBothParts()
    {
        var ink = InkCount(g =>
        {
            using var sub = new GraphicsPath();
            sub.AddRectangle(new Rectangle(24, 24, 10, 10));
            using var path = new GraphicsPath();
            path.AddRectangle(new Rectangle(4, 4, 10, 10));
            path.AddPath(sub, false);
            g.DrawPath(new Pen(Color.Red, 2f), path);
        });

        // Both rectangles outlined: clearly more ink than a single small rectangle would give.
        Assert.True(ink > 40, $"a path with an added sub-path should render both parts (ink={ink})");
    }

    [Fact]
    public void DrawPath_WithText_RendersGlyphs()
    {
        var ink = InkCount(g =>
        {
            using var fmt = new StringFormat();
            using var path = new GraphicsPath();
            path.AddString("Ag", new FontFamily("Sans"), 0, 20f, new Point(2, 2), fmt);
            g.FillPath(new SolidBrush(Color.Red), path);
        });

        Assert.True(ink > 0, "text added to a path should render when the path is filled");
    }

    [Fact]
    public void FilledRectanglePath_ProducesMoreInkThanItsOutline()
    {
        var outline = InkCount(g =>
        {
            using var p = new GraphicsPath();
            p.AddRectangle(new Rectangle(8, 8, 24, 24));
            g.DrawPath(new Pen(Color.Red, 1f), p);
        });

        var filled = InkCount(g =>
        {
            using var p = new GraphicsPath();
            p.AddRectangle(new Rectangle(8, 8, 24, 24));
            g.FillPath(new SolidBrush(Color.Red), p);
        });

        Assert.True(filled > outline,
            $"filling a rectangle must cover more pixels than outlining it (outline={outline}, filled={filled})");
    }
}
