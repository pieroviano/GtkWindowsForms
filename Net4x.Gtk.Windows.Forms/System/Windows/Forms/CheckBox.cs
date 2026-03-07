/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows
    /// <see cref="System.Windows.Forms.CheckBox" />
    /// </summary>
    /// <filterpriority> 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class CheckBox : Control
    {

        public readonly CheckBoxBase self = new CheckBoxBase();

        public event EventHandler CheckedChanged;

        public virtual event EventHandler CheckStateChanged;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.CheckBox" /> class.
        /// </summary>
        public CheckBox()
        {
            self.Toggled += Self_Toggled;
            self.ButtonReleaseEvent += Self_ButtonReleaseEvent;
        }
        /// <summary>
        /// Gets or set a value indicating whether the
        /// <see cref="System.Windows.Forms.CheckBox" /> is in the checked state.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.CheckBox" /> is in the checked state; otherwise, false. The default value is false.NoteIf the
        /// <see cref="System.Windows.Forms.CheckBox.ThreeState" /> property is set to true, the
        /// <see cref="System.Windows.Forms.CheckBox.Checked" /> property will return true for either a Checked or Indeterminate
        /// <see cref="System.Windows.Forms.CheckBox.CheckState" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Checked
        {
            get { return self.Active; }
            set { self.Active = value; if (self.IsRealized) { self.Inconsistent = false; } }
        }

        /// <summary>
        /// Gets or sets the state of the
        /// <see cref="System.Windows.Forms.CheckBox" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CheckState" /> enumeration values. The default value is Unchecked.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.CheckState" /> enumeration values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public CheckState CheckState
        {
            get { if (self.Inconsistent == true) { return CheckState.Indeterminate; } else { return self.Active ? CheckState.Checked : CheckState.Unchecked; } }
            set { self.Inconsistent = value == CheckState.Indeterminate; self.Active = value == CheckState.Checked; }
        }

        public override object GtkControl => self;

        public override string Text { get { return self.Label; } set { self.Label = value; } }

        private void Self_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
        {
            if (self.Inconsistent == true)
                self.Inconsistent = false;
        }

        private void Self_Toggled(object sender, EventArgs e)
        {
            if (CheckedChanged != null && self.IsVisible)
                CheckedChanged(this, EventArgs.Empty);
            if (CheckStateChanged != null && self.IsVisible)
                CheckStateChanged(this, EventArgs.Empty);
        }
    }
}
