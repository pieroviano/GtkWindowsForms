/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Creates a panel that is associated with a
/// <see cref="T:System.Windows.Forms.SplitContainer" />
/// </summary>
[DesignerCategory("Component")]
public sealed class SplitterPanel : Panel
{

    internal SplitContainer Owner;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SplitterPanel" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
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

    /// <summary>
    /// Gets or sets the dock.
    /// </summary>
    /// <value>
    /// The dock.
    /// </value>
    public override DockStyle Dock
    {
        get { return DockStyle.Fill; }
        set { }
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public override int Height
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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public override int Width
    {
        get;
        set;
    }

}