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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> class.
        /// </summary>
        public ContextMenuStrip()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> class and associates it with the specified container.
        /// </summary>
        /// <param name="container">A component that implements
        /// <see cref="System.ComponentModel.IContainer" /> that is the container of the
        /// <see cref="System.Windows.Forms.ContextMenuStrip" />
        /// </param>
        public ContextMenuStrip(IContainer container)
        {

        }

        /// <param name="visible">
        /// true to make the control visible; otherwise, false.
        /// </param>
        protected void SetVisibleCore(bool visible) { }

    }

}
