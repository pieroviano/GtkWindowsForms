namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Rendering regressions for the Gtk4 menu-item widget (<see cref="StripMenuItem" />), the backing widget
/// of every <see cref="ToolStripMenuItem" />. On Gtk4 the menu bar rendered as a row of scattered
/// checkboxes with missing captions; these tests pin the widget structure that fixes it.
/// </summary>
[Collection("NonParallelCollection")]
public class StripMenuItemRenderingTests
{
    // GTK's type system must be initialised before any native widget is constructed; these tests build
    // menu widgets directly (no Form), so init explicitly like the other bare-widget tests do.
    public StripMenuItemRenderingTests() => Application.Init();

    private static Gdk.Pixbuf NewPixbuf() => new(Gdk.Colorspace.Rgb, true, 8, 8, 8);

    // A real pointer click / Space flips the CheckButton's Active state, which fires 'toggled'. (Emitting
    // 'toggled' alone would not change Active, so it cannot exercise the click-toggle/revert behaviour.)
    private static void SimulateUserClick(ToolStripMenuItem item)
        => item.checkButton.Active = !item.checkButton.Active;

    private static System.Collections.Generic.List<Gtk.Widget> ChildrenOf(Gtk.Widget parent)
    {
        var list = new System.Collections.Generic.List<Gtk.Widget>();
        for (var child = parent.FirstChild; child != null; child = child.NextSibling)
        {
            list.Add(child);
        }

        return list;
    }

    [Fact]
    public void SettingImage_DoesNotClobberText()
    {
        // Regression: the Image setter used to do `checkButton.Child = new Gtk.Image(...)`, which on Gtk4
        // discards the button's label - so any menu item with an icon lost its caption (the empty-text menu
        // bar in the Gtk4 screenshot). Text must survive after an image is assigned.
        var item = new StripMenuItem { Text = "File" };

        using var pixbuf = NewPixbuf();
        item.Image = pixbuf;

        Assert.Equal("File", item.Text);
    }

    [Fact]
    public void IconAndLabel_CoexistInContentBox_BothVisible()
    {
        var item = new StripMenuItem { Text = "Edit" };
        using var pixbuf = NewPixbuf();
        item.Image = pixbuf;

        // The CheckButton hosts a horizontal [icon][label] box rather than a bare image or bare label.
        var content = item.checkButton.Child as Gtk.Box;
        Assert.NotNull(content);

        var children = ChildrenOf(content!);
        var image = children.OfType<Gtk.Image>().FirstOrDefault();
        var label = children.OfType<Gtk.Label>().FirstOrDefault();

        Assert.NotNull(image);
        Assert.NotNull(label);
        Assert.True(image!.Visible, "Icon should be visible once an image is assigned.");
        Assert.True(label!.Visible, "Caption should stay visible next to the icon.");
        Assert.Equal("Edit", label.Text);
    }

    [Fact]
    public void TextOnlyItem_KeepsIconHidden_ButLabelVisible()
    {
        var item = new StripMenuItem { Text = "View" };

        var content = (Gtk.Box)item.checkButton.Child;
        var children = ChildrenOf(content);
        var image = children.OfType<Gtk.Image>().First();
        var label = children.OfType<Gtk.Label>().First();

        Assert.False(image.Visible, "No image was set, so the icon slot must stay hidden.");
        Assert.True(label.Visible);
        Assert.Equal("View", label.Text);
    }

    [Fact]
    public void TopLevelMenuBarItem_TagsCheckButton_ToHideIndicator()
    {
        // Top-level items are tagged .MenuBarItem so the CSS collapses their check-indicator square
        // unconditionally (a menu-bar item is not a toggle). ToolStripItemCollection applies the tag when
        // the item is added to a MenuStrip.
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("Menu 1");
        menu.Items.Add(top);

        Assert.True(top.self.checkButton.StyleContext.HasClass("MenuBarItem"));
    }

