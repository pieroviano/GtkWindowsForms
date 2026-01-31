/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GtkSystem.Windows.Forms.GTKControls.ControlBase;

namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    public class VScrollBar : ScrollBar
    {
        public ScrollbarBase<Gtk.VScrollbar> self=new ScrollbarBase<Gtk.VScrollbar>(Gtk.Orientation.Vertical);
        public override object GtkControl => self;
        public override Gtk.Adjustment Adjustment { get => self.Adjustment; }
        public VScrollBar() : base()
        {

        }
    }
}
