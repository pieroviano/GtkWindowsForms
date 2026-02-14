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
    [DesignerCategory("Component")]
    public partial class RadioButton : Control
    {
        public readonly RadioButtonBase self = new RadioButtonBase();
        public override object GtkControl => self;
        public RadioButton():base() {
            self.ParentSet += Self_ParentSet;
        }

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
        public event EventHandler CheckedChanged;

        public override string Text { get { return self.Label; } set { self.Label = value;} }
        public bool Checked { get { return self.Active; } set { _Checked = true; self.Active = true; } }
        private bool _Checked;
    }
}