    [Fact]
    public void DropDownItem_IsNotTaggedMenuBarItem_SoItCanShowACheckmark()
    {
        // A sub-item added under a top-level item must NOT get the always-hide .MenuBarItem tag, so a
        // Checked drop-down entry can still render its checkmark.
        var top = new ToolStripMenuItem("Menu 1");
        var child = new ToolStripMenuItem("Checkable");
        top.DropDownItems.Add(child);

        Assert.False(child.self.checkButton.StyleContext.HasClass("MenuBarItem"));
    }

    [Fact]
    public void TextImageRelation_ImageAboveText_StacksIconAboveCaption()
    {
        // Regression: the demo's top-level items set TextImageRelation.ImageAboveText, but the setter only
        // tweaked Halign, so the icon stayed on the same row as the text. It must stack them vertically with
        // the icon first (above).
        var item = new ToolStripMenuItem("Menu 1") { TextImageRelation = TextImageRelation.ImageAboveText };

        var content = (Gtk.Box)item.checkButton.Child;
        Assert.Equal(Gtk.Orientation.Vertical, content.Orientation);
        Assert.IsType<Gtk.Image>(content.FirstChild); // icon is the first (top) child
    }

    [Fact]
    public void TextImageRelation_ImageBeforeText_KeepsIconBesideCaption()
    {
        var item = new ToolStripMenuItem("Edit") { TextImageRelation = TextImageRelation.ImageBeforeText };

        var content = (Gtk.Box)item.checkButton.Child;
        Assert.Equal(Gtk.Orientation.Horizontal, content.Orientation);
    }

    [Fact]
    public void MenuBarItems_PackTight_NotHexpanded()
    {
        // Regression: checkButton.Hexpand=true propagated up and stretched each item, so Menu 1 / Menu 2
        // drifted apart across the bar. Menu-bar items must not expand (they pack at the start).
        var menu = new MenuStrip();
        var m1 = new ToolStripMenuItem("Menu 1");
        var m2 = new ToolStripMenuItem("Menu 2");
        menu.Items.Add(m1);
        menu.Items.Add(m2);

        Assert.False(m1.self.Hexpand);
        Assert.False(m1.self.checkButton.Hexpand);
        Assert.Equal(Gtk.Align.Start, m1.self.Halign);
        Assert.False(m2.self.Hexpand);
    }

    [Fact]
    public void RightAlignedMenuItem_ExpandsToTheEnd()
    {
        // A right-aligned item (set after Add, mirroring the designer) opts back into expansion so it sits at
        // the far end while the others stay packed at the start.
        var menu = new MenuStrip();
        var normal = new ToolStripMenuItem("Menu 1");
        var right = new ToolStripMenuItem("A right-aligned menu");
        menu.Items.Add(normal);
        menu.Items.Add(right);

        right.Alignment = ToolStripItemAlignment.Right;

        Assert.Equal(Gtk.Align.End, right.self.Halign);
        Assert.True(right.self.Hexpand);
        // The ordinary item is unaffected.
        Assert.False(normal.self.Hexpand);
    }

