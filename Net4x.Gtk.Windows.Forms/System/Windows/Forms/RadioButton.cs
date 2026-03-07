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
    /// Enables the user to select a single option from a group of choices when paired with other
    /// <see cref="System.Windows.Forms.RadioButton" /> controls.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class RadioButton : Control
    {

        public readonly RadioButtonBase self = new RadioButtonBase();
        private bool _Checked;
        public event EventHandler CheckedChanged;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.RadioButton" /> class.
        /// </summary>
        public RadioButton() : base()
        {
            self.ParentSet += Self_ParentSet;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <returns>true if the check box is checked; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Checked { get { return self.Active; } set { _Checked = true; self.Active = true; } }

        public override object GtkControl => self;

        public override string Text { get { return self.Label; } set { self.Label = value; } }

        private void Self_ParentSet(object o, Gtk.ParentSetArgs args)
        {
            Gtk.Container con = self.Parent as Gtk.Container;
            foreach (var widget in con.Children)
            {
                if (widget is Gtk.RadioButton group)
                {
                    ((Gtk.RadioButton)o).Group = new Gtk.RadioButton[0];
                    // the first one added into the containerradio配组
                    ((Gtk.RadioButton)o).JoinGroup(group);
                    break;
                }
            }
            self.Active = _Checked;
            self.Toggled += Self_Toggled;
        }

        private void Self_Toggled(object sender, EventArgs e)
        {
            if (CheckedChanged != null && self.IsVisible)
                CheckedChanged(this, e);
        }
    }
}
