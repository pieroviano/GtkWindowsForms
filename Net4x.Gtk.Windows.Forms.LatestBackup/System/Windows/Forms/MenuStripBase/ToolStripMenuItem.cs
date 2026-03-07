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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> class.
        /// </summary>
        public ToolStripMenuItem() : base("ToolStripMenuItem")
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> is checked.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> is checked or is in an indeterminate state; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets a value indicating whether a
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> is in the checked, unchecked, or indeterminate state.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values. The default is Unchecked.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The
        /// <see cref="System.Windows.Forms.ToolStripMenuItem.CheckState" /> property is not set to one of the
        /// <see cref="System.Windows.Forms.CheckState" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
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
