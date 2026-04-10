extern alias gtk;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

public class SolidBrushTests
{
    #region Constructor / Property

    [Fact]
    public void Constructor_SetsColor()
    {
        var brush = new SolidBrush(Color.Red);

        Assert.Equal(Color.Red, brush.Color);
    }

    [Fact]
    public void Color_Get_ReturnsValue()
    {
        var brush = new SolidBrush(Color.Blue);

        var result = brush.Color;

        Assert.Equal(Color.Blue, result);
    }

    [Fact]
    public void Color_Set_ChangesValue()
    {
        var brush = new SolidBrush(Color.Red);

        brush.Color = Color.Green;

        Assert.Equal(Color.Green, brush.Color);
    }

    #endregion

    #region Clone

    [Fact]
    public void Clone_ReturnsNewInstance()
    {
        var brush = new SolidBrush(Color.Red);

        var clone = brush.Clone();

        Assert.NotSame(brush, clone);
        Assert.IsType<SolidBrush>(clone);
    }

    [Fact]
    public void Clone_CopiesColor()
    {
        var brush = new SolidBrush(Color.Purple);

        var clone = (SolidBrush)brush.Clone();

        Assert.Equal(brush.Color, clone.Color);
    }

    #endregion

    #region Dispose

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var brush = new SolidBrush(Color.Black);

        brush.Dispose();

        Assert.True(true); // just ensure no exception
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var brush = new SolidBrush(Color.Black);

        brush.Dispose();
        brush.Dispose();

        Assert.True(true);
    }

    #endregion
}