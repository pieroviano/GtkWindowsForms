/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a shortcut menu.
/// </summary>
public class ContextMenuStrip : ToolStripDropDownMenu
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> class.
    /// </summary>
    public ContextMenuStrip()
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> class.
    /// </summary>
    /// <param name="container">The container.
    /// </param>
    public ContextMenuStrip(IContainer container)
    {

    }

    /// <summary>
    /// Sets the visible core.
    /// </summary>
    /// <param name="visible">
    /// if set to
    /// <c>true
    /// </c> [visible].
    /// </param>
    protected void SetVisibleCore(bool visible) { }

}