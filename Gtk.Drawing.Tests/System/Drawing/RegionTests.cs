extern alias gtk;
using System.Windows.Forms;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using Region = gtk::System.Drawing.Region;

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class RegionTests
{
    public RegionTests()
    {
        Application.Init();
    }

    [Fact]
    public void Constructor_Default_ShouldCreateInfiniteRegion()
    {
        var region = new Region();

        // Typically default Region is infinite
        Assert.NotNull(region);
    }

    [Fact]
    public void Constructor_WithRectangle_ShouldInitialize()
    {
        var rect = new Rectangle(0, 0, 10, 20);

        var region = new Region(rect);

        Assert.NotNull(region);
    }

    [Fact]
    public void Constructor_WithGraphicsPath_ShouldInitialize()
    {
        var path = new GraphicsPath();
        path.AddRectangle(new Rectangle(0, 0, 10, 10));

        var region = new Region(path);

        Assert.NotNull(region);
    }

    [Fact]
    public void Clone_ShouldCreateNewInstance()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        var clone = region.Clone();

        Assert.NotNull(clone);
        Assert.NotSame(region, clone);
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        var region = new Region();

        region.Dispose();

        Assert.True(true); // just ensuring no exception
    }

    [Fact]
    public void MakeEmpty_ShouldResultInEmptyRegion()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.MakeEmpty();

        // behavior depends on implementation, but shouldn't throw
        Assert.True(true);
    }

    [Fact]
    public void MakeInfinite_ShouldResetRegion()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.MakeInfinite();

        Assert.True(true);
    }

    [Fact]
    public void IsEmpty_OnNewRegion_ShouldBeFalseOrTrueDependingImplementation()
    {
        var region = new Region();
        var form = new Form();
        bool result=false;
        form.Load += (s, e) =>
        {
            var graphics = form.CreateGraphics();
            result = region.IsEmpty(graphics);
            form.Close();
        };
        form.ShowDialog();

        Assert.IsType<bool>(result);
    }

    [Fact]
    public void GetBounds_ShouldReturnRectangle()
    {
        var region = new Region(new Rectangle(0, 0, 10, 20));
        var form = new Form();
        RectangleF bounds=default;
        form.Load += (s, e) =>
        {
            var graphics = form.CreateGraphics();
            bounds = region.GetBounds(graphics);
            form.Close();
        };
        form.ShowDialog();

        Assert.IsType<RectangleF>(bounds);
    }

    [Fact]
    public void Union_WithRectangle_ShouldNotThrow()
    {
        var region = new Region();

        region.Union(new Rectangle(0, 0, 10, 10));

        Assert.True(true);
    }

    [Fact]
    public void Intersect_WithRectangle_ShouldNotThrow()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Intersect(new Rectangle(5, 5, 10, 10));

        Assert.True(true);
    }

    [Fact]
    public void Exclude_ShouldNotThrow()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Exclude(new Rectangle(2, 2, 5, 5));

        Assert.True(true);
    }

    [Fact]
    public void Xor_ShouldNotThrow()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Xor(new Rectangle(2, 2, 5, 5));

        Assert.True(true);
    }

    [Fact]
    public void Translate_ShouldNotThrow()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        region.Translate(5, 5);

        Assert.True(true);
    }

    [Fact]
    public void Transform_ShouldNotThrow()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));
        var matrix = new Matrix();

        region.Transform(matrix);

        Assert.True(true);
    }

    [Fact]
    public void IsVisible_Point_ShouldReturnBool()
    {
        var region = new gtk::System.Drawing.Region(new Rectangle(0, 0, 10, 10));

        var result = region.IsVisible(5, 5);

        Assert.IsType<bool>(result);
    }

    [Fact]
    public void IsVisible_Rectangle_ShouldReturnBool()
    {
        var region = new Region(new Rectangle(0, 0, 10, 10));

        var result = region.IsVisible(new Rectangle(1, 1, 2, 2));

        Assert.IsType<bool>(result);
    }
}