using System.Drawing;
using Gtk.Windows.Forms.InternetExplorer;

namespace GtkWinForms.Tests.InternetExplorer;

/// <summary>
/// Behavioural tests for the geometry that places the browser over the control's area. Runnable with no
/// Trident involved, which is why the arithmetic lives apart from the window it drives.
/// </summary>
public class IeBoundsTests
{
    [Fact]
    public void At_Scale_One_The_Bounds_Are_The_Allocation()
    {
        Assert.Equal(new Rectangle(10, 20, 300, 200), IeBounds.Compute(10, 20, 300, 200, 1));
    }

    [Fact]
    public void At_Scale_Two_Both_Origin_And_Size_Double()
    {
        Assert.Equal(new Rectangle(20, 40, 600, 400), IeBounds.Compute(10, 20, 300, 200, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void A_Scale_Below_One_Is_Treated_As_One(int scale)
    {
        Assert.Equal(new Rectangle(10, 20, 300, 200), IeBounds.Compute(10, 20, 300, 200, scale));
    }

    [Fact]
    public void A_Negative_Size_Is_Clamped_To_Zero()
    {
        var bounds = IeBounds.Compute(0, 0, -5, -5, 2);

        Assert.Equal(0, bounds.Width);
        Assert.Equal(0, bounds.Height);
    }

    [Fact]
    public void A_Negative_Origin_Is_Kept_So_A_Scrolled_Out_Control_Moves_Off_Screen()
    {
        Assert.Equal(new Rectangle(-100, -40, 200, 100), IeBounds.Compute(-50, -20, 100, 50, 2));
    }

    [Fact]
    public void A_Mapped_Widget_With_An_Area_Is_Visible()
    {
        Assert.True(IeBounds.IsVisible(true, 10, 10));
    }

    [Theory]
    [InlineData(false, 10, 10)]
    [InlineData(true, 0, 10)]
    [InlineData(true, 10, 0)]
    [InlineData(true, 0, 0)]
    public void An_Unmapped_Or_Empty_Widget_Is_Hidden(bool mapped, int width, int height)
    {
        Assert.False(IeBounds.IsVisible(mapped, width, height));
    }
}
