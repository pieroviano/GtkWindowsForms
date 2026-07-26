extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;
using SmoothingMode = gtk::System.Drawing.Drawing2D.SmoothingMode;
using InterpolationMode = gtk::System.Drawing.Drawing2D.InterpolationMode;
using CompositingMode = gtk::System.Drawing.Drawing2D.CompositingMode;
using CompositingQuality = gtk::System.Drawing.Drawing2D.CompositingQuality;
using PixelOffsetMode = gtk::System.Drawing.Drawing2D.PixelOffsetMode;
using TextRenderingHint = gtk::System.Drawing.Text.TextRenderingHint;

namespace System.Drawing;

/// <summary>State/property behaviour of <see cref="Graphics"/>: settings round-trip and clip bounds track
/// the clip that was set.</summary>
[Collection("NonParallelCollection")]
public class GraphicsStateFunctionalTests
{
    private sealed class Target : IDisposable
    {
        private readonly Bitmap _bmp = new(40, 40);
        public Graphics G { get; }
        public Target() => G = Graphics.FromImage(_bmp);
        public void Dispose() { try { G.Dispose(); } catch { } _bmp.Dispose(); }
    }

    [Fact]
    public void RenderingQualitySettings_RoundTrip()
    {
        using var t = new Target();
        var g = t.G;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.CompositingMode = CompositingMode.SourceCopy;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        Assert.Equal(SmoothingMode.AntiAlias, g.SmoothingMode);
        Assert.Equal(InterpolationMode.HighQualityBicubic, g.InterpolationMode);
        Assert.Equal(CompositingMode.SourceCopy, g.CompositingMode);
        Assert.Equal(CompositingQuality.HighQuality, g.CompositingQuality);
        Assert.Equal(PixelOffsetMode.Half, g.PixelOffsetMode);
        Assert.Equal(TextRenderingHint.AntiAlias, g.TextRenderingHint);
    }

    [Fact]
    public void PageUnitAndScale_RoundTrip()
    {
        using var t = new Target();
        var g = t.G;

        g.PageUnit = GraphicsUnit.Millimeter;
        g.PageScale = 2.5f;

        Assert.Equal(GraphicsUnit.Millimeter, g.PageUnit);
        Assert.Equal(2.5f, g.PageScale, 3);
    }

    [Fact]
    public void Dpi_IsPositive()
    {
        using var t = new Target();

        Assert.True(t.G.DpiX > 0, $"DpiX should be positive but was {t.G.DpiX}");
        Assert.True(t.G.DpiY > 0, $"DpiY should be positive but was {t.G.DpiY}");
    }

    [Fact]
    public void ClipBounds_ReflectTheRectangleThatWasSet()
    {
        using var t = new Target();
        var g = t.G;

        g.SetClip(new Rectangle(5, 6, 20, 10));

        var b = g.ClipBounds;
        Assert.Equal(5, b.X, 3);
        Assert.Equal(6, b.Y, 3);
        Assert.Equal(20, b.Width, 3);
        Assert.Equal(10, b.Height, 3);
    }

    [Fact]
    public void ResetClip_RestoresTheFullSurfaceBounds()
    {
        using var t = new Target();
        var g = t.G;

        g.SetClip(new Rectangle(0, 0, 5, 5));
        g.ResetClip();

        var b = g.ClipBounds;
        Assert.True(b.Width >= 40, $"after ResetClip the clip should cover the surface but width was {b.Width}");
        Assert.True(b.Height >= 40, $"after ResetClip the clip should cover the surface but height was {b.Height}");
    }

    [Fact]
    public void IsClipEmpty_IsFalseForANormalSurface()
    {
        using var t = new Target();
        Assert.False(t.G.IsClipEmpty);
    }
}
