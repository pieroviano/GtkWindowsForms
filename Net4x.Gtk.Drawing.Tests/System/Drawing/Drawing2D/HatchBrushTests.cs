extern alias gtk;
using gtk::System.Drawing.Drawing2D;

namespace System.Drawing.Drawing2D;

public class HatchBrushTests
{
    [Fact]
    public void Ctor_TwoParams_SetsForegroundAndDefaultBackgroundTransparent()
    {
        var brush = new HatchBrush(HatchStyle.Cross, Color.Red);

        Assert.Equal(HatchStyle.Cross, brush.HatchStyle);
        Assert.Equal(Color.Red, brush.ForegroundColor);
        Assert.Equal(Color.Transparent, brush.BackgroundColor);
    }

    [Fact]
    public void Ctor_ThreeParams_SetsAllProperties()
    {
        var brush = new HatchBrush(HatchStyle.DiagonalCross, Color.Blue, Color.Yellow);

        Assert.Equal(HatchStyle.DiagonalCross, brush.HatchStyle);
        Assert.Equal(Color.Blue, brush.ForegroundColor);
        Assert.Equal(Color.Yellow, brush.BackgroundColor);
    }

    [Fact]
    public void Clone_ReturnsDistinctInstanceWithSameValues()
    {
        var original = new HatchBrush(HatchStyle.Horizontal, Color.Green, Color.Black);

        var clonedObj = original.Clone();
        Assert.IsType<HatchBrush>(clonedObj);

        var clone = (HatchBrush)clonedObj;

        // Must be a different instance
        Assert.NotSame(original, clone);

        // But properties should match
        Assert.Equal(original.HatchStyle, clone.HatchStyle);
        Assert.Equal(original.ForegroundColor, clone.ForegroundColor);
        Assert.Equal(original.BackgroundColor, clone.BackgroundColor);
    }
}