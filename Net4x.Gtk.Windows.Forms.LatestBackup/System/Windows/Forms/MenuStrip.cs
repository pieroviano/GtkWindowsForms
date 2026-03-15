/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides a menu system for a form.
/// </summary>
[DesignerCategory("Component")]
public partial class MenuStrip : ToolStrip
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MenuStrip" /> class.
    /// </summary>
    public MenuStrip() : base("MenuStrip")
    {
        this.self.StyleContext.AddClass("MenuStrip");
        this.Dock = DockStyle.Top;
    }

}