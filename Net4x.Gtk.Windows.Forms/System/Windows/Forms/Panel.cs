/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Xml.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Used to group collections of controls.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class Panel : ScrollableControl
    {

        public Gtk.Overlay contaner = new Gtk.Overlay();

        public readonly PanelBase self = new PanelBase();

        private ControlCollection _controls;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Panel" /> class.
        /// </summary>
        public Panel() : base()
        {
            _controls = new ControlCollection(this, contaner);
            contaner.Margin = 0;
            contaner.Halign = Align.Fill;
            contaner.Valign = Align.Fill;
            contaner.Hexpand = false;
            contaner.Vexpand = false;
            contaner.BorderWidth = 0;
            contaner.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
            self.Add(contaner);
        }

        public override ControlCollection Controls => _controls;

        public override object GtkControl => self;

        public override Padding Padding
        {
            get => base.Padding;
            set
            {
                base.Padding = value;
                contaner.MarginStart = value.Left;
                contaner.MarginTop = value.Top;
                contaner.MarginEnd = value.Right;
                contaner.MarginBottom = value.Bottom;
            }
        }
    }
}
