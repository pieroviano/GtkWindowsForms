/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Creates a panel that is associated with a
/// <see cref="T:System.Windows.Forms.SplitContainer" />
/// </summary>
[DesignerCategory("Component")]
public sealed class SplitterPanel : Panel
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SplitterPanel" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    public SplitterPanel(SplitContainer owner) : base()
    {
        self.Override.sender = this;
        self.StyleContext.AddClass("SplitterPanel");
        self.SetMargin(0);
        self.Halign = Gtk.Align.Fill;
        self.Valign = Gtk.Align.Fill;
        self.Hexpand = false;
        self.Vexpand = false;
    }

    /// <summary>
    /// Gets or sets the anchor.
    /// </summary>
    /// <value>
    /// The anchor.
    /// </value>
    public override AnchorStyles Anchor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic size.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic size; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool AutoSize
    {
        get => base.AutoSize;
        set { }
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