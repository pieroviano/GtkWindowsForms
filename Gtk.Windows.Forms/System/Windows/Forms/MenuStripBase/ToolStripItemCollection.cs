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
        else if (owner.Widget?.ToolItem is Gtk.MenuToolButton tmenu)
        {
            this.menu = new Gtk.Popover();
            tmenu.Menu = this.menu;
            isToolStripDropDown = true;
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
        set { menu?.Insert(value.Widget.MenuItem, index); base[index] = value; }
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
    public int AddMemu(ToolStripItem item)
    {
        item.Parent = owner;
        if (isToolStrip == true)
        {
            toolStrip?.self.Add(item.Widget.ToolItem);
        }
        else if (isStatusStrip == true)
        {
            statusStrip?.self.Add(item.Widget.ToolItem);
        }
        else if (isMenuStrip == true)
        {
            toolStrip?.self.Add(item.Widget.MenuItem);
        }
        else if (isToolStripDropDown == true)
        {
            menu?.Add(item.Widget.MenuItem);
        }
        else
        {
            if (owner?.MenuItem != null && owner.MenuItem.Submenu == null)
            {
                this.menu = new Gtk.Popover();
                owner.MenuItem.Submenu = this.menu;
            }
            menu?.Add(item.Widget.MenuItem);
        }

        base.Add(item);

        return Count;
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
