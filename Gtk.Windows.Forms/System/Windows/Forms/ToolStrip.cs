/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Drawing;
using Gtk;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Provides a container for Windows toolbar objects.
/// </summary>
public partial class ToolStrip : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly ToolStripBase self = new();

    /// <summary>
    /// The tool strip item collection
    /// </summary>
    public ToolStripItemCollection toolStripItemCollection;

    private Size _ImageScalingSize;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler? CheckedChanged;

    /// <summary>
    /// Occurs when check state changed.
    /// </summary>
    public virtual event EventHandler? CheckStateChanged;

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public override event EventHandler? Click;

    /// <summary>
    /// Occurs when drop down item clicked.
    /// </summary>
    public virtual event ToolStripItemClickedEventHandler? DropDownItemClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStrip" /> class.
    /// </summary>
    public ToolStrip() : base()
    {
        toolStripItemCollection = new ToolStripItemCollection(this);
        WireActivateCurrent();
    }

    /// <summary>
    /// Bridges the backing widget's activation to the WinForms event surface. Gtk4 removed Gtk.MenuBar and
    /// its ActivateCurrent signal; the custom menu widget re-raises activation through
    /// <see cref="ToolStripBase.RaiseActivateCurrent" />, and this maps a single activation onto the click /
    /// check / drop-down events a ToolStrip consumer subscribes to.
    /// </summary>
    private void WireActivateCurrent()
    {
        self.ActivateCurrent += (_, _) =>
        {
            OnClick(System.EventArgs.Empty);
            OnCheckedChanged(System.EventArgs.Empty);
            OnCheckStateChanged(System.EventArgs.Empty);
            OnDropDownItemClicked(new ToolStripItemClickedEventArgs(null!));
        };
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStrip" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public ToolStrip(string owner) : base()
    {
        self.Hexpand = false;
        self.Vexpand = false;
        self.Valign = Gtk.Align.Start;
        self.Halign = Gtk.Align.Start;
        toolStripItemCollection = new ToolStripItemCollection(this, owner);
        WireActivateCurrent();
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the size of the image scaling.
    /// </summary>
    /// <value>
    /// The size of the image scaling.
    /// </value>
    public Size ImageScalingSize
    {
        get => _ImageScalingSize;
        set
        {
            _ImageScalingSize = value;
            self.ImageScalingSize = _ImageScalingSize;
        }
    }

    /// <summary>
    /// Gets or sets the grip style.
    /// </summary>
    /// <value>
    /// The grip style.
    /// </value>
    public ToolStripGripStyle GripStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the render mode.
    /// </summary>
    /// <value>
    /// The render mode.
    /// </value>
    public ToolStripRenderMode RenderMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the relative position of the image and the text.
    /// </summary>
    /// <value>
    /// The text image relation.
    /// </value>
    public TextImageRelation TextImageRelation
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public ToolStripItemCollection Items
    {
        get
        {
            return toolStripItemCollection;
        }
    }

    /// <summary>
    /// Gets or sets the layout style.
    /// </summary>
    /// <value>
    /// The layout style.
    /// </value>
    public ToolStripLayoutStyle LayoutStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size
    {
        get => base.Size;
        // WinForms contract: a strip with AutoSize off uses its explicit Size; with AutoSize on it is sized
        // by its content. The previous code clamped every strip's height to a hardcoded 30px, ignoring both -
        // so an AutoSize=false MenuStrip/ToolStrip (e.g. a tall icon-over-text menu) collapsed to 30px.
        set => base.Size = AutoSize ? new Size(value.Width, -1) : value;
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected virtual void OnCheckStateChanged(System.EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Click event.
    /// </summary>
    protected internal override void OnClick(System.EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownItemClicked event.
    /// </summary>
    protected virtual void OnDropDownItemClicked(System.Windows.Forms.ToolStripItemClickedEventArgs e)
    {
        DropDownItemClicked?.Invoke(this, e);
    }
}