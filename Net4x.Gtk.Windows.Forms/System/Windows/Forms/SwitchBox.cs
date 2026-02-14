/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GTKControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    public partial class SwitchBox : Control
    {
        public readonly SwitchBoxBase self = new SwitchBoxBase();
        public override object GtkControl => self;
        public SwitchBox() {
            self.ButtonReleaseEvent += Self_ButtonReleaseEvent;
        }
        private void Self_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
        {
            if (CheckedChanged != null && self.IsVisible)
                CheckedChanged(this, EventArgs.Empty);
        }
        public override string Text { get { return self.TooltipText; } set { self.TooltipText = value; } }
        public  bool Checked { get { return self.Active; } 
            set { 
                self.Active = value;
                if (CheckedChanged != null && self.IsVisible)
                    CheckedChanged(this, EventArgs.Empty);
            } 
        }
        public event EventHandler CheckedChanged;
    }
}
