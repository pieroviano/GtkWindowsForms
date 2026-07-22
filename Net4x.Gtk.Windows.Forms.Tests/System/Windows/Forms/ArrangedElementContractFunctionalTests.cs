using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the <c>IArrangedElement</c> contract on <see cref="Control" /> and
/// <see cref="ToolStripItem" />. Several members threw <see cref="NotImplementedException" />, so a layout
/// engine walking the element tree failed at the first element it touched. The interface is internal, so
/// these tests exercise it through the members the library itself reaches.
/// </summary>
[Collection("NonParallelCollection")]
public class ArrangedElementContractFunctionalTests
{
    [Fact]
    public void ToolStripItem_Children_IsAnEmptyCollectionRatherThanAThrow()
    {
        var item = new ToolStripMenuItem("File");

        var children = item.Children;

        Assert.NotNull(children);
        Assert.Equal(0, children.Count);
    }

    [Fact]
    public void ToolStripItem_ParticipatesInLayout_TracksVisibility()
    {
        var item = new ToolStripMenuItem("File") { Visible = true };

        Assert.True(item.ParticipatesInLayout);

        item.Visible = false;

        Assert.False(item.ParticipatesInLayout, "an invisible item must be skipped by flow layouts");
    }

    [Fact]
    public void ToolStripItem_GetPreferredSize_IsBoundedByTheProposedSize()
    {
        var item = new ToolStripMenuItem("File") { Size = new global::System.Drawing.Size(120, 40) };

        var unconstrained = item.GetPreferredSize(global::System.Drawing.Size.Empty);
        var constrained = item.GetPreferredSize(new global::System.Drawing.Size(50, 20));

        Assert.Equal(new global::System.Drawing.Size(120, 40), unconstrained);
        Assert.Equal(new global::System.Drawing.Size(50, 20), constrained);
    }

    [Fact]
    public void ToolStripItem_SetBounds_AppliesTheRequestedSize()
    {
        var item = new ToolStripMenuItem("File");

        item.SetBounds(new global::System.Drawing.Rectangle(0, 0, 88, 22), BoundsSpecified.Size);

        Assert.Equal(88, item.Width);
        Assert.Equal(22, item.Height);
    }

    [Fact]
    public void ToolStripItem_Bounds_ReflectsSize()
    {
        var item = new ToolStripMenuItem("File") { Size = new global::System.Drawing.Size(64, 24) };

        Assert.Equal(64, item.Bounds.Width);
        Assert.Equal(24, item.Bounds.Height);
    }

    [Fact]
    public void ToolStripItem_DisplayRectangle_ExcludesPadding()
    {
        var item = new ToolStripMenuItem("File")
        {
            Size = new global::System.Drawing.Size(100, 40),
            Padding = new Padding(5)
        };

        var display = item.DisplayRectangle;

        Assert.Equal(90, display.Width);
        Assert.Equal(30, display.Height);
    }

    [Fact]
    public void Control_PerformLayout_ForAnAffectedControl_DoesNotThrow()
    {
        using var parent = new Panel();
        using var child = new Button();
        parent.Controls.Add(child);

        // The IArrangedElement overload used to throw outright.
        var exception = Record.Exception(() => parent.PerformLayout(child, "Bounds"));

        Assert.Null(exception);
    }
}
