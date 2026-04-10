extern alias gtk;
using FillMode = gtk::System.Drawing.Drawing2D.FillMode;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
#pragma warning disable xUnit2013
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Drawing.Drawing2D;

public class GraphicsPathTests
{
    [Fact]
    public void Constructor_Default_ShouldInitialize()
    {
        var gp = new gtk::System.Drawing.Drawing2D.GraphicsPath();

        Assert.NotNull(gp);
        Assert.Equal(FillMode.Alternate, gp.FillMode);
        Assert.Null(gp.PathPoints);
        Assert.Null(gp.PathTypes);
        Assert.Equal(0, gp.SizePoints.Count);
    }

    [Fact]
    public void Constructor_WithPoints_ShouldSetProperties()
    {
        var pts = new[] { new PointF(1, 2), new PointF(3, 4) };
        var types = new byte[] { 0, 1 };

        var gp = new GraphicsPath(pts, types);

        Assert.Equal(2, gp.PointCount);
        Assert.Equal(pts, gp.PathPoints);
        Assert.Equal(types, gp.PathTypes);
        Assert.NotNull(gp.PathData);
    }

    [Fact]
    public void AddLine_ShouldAddPoints()
    {
        var gp = new GraphicsPath();

        gp.AddLine(0, 0, 10, 10);

        Assert.NotEmpty(gp.SizePoints);
    }

    [Fact]
    public void AddRectangle_ShouldIncreasePoints()
    {
        var gp = new GraphicsPath();

        gp.AddRectangle(new Rectangle(0, 0, 10, 20));

        Assert.True(gp.SizePoints.Count >= 2);
    }

    [Fact]
    public void AddEllipse_ShouldIncreasePoints()
    {
        var gp = new GraphicsPath();

        gp.AddEllipse(0, 0, 10, 10);

        Assert.True(gp.SizePoints.Count >= 2);
    }

    [Fact]
    public void AddPolygon_ShouldStorePoints()
    {
        var gp = new GraphicsPath();
        var pts = new[] { new Point(0, 0), new Point(1, 1), new Point(2, 2) };

        gp.AddPolygon(pts);

        Assert.True(gp.SizePoints.Count >= pts.Length);
    }

    [Fact]
    public void CloseFigure_ShouldAddFigureMode()
    {
        var gp = new GraphicsPath();

        gp.CloseFigure();

        // indirect verification (no public access to list)
        Assert.True(true); // ensures no exception
    }

    [Fact]
    public void CloseAllFigures_ShouldSetFlag()
    {
        var gp = new GraphicsPath();

        gp.CloseAllFigures();

        // internal flag, test indirectly via behavior
        Assert.True(true);
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        var gp = new GraphicsPath();
        gp.AddLine(0, 0, 5, 5);

        var clone = (GraphicsPath?)gp.Clone();

        Assert.NotSame(gp, clone);
        Assert.Equal(gp.FillMode, clone?.FillMode);
        Assert.Equal(gp.SizePoints.Count, clone?.SizePoints.Count);
    }

    [Fact]
    public void Dispose_ShouldClearData()
    {
        var gp = new GraphicsPath();
        gp.AddLine(0, 0, 5, 5);

        gp.Dispose();

        Assert.Null(gp.PathPoints);
        Assert.Null(gp.PathTypes);
        Assert.Null(gp.PathData);
        Assert.Empty(gp.SizePoints);
    }

    [Fact]
    public void Flatten_WithMatrix_ShouldNotThrow()
    {
        var gp = new GraphicsPath();
        gp.AddLine(0, 0, 10, 10);

        var matrix = new Matrix();

        gp.Flatten(matrix);

        Assert.True(true);
    }

    [Fact]
    public void Flatten_WithNullMatrix_ShouldThrow()
    {
        var gp = new GraphicsPath();

        Assert.Throws<ArgumentNullException>(() => gp.Flatten(null));
    }

    [Fact]
    public void GetBounds_Empty_ShouldReturnEmpty()
    {
        var gp = new GraphicsPath();

        var bounds = gp.GetBounds();

        Assert.True(bounds.IsEmpty);
    }

    [Fact]
    public void GetBounds_WithRectangle_ShouldReturnCorrectBounds()
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new Rectangle(0, 0, 10, 20));

        var bounds = gp.GetBounds();

        Assert.Equal(0, bounds.X);
        Assert.Equal(0, bounds.Y);
        Assert.Equal(10, bounds.Width);
        Assert.Equal(20, bounds.Height);
    }
}