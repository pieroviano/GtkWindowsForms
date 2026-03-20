using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a line used to group items of a
/// <see cref="T:System.Windows.Forms.ToolStrip" /> or the drop-down items of a
/// <see cref="T:System.Windows.Forms.MenuStrip" /> or
/// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> or other
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> control.
/// </summary>
public class ToolStripSeparator : WidgetToolStrip<Gtk.SeparatorMenuItem>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSeparator" /> class.
    /// </summary>
    public ToolStripSeparator() : base("ToolStripSeparator")
    {
        this.MenuItem.Hexpand = false;
        this.MenuItem.Vexpand = false;
        this.MenuItem.Halign = Gtk.Align.Center;
        this.MenuItem.Valign = Gtk.Align.Center;
        this.MenuItem.StyleContext.AddClass("ToolStripSeparator");
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size
    {
        get => base.Size;

        set
        {
            if (value.Width > value.Height)
                base.Size = new Size(value.Width, 2);
            else
                base.Size = new Size(2, value.Height);
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public override int Width
    {
        get;
        set;
    }

}