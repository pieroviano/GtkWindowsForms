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
using System.Xml.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides an empty control that can be used to create other controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignerCategory("UserControl")]
    [ToolboxItem(true)]
    public partial class UserControl : ContainerControl
    {

        public readonly UserControlBase self = new UserControlBase();

        private ControlCollection _controls;

        private Gtk.Overlay contaner;

        public override event EventHandler Load;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.UserControl" /> class.
        /// </summary>
        public UserControl() : base()
        {
            contaner = new Gtk.Overlay();
            contaner.MarginStart = 0;
            contaner.MarginTop = 0;
            contaner.BorderWidth = 0;
            contaner.Halign = Align.Fill;
            contaner.Valign = Align.Fill;
            contaner.Hexpand = false;
            contaner.Vexpand = false;
            contaner.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
            _controls = new ControlCollection(this, contaner);
            self.Add(contaner);
            self.Override.Paint += Override_Paint;
            self.ParentSet += Self_ParentSet;
        }

        public System.Drawing.SizeF AutoScaleDimensions { get; set; }

        public System.Windows.Forms.AutoScaleMode AutoScaleMode { get; set; }

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

        public override void ResumeLayout(bool performLayout)
        {

        }

        public override void SuspendLayout()
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
        }

        protected override void OnParentChanged(EventArgs e)
        {
        }

        private void Override_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        private void Self_ParentSet(object o, ParentSetArgs args)
        {
            OnParentChanged(EventArgs.Empty);
        }

    }
}
