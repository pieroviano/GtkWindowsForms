extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using Brush = gtk::System.Drawing.Brush;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Region = gtk::System.Drawing.Region;
using LinearGradientBrush = gtk::System.Drawing.Drawing2D.LinearGradientBrush;
using PathGradientBrush = gtk::System.Drawing.Drawing2D.PathGradientBrush;
using HatchBrush = gtk::System.Drawing.Drawing2D.HatchBrush;
using HatchStyle = gtk::System.Drawing.Drawing2D.HatchStyle;

namespace System.Drawing;

/// <summary>
/// Every brush kind must actually paint through every fill primitive, and a pen built from any brush must
/// stroke. This exercises the brush-selection branches of the renderer.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsBrushKindsFunctionalTests
{
    private const int Size = 40;
    private static readonly Rectangle Area = new(4, 4, 32, 32);

    private static int Ink(Action<Graphics> draw)
    {
        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        var n = 0;
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                var c = bmp.GetPixel(x, y);
                if (c.R < 200 || c.G < 200 || c.B < 200)
                {
                    n++;
                }
            }
        }

        return n;
    }

    public static IEnumerable<object[]> BrushKinds()
    {
        yield return new object[] { "SolidBrush", (Func<Brush>)(() => new SolidBrush(Color.Red)) };
        yield return new object[] { "LinearGradientBrush", (Func<Brush>)(() => new LinearGradientBrush(Area, Color.Red, Color.Blue, 45f)) };
        yield return new object[] { "HatchBrush", (Func<Brush>)(() => new HatchBrush(HatchStyle.Cross, Color.Red, Color.Blue)) };
        yield return new object[] { "PathGradientBrush", (Func<Brush>)(() => new PathGradientBrush(new[]
        {
            new Point(4, 4), new Point(36, 4), new Point(36, 36), new Point(4, 36)
        })
        {
            CenterColor = Color.Red,
            SurroundColors = new[] { Color.Blue }
        }) };
    }

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillRectangle_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(Ink(g => g.FillRectangle(make(), Area)) > 0, $"FillRectangle painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillEllipse_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(Ink(g => g.FillEllipse(make(), Area)) > 0, $"FillEllipse painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillPolygon_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(
            Ink(g => g.FillPolygon(make(), new[] { new Point(20, 4), new Point(36, 34), new Point(4, 34) })) > 0,
            $"FillPolygon painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillPath_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(
            Ink(g =>
            {
                using var p = new GraphicsPath();
                p.AddEllipse(Area);
                g.FillPath(make(), p);
            }) > 0,
            $"FillPath painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillPie_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(Ink(g => g.FillPie(make(), 4, 4, 32, 32, 0, 150)) > 0, $"FillPie painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void FillRegion_PaintsWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(
            Ink(g =>
            {
                using var r = new Region(Area);
                g.FillRegion(make(), r);
            }) > 0,
            $"FillRegion painted nothing with {name}");

    [Theory]
    [MemberData(nameof(BrushKinds))]
    public void PenBuiltFromBrush_StrokesWithEveryBrushKind(string name, Func<Brush> make) =>
        Assert.True(Ink(g => g.DrawRectangle(new Pen(make(), 3f), Area)) > 0,
            $"a pen built from {name} stroked nothing");
}
