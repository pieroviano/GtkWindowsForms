extern alias gtk;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using Pen = gtk::System.Drawing.Pen;
using FontFamily = gtk::System.Drawing.FontFamily;
using StringFormat = gtk::System.Drawing.StringFormat;

namespace System.Drawing.Drawing2D;

/// <summary>Advanced <see cref="GraphicsPath"/> operations: bounds under a matrix, widening, flattening
/// and outline hit-testing.</summary>
[Collection("NonParallelCollection")]
public class GraphicsPathAdvancedFunctionalTests
{
    private static GraphicsPath Square()
    {
        var p = new GraphicsPath();
        p.AddRectangle(new Rectangle(10, 10, 40, 40));
        return p;
    }

    [Fact]
    public void GetBounds_WithTranslationMatrix_ReportsTheMovedBounds()
    {
        using var p = Square();
        using var translate = new Matrix(1, 0, 0, 1, 100, 200);

        var b = p.GetBounds(translate);

        Assert.Equal(110, b.X, 3);
        Assert.Equal(210, b.Y, 3);
        Assert.Equal(40, b.Width, 3);
        Assert.Equal(40, b.Height, 3);
    }

    [Fact]
    public void Flatten_KeepsTheOverallBoundsOfARectangle()
    {
        using var p = Square();
        var before = p.GetBounds();

        p.Flatten();

        var after = p.GetBounds();
        Assert.Equal(before.X, after.X, 2);
        Assert.Equal(before.Y, after.Y, 2);
        Assert.Equal(before.Width, after.Width, 2);
        Assert.Equal(before.Height, after.Height, 2);
    }

    [Fact]
    public void Widen_DoesNotShrinkThePath()
    {
        using var p = Square();
        var before = p.GetBounds();

        p.Widen(new Pen(Color.Black, 6f));

        var after = p.GetBounds();
        Assert.True(after.Width >= before.Width - 0.01f,
            $"widening must not shrink the path (before={before.Width}, after={after.Width})");
        Assert.True(after.Height >= before.Height - 0.01f,
            $"widening must not shrink the path (before={before.Height}, after={after.Height})");
    }

    [Fact]
    public void IsOutlineVisible_TrueOnTheEdge_FalseFarAway()
    {
        using var p = Square();
        using var pen = new Pen(Color.Black, 4f);

        Assert.True(p.IsOutlineVisible(new Point(10, 30), pen), "a point on the left edge is on the outline");
        Assert.False(p.IsOutlineVisible(new Point(500, 500), pen), "a far point is not on the outline");
    }

    [Fact]
    public void AddString_ProducesNonEmptyBounds()
    {
        using var p = new GraphicsPath();
        using var fmt = new StringFormat();

        p.AddString("Hello", new FontFamily("Sans"), 0, 16f, new Point(0, 0), fmt);

        var b = p.GetBounds();
        Assert.True(b.Width > 0 && b.Height > 0,
            $"adding text should contribute geometry to the path but bounds were {b}");
    }

    [Fact]
    public void ClonedPath_TransformsIndependently()
    {
        using var p = Square();
        using var clone = Assert.IsType<GraphicsPath>(p.Clone());
        using var translate = new Matrix(1, 0, 0, 1, 50, 0);

        clone.Transform(translate);

        Assert.Equal(10, p.GetBounds().X, 3);      // original untouched
        Assert.Equal(60, clone.GetBounds().X, 3);  // clone moved
    }
}
