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
/// Provides a lightweight container control that can host other controls.
/// This class is a GTK-backed implementation intended to mirror the WinForms
/// <see cref="T:System.Windows.Forms.UserControl" /> API.
/// </summary>
[DesignerCategory("UserControl")]
[ToolboxItem(true)]
public partial class UserControl : ContainerControl
{
    /// <summary>
    /// Underlying GTK-based control that implements the native behavior for this
    /// <see cref="T:System.Windows.Forms.UserControl" />.
    /// </summary>
    public readonly UserControlBase self = new();

    private readonly ControlCollection _controls;

    private readonly Gtk.Overlay contaner;

    private bool Is_Control_Shown;

    /// <summary>
    /// Occurs when the control is loaded.
    /// </summary>
    public override event EventHandler? Load;

    /// <summary>
    /// Occurs when paint.
    /// </summary>
    public override event PaintEventHandler? Paint;

    /// <summary>
    /// Occurs when parent changed.
    /// </summary>
    public override event EventHandler? ParentChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.UserControl" /> class.
    /// </summary>
    public UserControl() : base()
    {
        self.Override.sender = this;
        contaner = new Gtk.Overlay
        {
            MarginStart = 0,
            MarginTop = 0,
            BorderWidth = 0,
            Halign = Align.Fill,
            Valign = Align.Fill,
            Hexpand = false,
            Vexpand = false
        };
        contaner.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
        _controls = new ControlCollection(this, contaner);
        self.Add(contaner);
        self.Override.Paint += Override_Paint;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
        self.Shown += Self_Shown;
    }


    /// <summary>
    /// Gets the collection of child controls contained within this
    /// <see cref="T:System.Windows.Forms.UserControl" />.
    /// </summary>
    /// <value>
    /// The controls.
    /// </value>
    public override ControlCollection Controls => _controls;

    /// <summary>
    /// Gets the underlying GTK control used by this
    /// <see cref="T:System.Windows.Forms.UserControl" />.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the padding within the control. Setting this value updates the internal GTK overlay margins.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
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
    /// <param name="performLayout">
    /// If true, forces an immediate layout after resuming.
    /// </param>
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
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.Windows.Forms.UserControl" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    /// <summary>
    /// Raises the Load event.
    /// </summary>
    protected internal new virtual void OnLoad(System.EventArgs e)
    {
        Load?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Paint" /> event.
    /// Override to perform custom painting for the control.
    /// </summary>
    /// <param name="e">
    /// A
    /// <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.
    /// </param>
    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
    }

    /// <summary>
    /// Called when the parent of the control changes.
    /// Override to react to parent changes.
    /// </summary>
    /// <param name="e">
    /// An
    /// <see cref="T:System.EventArgs" /> that contains the event data.
    /// </param>
    protected override void OnParentChanged(EventArgs e)
    {
        ParentChanged?.Invoke(this, e);
    }

    private void Override_Paint(object sender, PaintEventArgs e)
    {
        OnPaint(e);
    }

    private void Self_Shown(object sender, EventArgs e)
    {
        if (Is_Control_Shown == false)
        {
            Is_Control_Shown = true;
            OnLoad(EventArgs.Empty);
        }
    }
}