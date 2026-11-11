using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Pins the classic-WinForms contract that CheckedListBox's child-activated handler raises its
/// selection events <em>through</em> their <c>OnXxx</c> methods. The GTK handler used to invoke the events
/// directly, so <c>OnSelectedIndexChanged</c> / <c>OnSelectedValueChanged</c> / <c>OnSelectedItemChanged</c>
/// overrides never ran.</summary>
[Collection("NonParallelCollection")]
public class CheckedListBoxSelectionDispatchFunctionalTests
{
    /// <summary>A CheckedListBox that records which OnXxx funnels were invoked.</summary>
    private sealed class SpyCheckedListBox : CheckedListBox
    {
        public int IndexChanged { get; private set; }
        public int ValueChanged { get; private set; }
        public int ItemChanged { get; private set; }

        protected override void OnSelectedIndexChanged(EventArgs e) => IndexChanged++;
        protected override void OnSelectedValueChanged(EventArgs e) => ValueChanged++;
        protected override void OnSelectedItemChanged(EventArgs e) => ItemChanged++;
    }

    [Fact]
    public void ChildActivation_InvokesEveryOnSelectionOverride()
    {
        using var box = new SpyCheckedListBox();

        box.Control_ChildActivated(null!, null!); // the GTK child-activated handler

        Assert.Equal(1, box.IndexChanged);
        Assert.Equal(1, box.ValueChanged);
        Assert.Equal(1, box.ItemChanged);
    }

    [Fact]
    public void ChildActivation_RaisesTheSelectionEventsThroughTheFunnel()
    {
        using var box = new CheckedListBox();
        int index = 0, value = 0, item = 0;
        box.SelectedIndexChanged += (_, _) => index++;
        box.SelectedValueChanged += (_, _) => value++;
        box.SelectedItemChanged += (_, _) => item++;

        box.Control_ChildActivated(null!, null!);

        Assert.Equal(1, index);
        Assert.Equal(1, value);
        Assert.Equal(1, item);
    }
}
