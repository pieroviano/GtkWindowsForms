/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.Windows.Forms.GtkRender;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewImageCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewImageColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewImageColumn" /> class.
    /// </summary>
    public DataGridViewImageColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewImageColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewImageColumn(DataGridView? ownerGridView) : base(0, ownerGridView, new DataGridViewImageCell())
    {
        ValueType = typeof(Drawing.Image);
        var renderer = new CellRendererPixbufValue(this)
        {
            Height = RowHeight,
            Editable = false
        };
        base.PackStart(renderer, false);
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell? NewCell(object? value = null, Type? valueType = null)
    {
        var newcell = new DataGridViewImageCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }
}