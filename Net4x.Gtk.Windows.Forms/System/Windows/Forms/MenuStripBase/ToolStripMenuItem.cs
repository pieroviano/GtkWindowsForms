using Gtk;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a selectable option displayed on a
    /// <see cref="System.Windows.Forms.MenuStrip" /> or
    /// <see cref="System.Windows.Forms.ContextMenuStrip" /> Although
    /// <see cref="System.Windows.Forms.ToolStripMenuItem" />
    /// replaces and adds functionality to the
    /// <see cref="!:System.Windows.Forms.MenuItem" /> control of previous versions,
    /// <see cref="!:System.Windows.Forms.MenuItem" />
    /// is retained for both backward compatibility and future use if you choose.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripMenuItem : WidgetToolStrip<Gtk.MenuItem>
    {
        public ToolStripMenuItem() : base("ToolStripMenuItem")
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }
        public override bool Checked
        {
            get
            {
                if (this.flagBox.Child is Gtk.CheckButton checkbutton)
                {
                    return checkbutton.Active;
                }
                else if (this.flagBox.Child is Gtk.RadioButton radiobutton)
                {
                    return radiobutton.Active;
                }

                return base.Checked;
            }
            set
            {
                base.Checked = value;
                if (this.flagBox.Child is Gtk.CheckButton checkbutton)
                {
                    checkbutton.Active = value;
                }
                else if (this.flagBox.Child is Gtk.RadioButton radiobutton)
                {
                    radiobutton.Active = value;
                }
            }
        }

        public override CheckState CheckState
        {
            get => base.CheckState;
            set
            {
                base.CheckState = value;
            }
        }
    }
}
