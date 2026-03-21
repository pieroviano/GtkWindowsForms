using System;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a selectable option displayed on a
/// <see cref="T:System.Windows.Forms.MenuStrip" /> or
/// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> Although
/// <see cref="T:System.Windows.Forms.ToolStripMenuItem" />
/// replaces and adds functionality to the
/// <see cref="!:System.Windows.Forms.MenuItem" /> control of previous versions,
/// <see cref="!:System.Windows.Forms.MenuItem" />
/// is retained for both backward compatibility and future use if you choose.
/// </summary>
public class ToolStripMenuItem : WidgetToolStrip<Gtk.MenuItem>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripMenuItem" /> class.
    /// </summary>
    public ToolStripMenuItem() : base("ToolStripMenuItem")
    {
        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
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

    /// <summary>
    /// Gets or sets the state of the check.
    /// </summary>
    /// <value>
    /// The state of the check.
    /// </value>
    public override CheckState CheckState
    {
        get => base.CheckState;
        set
        {
            base.CheckState = value;
        }
    }
}