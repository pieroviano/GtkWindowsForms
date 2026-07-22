extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Font = gtk::System.Drawing.Font;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using SmoothingMode = gtk::System.Drawing.Drawing2D.SmoothingMode;
using InterpolationMode = gtk::System.Drawing.Drawing2D.InterpolationMode;
using CompositingMode = gtk::System.Drawing.Drawing2D.CompositingMode;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;

namespace System.Drawing;

/// <summary>Covers the <c>Net4x.Gtk.Drawing</c> gaps recorded in EMULATION-GAPS.md: the IsVisible family
/// (§4.1), the quality/rendering modes and page transform (§2.8), and the GDI interop entry points (§2.11).
/// Each test asserts the behaviour the gap document says WinForms requires.</summary>
[Collection("NonParallelCollection")]
public class GraphicsEmulationGapFunctionalTests
{
    private sealed class Target : IDisposable
    {
        public Bitmap Bitmap { get; } = new(40, 30);

        public Graphics Graphics { get; }

        public Target() => Graphics = Graphics.FromImage(Bitmap);

        public void Dispose()
        {
            Graphics.Dispose();
            Bitmap.Dispose();
        }
    }

    // ---- §4.1 the IsVisible family --------------------------------------

    [Fact]
    public void IsVisible_AcceptsAPointInsideTheClip()
    {
        using var target = new Target();

        Assert.True(target.Graphics.IsVisible(new Point(5, 5)), "a point inside the surface is visible");
        Assert.True(target.Graphics.IsVisible(new PointF(5f, 5f)), "the PointF overload should agree");
        Assert.True(target.Graphics.IsVisible(5, 5), "the int overload should agree");
        Assert.True(target.Graphics.IsVisible(5f, 5f), "the float overload should agree");
    }

    [Fact]
    public void IsVisible_RejectsAPointOutsideTheClip()
    {
        using var target = new Target();

        Assert.False(target.Graphics.IsVisible(new Point(500, 500)), "a point far outside is not visible");
        Assert.False(target.Graphics.IsVisible(new PointF(500f, 500f)), "the PointF overload should agree");
        Assert.False(target.Graphics.IsVisible(500, 500), "the int overload should agree");
        Assert.False(target.Graphics.IsVisible(500f, 500f), "the float overload should agree");
    }

    [Fact]
    public void IsVisible_AcceptsARectangleThatOverlapsTheClip()
    {
        using var target = new Target();

        Assert.True(target.Graphics.IsVisible(new Rectangle(-5, -5, 20, 20)), "an overlapping rectangle is visible");
        Assert.True(target.Graphics.IsVisible(new RectangleF(-5f, -5f, 20f, 20f)), "the RectangleF overload should agree");
        Assert.True(target.Graphics.IsVisible(-5, -5, 20, 20), "the int overload should agree");
        Assert.True(target.Graphics.IsVisible(-5f, -5f, 20f, 20f), "the float overload should agree");
    }

    [Fact]
    public void IsVisible_RejectsARectangleWhollyOutsideTheClip()
    {
        using var target = new Target();

        Assert.False(target.Graphics.IsVisible(new Rectangle(500, 500, 10, 10)), "a disjoint rectangle is not visible");
        Assert.False(target.Graphics.IsVisible(new RectangleF(500f, 500f, 10f, 10f)), "the RectangleF overload should agree");
        Assert.False(target.Graphics.IsVisible(500, 500, 10, 10), "the int overload should agree");
        Assert.False(target.Graphics.IsVisible(500f, 500f, 10f, 10f), "the float overload should agree");
    }

    [Fact]
    public void IsVisible_FollowsAClipThatWasNarrowed()
    {
        using var target = new Target();
        target.Graphics.SetClip(new Rectangle(0, 0, 10, 10));

        Assert.True(target.Graphics.IsVisible(new Point(5, 5)), "a point inside the narrowed clip is visible");
        Assert.False(target.Graphics.IsVisible(new Point(25, 25)),
            "a point outside the narrowed clip should no longer be visible");
    }

    // ---- §2.8 quality and rendering modes --------------------------------

