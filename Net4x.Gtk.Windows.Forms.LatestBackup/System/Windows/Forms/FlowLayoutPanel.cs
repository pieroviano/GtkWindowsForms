/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a panel that dynamically lays out its contents horizontally or vertically.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [ProvideProperty("FlowBreak", typeof(Control))]
    [DefaultProperty("FlowDirection")]
    [DesignerCategory("Component")]
    public partial class FlowLayoutPanel : Control, IExtenderProvider
    {

        public readonly FlowLayoutPanelBase self = new FlowLayoutPanelBase();

        private ObjectCollection _controls;

        private FlowDirection _FlowDirection;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.FlowLayoutPanel" /> class.
        /// </summary>
        public FlowLayoutPanel() : base()
        {
            self.Orientation = Gtk.Orientation.Horizontal;
            self.Halign = Align.Start;
            self.Valign = Align.Start;
            self.MinChildrenPerLine = 1;
            self.MaxChildrenPerLine = 999;
            self.ColumnSpacing = 0;
            self.BorderWidth = 0;
            _controls = new ObjectCollection(this);
        }

        public override ControlCollection Controls => _controls;

        /// <summary>
        /// Gets or sets a value indicating the flow direction of the
        /// <see cref="System.Windows.Forms.FlowLayoutPanel" /> control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.FlowDirection" /> values indicating the direction of consecutive placement of controls in the panel. The default is
        /// <see cref="System.Windows.Forms.FlowDirection.LeftToRight" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public FlowDirection FlowDirection
        {
            get { return _FlowDirection; }
            set
            {
                if (value == FlowDirection.LeftToRight || value == FlowDirection.RightToLeft) { self.Orientation = Gtk.Orientation.Horizontal; }
                else if (value == FlowDirection.TopDown || value == FlowDirection.BottomUp) { self.Orientation = Gtk.Orientation.Vertical; }
            }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.FlowLayoutPanel" /> control should wrap its contents or let the contents be clipped.
        /// </summary>
        /// <returns>true if the contents should be wrapped; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool WrapContents { get; set; }

        public bool CanExtend(object extendee)
        {
            return true;
        }

        /// <summary>
        /// Returns a value that represents the flow-break setting of the
        /// <see cref="System.Windows.Forms.FlowLayoutPanel" /> control.
        /// </summary>
        /// <returns>true if the flow break is set; otherwise, false.
        /// </returns>
        /// <param name="control">The child control.
        /// </param>
        public bool GetFlowBreak(Control control)
        {
            return false;
        }

        /// <summary>
        /// Sets the value that represents the flow-break setting of the
        /// <see cref="System.Windows.Forms.FlowLayoutPanel" /> control.
        /// </summary>
        /// <param name="control">The child control.
        /// </param>
        /// <param name="value">The flow-break value to set.
        /// </param>
        public void SetFlowBreak(Control control, bool value)
        {

        }

        public class ObjectCollection : ControlCollection
        {

            private FlowLayoutPanel _owner;

            public ObjectCollection(FlowLayoutPanel owner) : base(owner)
            {
                _owner = owner;
            }

            public override void Add(Control control)
            {
                Gtk.FlowBoxChild box = new FlowBoxChild();
                box.Valign = Align.Start;
                box.Halign = Align.Start;
                box.Expand = false;
                control.Location = new Drawing.Point(0, 0);
                control.LockLocation = true;
                control.Parent = _owner;
                Gtk.Widget widg = control.Widget;
                widg.Valign = Align.Start;
                widg.Halign = Align.Start;
                widg.Expand = false;
                box.Add(widg);
                _owner.self.Add(box);
                base.AddWidget(box, control);
            }

        }
    }
}
