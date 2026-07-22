extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;

namespace System.Drawing.Drawing2D;

/// <summary>Geometry behaviour of <see cref="GraphicsPath"/>: each Add* contributes its shape to the
/// path's bounds, and transforms move those bounds.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathGeometryFunctionalTests
{
    [Fact]
    public void AddEllipse_BoundsMatchTheEllipseBox()
    {
        using var p = new GraphicsPath();
        p.AddEllipse(10, 15, 20, 30);

        var b = p.GetBounds();
        Assert.Equal(10, b.X, 3);
        Assert.Equal(15, b.Y, 3);
        Assert.Equal(20, b.Width, 3);
        Assert.Equal(30, b.Height, 3);
    }

    [Fact]
    public void AddPolygon_BoundsSpanAllVertices()
    {
        using var p = new GraphicsPath();
        p.AddPolygon(new[] { new Point(5, 5), new Point(25, 10), new Point(15, 35) });

        var b = p.GetBounds();
        Assert.Equal(5, b.X, 3);
        Assert.Equal(5, b.Y, 3);
        Assert.Equal(20, b.Width, 3);  // 25 - 5
        Assert.Equal(30, b.Height, 3); // 35 - 5
    }

    [Fact]
    public void TwoLines_BoundsSpanBothSegments()
    {
        using var p = new GraphicsPath();
        p.AddLine(0, 0, 10, 0);
        p.AddLine(10, 0, 10, 40);

        var b = p.GetBounds();
        Assert.Equal(0, b.X, 3);
        Assert.Equal(0, b.Y, 3);
        Assert.Equal(10, b.Width, 3);
        Assert.Equal(40, b.Height, 3);
    }

    [Fact]
    public void AddPath_UnionsTheSubPathIntoTheBounds()
    {
        using var sub = new GraphicsPath();
        sub.AddRectangle(new Rectangle(50, 50, 10, 10));

        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 10, 10));
        p.AddPath(sub, false);

        var b = p.GetBounds();
        Assert.Equal(0, b.X, 3);
        Assert.Equal(0, b.Y, 3);
        Assert.Equal(60, b.Width, 3);  // spans 0..60
        Assert.Equal(60, b.Height, 3);
    }

    [Fact]
    public void Transform_TranslatesTheBounds()
    {
        using var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(0, 0, 10, 10));
        using var translate = new Matrix(1, 0, 0, 1, 25, 40);

        p.Transform(translate);

        var b = p.GetBounds();
        Assert.Equal(25, b.X, 3);
        Assert.Equal(40, b.Y, 3);
        Assert.Equal(10, b.Width, 3);
        Assert.Equal(10, b.Height, 3);
    }

    [Fact]
    public void AddRectangles_BoundsCoverEveryRectangle()
    {
        using var p = new GraphicsPath();
        p.AddRectangles(new[] { new Rectangle(0, 0, 5, 5), new Rectangle(20, 30, 5, 5) });

        var b = p.GetBounds();
        Assert.Equal(0, b.X, 3);
        Assert.Equal(0, b.Y, 3);
        Assert.Equal(25, b.Width, 3);
        Assert.Equal(35, b.Height, 3);
    }
}
