using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the overflow chevron surface (§3 of API-COMPLETENESS), which became additive once
/// <see cref="ToolStripDropDown"/> was re-parented onto <see cref="Control"/>. <see cref="ToolStripOverflow"/>
/// is the drop-down holding items that do not fit; <see cref="ToolStripOverflowButton"/> is the chevron.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripOverflowFunctionalTests
{
    static ToolStripOverflowFunctionalTests()
    {
        using var _ = new Form();
    }

    [Fact]
    public void ToolStripOverflow_IsADropDownControl_WithDisplayedItems()
    {
        using var owner = new ToolStripButton("x");
        using var overflow = new ToolStripOverflow(owner);

        Assert.IsAssignableFrom<ToolStripDropDown>(overflow);
        Assert.IsAssignableFrom<Control>(overflow);
        Assert.NotNull(overflow.DisplayedItems);
        Assert.Same(overflow.Items, overflow.DisplayedItems);
    }

    [Fact]
    public void ToolStripOverflowButton_IsADropDownButton()
    {
        using var button = new ToolStripOverflowButton();

        Assert.IsAssignableFrom<ToolStripDropDownButton>(button);
    }
}
