using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Pins the ToolStrip base-class fix: <see cref="ToolStrip"/> now derives from
/// <see cref="ScrollableControl"/> (it was a plain <see cref="Control"/>), matching WinForms, so the whole
/// ToolStrip cluster inherits the scrollable-control surface (AutoScroll, HScroll/VScroll, DockPadding, …).
/// Scrolling itself is a no-op on a toolbar's non-scrollable GTK backing, which is fine.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripScrollableReparentFunctionalTests
{
    static ToolStripScrollableReparentFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void ToolStrip_IsNowAScrollableControl()
    {
        using var strip = new ToolStrip();

        Assert.IsAssignableFrom<ScrollableControl>(strip);
        Assert.IsAssignableFrom<Control>(strip);
    }

    [Fact]
    public void TheClusterInheritsTheScrollableSurface()
    {
        // A representative slice of the ScrollableControl surface, now present on the cluster's members.
        using var menu = new ContextMenuStrip();
        using var status = new StatusStrip();

        Assert.IsAssignableFrom<ScrollableControl>(menu);
        Assert.IsAssignableFrom<ScrollableControl>(status);

        // AutoScroll resolves and round-trips (it drives no real scrolling on the toolbar backing).
        menu.AutoScroll = true;
        Assert.True(menu.AutoScroll);
    }
}
