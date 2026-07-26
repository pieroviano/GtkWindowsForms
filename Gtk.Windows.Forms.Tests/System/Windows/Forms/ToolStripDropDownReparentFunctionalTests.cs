using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Pins the §2 structural fix: <see cref="ToolStripDropDown"/> now derives from <see cref="ToolStrip"/> →
/// <see cref="Control"/> (it was a <c>ToolStripItem</c>), so a drop-down and every <see cref="ContextMenuStrip"/>
/// carry the whole <c>Control</c> surface. The drop-down keeps its native <c>Gtk.Menu</c> backing, so the
/// existing menu model — adding items through a menu item's <c>DropDownItems</c> — must still work.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripDropDownReparentFunctionalTests
{
    static ToolStripDropDownReparentFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void ToolStripDropDown_IsNowAControl()
    {
        using var dropDown = new ToolStripDropDown();

        Assert.IsAssignableFrom<ToolStrip>(dropDown);
        Assert.IsAssignableFrom<Control>(dropDown);
    }

    [Fact]
    public void ContextMenuStrip_IsAControl_WithTheControlSurface()
    {
        using var menu = new ContextMenuStrip();

        Assert.IsAssignableFrom<Control>(menu);

        // A representative slice of the Control surface that was previously missing from every drop-down:
        // events and properties now resolve and can be used.
        var backColorChanges = 0;
        menu.BackColorChanged += (_, _) => backColorChanges++;
        menu.Text = "ctx";
        Assert.Equal("ctx", menu.Text);
    }

    [Fact]
    public void MenuModelStillWorks_ItemsAddedThroughDropDownItems()
    {
        // The native-menu population path (a menu item owning a drop-down) is unchanged by the re-parent.
        using var parent = new ToolStripMenuItem("File");
        var open = new ToolStripMenuItem("Open");
        var save = new ToolStripMenuItem("Save");

        parent.DropDownItems.Add(open);
        parent.DropDownItems.Add(save);

        Assert.Equal(2, parent.DropDownItems.Count);
        Assert.Same(open, parent.DropDownItems[0]);
    }
}
