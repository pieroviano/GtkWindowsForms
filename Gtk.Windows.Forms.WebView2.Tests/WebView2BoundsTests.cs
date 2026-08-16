using System.Drawing;
using Gtk.Windows.Forms.WebView2;

namespace GtkWinForms.Tests.WebView2;

/// <summary>
/// Behavioural tests for the geometry that places the browser over the control's area.
/// </summary>
/// <remarks>
/// Deliberately runnable with no WebView2 runtime installed, which is why the arithmetic lives apart from
/// the controller it drives. The bug this pins down is the classic one: GTK reports allocations in
/// logical pixels and WebView2 takes bounds in physical ones, so forgetting the scale factor makes the
/// browser occupy a quarter of the control on a 200% display.
/// </remarks>
public class WebView2BoundsTests
{
    [Fact]
    public void At_Scale_One_The_Bounds_Are_The_Allocation()
    {
        Assert.Equal(new Rectangle(10, 20, 300, 200), WebView2Bounds.Compute(10, 20, 300, 200, 1));
    }

    [Fact]
    public void At_Scale_Two_Both_Origin_And_Size_Double()
    {
        Assert.Equal(new Rectangle(20, 40, 600, 400), WebView2Bounds.Compute(10, 20, 300, 200, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void A_Scale_Below_One_Is_Treated_As_One(int scale)
    {
        // GTK reports zero for a surface that is not yet realised, and a zero scale would collapse the
        // browser to nothing rather than merely misplace it.
        Assert.Equal(new Rectangle(10, 20, 300, 200), WebView2Bounds.Compute(10, 20, 300, 200, scale));
    }

    [Fact]
    public void A_Negative_Size_Is_Clamped_To_Zero()
    {
        var bounds = WebView2Bounds.Compute(0, 0, -5, -5, 2);

        Assert.Equal(0, bounds.Width);
        Assert.Equal(0, bounds.Height);
    }

    [Fact]
    public void A_Negative_Origin_Is_Kept_So_A_Scrolled_Out_Control_Moves_Off_Screen()
    {
        // Clamping the origin would park the browser at the top-left of the window instead of following
        // the control out of view.
        Assert.Equal(new Rectangle(-100, -40, 200, 100), WebView2Bounds.Compute(-50, -20, 100, 50, 2));
    }

    [Fact]
    public void A_Mapped_Widget_With_An_Area_Is_Visible()
    {
        Assert.True(WebView2Bounds.IsVisible(true, 10, 10));
    }

    [Theory]
    [InlineData(false, 10, 10)]
    [InlineData(true, 0, 10)]
    [InlineData(true, 10, 0)]
    [InlineData(true, 0, 0)]
    [InlineData(false, 0, 0)]
    public void An_Unmapped_Or_Empty_Widget_Is_Hidden(bool mapped, int width, int height)
    {
        // Nothing else clips a child HWND: a controller left visible over a zero-sized or unmapped widget
        // keeps painting its last frame on top of whatever GTK draws there.
        Assert.False(WebView2Bounds.IsVisible(mapped, width, height));
    }
}
