/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Provides a container for Windows toolbar objects.
/// </summary>
public class ToolStrip : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly ToolStripBase self = new ToolStripBase();

    /// <summary>
    /// The tool strip item collection
    /// </summary>
    public ToolStripItemCollection toolStripItemCollection;

    /// <summary>
    /// Occurs when [checked changed.
    /// </summary>
    public event EventHandler CheckedChanged;

    /// <summary>
    /// Occurs when [check state changed.
    /// </summary>
    public event EventHandler CheckStateChanged;

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public override event EventHandler Click;

    /// <summary>
    /// Occurs when [drop down item clicked.
    /// </summary>
    public event ToolStripItemClickedEventHandler DropDownItemClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStrip" /> class.
    /// </summary>
    public ToolStrip() : base()
    {
        toolStripItemCollection = new ToolStripItemCollection(this);
        self.ActivateCurrent += ToolStripItem_Activated;
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
        self.ActivateCurrent += ToolStripItem_Activated;
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
    public Size ImageScalingSize { get; set; }

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
    public ToolStripLayoutStyle LayoutStyle { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size { get => base.Size; set => base.Size = new Size(value.Width, 30); }

    /// <summary>
    /// Tools the strip item activated.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ToolStripItem_Activated(object sender, ActivateCurrentArgs e)
    {
        DropDownItemClicked?.Invoke(this, new ToolStripItemClickedEventArgs(new ToolStripItem()));
        Click?.Invoke(sender, e);
        CheckedChanged?.Invoke(this, e);
        CheckStateChanged?.Invoke(this, e);
    }

}