extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using Font = gtk::System.Drawing.Font;
using StringFormat = gtk::System.Drawing.StringFormat;
using CharacterRange = gtk::System.Drawing.CharacterRange;
using CoordinateSpace = gtk::System.Drawing.Drawing2D.CoordinateSpace;

namespace System.Drawing;

/// <summary>Behaviour of the remaining <see cref="Graphics"/> API surface.</summary>
[Collection("NonParallelCollection")]
public class GraphicsApiFunctionalTests
{
    private sealed class Target : IDisposable
    {
        private readonly Bitmap _bmp = new(60, 60);
        public Graphics G { get; }
        public Target() => G = Graphics.FromImage(_bmp);
        public void Dispose() { try { G.Dispose(); } catch { } _bmp.Dispose(); }
    }

    [Fact]
    public void TransformPoints_AppliesTheCurrentTranslation()
    {
        using var t = new Target();
        var g = t.G;
        g.TranslateTransform(10, 20);

        var pts = new[] { new PointF(1, 2) };
        g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pts);

        Assert.Equal(11, pts[0].X, 3);
        Assert.Equal(22, pts[0].Y, 3);
    }

    [Fact]
    public void GetNearestColor_ReturnsTheSameColourOnAFullColourSurface()
    {
        using var t = new Target();
        var c = t.G.GetNearestColor(Color.FromArgb(255, 12, 34, 56));

        Assert.Equal(12, c.R);
        Assert.Equal(34, c.G);
        Assert.Equal(56, c.B);
    }

    [Fact]
    public void BeginContainer_EndContainer_IsolatesTransformChanges()
    {
        using var t = new Target();
        var g = t.G;

        var container = g.BeginContainer();
        g.TranslateTransform(30, 30);
        g.EndContainer(container);

        // After EndContainer the transform inside the container must not leak out.
        var pts = new[] { new PointF(0, 0) };
        g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pts);
        Assert.Equal(0, pts[0].X, 3);
        Assert.Equal(0, pts[0].Y, 3);
    }

    [Fact]
    public void MeasureCharacterRanges_ReturnsOneRegionPerRequestedRange()
    {
        using var t = new Target();
        using var fmt = new StringFormat();
        fmt.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, 2), new CharacterRange(2, 3) });

        var regions = t.G.MeasureCharacterRanges("abcde", new Font("Sans", 10f), new RectangleF(0, 0, 200, 50), fmt);

        Assert.NotNull(regions);
        Assert.Equal(2, regions.Length);
    }

    [Fact]
    public void MeasureString_WithBoundingWidth_WrapsToAGreaterHeight()
    {
        using var t = new Target();
        var font = new Font("Sans", 12f);
        const string text = "the quick brown fox jumps over the lazy dog";

        var unbounded = t.G.MeasureString(text, font);
        var wrapped = t.G.MeasureString(text, font, 60);

        Assert.True(wrapped.Height >= unbounded.Height,
            $"constraining the width should not reduce the measured height (unbounded={unbounded.Height}, wrapped={wrapped.Height})");
        Assert.True(wrapped.Width <= unbounded.Width + 0.01f,
            $"constraining the width should not make it wider (unbounded={unbounded.Width}, wrapped={wrapped.Width})");
    }

    [Fact]
    public void DrawString_IntoARectangle_PaintsWithinThatRectangle()
    {
        using var bmp = new Bitmap(60, 60);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.DrawString("Hi", new Font("Sans", 12f), new SolidBrush(Color.Black), new RectangleF(0, 0, 30, 20));
        }

        var paintedInside = false;
        for (var x = 0; x < 32 && !paintedInside; x++)
        {
            for (var y = 0; y < 24; y++)
            {
                var c = bmp.GetPixel(x, y);
                if (c.R < 200 || c.G < 200 || c.B < 200)
                {
                    paintedInside = true;
                    break;
                }
            }
        }

        Assert.True(paintedInside, "text drawn into a layout rectangle should mark pixels inside it");
    }
}
