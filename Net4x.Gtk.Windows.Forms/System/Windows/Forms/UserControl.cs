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
    /// Provides a lightweight container control that can host other controls.
    /// This class is a GTK-backed implementation intended to mirror the WinForms <see cref="UserControl" /> API.
    /// </summary>
    [DesignerCategory("UserControl")]
    [ToolboxItem(true)]
    public partial class UserControl : ContainerControl
    {
        /// <summary>
        /// Underlying GTK-based control that implements the native behavior for this <see cref="UserControl" />.
        /// </summary>
        public readonly UserControlBase self = new UserControlBase();

        /// <summary>
        /// The controls
        /// </summary>
        private ControlCollection _controls;

        /// <summary>
        /// The contaner
        /// </summary>
        private Gtk.Overlay contaner;

        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        public override event EventHandler Load;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl" /> class.
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

        /// <summary>
        /// Gets or sets the design-time AutoScale dimensions for this control.
        /// </summary>
        /// <value>The automatic scale dimensions.</value>
        public System.Drawing.SizeF AutoScaleDimensions { get; set; }

        /// <summary>
        /// Gets or sets the automatic scaling mode for this control.
        /// </summary>
        /// <value>The automatic scale mode.</value>
        public System.Windows.Forms.AutoScaleMode AutoScaleMode { get; set; }

        /// <summary>
        /// Gets the collection of child controls contained within this <see cref="UserControl" />.
        /// </summary>
        /// <value>The controls.</value>
        public override ControlCollection Controls => _controls;

        /// <summary>
        /// Gets the underlying GTK control used by this <see cref="UserControl" />.
        /// </summary>
        /// <value>The GTK control.</value>
        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the padding within the control. Setting this value updates the internal GTK overlay margins.
        /// </summary>
        /// <value>The padding.</value>
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

        /// <summary>
        /// Resumes normal layout logic for the control.
        /// </summary>
        /// <param name="performLayout">If true, forces an immediate layout after resuming.</param>
        public override void ResumeLayout(bool performLayout)
        {

        }

        /// <summary>
        /// Suspends layout logic for the control.
        /// </summary>
        public override void SuspendLayout()
        {

        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="UserControl" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the <see cref="Control.Paint" /> event.
        /// Override to perform custom painting for the control.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
        }

        /// <summary>
        /// Called when the parent of the control changes.
        /// Override to react to parent changes.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Paint event of the Override control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        private void Override_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        /// <summary>
        /// Selfs the parent set.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void Self_ParentSet(object o, ParentSetArgs args)
        {
            OnParentChanged(EventArgs.Empty);
        }

    }
}

