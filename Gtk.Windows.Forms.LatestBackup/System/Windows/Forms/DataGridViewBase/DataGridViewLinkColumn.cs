/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace System.Windows.Forms;

/// <summary>
/// Represents a column of cells that contain links in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewLinkColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewLinkColumn" /> class.
    /// </summary>
    public DataGridViewLinkColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewLinkColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewLinkColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewLinkCell())
    {

    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewLinkCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }
}