using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
using GLib;
using Pango;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.ToolStripItem" /> objects.
/// </summary>
public class ToolStripItemCollection : List<ToolStripItem>, ICollection, IEnumerable
{
    /// <summary>
    /// The is menu strip
    /// </summary>
    private readonly bool isMenuStrip;

    /// <summary>
    /// The is status strip
    /// </summary>
    private readonly bool isStatusStrip;

    /// <summary>
    /// The is tool strip
    /// </summary>
    private readonly bool isToolStrip;

    /// <summary>
    /// The is tool strip drop down
    /// </summary>
    private readonly bool isToolStripDropDown;

    /// <summary>
    /// The menu
    /// </summary>
    private Gtk.Menu menu;

    /// <summary>
    /// The owner
    /// </summary>
    private readonly ToolStripItem owner;

    /// <summary>
    /// The status strip
    /// </summary>
    private readonly StatusStrip statusStrip;

    /// <summary>
    /// The tool strip
    /// </summary>
    private readonly ToolStrip toolStrip;

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
        this.menu = owner.Widget as Gtk.Menu;
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
        if (owner.Widget is Gtk.Menu gmenu)
        {
            this.menu = gmenu;
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

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="itemsCollection">
    /// if set to
    /// <c>
    /// true
    /// </c>items collection.
    /// </param>
    internal ToolStripItemCollection(ToolStripItem owner, bool itemsCollection)
                : this(owner, itemsCollection, isReadOnly: false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="itemsCollection">
    /// if set to
    /// <c>
    /// true
    /// </c>items collection.
    /// </param>
    /// <param name="isReadOnly">
    /// if set to
    /// <c>
    /// true
    /// </c>is read only.
    /// </param>
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
        set { menu.Insert(value.Widget, index); base[index] = value; }
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
    public ToolStripItem Add(string text, Image image, EventHandler onClick)
    {
        ToolStripItem toolStripItem = new ToolStripLabel();
        AddMemu(toolStripItem);

        return toolStripItem;
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
            toolStrip.self.Add(item.Widget);
        }
        else if (isStatusStrip == true)
        {
            statusStrip.self.Add(item.Widget);
        }
        else if (isMenuStrip == true)
        {
            toolStrip.self.Add(item.Widget);
        }
        else if (isToolStripDropDown == true)
        {
            menu.Add(item.Widget);
        }
        else
        {
            if (owner.MenuItem.Submenu == null)
            {
                this.menu = new Gtk.Menu();
                owner.MenuItem.Submenu = this.menu;
            }
            menu.Add(item.Widget);
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
        for (int i = 0; i < toolStripItems.Length; i++)
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
        int count = toolStripItems.Count;
        for (int i = 0; i < count; i++)
        {
            AddMemu(toolStripItems[i]);
        }
    }

}