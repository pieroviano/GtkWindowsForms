using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the common ToolStrip configuration members added to the base (they propagate to the whole
/// cluster — MenuStrip, StatusStrip, ContextMenuStrip, the drop-downs, BindingNavigator). They compile and
/// round-trip; the GTK substrate does not fully honour the layout/rendering they name, but ported source
/// that reads or sets them works.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripMembersFunctionalTests
{
    static ToolStripMembersFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void ConfigurationFlags_HaveWinFormsDefaultsAndRoundTrip()
    {
        using var strip = new ToolStrip();

        // WinForms defaults.
        Assert.True(strip.AllowMerge);
        Assert.True(strip.CanOverflow);
        Assert.True(strip.ShowItemToolTips);
        Assert.False(strip.AllowItemReorder);
        Assert.False(strip.Stretch);

        strip.AllowItemReorder = true;
        strip.CanOverflow = false;
        Assert.True(strip.AllowItemReorder);
        Assert.False(strip.CanOverflow);
    }

    [Fact]
    public void Renderer_IsNeverNull_AndRoundTrips()
    {
        using var strip = new ToolStrip();

        Assert.NotNull(strip.Renderer);

        var custom = new ToolStripProfessionalRenderer();
        strip.Renderer = custom;
        Assert.Same(custom, strip.Renderer);
    }

    [Fact]
    public void OverflowButton_IsNeverNull()
    {
        using var strip = new ToolStrip();

        Assert.NotNull(strip.OverflowButton);
        Assert.Same(strip.OverflowButton, strip.OverflowButton);
    }

    [Fact]
    public void IsDropDown_FalseForAStrip_TrueForADropDown()
    {
        using var strip = new ToolStrip();
        using var menu = new ContextMenuStrip();

        Assert.False(strip.IsDropDown);
        Assert.True(menu.IsDropDown);
    }

    [Fact]
    public void TheClusterInheritsTheMembers()
    {
        using var menu = new MenuStrip();

        // These now resolve on every ToolStrip-derived type.
        menu.ImageList = new ImageList();
        menu.TextDirection = ToolStripTextDirection.Vertical90;

        Assert.NotNull(menu.ImageList);
        Assert.Equal(ToolStripTextDirection.Vertical90, menu.TextDirection);
        Assert.Equal(Orientation.Horizontal, menu.Orientation);
    }
}
