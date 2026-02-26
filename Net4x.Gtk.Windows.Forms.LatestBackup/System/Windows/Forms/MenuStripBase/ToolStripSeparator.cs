using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a line used to group items of a
    /// <see cref="System.Windows.Forms.ToolStrip" />
    /// or the drop-down items of a
    /// <see cref="System.Windows.Forms.MenuStrip" /> or
    /// <see cref="System.Windows.Forms.ContextMenuStrip" /> or other
    /// <see cref="System.Windows.Forms.ToolStripDropDown" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripSeparator : WidgetToolStrip<Gtk.SeparatorMenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSeparator" /> class.
        /// </summary>
        public ToolStripSeparator() : base("ToolStripSeparator")
        {
            this.MenuItem.Hexpand = false;
            this.MenuItem.Vexpand = false;
            this.MenuItem.Halign = Gtk.Align.Center;
            this.MenuItem.Valign = Gtk.Align.Center;
            this.MenuItem.StyleContext.AddClass("ToolStripSeparator");
        }
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
        public override int Width { get; set; }
    }
}
