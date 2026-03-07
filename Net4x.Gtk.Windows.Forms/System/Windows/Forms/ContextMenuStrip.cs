/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a shortcut menu.
    /// </summary>
    public class ContextMenuStrip : ToolStripDropDownMenu
    {
        public ContextMenuStrip()
        {

        }

        public ContextMenuStrip(IContainer container)
        {

        }
        protected void SetVisibleCore(bool visible) { }
    }

}
