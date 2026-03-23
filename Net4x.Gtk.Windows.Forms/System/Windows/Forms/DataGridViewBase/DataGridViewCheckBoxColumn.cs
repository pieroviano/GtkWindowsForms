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
/// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewCheckBoxColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxColumn" /> class.
    /// </summary>
    public DataGridViewCheckBoxColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewCheckBoxColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewCheckBoxCell())
    {
        ValueType = typeof(bool);

        var renderer = new CellRendererToggleValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Activatable,
            Height = RowHeight
        };
        renderer.Toggled += CellName_Toggled;
        base.PackStart(renderer, true);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewCheckBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void CellName_Toggled(object o, ToggledArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _treeView.Model;
        model.GetIter(out var iter, path);
        var cell = model.GetValue(iter, this.Index);
        if (cell is DataGridViewCell val)
        {
            var tggle = (CellRendererToggleValue)o;
            val.Value = !tggle.Active;
            _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
        }
    }
}