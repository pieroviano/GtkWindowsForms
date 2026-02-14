/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing;


namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    public sealed class SplitterPanel : Panel
    {
        internal SplitContainer Owner;
        public SplitterPanel(SplitContainer owner) : base()
        {
            self.Override.AddClass("SplitterPanel");
            Owner = owner;
            self.BorderWidth = 0;
            self.ShadowType = Gtk.ShadowType.None;
            self.Margin = 0;
            self.Halign = Gtk.Align.Fill;
            self.Valign = Gtk.Align.Fill;
            self.Hexpand = false;
            self.Vexpand = false;
        }

        public override DockStyle Dock { get { return DockStyle.Fill; } set { } }
        public override Size Size { get; set; }
        public override int Width { get; set; }
        public override int Height { get; set; }
    }
}
