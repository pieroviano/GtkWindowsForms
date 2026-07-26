using System.Collections;
using System.Windows.Forms;
using Point = global::System.Drawing.Point;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>The legacy menu and status API. These types are obsolete in
/// modern WinForms but, as the document puts it, are "exactly what an old application — the migration
/// target this project exists to serve — will be using". They were missing entirely, so such an application
/// failed to compile rather than failing at runtime. They are adapters over the ToolStrip family, so the
/// tests check both the legacy surface and that it drives the modern one.</summary>
[Collection("NonParallelCollection")]
public class LegacyMenuFunctionalTests
{
    // ---- MenuItem --------------------------------------------------------

    [Fact]
    public void MenuItem_CarriesItsCaption()
    {
        var item = new MenuItem("&File");

        Assert.Equal("&File", item.Text);
    }

    [Fact]
    public void MenuItem_StatePropertiesRoundTrip()
    {
        var item = new MenuItem("&Save")
        {
            Enabled = false,
            Checked = true,
            Visible = false,
            Shortcut = Shortcut.CtrlS,
            ShowShortcut = false,
            Name = "save",
            Tag = 7
        };

        Assert.False(item.Enabled);
        Assert.True(item.Checked);
        Assert.False(item.Visible);
        Assert.Equal(Shortcut.CtrlS, item.Shortcut);
        Assert.False(item.ShowShortcut);
        Assert.Equal("save", item.Name);
        Assert.Equal(7, item.Tag);
    }

    [Fact]
    public void MenuItem_TheCaptionReachesTheModernItem()
    {
        var item = new MenuItem("&File");

        item.Text = "&Edit";

        Assert.Equal("&Edit", item.Text);
    }

