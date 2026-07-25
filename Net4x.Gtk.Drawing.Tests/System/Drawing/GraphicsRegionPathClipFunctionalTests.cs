extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Region = gtk::System.Drawing.Region;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using CombineMode = gtk::System.Drawing.Drawing2D.CombineMode;

namespace System.Drawing;

/// <summary>
/// Behavioural tests for clipping with a <see cref="Region" /> built from a
/// <see cref="GraphicsPath" />.
/// <para>
/// The region's interior is stored as a band list - disjoint rectangles - and a path contributes only its
/// bounding rectangle to that list. Clipping through the bands therefore squared off every curve: clipping
/// to an ellipse clipped to its bounding box, so the corners outside the curve were painted. The region now
/// keeps the real path while it is still the exact description of the interior, and the clip is applied
/// through that path.
/// </para>
/// The oracle is the corner pixel: it lies inside the bounding box and outside the ellipse, so it
/// distinguishes the two behaviours - no reported state can.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsRegionPathClipFunctionalTests
{
    private const int Size = 40;

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

    private static GraphicsPath Ellipse()
    {
        var path = new GraphicsPath();
        path.AddEllipse(0, 0, Size, Size);

        return path;
    }

    [Fact]
    public void SetClip_WithEllipticalRegion_ClipsOnTheCurveNotTheBoundingBox()
    {
        using var bmp = Paint(g =>
        {
            g.SetClip(new Region(Ellipse()));
            FillEverything(g);
        });

        // Centre of the ellipse: inside under either behaviour.
        Assert.True(IsRed(bmp.GetPixel(Size / 2, Size / 2)), "the interior of the ellipse must be painted");

        // Top-left corner: inside the bounding box, well outside the inscribed ellipse.
        Assert.True(
            IsWhite(bmp.GetPixel(1, 1)),
            "the corner lies outside the ellipse and must not be painted - clipping used the bounding box");
    }

    [Fact]
    public void Clip_Setter_WithEllipticalRegion_ClipsOnTheCurve()
    {
        using var bmp = Paint(g =>
        {
            g.Clip = new Region(Ellipse());
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(Size / 2, Size / 2)));
        Assert.True(IsWhite(bmp.GetPixel(1, 1)), "assigning an elliptical Clip must clip on the curve");
    }

    [Fact]
    public void SetClip_WithPath_ClipsOnTheCurve()
    {
        using var bmp = Paint(g =>
        {
            // The GraphicsPath overload funnels through the Region one, so it gains the same exactness.
            g.SetClip(Ellipse());
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(Size / 2, Size / 2)));
        Assert.True(IsWhite(bmp.GetPixel(1, 1)));
    }

    [Fact]
    public void SetClip_WithRectangularRegion_StillClipsToTheRectangle()
    {
        // The band path is what a rectangular region uses, and it must be untouched by the path route.
        using var bmp = Paint(g =>
        {
            g.SetClip(new Region(new Rectangle(0, 0, 20, 20)));
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(10, 10)));
        Assert.True(IsWhite(bmp.GetPixel(30, 30)));
    }

    [Fact]
    public void CombinedEllipticalRegion_StillClipsOnTheCurve()
    {
        // A combine drops the exact path and leaves the band list in charge - but the bands are now the
        // ellipse's scan-converted interior, not its bounding box, so the corner stays outside.
        using var bmp = Paint(g =>
        {
            var region = new Region(Ellipse());
            region.Intersect(new Rectangle(0, 0, Size, Size));
            g.SetClip(region, CombineMode.Replace);
            FillEverything(g);
        });

        Assert.True(IsRed(bmp.GetPixel(Size / 2, Size / 2)));
        Assert.True(IsWhite(bmp.GetPixel(1, 1)), "combining must not widen the ellipse to its bounding box");
    }

    [Fact]
    public void EllipticalClip_ReportsTheBoundsOfThePath()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);

        g.SetClip(new Region(Ellipse()));

        var bounds = g.ClipBounds;
        Assert.Equal(0f, bounds.X, 3);
        Assert.Equal(0f, bounds.Y, 3);
        Assert.Equal((float)Size, bounds.Width, 3);
        Assert.Equal((float)Size, bounds.Height, 3);
    }
}
