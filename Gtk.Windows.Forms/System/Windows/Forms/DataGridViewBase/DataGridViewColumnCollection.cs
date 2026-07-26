/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> objects that belong to a
/// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection.DataGridView" />.
/// Provides helpers for adding, removing and querying the set of columns and synchronizes with the underlying GTK tree view.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewColumnCollection : List<DataGridViewColumn>
{

    private readonly DataGridView _owner;

    private readonly Gtk.TreeView GridView;

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public virtual event CollectionChangeEventHandler? CollectionChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection" /> class for the specified grid.
    /// </summary>
    /// <param name="dataGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection.DataGridView" />.
    /// </param>
    public DataGridViewColumnCollection(DataGridView dataGridView)
    {
        _owner = dataGridView;
        GridView = dataGridView.GridView;
    }

    /// <summary>
    /// Gets the owning
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection.DataGridView" />.
    /// </summary>
    protected DataGridView DataGridView { get { return _owner; } }

    /// <summary>
    /// Gets the column with the specified name.
    /// </summary>
    /// <param name="columnName">
    /// The name of the column to locate.
    /// </param>
    /// <returns>
    /// The
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> with the specified name, or null if not found.
    /// </returns>
    public virtual DataGridViewColumn this[string columnName] { get { return base.Find(m => m.Name == columnName); } }

    /// <summary>
    /// Adds a new column with the specified name and header text.
    /// </summary>
    /// <param name="columnName">
    /// Column name.
    /// </param>
    /// <param name="headerText">
    /// Header text to display.
    /// </param>
    public void Add(string columnName, string headerText)
    {
        var column = new DataGridViewColumn() { Name = columnName, HeaderText = headerText };
        Add(column);
    }

    /// <summary>
    /// Adds the specified column to the collection and synchronizes it with the underlying GTK tree view.
    /// </summary>
    /// <param name="column">
    /// The
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> to add.
    /// </param>
    public new void Add(DataGridViewColumn column)
    {
        AddInternal(column);
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection" />.
    /// </summary>
    /// <param name="columns">
    /// The collection whose elements should be added.
    /// </param>
    public new void AddRange(IEnumerable<DataGridViewColumn> columns)
    {
        foreach (var column in columns)
        {
            AddInternal(column);
        }
        _owner.Columns.Invalidate();
    }

    /// <summary>
    /// Removes all columns from the collection and clears associated GTK columns.
    /// </summary>
    public new void Clear()
    {
        foreach (var wik in GridView.Columns)
            GridView.RemoveColumn(wik);
        base.Clear();
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, _owner));
    }

    /// <summary>
    /// Returns the number of columns that match the specified element state filter.
    /// </summary>
    /// <param name="includeFilter">
    /// The
    /// <see cref="T:System.Windows.Forms.DataGridViewElementStates" /> to include.
    /// </param>
    /// <returns>
    /// The count of matching columns.
    /// </returns>
    public int GetColumnCount(DataGridViewElementStates includeFilter)
    {
        return FindAll(m => m.State == includeFilter).Count;
    }

    /// <summary>
    /// Returns the width of the first column that matches the specified element state filter.
    /// If none found, returns the owner's row header width.
    /// </summary>
    /// <param name="includeFilter">
    /// The element state to include.
    /// </param>
    /// <returns>
    /// The width in pixels.
    /// </returns>
    public int GetColumnsWidth(DataGridViewElementStates includeFilter)
    {
        var co = Find(m => m.State == includeFilter);

        return co == null ? _owner.RowHeadersWidth : co.Width;
    }

    /// <summary>
    /// Gets the first column that matches the specified element state filter.
    /// </summary>
    /// <param name="includeFilter">
    /// The element state to include.
    /// </param>
    /// <returns>
    /// The first matching
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" />, or null.
    /// </returns>
    public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter)
    {
        return Find(m => m.State == includeFilter);
    }

    /// <summary>
    /// Gets the first column that matches both include and exclude filters.
    /// </summary>
    /// <param name="includeFilter">
    /// The element state to include.
    /// </param>
    /// <param name="excludeFilter">
    /// The element state to exclude.
    /// </param>
    /// <returns>
    /// The first matching
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" />, or null.
    /// </returns>
    public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        return Find(m => m.State == includeFilter && m.State == excludeFilter);
    }

    /// <summary>
    /// Gets the last column that matches both include and exclude filters.
    /// </summary>
    /// <param name="includeFilter">
    /// The element state to include.
    /// </param>
    /// <param name="excludeFilter">
    /// The element state to exclude.
    /// </param>
    /// <returns>
    /// The last matching
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" />, or null.
    /// </returns>
    public DataGridViewColumn GetLastColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        return FindLast(m => m.State == includeFilter && m.State == excludeFilter);
    }

    /// <summary>
    /// Returns the next column after the provided start column that matches the filters.
    /// </summary>
    /// <param name="dataGridViewColumnStart">
    /// Start column for search.
    /// </param>
    /// <param name="includeFilter">
    /// Element state to include.
    /// </param>
    /// <param name="excludeFilter">
    /// Element state to exclude.
    /// </param>
    /// <returns>
    /// The next matching
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> or null.
    /// </returns>
    public DataGridViewColumn? GetNextColumn(DataGridViewColumn dataGridViewColumnStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        var ix = FindIndex(m => m.Name == dataGridViewColumnStart.Name && m.State == includeFilter && m.State == excludeFilter);

        return ix < Count ? base[ix] : null;
    }

    /// <summary>
    /// Ensures the underlying GTK tree store and column sort functions are synchronized with the current column collection.
    /// </summary>
    public void Invalidate()
    {
        if (_owner.UseModelFilter)
        {
            if (_owner.GridView.Model == null)
            {
                var modelFilter = new Gtk.TreeModelFilter(_owner.Store, null)
                {
                    VisibleFunc = new Gtk.TreeModelFilterVisibleFunc((model, iter) =>
                    {
                        if (model.GetValue(iter, 1) is DataGridViewRow row)
                            return row.Visible;
                        else
                            return true;
                    })
                };
                _owner.GridView.Model = modelFilter;
            }
            foreach (var column in this)
            {
                column.SortColumnId = -1;
            }
        }
        else
        {
            if (_owner.GridView.Model == null)
            {
                _owner.GridView.Model = _owner.Store;
                _owner.Store.DefaultSortFunc = new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) => { return 0; });
            }
            foreach (var column in this)
            {
                if (column.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    var colIndex = column.Index;
                    _owner.Store.SetSortFunc(colIndex, new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) =>
                    {
                        var v1 = ((DataGridViewRow)m.GetValue(t1, 0)).Cells[colIndex] as DataGridViewCell;
                        var v2 = ((DataGridViewRow)m.GetValue(t2, 0)).Cells[colIndex] as DataGridViewCell;
                        if (v1 == null || v2 == null || v1.Value == null || v2.Value == null)
                            return 0;
                        else if (v1.ValueType?.IsValueType == true && long.TryParse(v1.Value.ToString(), out var rv1) && long.TryParse(v2.Value.ToString(), out var rv2))
                            return rv1.CompareTo(rv2);
                        else if (v1.ValueType?.Name == "DateTime")
                            return ((DateTime)v1.Value).CompareTo((DateTime)v2.Value);
                        else
                            return v1.Value.ToString().CompareTo(v2.Value.ToString());
                    }));
                }
            }
        }

        foreach (var col in this)
        {
            col.UpdateDefaultStyle();
        }
    }

    /// <summary>
    /// Removes the specified column from the collection and underlying GTK view.
    /// </summary>
    /// <param name="column">
    /// The column to remove.
    /// </param>
    /// <returns>
    /// True if the column was removed; otherwise false.
    /// </returns>
    public new bool Remove(DataGridViewColumn column)
    {
        _owner.GridView.RemoveColumn(column);
        var ir= base.Remove(column);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));

        return ir;
    }

    /// <summary>
    /// Removes the column at the specified index.
    /// </summary>
    /// <param name="index">
    /// Index of the column to remove.
    /// </param>
    public new void RemoveAt(int index)
    {
        this.Remove(this[index]);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection.CollectionChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
    {
        CollectionChanged?.Invoke(_owner, e);
    }

    private void AddInternal(DataGridViewColumn column)
    {
        column.DataGridView = _owner;
        column.Index = Count;
        base.Add(column);
        GridView.AppendColumn(column);
        if (_owner.GridView.IsRealized)
        {
            _owner.Columns.Invalidate();
        }
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
    }
}