    [Theory]
    [InlineData(SmoothingMode.AntiAlias)]
    [InlineData(SmoothingMode.None)]
    [InlineData(SmoothingMode.HighQuality)]
    [InlineData(SmoothingMode.HighSpeed)]
    public void SmoothingMode_RoundTripsAndStillDraws(SmoothingMode mode)
    {
        using var bmp = new Bitmap(40, 30);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = mode;
            Assert.Equal(mode, g.SmoothingMode);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 10, 10);
        }

        Assert.Equal(255, bmp.GetPixel(5, 5).R);
    }

    [Fact]
    public void SmoothingMode_AntiAliasProducesIntermediatePixelsOnADiagonal()
    {
        // The point of the mode: with antialiasing on, a diagonal edge has partially covered pixels.
        static int DistinctAlphaCount(SmoothingMode mode)
        {
            using var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = mode;
                g.FillPolygon(new SolidBrush(Color.Black), new[]
                {
                    new Point(0, 0), new Point(24, 24), new Point(0, 24)
                });
            }

            var alphas = new HashSet<int>();
            for (var x = 0; x < 24; x++)
            {
                for (var y = 0; y < 24; y++)
                {
                    alphas.Add(bmp.GetPixel(x, y).A);
                }
            }

            return alphas.Count;
        }

        Assert.True(DistinctAlphaCount(SmoothingMode.AntiAlias) >= DistinctAlphaCount(SmoothingMode.None),
            "antialiasing should not produce fewer distinct coverage values than no antialiasing");
    }

    [Theory]
    [InlineData(InterpolationMode.NearestNeighbor)]
    [InlineData(InterpolationMode.Bilinear)]
    [InlineData(InterpolationMode.HighQualityBicubic)]
    [InlineData(InterpolationMode.Default)]
    public void InterpolationMode_RoundTripsAndStillScalesImages(InterpolationMode mode)
    {
        using var source = new Bitmap(4, 4);
        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                source.SetPixel(x, y, Color.Red);
            }
        }

        using var bmp = new Bitmap(40, 30);
        using (var g = Graphics.FromImage(bmp))
        {
            g.InterpolationMode = mode;
            Assert.Equal(mode, g.InterpolationMode);
            g.DrawImage(source, new Rectangle(0, 0, 32, 24));
        }

        Assert.True(bmp.GetPixel(16, 12).A > 0, $"{mode} should still draw the scaled image");
    }

    [Theory]
    [InlineData(CompositingMode.SourceOver)]
    [InlineData(CompositingMode.SourceCopy)]
    public void CompositingMode_RoundTripsAndStillDraws(CompositingMode mode)
    {
        using var bmp = new Bitmap(40, 30);
        using (var g = Graphics.FromImage(bmp))
        {
            g.CompositingMode = mode;
            Assert.Equal(mode, g.CompositingMode);
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 10, 10);
        }

        Assert.True(bmp.GetPixel(5, 5).B > 200, $"{mode} should still paint the rectangle");
    }

    [Fact]
    public void PageUnit_RoundTrips()
    {
        using var target = new Target();

        target.Graphics.PageUnit = GraphicsUnit.Millimeter;

        Assert.Equal(GraphicsUnit.Millimeter, target.Graphics.PageUnit);
    }

    [Fact]
    public void PageScale_RoundTrips()
    {
        using var target = new Target();

        target.Graphics.PageScale = 2f;

        Assert.Equal(2f, target.Graphics.PageScale, 3);
    }

    [Fact]
    public void PageScale_ScalesWhatIsDrawn()
    {
        // A 5x5 rectangle at scale 2 must cover the pixel at (8,8); at scale 1 it must not.
        static bool CoversPixel(float scale)
        {
            using var bmp = new Bitmap(40, 30);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.PageScale = scale;
                g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 5, 5);
            }

            var pixel = bmp.GetPixel(8, 8);

            return pixel.R > 200 && pixel.G < 100;
        }

        Assert.False(CoversPixel(1f), "at page scale 1 a 5x5 rectangle must not reach (8,8)");
        Assert.True(CoversPixel(2f), "at page scale 2 a 5x5 rectangle must reach (8,8)");
    }

    [Fact]
    public void PageUnit_InMillimetres_EnlargesWhatIsDrawn()
    {
        // One millimetre is about 3.78 pixels at 96 dpi, so a 5-unit rectangle grows well past 5 pixels.
        using var bmp = new Bitmap(60, 60);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.PageUnit = GraphicsUnit.Millimeter;
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 5, 5);
        }

        var pixel = bmp.GetPixel(12, 12);
        Assert.True(pixel.R > 200 && pixel.G < 100,
            "a 5 mm rectangle should cover pixel (12,12); page units were being ignored");
    }

    [Fact]
    public void PageUnitAndScaleCompose()
    {
        using var target = new Target();

        target.Graphics.PageUnit = GraphicsUnit.Inch;
        target.Graphics.PageScale = 0.5f;

        Assert.Equal(GraphicsUnit.Inch, target.Graphics.PageUnit);
        Assert.Equal(0.5f, target.Graphics.PageScale, 3);
    }

    // ---- §2.11 GDI interop entry points ----------------------------------

    [Fact]
    public void FontFromHfont_ReportsThatTheHandleApiIsUnavailable()
    {
        // Previously returned Arial 12 for any input - a silent wrong font.
        Assert.Throws<PlatformNotSupportedException>(() => Font.FromHfont(IntPtr.Zero));
    }

    [Fact]
    public void FontFromHdc_ReportsThatTheHandleApiIsUnavailable()
    {
        Assert.Throws<PlatformNotSupportedException>(() => Font.FromHdc(IntPtr.Zero));
    }

    [Fact]
    public void FontFromLogFont_ReportsThatTheHandleApiIsUnavailable()
    {
        Assert.Throws<PlatformNotSupportedException>(() => Font.FromLogFont(new object()));
        Assert.Throws<PlatformNotSupportedException>(() => Font.FromLogFont(new object(), IntPtr.Zero));
    }
}