    [Fact]
    public void MenuItem_ConstructedWithAHandlerRaisesItOnClick()
    {
        var clicks = 0;
        var item = new MenuItem("&Open", (_, _) => clicks++);

        item.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void MenuItem_ConstructedWithAShortcutKeepsIt()
    {
        var item = new MenuItem("&Open", (_, _) => { }, Shortcut.CtrlO);

        Assert.Equal(Shortcut.CtrlO, item.Shortcut);
    }

    [Fact]
    public void MenuItem_ConstructedWithSubItemsHoldsThem()
    {
        var item = new MenuItem("&File", new[] { new MenuItem("&New"), new MenuItem("&Open") });

        Assert.Equal(2, item.MenuItems.Count);
        Assert.Equal("&New", item.MenuItems[0].Text);
    }

    [Fact]
    public void MenuItem_ClickEventFires()
    {
        var item = new MenuItem("&Open");
        var clicks = 0;
        item.Click += (_, _) => clicks++;

        item.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void MenuItem_SubItemsNestAndReportTheirParent()
    {
        var file = new MenuItem("&File");
        var open = file.MenuItems.Add("&Open");

        Assert.Single(file.MenuItems);
        Assert.Same(file, open.Parent);
        Assert.Equal(0, open.Index);
    }

    [Fact]
    public void MenuItem_CloneMenuCopiesTheItemAndItsChildren()
    {
        var file = new MenuItem("&File") { Name = "file", Checked = true };
        file.MenuItems.Add("&Open");

        var clone = file.CloneMenu();
        clone.Text = "&Changed";

        Assert.Equal("&File", file.Text);
        Assert.Equal("file", clone.Name);
        Assert.True(clone.Checked);
        Assert.Single(clone.MenuItems);
        Assert.Equal("&Open", clone.MenuItems[0].Text);
    }

    // ---- MenuItemCollection ----------------------------------------------

    [Fact]
    public void MenuItems_AddByCaptionCreatesTheItem()
    {
        var menu = new MainMenu();

        var item = menu.MenuItems.Add("&File");

        Assert.Single(menu.MenuItems);
        Assert.Equal("&File", item.Text);
    }

    [Fact]
    public void MenuItems_AddByCaptionAndHandlerWiresTheHandler()
    {
        var menu = new MainMenu();
        var clicks = 0;

        var item = menu.MenuItems.Add("&File", (_, _) => clicks++);
        item.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void MenuItems_AddRangeAddsEveryItemInOrder()
    {
        var menu = new MainMenu();

        menu.MenuItems.AddRange(new[] { new MenuItem("&File"), new MenuItem("&Edit"), new MenuItem("&Help") });

        Assert.Equal(3, menu.MenuItems.Count);
        Assert.Equal("&Help", menu.MenuItems[2].Text);
    }

    [Fact]
    public void MenuItems_InsertPlacesTheItemAtThatPosition()
    {
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Help");

        menu.MenuItems.Insert(1, new MenuItem("&Edit"));

        Assert.Equal(3, menu.MenuItems.Count);
        Assert.Equal("&Edit", menu.MenuItems[1].Text);
    }

    [Fact]
    public void MenuItems_RemoveDropsTheItemAndClearsItsParent()
    {
        var menu = new MainMenu();
        var file = menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Edit");

        menu.MenuItems.Remove(file);

        Assert.Single(menu.MenuItems);
        Assert.Null(file.Parent);
        Assert.False(menu.MenuItems.Contains(file));
    }

    [Fact]
    public void MenuItems_RemoveAtDropsThatPosition()
    {
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Edit");

        menu.MenuItems.RemoveAt(0);

        Assert.Single(menu.MenuItems);
        Assert.Equal("&Edit", menu.MenuItems[0].Text);
    }

    [Fact]
    public void MenuItems_ClearEmptiesTheMenu()
    {
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Edit");

        menu.MenuItems.Clear();

        Assert.Empty(menu.MenuItems);
        Assert.False(menu.IsParent, "an emptied menu has no items");
    }

    [Fact]
    public void MenuItems_KeyedLookupFindsAnItemByName()
    {
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");
        var edit = menu.MenuItems.Add("&Edit");
        edit.Name = "edit";

        Assert.True(menu.MenuItems.ContainsKey("edit"));
        Assert.Equal(1, menu.MenuItems.IndexOfKey("edit"));
        Assert.Same(edit, menu.MenuItems["edit"]);
    }

    [Fact]
    public void MenuItems_EnumerationYieldsEveryItem()
    {
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Edit");

        var captions = new List<string>();
        foreach (MenuItem item in menu.MenuItems)
        {
            captions.Add(item.Text);
        }

        Assert.Equal(new[] { "&File", "&Edit" }, captions);
    }

    [Fact]
    public void MenuItems_TheIListFaceRejectsNonMenuItems()
    {
        var menu = new MainMenu();
        IList list = menu.MenuItems;

        Assert.Throws<ArgumentException>(() => list.Add("not a menu item"));
    }

    // ---- the legacy items drive the modern strip -------------------------

    [Fact]
    public void AddingToAMainMenuAddsToTheUnderlyingMenuStrip()
    {
        using var form = new Form();
        var menu = new MainMenu();
        menu.MenuItems.Add("&File");

        form.Menu = menu;

        Assert.NotNull(form.MainMenuStrip);
        Assert.Single(form.MainMenuStrip!.Items);
        Assert.Equal("&File", form.MainMenuStrip.Items[0].Text);
    }

    [Fact]
    public void NestedMenuItemsBecomeDropDownItems()
    {
        var file = new MenuItem("&File");
        file.MenuItems.Add("&New");
        file.MenuItems.Add("&Open");

        var menu = new MainMenu();
        menu.MenuItems.Add(file);

        using var form = new Form();
        form.Menu = menu;

        var strip = form.MainMenuStrip!;
        var fileItem = Assert.IsType<ToolStripMenuItem>(strip.Items[0]);
        Assert.Equal(2, fileItem.DropDownItems.Count);
        Assert.Equal("&Open", fileItem.DropDownItems[1].Text);
    }

    [Fact]
    public void RemovingALegacyItemRemovesItFromTheStrip()
    {
        using var form = new Form();
        var menu = new MainMenu();
        var file = menu.MenuItems.Add("&File");
        menu.MenuItems.Add("&Edit");
        form.Menu = menu;

        menu.MenuItems.Remove(file);

        Assert.Single(form.MainMenuStrip!.Items);
        Assert.Equal("&Edit", form.MainMenuStrip.Items[0].Text);
    }

    [Fact]
    public void ClickingTheModernItemRaisesTheLegacyClick()
    {
        var clicks = 0;
        var item = new MenuItem("&Open", (_, _) => clicks++);
        var menu = new MainMenu();
        menu.MenuItems.Add(item);

        using var form = new Form();
        form.Menu = menu;

        // Raise it on the ToolStripMenuItem the legacy item is driving.
        var modern = Assert.IsType<ToolStripMenuItem>(form.MainMenuStrip!.Items[0]);
        modern.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void MenuItemCheckedReachesTheModernItem()
    {
        var item = new MenuItem("&Bold") { Checked = true };
        var menu = new MainMenu();
        menu.MenuItems.Add(item);

        using var form = new Form();
        form.Menu = menu;

        var modern = Assert.IsType<ToolStripMenuItem>(form.MainMenuStrip!.Items[0]);
        Assert.True(modern.Checked, "the legacy Checked flag should reach the modern item");
    }

    // ---- Form.Menu -------------------------------------------------------

    [Fact]
    public void FormMenuRoundTrips()
    {
        using var form = new Form();
        var menu = new MainMenu();

        form.Menu = menu;

        Assert.Same(menu, form.Menu);
    }

    [Fact]
    public void ReplacingTheFormMenuDetachesTheOldOne()
    {
        using var form = new Form();
        var first = new MainMenu();
        first.MenuItems.Add("&First");
        var second = new MainMenu();
        second.MenuItems.Add("&Second");

        form.Menu = first;
        form.Menu = second;

        Assert.Same(second, form.Menu);
        Assert.Same(second.Strip, form.MainMenuStrip);
        Assert.False(form.Controls.Contains(first.Strip), "the previous menu should be detached from the form");
    }

    [Fact]
    public void MainMenuCloneCopiesItsItems()
    {
        var menu = new MainMenu(new[] { new MenuItem("&File"), new MenuItem("&Edit") });

        var clone = menu.CloneMenu();

        Assert.Equal(2, clone.MenuItems.Count);
        Assert.Equal("&File", clone.MenuItems[0].Text);
        Assert.NotSame(menu.MenuItems[0], clone.MenuItems[0]);
    }

    [Fact]
    public void AMenuItemCanFindTheMainMenuItBelongsTo()
    {
        var menu = new MainMenu();
        var file = menu.MenuItems.Add("&File");
        var open = file.MenuItems.Add("&Open");

        Assert.Same(menu, open.GetMainMenu());
    }

    // ---- ContextMenu -----------------------------------------------------

    [Fact]
    public void ContextMenuHoldsItsItems()
    {
        var menu = new ContextMenu(new[] { new MenuItem("Cu&t"), new MenuItem("&Copy") });

        Assert.Equal(2, menu.MenuItems.Count);
        Assert.Equal("&Copy", menu.MenuItems[1].Text);
    }

    [Fact]
    public void ContextMenuItemsReachTheUnderlyingStrip()
    {
        var menu = new ContextMenu();
        menu.MenuItems.Add("Cu&t");

        Assert.Single(menu.Strip.Items);
        Assert.Equal("Cu&t", menu.Strip.Items[0].Text);
    }

    [Fact]
    public void AssigningAContextMenuToAControlAlsoSetsTheStrip()
    {
        using var panel = new Panel();
        var menu = new ContextMenu();
        menu.MenuItems.Add("Cu&t");

        panel.ContextMenu = menu;

        Assert.Same(menu, panel.ContextMenu);
        Assert.Same(menu.Strip, panel.ContextMenuStrip);
    }

    [Fact]
    public void ShowingAContextMenuRecordsTheSourceAndRaisesPopup()
    {
        using var panel = new Panel();
        var menu = new ContextMenu();
        menu.MenuItems.Add("Cu&t");
        var popups = 0;
        menu.Popup += (_, _) => popups++;

        menu.Show(panel, new Point(5, 5));

        Assert.Same(panel, menu.SourceControl);
        Assert.Equal(1, popups);
    }

    [Fact]
    public void ShowingAContextMenuWithoutAControlIsRejected()
    {
        var menu = new ContextMenu();

        Assert.Throws<ArgumentNullException>(() => menu.Show(null!, new Point(0, 0)));
    }

    [Fact]
    public void AMenuItemCanFindTheContextMenuItBelongsTo()
    {
        var menu = new ContextMenu();
        var cut = menu.MenuItems.Add("Cu&t");

        Assert.Same(menu, cut.GetContextMenu());
        Assert.Null(cut.GetMainMenu());
    }

    // ---- StatusBar -------------------------------------------------------

    [Fact]
    public void StatusBarIsAControlThatCanBeAddedToAForm()
    {
        using var form = new Form();
        var bar = new StatusBar();

        form.Controls.Add(bar);

        Assert.True(form.Controls.Contains(bar), "a StatusBar should be a real control");
    }

    [Fact]
    public void StatusBarPanelsAddInOrder()
    {
        using var bar = new StatusBar();

        bar.Panels.Add("Ready");
        var second = bar.Panels.Add("Line 1");

        Assert.Equal(2, bar.Panels.Count);
        Assert.Equal("Ready", bar.Panels[0].Text);
        Assert.Same(second, bar.Panels[1]);
    }

    [Fact]
    public void StatusBarPanelsReachTheUnderlyingStrip()
    {
        using var bar = new StatusBar();

        bar.Panels.Add("Ready");

        Assert.Single(bar.Items);
        Assert.Equal("Ready", bar.Items[0].Text);
    }

    [Fact]
    public void StatusBarPanelPropertiesRoundTrip()
    {
        var panel = new StatusBarPanel
        {
            Text = "Ready",
            Name = "status",
            ToolTipText = "the current state",
            Width = 120,
            MinWidth = 40,
            Alignment = HorizontalAlignment.Center,
            BorderStyle = StatusBarPanelBorderStyle.Raised,
            AutoSize = StatusBarPanelAutoSize.Spring,
            Style = StatusBarPanelStyle.OwnerDraw,
            Tag = 3
        };

        Assert.Equal("Ready", panel.Text);
        Assert.Equal("status", panel.Name);
        Assert.Equal("the current state", panel.ToolTipText);
        Assert.Equal(120, panel.Width);
        Assert.Equal(40, panel.MinWidth);
        Assert.Equal(HorizontalAlignment.Center, panel.Alignment);
        Assert.Equal(StatusBarPanelBorderStyle.Raised, panel.BorderStyle);
        Assert.Equal(StatusBarPanelAutoSize.Spring, panel.AutoSize);
        Assert.Equal(StatusBarPanelStyle.OwnerDraw, panel.Style);
        Assert.Equal(3, panel.Tag);
    }

    [Fact]
    public void RemovingAStatusBarPanelRemovesItFromTheStrip()
    {
        using var bar = new StatusBar();
        var first = bar.Panels.Add("Ready");
        bar.Panels.Add("Line 1");

        bar.Panels.Remove(first);

        Assert.Single(bar.Panels);
        Assert.Single(bar.Items);
        Assert.Null(first.Parent);
    }

    [Fact]
    public void ClearingStatusBarPanelsEmptiesTheStrip()
    {
        using var bar = new StatusBar();
        bar.Panels.Add("Ready");
        bar.Panels.Add("Line 1");

        bar.Panels.Clear();

        Assert.Empty(bar.Panels);
        Assert.Empty(bar.Items);
    }

    [Fact]
    public void StatusBarPanelsSupportKeyedLookup()
    {
        using var bar = new StatusBar();
        bar.Panels.Add("Ready");
        var line = bar.Panels.Add("Line 1");
        line.Name = "line";

        Assert.True(bar.Panels.ContainsKey("line"));
        Assert.Same(line, bar.Panels["line"]);
        Assert.Equal(1, bar.Panels.IndexOfKey("line"));
    }

    [Fact]
    public void AStatusBarPanelKnowsItsParent()
    {
        using var bar = new StatusBar();

        var panel = bar.Panels.Add("Ready");

        Assert.Same(bar, panel.Parent);
    }
}
