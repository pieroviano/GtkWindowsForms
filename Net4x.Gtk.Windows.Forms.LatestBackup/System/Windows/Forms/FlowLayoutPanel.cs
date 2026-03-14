/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a panel that dynamically lays out its contents horizontally or vertically.
/// </summary>
[ProvideProperty("FlowBreak", typeof(Control))]
[DefaultProperty("FlowDirection")]
[DesignerCategory("Component")]
public partial class FlowLayoutPanel : Control, IExtenderProvider
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly FlowLayoutPanelBase self = new FlowLayoutPanelBase();

    /// <summary>
    /// The controls
    /// </summary>
    private readonly ObjectCollection _controls;

    /// <summary>
    /// The flow direction
    /// </summary>
    private readonly FlowDirection _FlowDirection;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.FlowLayoutPanel" /> class.
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

    /// <summary>
    /// Gets the controls.
    /// </summary>
    /// <value>
    /// The controls.
    /// </value>
    public override ControlCollection Controls => _controls;

    /// <summary>
    /// Gets or sets the flow direction.
    /// </summary>
    /// <value>
    /// The flow direction.
    /// </value>
    public FlowDirection FlowDirection
    {
        get { return _FlowDirection; }
        set
        {
            if (value == FlowDirection.LeftToRight || value == FlowDirection.RightToLeft) { self.Orientation = Gtk.Orientation.Horizontal; }
            else if (value == FlowDirection.TopDown || value == FlowDirection.BottomUp) { self.Orientation = Gtk.Orientation.Vertical; }
        }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets a value indicating whether wrap contents.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if wrap contents; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool WrapContents { get; set; }

    /// <summary>
    /// Specifies whether this object can provide its extender properties to the specified object.
    /// </summary>
    /// <param name="extendee">
    /// The
    /// <see cref="T:System.Object">
    /// </see> to receive the extender properties.
    /// </param>
    /// <returns>
    /// true if this object can provide extender properties to the specified object; otherwise, false.
    /// </returns>
    public bool CanExtend(object extendee)
    {
        return true;
    }

    /// <summary>
    /// Gets the flow break.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    public bool GetFlowBreak(Control control)
    {
        return false;
    }

    /// <summary>
    /// Sets the flow break.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="value">
    /// if set to
    /// <c>
    /// true
    /// </c>value.
    /// </param>
    public void SetFlowBreak(Control control, bool value)
    {

    }

    /// <summary>
    /// Class ObjectCollection. Implements the
    /// <see cref="T:System.Windows.Forms.Control.ControlCollection" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Forms.Control.ControlCollection" />
    public class ObjectCollection : ControlCollection
    {
        /// <summary>
        /// The owner
        /// </summary>
        private readonly FlowLayoutPanel _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.FlowLayoutPanel.ObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ObjectCollection(FlowLayoutPanel owner) : base(owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds a
        /// <see cref="T:System.Windows.Forms.Control" /> to the collection and to the underlying GTK container.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public override void Add(Control control)
        {
            Gtk.FlowBoxChild box = new FlowBoxChild
            {
                Valign = Align.Start,
                Halign = Align.Start,
                Expand = false
            };
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