    [Fact]
    public void MenuItemClick_FiresWhenCheckButtonToggledByUser()
    {
        // Regression: the item's GestureClick is starved by the inner CheckButton, and Gtk4's GtkCheckButton
        // has no Clicked signal - so menu items never raised Click (clicking a menu did nothing, mirroring
        // the dead "Open Main Window" button). A user toggle now bridges to WinForms Click.
        var item = new ToolStripMenuItem("Open");
        var clicks = 0;
        item.Click += (s, e) => clicks++;

        SimulateUserClick(item);

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void DropDownItems_AllAppearInPopover_NotOverwritten()
    {
        // Regression: a Gtk.Popover holds a single Child, and the old code did popover.Child = item for each
        // add, so only the last drop-down entry survived. Items now go into a vertical box - all appear.
        // The item is put on a MenuStrip because the left-spill assertion below is about a menu-bar entry's
        // drop-down; a submenu parent is placed by a different rule (see the cascade tests).
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("File");
        menu.Items.Add(top);
        top.DropDownItems.Add(new ToolStripMenuItem("New"));
        top.DropDownItems.Add(new ToolStripMenuItem("Open"));
        top.DropDownItems.Add(new ToolStripMenuItem("Save"));

        Assert.True(top.self.HasDropDown);

        var box = top.self.EnsureDropDownContent();
        var count = 0;
        for (var c = box.FirstChild; c != null; c = c.NextSibling)
        {
            count++;
        }

        Assert.Equal(3, count);

        // The drop-down popover left-aligns with the parent item (Halign.Start) rather than centring on it
        // and spilling to the left.
        Assert.Equal(Gtk.Align.Start, top.self.DropDownAlign);
    }

    [Fact]
    public void MenuItemImage_AppearsWithoutWaitingForRealization()
    {
        // Regression: SetIconImage was gated on self.IsRealized, but the Designer assigns Image during
        // InitializeComponent - long before anything is on screen - so the icon was dropped on the floor.
        // In the sample this left "b Menu" with no image where Gtk3 shows one.
        var item = new ToolStripMenuItem("b Menu") { Image = new gtk::System.Drawing.Bitmap(8, 8) { Pixbuf = NewPixbuf() } };

        Assert.NotNull(item.self.Image);

        var content = (Gtk.Box)item.checkButton.Child;
        var icon = ChildrenOf(content).OfType<Gtk.Image>().First();
        Assert.True(icon.Visible, "the icon must be visible as soon as Image is assigned");
    }

    [Fact]
    public void ClearingMenuItemImage_HidesTheIcon()
    {
        var item = new ToolStripMenuItem("b Menu") { Image = new gtk::System.Drawing.Bitmap(8, 8) { Pixbuf = NewPixbuf() } };

        item.Image = null;

        var content = (Gtk.Box)item.checkButton.Child;
        var icon = ChildrenOf(content).OfType<Gtk.Image>().First();
        Assert.False(icon.Visible, "clearing Image must clear the icon, not leave the old one on screen");
    }

    [Fact]
    public void HoveredMenuRow_IsStyledWithTheSelectionColours_ButNotMenuBarEntries()
    {
        // Gtk3 drew the hovered menu row in the theme's selection colours via GtkMenuItem; Gtk4 has no
        // menu-item widget, so a drop-down row was left with the theme's plain button hover and gave no
        // feedback about which entry the pointer was on. The rules must sit at User priority, or the
        // theme's own button:hover wins.
        var css = Application.UserPriorityCss;

        Assert.Contains(".MenuItemButton:not(.MenuBarItem):hover", css);
        Assert.Contains("@theme_selected_bg_color", css);
    }

    [Fact]
    public void HoverForeground_IsScopedToTheCaptionRow_SoItCannotCascadeIntoAnOpenSubmenu()
    {
        // The drop-down popover is parented to the row's button, so an inheritable colour on the button
        // reaches every caption inside the submenu it opens and paints them white on white - the menu then
        // renders empty. The foreground rule must therefore target the caption row, never the button.
        foreach (var rule in Application.UserPriorityCss.Split('\n'))
        {
            if (rule.Contains("@theme_selected_fg_color"))
            {
                Assert.Contains(">box", rule);
            }
        }
    }

    [Fact]
    public void IndeterminateMenuItem_KeepsAnIndicator()
    {
        // Regression: CheckState.Indeterminate collapsed to unchecked, so the sample's "c Menu" drew no
        // indicator at all where the Gtk3 build shows one. Gtk4 models the third state as Inconsistent.
        var item = new ToolStripMenuItem("c Menu") { Checked = true, CheckState = CheckState.Indeterminate };

        Assert.Equal(CheckState.Indeterminate, item.CheckState);
        Assert.True(item.self.checkButton.Inconsistent, "the indicator must render in its third state");

        // WinForms reports an indeterminate item as checked.
        Assert.True(item.Checked);
    }

    [Fact]
    public void ClearingAnIndeterminateItem_ReturnsItToUnchecked()
    {
        var item = new ToolStripMenuItem("c Menu") { CheckState = CheckState.Indeterminate };

        item.CheckState = CheckState.Unchecked;

        Assert.Equal(CheckState.Unchecked, item.CheckState);
        Assert.False(item.self.checkButton.Inconsistent);
        Assert.False(item.Checked);
    }

    [Fact]
    public void MenuBarItem_DropsItsMenuDownwards()
    {
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("Menu 2");
        menu.Items.Add(top);
        top.DropDownItems.Add(new ToolStripMenuItem("a Menu"));

        Assert.Equal(Gtk.PositionType.Bottom, top.self.DropDownPosition);
    }

    [Fact]
    public void SubmenuParent_CascadesToTheRight_NotOverItsOwnMenu()
    {
        // Regression: every drop-down popover was created at PositionType.Bottom, so a submenu opened
        // downwards on top of the menu it came from - hiding the entries below its parent (in the sample,
        // "b Menu"'s submenu covered the separator and "c Menu"). Gtk3 got this right because
        // GtkMenuItem.Submenu placed cascading menus itself; Gtk4 has no menu widgets, so we place them.
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("Menu 2");
        menu.Items.Add(top);

        var parent = new ToolStripMenuItem("b Menu");
        top.DropDownItems.Add(parent);
        parent.DropDownItems.Add(new ToolStripMenuItem("bb Menu"));
        parent.DropDownItems.Add(new ToolStripMenuItem("bb Menu 2"));

        Assert.Equal(Gtk.PositionType.Right, parent.self.DropDownPosition);
        // Top edge level with the parent item, rather than centred on it.
        Assert.Equal(Gtk.Align.Start, parent.self.DropDownValign);
    }

    [Fact]
    public void SubmenuParent_ShowsAnArrow_AndMenuBarItemDoesNot()
    {
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("Menu 2");
        menu.Items.Add(top);

        var parent = new ToolStripMenuItem("b Menu");
        top.DropDownItems.Add(parent);
        parent.DropDownItems.Add(new ToolStripMenuItem("bb Menu"));

        Assert.True(parent.self.ShowsSubmenuArrow, "a submenu parent must indicate that it opens a submenu");
        Assert.False(top.self.ShowsSubmenuArrow, "a menu-bar entry never shows a submenu arrow");
    }

    [Fact]
    public void LeafItem_ShowsNoSubmenuArrow()
    {
        var top = new ToolStripMenuItem("File");
        var leaf = new ToolStripMenuItem("New");
        top.DropDownItems.Add(leaf);

        Assert.False(leaf.self.ShowsSubmenuArrow);
    }

    [Fact]
    public void SubmenuPlacement_SurvivesItemsBeingFilledBeforeTheBarIsBuilt()
    {
        // The Designer routinely populates DropDownItems before adding the item to the MenuStrip, so the
        // placement cannot be decided once at popover-construction time.
        var top = new ToolStripMenuItem("Menu 2");
        top.DropDownItems.Add(new ToolStripMenuItem("a Menu"));

        var menu = new MenuStrip();
        menu.Items.Add(top);

        Assert.Equal(Gtk.PositionType.Bottom, top.self.DropDownPosition);
        Assert.False(top.self.ShowsSubmenuArrow);
    }

    [Fact]
    public void DropDownRows_SpanTheMenuWidth_UnlikeMenuBarEntries()
    {
        // A menu row must fill the menu, both so the whole row highlights on hover and so a submenu opened
        // to the right starts at the menu's edge: a popover anchors to the widget it is attached to, so a
        // caption-width button pushed the submenu back over the menu it came from. A menu-bar entry keeps
        // hugging its caption, otherwise the bar's entries spread out across it.
        var menu = new MenuStrip();
        var top = new ToolStripMenuItem("Menu 2");
        menu.Items.Add(top);

        var row = new ToolStripMenuItem("a Menu");
        top.DropDownItems.Add(row);

        Assert.Equal(Gtk.Align.Fill, row.self.checkButton.Halign);
        Assert.True(row.self.checkButton.Hexpand);
        Assert.Equal(Gtk.Align.Start, top.self.checkButton.Halign);
    }

    [Fact]
    public void ContextMenuRows_AlsoSpanTheMenuWidth()
    {
        var context = new ContextMenuStrip();
        var row = new ToolStripMenuItem("Cut");
        context.Items.Add(row);

        Assert.Equal(Gtk.Align.Fill, row.self.checkButton.Halign);
    }

    [Fact]
    public void ContextMenuSubmenu_AlsoCascadesToTheRight()
    {
        // A context menu has no menu bar, so every one of its items that owns a drop-down is a submenu
        // parent and must cascade sideways.
        var context = new ContextMenuStrip();
        var parent = new ToolStripMenuItem("More");
        context.Items.Add(parent);
        parent.DropDownItems.Add(new ToolStripMenuItem("Deeper"));

        Assert.Equal(Gtk.PositionType.Right, parent.self.DropDownPosition);
        Assert.True(parent.self.ShowsSubmenuArrow);
    }

    [Fact]
    public void SubmenuArrow_StaysTrailing_WhenTextImageRelationChanges()
    {
        // SetTextImageRelation rebuilds the content row; the arrow must end up last whatever the order of
        // icon and caption, or it would appear in the middle of the item.
        var top = new ToolStripMenuItem("Menu 2");
        var parent = new ToolStripMenuItem("b Menu");
        top.DropDownItems.Add(parent);
        parent.DropDownItems.Add(new ToolStripMenuItem("bb Menu"));

        parent.TextImageRelation = TextImageRelation.TextBeforeImage;

        var content = (Gtk.Box)parent.checkButton.Child;
        var children = ChildrenOf(content);

        Assert.Same(parent.self.SubmenuArrowWidget, children[^1]);
        Assert.True(parent.self.ShowsSubmenuArrow);
    }

    [Fact]
    public void ClickingItemWithDropDown_OpensIt_WithoutRaisingLeafClick()
    {
        // A top-level item that owns a drop-down opens it rather than firing a command; its own Click must
        // not be raised (WinForms behaviour). The popover only actually pops up once on screen, but the
        // branch (open vs. leaf-click) is what this pins.
        var top = new ToolStripMenuItem("File");
        top.DropDownItems.Add(new ToolStripMenuItem("New"));
        var topClicks = 0;
        var dropDownOpened = 0;
        top.Click += (s, e) => topClicks++;
        top.DropDownOpened += (s, e) => dropDownOpened++;

        SimulateUserClick(top);

        Assert.Equal(0, topClicks);
        Assert.Equal(1, dropDownOpened);
    }

    [Fact]
    public void ClickingLeafItem_RaisesClick_NotDropDown()
    {
        var leaf = new ToolStripMenuItem("New"); // no children
        var clicks = 0;
        var opened = 0;
        leaf.Click += (s, e) => clicks++;
        leaf.DropDownOpened += (s, e) => opened++;

        SimulateUserClick(leaf);

        Assert.Equal(1, clicks);
        Assert.Equal(0, opened);
    }

    [Fact]
    public void ClickingLeafItem_DoesNotChangeChecked()
    {
        // WinForms default (CheckOnClick=false): clicking a menu command must not leave it checked. The
        // Gtk4 CheckButton toggles on click, so without the revert a command item would show a stray
        // checkmark and stay "checked".
        var leaf = new ToolStripMenuItem("New");
        Assert.False(leaf.Checked);

        SimulateUserClick(leaf);

        Assert.False(leaf.Checked);
    }

    [Fact]
    public void ProgrammaticallyCheckedItem_StaysCheckedAfterClick()
    {
        // A checkable item is driven via the Checked property (e.g. the demo's cMenu). Clicking it must not
        // flip that state off.
        var item = new ToolStripMenuItem("cMenu") { Checked = true };
        Assert.True(item.Checked);

        SimulateUserClick(item);

        Assert.True(item.Checked);
    }

    [Fact]
    public void ClickingLeafItem_ClosingMenu_DoesNotThrow()
    {
        // The leaf-selected path walks up to close the containing popover; with no popover ancestor it must
        // be a safe no-op (and it must not throw when there is one, either).
        var top = new ToolStripMenuItem("File");
        var leaf = new ToolStripMenuItem("New");
        top.DropDownItems.Add(leaf);

        var ex = Record.Exception(() => SimulateUserClick(leaf));

        Assert.Null(ex);
    }

    [Fact]
    public void CheckOnClick_DefaultsToFalse()
    {
        // WinForms default: an item only becomes checkable when the application opts in.
        Assert.False(new ToolStripMenuItem("New").CheckOnClick);
    }

    [Fact]
    public void CheckOnClickItem_TogglesCheckedOnEveryClick()
    {
        // Regression: a checked check-on-click item could not be unchecked by clicking it. The Gtk4 widget
        // toggle is always reverted, and nothing put the WinForms state back the other way, so the item
        // stayed checked however often it was clicked (the sample's "Dropdown List 1 / Item 1").
        var item = new ToolStripMenuItem("Item 1") { CheckOnClick = true, Checked = true };

        SimulateUserClick(item);

        Assert.False(item.Checked);
        Assert.Equal(CheckState.Unchecked, item.CheckState);
        Assert.False(item.checkButton.Active); // the indicator follows, so a re-opened menu shows no check

        SimulateUserClick(item);

        Assert.True(item.Checked);
        Assert.Equal(CheckState.Checked, item.CheckState);
        Assert.True(item.checkButton.Active);
    }

    [Fact]
    public void CheckOnClickItem_RaisesCheckedChangedAndCheckStateChanged_BeforeClick()
    {
        // WinForms flips the state from its click handling and notifies before raising Click, so a Click
        // handler already sees the new state.
        var item = new ToolStripMenuItem("Item 1") { CheckOnClick = true };
        var order = new System.Collections.Generic.List<string>();
        item.CheckedChanged += (s, e) => order.Add("checked");
        item.CheckStateChanged += (s, e) => order.Add("checkstate");
        item.Click += (s, e) => order.Add("click:" + item.Checked);

        SimulateUserClick(item);

        Assert.Equal(new[] { "checked", "checkstate", "click:True" }, order);
    }

    [Fact]
    public void CheckOnClickItem_PerformClick_TogglesCheckedToo()
    {
        // PerformClick is the programmatic form of a click and must behave the same way.
        var item = new ToolStripMenuItem("Item 1") { CheckOnClick = true, Checked = true };

        item.PerformClick();

        Assert.False(item.Checked);
    }

    [Fact]
    public void SettingCheckState_RaisesCheckedChangedAndCheckStateChanged_OnceEach()
    {
        // The state used to change silently, so nothing driven off CheckedChanged (a menu mirrored into a
        // tray icon, an enabled/disabled command) ever heard about a programmatic change.
        var item = new ToolStripMenuItem("Item 1");
        var checkedChanged = 0;
        var stateChanged = 0;
        item.CheckedChanged += (s, e) => checkedChanged++;
        item.CheckStateChanged += (s, e) => stateChanged++;

        item.Checked = true;
        item.Checked = true; // no change - no second notification

        Assert.Equal(1, checkedChanged);
        Assert.Equal(1, stateChanged);
    }

    [Fact]
    public void IndeterminateCheckState_LeavesToggleInactive()
    {
        // CheckState.Indeterminate is "inconsistent", not "active"; the state setter used to assign Checked
        // afterwards as well, which switched the toggle on and drew a full check mark.
        var item = new ToolStripMenuItem("Item 1") { CheckState = CheckState.Indeterminate };

        Assert.Equal(CheckState.Indeterminate, item.CheckState);
        Assert.True(item.Checked); // WinForms counts indeterminate as checked
        Assert.True(item.checkButton.Inconsistent);
        Assert.False(item.checkButton.Active);
    }

    [Fact]
    public void SettingCheckedInCode_DoesNotRaiseClick()
    {
        // The Toggled->Click bridge must fire only for user activation. Setting Checked/CheckState in code
        // also toggles the underlying CheckButton (firing Toggled), but that must NOT be reported as a click
        // - otherwise initializing a checked menu item would run its Click handler.
        var item = new ToolStripMenuItem("Toggle");
        var clicks = 0;
        item.Click += (s, e) => clicks++;

        item.Checked = true;
        item.Checked = false;
        item.CheckState = CheckState.Checked;

        Assert.Equal(0, clicks);
    }
}
