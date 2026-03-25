/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace System.Windows.Forms;

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewTextBoxCell" /> cells.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewTextBoxColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewTextBoxColumn" /> class using the default text box cell template.
    /// </summary>
    public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewTextBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewTextBoxColumn(DataGridView ownerGridView) : base(ownerGridView, new DataGridViewTextBoxCell())
    {
    }
}