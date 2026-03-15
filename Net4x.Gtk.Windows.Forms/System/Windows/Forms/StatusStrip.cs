/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows status bar control.
/// </summary>
public class StatusStrip : ToolStrip
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.StatusStrip" /> class.
    /// </summary>
    public StatusStrip() : base()
    {
        this.self.StyleContext.RemoveClass("ToolStrip");
        this.self.StyleContext.AddClass("StatusStrip");
        Dock = DockStyle.Bottom;
    }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>
    /// The location.
    /// </value>
    public override Point Location { get => base.Location; set => base.Location = new Point(value.X, value.Y - 12); }

    /// <summary>
    /// Gets or sets a value indicating whether show item tool tips.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show item tool tips; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool ShowItemToolTips { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size { get => base.Size; set => base.Size = new Size(value.Width, value.Height + 12); }

    /// <summary>
    /// Gets or sets a value indicating whether sizing grip.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if sizing grip; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool SizingGrip { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.StatusStrip" /> is stretch.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if stretch; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool Stretch { get; set; }

}