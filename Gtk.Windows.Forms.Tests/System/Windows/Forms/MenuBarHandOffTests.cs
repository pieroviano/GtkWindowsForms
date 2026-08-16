namespace System.Windows.Forms;

/// <summary>
/// With one menu open, clicking a sibling entry must close the first menu and open the second.
/// </summary>
/// <remarks>
/// <para>
/// Gtk3 got this from <c>GtkMenuShell</c>: the whole bar stayed active while any of its menus was up, so a
/// click on another entry just moved the selection. Gtk4 has no menu shell - every drop-down is an
/// independent autohiding <see cref="Gtk.Popover" />, and an autohiding popover takes a grab, so the click
/// that dismisses it is consumed by the dismissal and never reaches the entry underneath. The first click
/// only closed the open menu; it took a second click to open the other one.
/// </para>
/// <para>
/// The remedy is a hand-off: the popover being dismissed asks what is under the pointer and opens that
/// entry. The grab itself needs a display server and a real button press, so what these tests drive is the
/// decision - given the widget the pointer landed on, which entry (if any) should open. Every case below is
/// a distinct way the menu bar misbehaves when that decision is wrong.
/// </para>
/// </remarks>
[Collection("NonParallelCollection")]
public class MenuBarHandOffTests
{
    // Constructing these controls builds GTK widgets, which needs the type system initialised.
    public MenuBarHandOffTests() => Application.Init();

    [Fact]
    public void ClickingASiblingEntry_HandsTheMenuOverToIt()
    {
        // The defect itself: this is the click that used to be swallowed by the dismissal.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");
        var other = NewEntryWithDropDown(bar, "Button menu");

        // A click lands on a leaf inside the entry - its caption - not on the entry box itself.
        var target = open.ResolveHandOffTarget(LeafOf(other));

        Assert.Same(other, target);
    }

    [Fact]
    public void ClickingTheOpenEntryItself_HandsOverToNobody()
    {
        // Clicking the open entry closes its menu, and its own button handles that. Treating it as a
        // hand-off target would immediately reopen the menu the user just dismissed.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");
        NewEntryWithDropDown(bar, "Button menu");

        var target = open.ResolveHandOffTarget(LeafOf(open));

        Assert.Null(target);
    }

    [Fact]
    public void ClickingAnEntryWithNoDropDown_HandsOverToNobody()
    {
        // A plain command on the bar has nothing to open; handing over would close the live menu and leave
        // the bar showing nothing at all.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");
        var command = new StripMenuItem { Text = "Test" };
        bar.Append(command);

        var target = open.ResolveHandOffTarget(LeafOf(command));

        Assert.Null(target);
    }

    [Fact]
    public void ClickingAnItemInsideSomeOtherMenu_HandsOverToNobody()
    {
        // An item in an open drop-down is parented to that popover's content box, not to our bar. Without
        // the shared-parent test a menu would hand itself over to its own children.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");
        var neighbour = NewEntryWithDropDown(bar, "Button menu");

        var insideNeighboursMenu = new StripMenuItem { Text = "Item 1" };
        neighbour.EnsureDropDownContent().Append(insideNeighboursMenu);
        insideNeighboursMenu.EnsureDropDownContent().Append(new StripMenuItem { Text = "Nested" });

        var target = open.ResolveHandOffTarget(LeafOf(insideNeighboursMenu));

        Assert.Null(target);
    }

    [Fact]
    public void ClickingNothingInParticular_HandsOverToNobody()
    {
        // Clicking empty space, or off the window entirely, just closes the menu.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");

        Assert.Null(open.ResolveHandOffTarget(null));
        Assert.Null(open.ResolveHandOffTarget(new Gtk.Label("elsewhere")));
    }

    [Fact]
    public void WithNothingOnScreen_TheLookupAnswersNullRatherThanThrowing()
    {
        // ItemUnderPointer runs from inside the popover's Closed handler; an exception there would take the
        // menu bar down with it. Nothing here is realized, so there is no pointer position to be had.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var open = NewEntryWithDropDown(bar, "Dropdown List 1");
        NewEntryWithDropDown(bar, "Button menu");

        Assert.Null(open.ItemUnderPointer());
    }

    [Fact]
    public void ClosingTheDropDownInCode_ClearsTheOpenAppearance()
    {
        // HideDropDown is the programmatic path and must never hand over - only a user's click away may.
        var bar = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
        var entry = NewEntryWithDropDown(bar, "Dropdown List 1");

        entry.SetDropDownOpenAppearance(true);
        Assert.True(entry.IsDropDownOpenStyled);

        entry.HideDropDown();

        Assert.False(entry.IsDropDownOpenStyled);
    }

    // The deepest widget inside an entry, which is what a click actually picks.
    private static Gtk.Widget LeafOf(StripMenuItem item)
    {
        Gtk.Widget widget = item;
        while (widget.FirstChild != null)
        {
            widget = widget.FirstChild;
        }

        return widget;
    }

    private static StripMenuItem NewEntryWithDropDown(Gtk.Box bar, string text)
    {
        var entry = new StripMenuItem { Text = text };
        bar.Append(entry);

        // A drop-down only counts as present once it holds an item - an empty popover is nothing to open.
        entry.EnsureDropDownContent().Append(new StripMenuItem { Text = text + " command" });

        return entry;
    }
}
