/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.GtkRender;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewButtonCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewButtonColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewButtonColumn" /> class.
    /// </summary>
    public DataGridViewButtonColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewButtonColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewButtonColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewButtonCell())
    {
        var renderer = new CellRendererButtonValue(this)
        {
            Editable = false,
            Mode = CellRendererMode.Activatable,
            Height = RowHeight
        };
        renderer.SetAlignment(0.5f, 0.5f);
        base.PackStart(renderer, false);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewButtonCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void TreeView_RowActivated(object o, RowActivatedArgs args)
    {
        if (args.Column.Handle == this.Handle)
        {
            if (args.Column.Cells[0] is CellRendererText cell)
            {
                TreePath path = args.Path;
                _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
            }
        }
    }
}