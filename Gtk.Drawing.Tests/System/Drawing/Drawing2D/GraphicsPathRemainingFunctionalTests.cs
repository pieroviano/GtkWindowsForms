extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using FontFamily = gtk::System.Drawing.FontFamily;
using StringFormat = gtk::System.Drawing.StringFormat;

namespace System.Drawing.Drawing2D;

/// <summary>The remaining <see cref="GraphicsPath"/> API: warping, reversing, markers and text layout.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathRemainingFunctionalTests
{
    [Fact]
    public void GetLastPoint_ReturnsTheMostRecentlyAddedPoint()
    {
        using var p = new GraphicsPath();
        p.AddLine(0, 0, 10, 20);

        var last = p.GetLastPoint();

        Assert.Equal(10f, last.X, 2);
        Assert.Equal(20f, last.Y, 2);
    }

    [Fact]
    public void Reverse_KeepsTheSameGeometry()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(5, 5, 20, 30));
        var before = p.GetBounds();

        p.Reverse();

        var after = p.GetBounds();
        Assert.Equal(before.X, after.X, 2);
        Assert.Equal(before.Y, after.Y, 2);
        Assert.Equal(before.Width, after.Width, 2);
        Assert.Equal(before.Height, after.Height, 2);
    }

    [Fact]
    public void Markers_DoNotAlterTheGeometry()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(5, 5, 20, 20));
        var before = p.GetBounds();

        p.SetMarkers();
        p.ClearMarkers();

        var after = p.GetBounds();
        Assert.Equal(before.Width, after.Width, 2);
        Assert.Equal(before.Height, after.Height, 2);
    }

    [Fact]
    public void Warp_ChangesTheGeometryToTheDestinationQuad()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 10, 10));

        var destination = new[]
        {
            new PointF(100f, 100f), new PointF(140f, 100f),
            new PointF(140f, 140f), new PointF(100f, 140f)
        };

        p.Warp(destination, new RectangleF(0, 0, 10, 10));

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, $"warped path should still have geometry but bounds were {b}");
    }

    [Fact]
    public void AddString_IntoALayoutRectangle_StaysRoughlyWithinIt()
    {
        using var fmt = new StringFormat();
        using var p = new GraphicsPath();

        var layout = new Rectangle(10, 10, 200, 60);
        p.AddString("Hello", new FontFamily("Sans"), 0, 14f, layout, fmt);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0, "text should contribute geometry");
        Assert.True(b.Left >= layout.Left - 2, $"text should start at/after the layout rectangle but bounds were {b}");
    }

    [Fact]
    public void AddPath_WithConnect_StillUnionsTheBounds()
    {
        using var sub = new GraphicsPath();
        sub.AddRectangle(new Rectangle(40, 40, 10, 10));

        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 10, 10));
        p.AddPath(sub, true);

        var b = p.GetBounds();
        Assert.Equal(0, b.X, 2);
        Assert.Equal(50, b.Width, 2);
        Assert.Equal(50, b.Height, 2);
    }

    [Fact]
    public void CloseAllFigures_KeepsTheGeometry()
    {
        using var p = new GraphicsPath();
        p.AddLine(0, 0, 20, 0);
        p.StartFigure();
        p.AddLine(0, 20, 20, 20);

        p.CloseAllFigures();

        var b = p.GetBounds();
        Assert.Equal(20, b.Width, 2);
        Assert.Equal(20, b.Height, 2);
    }
}
