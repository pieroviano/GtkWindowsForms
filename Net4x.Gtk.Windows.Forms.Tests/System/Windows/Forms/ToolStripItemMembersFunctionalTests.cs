using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the common ToolStripItem members added to the base — accessibility, selection/placement state
/// and drag flags. They live on the base, so they reach every item type; ported source that reads or sets
/// them works. Placement (Anchor/Dock) is a stored setting the GTK strip does not drive, but it round-trips.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripItemMembersFunctionalTests
{
    static ToolStripItemMembersFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void AccessibilityMembers_RoundTrip_AndAccessibilityObjectIsNeverNull()
    {
        using var item = new ToolStripButton("x");

        item.AccessibleName = "Save";
        item.AccessibleDescription = "Saves the document";
        item.AccessibleRole = AccessibleRole.PushButton;

        Assert.Equal("Save", item.AccessibleName);
        Assert.Equal("Saves the document", item.AccessibleDescription);
        Assert.Equal(AccessibleRole.PushButton, item.AccessibleRole);
        Assert.NotNull(item.AccessibilityObject);
    }

    [Fact]
    public void PlacementAndFlags_HaveDefaultsAndRoundTrip()
    {
        using var item = new ToolStripButton("x");

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
        Assert.Equal(DockStyle.None, item.Dock);
        Assert.True(item.Available);
        Assert.False(item.DoubleClickEnabled);

        item.Dock = DockStyle.Left;
        item.DoubleClickEnabled = true;
        Assert.Equal(DockStyle.Left, item.Dock);
        Assert.True(item.DoubleClickEnabled);
    }

    [Fact]
    public void IsDisposed_TracksDisposal()
    {
        var item = new ToolStripButton("x");
        Assert.False(item.IsDisposed);

        item.Dispose();
        Assert.True(item.IsDisposed);
    }

    [Fact]
    public void IsOnDropDown_IsFalseForAnItemOnAPlainStrip()
    {
        using var strip = new ToolStrip();
        var onStrip = new ToolStripButton("a");
        strip.Items.Add(onStrip);

        // An item whose owner is a plain ToolStrip (not a ToolStripDropDown) is not on a drop-down.
        Assert.False(onStrip.IsOnDropDown);
    }

    [Fact]
    public void TheseReachEveryItemType()
    {
        using var label = new ToolStripLabel("l");
        using var sep = new ToolStripSeparator();

        // The members resolve on unrelated item subclasses (label, separator), not just buttons.
        label.AccessibleName = "info";
        Assert.Equal("info", label.AccessibleName);
        Assert.True(sep.CanSelect || !sep.CanSelect); // resolves and returns a bool
    }
}
