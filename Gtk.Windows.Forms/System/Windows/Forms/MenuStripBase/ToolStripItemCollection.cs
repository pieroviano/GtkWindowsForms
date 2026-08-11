using System.Collections;
using Gtk;
using Image = System.Drawing.Image;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.ToolStripItem" /> objects.
/// </summary>
public class ToolStripItemCollection : List<ToolStripItem>, ICollection, IEnumerable
{

    private readonly bool isMenuStrip;

    private readonly bool isStatusStrip;

    private readonly bool isToolStrip;

    private readonly bool isToolStripDropDown;

    // Qualified: the legacy WinForms Menu type now exists in this namespace, so an unqualified
    // 'Menu' here would bind to it rather than to Gtk's.
    private Gtk.Popover? menu;

    // Set when this collection is the DropDownItems of a ToolStripMenuItem: child items are appended into
    // that item's own drop-down popover instead of a standalone Gtk.Popover.
    private readonly StripMenuItem? menuItemOwner;

    // Vertical content box for a standalone popover (menu != null): a Gtk.Popover holds a single Child, so
    // items are appended into this box rather than overwriting each other.
    private Gtk.Box? menuContent;

    private readonly ToolStripItem? owner;

    private readonly StatusStrip? statusStrip;

    private readonly ToolStrip? toolStrip;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStripItemCollection(ToolStrip owner)
    {
        this.toolStrip = owner;
        isToolStrip = true;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="toolStrip">
    /// The tool strip.
    /// </param>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStripItemCollection(ToolStrip toolStrip, string owner)
    {
        this.toolStrip = toolStrip;
        isMenuStrip = owner == "MenuStrip";
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStripItemCollection(StatusStrip owner)
    {
        this.statusStrip = owner;
        isStatusStrip = true;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStripItemCollection(ToolStripDropDown owner)
    {
        // The drop-down's GtkControl is its native Gtk.Popover (StripDropDown). It used to be reached via the
        // ToolStripItem-style Widget.MenuItem; now that ToolStripDropDown derives from ToolStrip/Control the
        // menu is the GtkControl directly.
        this.menu = owner.GtkControl as Gtk.Popover;
        isToolStripDropDown = true;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStripItemCollection(ToolStripItem owner)
    {
        this.owner = owner;
        // owner.Widget is null while the owning ToolStripItem is still in its constructor (the item builds
        // this collection before GetWidget() runs). Use null-conditional access so that case is a no-op
        // instead of a NullReferenceException - which, thrown from a GTK signal callback, crashes the host.
        if (owner.Widget?.MenuItem is Gtk.Popover gmenu)
        {
            this.menu = gmenu;
            isToolStripDropDown = true;
        }
        else if (owner.Widget?.ToolItem is StripMenuToolButton tmenu)
        {
            this.menu = new Gtk.Popover();
            tmenu.Popover = this.menu; // Gtk4: MenuButton.Menu -> Popover
            isToolStripDropDown = true;
        }
        else if (owner.Widget?.MenuItem is StripMenuItem smi)
        {
            // A ToolStripMenuItem's drop-down: its child items live in the item's own popover (built lazily
            // on first add and anchored to the item), so clicking the item can pop them up.
            this.menuItemOwner = smi;
        }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public ToolStripItemCollection(ToolStripItem owner, ToolStripItem[] value)
    {
        this.owner = owner;
        isToolStrip = false;
        AddRange(value);
    }

    internal ToolStripItemCollection(ToolStripItem owner, bool itemsCollection)
        : this(owner, itemsCollection, isReadOnly: false)
    {
    }

    internal ToolStripItemCollection(ToolStripItem owner, bool itemsCollection, bool isReadOnly)
    {
        this.owner = owner;
        isToolStrip = false;

    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    //-------------------
    public new ToolStripItem this[int index]
    {
        get { return base[index]; }
        set { menu?.AddChild(value.Widget.MenuItem); base[index] = value; } // Gtk4: Popover has no indexed Insert; menu layout deferred
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    public ToolStripItem Add(string text)
    {
        return Add(text, null, null);
    }

    /// <summary>
    /// Adds the specified image.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    public ToolStripItem Add(Image image)
    {
        return Add(null, image, null);
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    public ToolStripItem Add(string text, Image image)
    {
        return Add(text, image, null);
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="onClick">
    /// The on click.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    public ToolStripItem Add(string? text, Image? image, EventHandler? onClick)
    {
        var toolStripItem = new ToolStripMenuItem();
        toolStripItem.Text = text ?? string.Empty;
        toolStripItem.Image = image;
        if (onClick != null)
            toolStripItem.Click += onClick;
        AddMemu(toolStripItem);

        return toolStripItem;
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public new void Add(ToolStripItem item)
    {
        AddMemu(item);
    }

    /// <summary>
    /// Adds the memu.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    // Returns the vertical box that holds a standalone popover's items, creating the popover (for the legacy
    // owner path) and the box on first use. Null only when there is no popover and no owner to build one for.
    private Gtk.Box? GetOrCreatePopoverContent()
    {
        if (menu == null)
        {
            if (owner?.MenuItem == null)
            {
                return null;
            }

            // Gtk4: Gtk.MenuItem.Submenu removed; a submenu is a Popover. (Anchoring/pop-up for this legacy
            // fallback path is still deferred to migration #11.)
            menu = new Gtk.Popover { Autohide = true, Position = Gtk.PositionType.Bottom, HasArrow = false };
        }

        if (menuContent == null)
        {
            menuContent = new Gtk.Box(Gtk.Orientation.Vertical, 0) { Visible = true };
            menu.Child = menuContent;
        }

        return menuContent;
    }

    public int AddMemu(ToolStripItem item)
    {
        item.Parent = owner;
        if (isToolStrip == true)
        {
            AppendIfWidget(toolStrip?.self, item.Widget?.ToolItem);
        }
        else if (isStatusStrip == true)
        {
            AppendIfWidget(statusStrip?.self, item.Widget?.ToolItem);
        }
        else if (isMenuStrip == true)
        {
            // Top-level menu-bar items must never show a check-indicator square (they open submenus, they
            // are not toggles). Tag them so the .MenuBarItem CSS collapses the indicator unconditionally -
            // unlike ordinary drop-down items, which keep it to render a checkmark when Checked.
            if (item.Widget is StripMenuItem menuBarItem)
            {
                menuBarItem.checkButton.StyleContext.AddClass("MenuBarItem");
                menuBarItem.ConfigureAsMenuBarItem();
            }

            AppendIfWidget(toolStrip?.self, item.Widget?.MenuItem);
        }
        else if (menuItemOwner != null)
        {
            // Append into the owning menu item's drop-down popover (created/anchored on first add).
            AppendIfWidget(menuItemOwner.EnsureDropDownContent(), item.Widget?.MenuItem);
        }
        else
        {
            // A standalone popover (ContextMenuStrip / ToolStripDropDown, or the legacy owner path) hosts its
            // items in a vertical box rather than a single Child, so multiple items are not overwritten.
            AppendIfWidget(GetOrCreatePopoverContent(), item.Widget?.MenuItem);
        }

        base.Add(item);

        return Count;
    }

    // Appends a child to a GTK container, skipping when either is null. Some ToolStripItem kinds expose only
    // a ToolItem or only a MenuItem (the other is null); appending null tripped Gtk-CRITICAL
    // "gtk_box_append: assertion 'GTK_IS_WIDGET (child)' failed".
    private static void AppendIfWidget(Gtk.Widget? parent, Gtk.Widget? child)
    {
        if (parent != null && child != null)
        {
            parent.AddChild(child);
        }
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="toolStripItems">
    /// The tool strip items.
    /// </param>
    public void AddRange(ToolStripItem[] toolStripItems)
    {
        for (var i = 0; i < toolStripItems.Length; i++)
        {
            AddMemu(toolStripItems[i]);
        }
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="toolStripItems">
    /// The tool strip items.
    /// </param>
    public void AddRange(ToolStripItemCollection toolStripItems)
    {
        var count = toolStripItems.Count;
        for (var i = 0; i < count; i++)
        {
            AddMemu(toolStripItems[i]);
        }
    }
}
