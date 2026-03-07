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
        public CheckBox()
        {
            self.Toggled += Self_Toggled;
            self.ButtonReleaseEvent += Self_ButtonReleaseEvent;
        }
        public bool Checked
        {
            get { return self.Active; }
            set { self.Active = value; if (self.IsRealized) { self.Inconsistent = false; } }
        }

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
