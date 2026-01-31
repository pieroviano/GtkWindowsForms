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
    public class HScrollBar : ScrollBar
    {
        public ScrollbarBase<Gtk.HScrollbar> self = new ScrollbarBase<Gtk.HScrollbar>(Gtk.Orientation.Horizontal);
        public override object GtkControl => self;
        public override Gtk.Adjustment Adjustment { get => self.Adjustment; }
        public HScrollBar() : base()
        {
 
        }

    }
}
