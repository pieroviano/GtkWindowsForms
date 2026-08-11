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
}
