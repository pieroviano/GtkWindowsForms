using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="BindingNavigator"/> — the data-navigation toolbar that designer code
/// generates for bound forms. It was entirely absent (a compile break). These exercise the contract that
/// matters to ported source: the standard items exist, clicking them moves the bound <see cref="BindingSource"/>,
/// the position/count readouts track the list, and enabled state reflects the current position.
/// </summary>
[Collection("NonParallelCollection")]
public class BindingNavigatorFunctionalTests
{
    static BindingNavigatorFunctionalTests()
    {
        // Seed GTK so constructing a ToolStrip-derived control in isolation does not hit the
        // control-before-Form Gtk-CRITICAL crash.
        using var _ = new Form();
    }

    private sealed class Row
    {
        public string? Name { get; set; }
    }

    private static BindingSource ThreeRows() =>
        new() { DataSource = new List<Row> { new() { Name = "a" }, new() { Name = "b" }, new() { Name = "c" } } };

    [Fact]
    public void AddStandardItems_PopulatesTheStandardSet()
    {
        using var nav = new BindingNavigator(addStandardItems: true);

        Assert.NotNull(nav.MoveFirstItem);
        Assert.NotNull(nav.MovePreviousItem);
        Assert.NotNull(nav.MoveNextItem);
        Assert.NotNull(nav.MoveLastItem);
        Assert.NotNull(nav.AddNewItem);
        Assert.NotNull(nav.DeleteItem);
        Assert.NotNull(nav.PositionItem);
        Assert.NotNull(nav.CountItem);
        // Six buttons + position box + count label + three separators.
        Assert.Equal(11, nav.Items.Count);
    }

    [Fact]
    public void BindingToASource_ShowsPositionAndCount()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        Assert.Equal("1", nav.PositionItem!.Text);
        Assert.Equal("of 3", nav.CountItem!.Text);
    }

    [Fact]
    public void MoveNext_AdvancesTheSourceAndReadout()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        nav.MoveNextItem!.PerformClick();

        Assert.Equal(1, source.Position);
        Assert.Equal("2", nav.PositionItem!.Text);
    }

    [Fact]
    public void MoveLastThenMoveFirst_WalkTheEnds()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        nav.MoveLastItem!.PerformClick();
        Assert.Equal(2, source.Position);
        Assert.Equal("3", nav.PositionItem!.Text);

        nav.MoveFirstItem!.PerformClick();
        Assert.Equal(0, source.Position);
        Assert.Equal("1", nav.PositionItem!.Text);
    }

    [Fact]
    public void EnabledState_ReflectsPosition()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        // At the first row: cannot go back, can go forward.
        Assert.False(nav.MoveFirstItem!.Enabled);
        Assert.False(nav.MovePreviousItem!.Enabled);
        Assert.True(nav.MoveNextItem!.Enabled);
        Assert.True(nav.MoveLastItem!.Enabled);

        nav.MoveLastItem.PerformClick();

        // At the last row: cannot go forward, can go back.
        Assert.True(nav.MoveFirstItem.Enabled);
        Assert.True(nav.MovePreviousItem.Enabled);
        Assert.False(nav.MoveNextItem.Enabled);
        Assert.False(nav.MoveLastItem.Enabled);
    }

    [Fact]
    public void SourcePositionChange_RefreshesTheReadout()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        // Change position on the source directly; the navigator must follow via its event subscription.
        source.Position = 2;

        Assert.Equal("3", nav.PositionItem!.Text);
    }

    [Fact]
    public void DeleteItem_RemovesTheCurrentRow()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);

        nav.DeleteItem!.PerformClick();

        Assert.Equal(2, source.Count);
        Assert.Equal("of 2", nav.CountItem!.Text);
    }

    [Fact]
    public void CountItemFormat_IsApplied()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source) { CountItemFormat = "{0} total" };

        Assert.Equal("3 total", nav.CountItem!.Text);
    }

    [Fact]
    public void RefreshItemsEvent_FiresOnStateChange()
    {
        using var source = ThreeRows();
        using var nav = new BindingNavigator(source);
        var fired = 0;
        nav.RefreshItems += (_, _) => fired++;

        source.Position = 1;

        Assert.True(fired >= 1);
    }
}
