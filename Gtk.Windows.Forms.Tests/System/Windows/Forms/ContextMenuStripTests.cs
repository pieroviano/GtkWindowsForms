namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Behaviour of the Gtk4 right-click shortcut menu. A <see cref="ContextMenuStrip" /> is a Gtk.Popover
/// (<c>self</c>); its items must land in that popover, and showing it must anchor the popover to the
/// target control (a Gtk4 popover has to be parented before <c>Popup()</c> or it aborts).
/// </summary>
[Collection("NonParallelCollection")]
public class ContextMenuStripTests
{
    public ContextMenuStripTests() => Application.Init();

    private static int CountChildren(Gtk.Widget? parent)
    {
        var n = 0;
        for (var c = parent?.FirstChild; c != null; c = c.NextSibling)
        {
            n++;
        }

        return n;
    }

    [Fact]
    public void Items_LandInThePopover_NotAHiddenToolbarBox()
    {
        // Regression: ToolStripDropDown inherited ToolStrip's Items collection, which targeted the (hidden)
        // toolbar box; a menu item's ToolItem is null, so items vanished and the popover was empty. They must
        // now accumulate in the popover's content box.
        var menu = new ContextMenuStrip();
        menu.Items.Add(new ToolStripMenuItem("Cut"));
        menu.Items.Add(new ToolStripMenuItem("Copy"));
        menu.Items.Add(new ToolStripMenuItem("Paste"));

        var content = menu.self.Child as Gtk.Box;
        Assert.NotNull(content);
        Assert.Equal(3, CountChildren(content));
    }

    [Fact]
    public void Show_AnchorsPopoverToControl_AndDoesNotThrow()
    {
        var panel = new Panel();
        var menu = new ContextMenuStrip();
        menu.Items.Add(new ToolStripMenuItem("Refresh"));

        // Gtk4 requires a popover to be parented before Popup(); Show must anchor it to the control's widget.
        var ex = Record.Exception(() => menu.Show(panel, 12, 20));

        Assert.Null(ex);
        Assert.Same(panel.Widget, menu.self.Parent);
    }
}
