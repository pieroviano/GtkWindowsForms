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

    private readonly DataGridView __owner;

    private readonly Gtk.TreeView GridView;

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public virtual event CollectionChangeEventHandler CollectionChanged;

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
        __owner = dataGridView;
        GridView = dataGridView.GridView;
    }

    /// <summary>
    /// Gets the owning
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnCollection.DataGridView" />.
    /// </summary>
    protected DataGridView DataGridView { get { return __owner; } }

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
        column.DataGridView = __owner;
        column.Index = Count;
        column.DisplayIndex = column.Index;
        column.SetGridViewDefaultStyle(__owner.DefaultCellStyle);

        var _cellStyle = column.DefaultCellStyle;
        if (__owner.ColumnHeadersDefaultCellStyle != null)
            _cellStyle = __owner.ColumnHeadersDefaultCellStyle;

        if (_cellStyle != null)
        {
            var header = ((Gtk.Button)column.Button);
            var style = "";
            if (_cellStyle.BackColor.Name != "0")
            {
                var backcolor = $"rgba({_cellStyle.BackColor.R},{_cellStyle.BackColor.G},{_cellStyle.BackColor.B},{_cellStyle.BackColor.A})";
                style += $".columnheaderbackcolor{{background-color:{backcolor};}} ";
                header.StyleContext.AddClass("columnheaderbackcolor");
            }
            if (_cellStyle.ForeColor.Name != "0")
            {
                var forecolor = $"rgba({_cellStyle.ForeColor.R},{_cellStyle.ForeColor.G},{_cellStyle.ForeColor.B},{_cellStyle.ForeColor.A})";
                style += $".columnheaderforecolor{{color:{forecolor};}} ";
                header.StyleContext.AddClass("columnheaderforecolor");
            }
            if (style.Length > 9)
            {
                var css = new Gtk.CssProvider();
                css.LoadFromData(style);
                header.StyleContext.AddProvider(css, 800);
            }
            switch (_cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.MiddleLeft:
                case DataGridViewContentAlignment.BottomLeft:
                    column.Alignment = 0f;
                    break;

                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    column.Alignment = 0.5f;
                    break;

                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.BottomRight:
                    column.Alignment = 1.0f;
                    break;

                default:
                    column.Alignment = 0f;
                    break;
            }
        }

        base.Add(column);
        if (__owner.self.IsRealized)
        {
            Invalidate();
        }
        GridView.AppendColumn(column);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
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
            Add(column);
        }
    }

    /// <summary>
    /// Removes all columns from the collection and clears associated GTK columns.
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        foreach (var wik in GridView.Columns)
            GridView.RemoveColumn(wik);

        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, __owner));
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

        return co == null ? __owner.RowHeadersWidth : co.Width;
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
    public DataGridViewColumn GetNextColumn(DataGridViewColumn dataGridViewColumnStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        var ix = FindIndex(m => m.Name == dataGridViewColumnStart.Name && m.State == includeFilter && m.State == excludeFilter);

        return ix < Count ? base[ix] : null;
    }

    /// <summary>
    /// Ensures the underlying GTK tree store and column sort functions are synchronized with the current column collection.
    /// </summary>
    public void Invalidate()
    {
        if (Count > __owner.Store.NColumns)
        {
            var columnTypes = new object[Count];
            __owner.Store.Clear();
            __owner.Store = new Gtk.TreeStore(Array.ConvertAll(columnTypes, o => typeof(DataGridViewCell)));
            __owner.GridView.Model = __owner.Store;
        }
        else if (__owner.GridView.Model == null)
        {
            __owner.GridView.Model = __owner.Store;
        }
        __owner.Store.DefaultSortFunc = new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) => { return 0; });
        for (var i = 0; i < __owner.Store.NColumns; i++)
        {
            __owner.Store.SetSortFunc(i, new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) =>
            {
                ((Gtk.TreeStore)m).GetSortColumnId(out var sortid, out var order);
                var v1 = m.GetValue(t1, sortid) as DataGridViewCell;
                var v2 = m.GetValue(t2, sortid) as DataGridViewCell;
                if (v1?.Value == null || v2?.Value == null)
                    return 0;
                else if (int.TryParse(v1.Value.ToString(), out var rv1) && int.TryParse(v2.Value.ToString(), out var rv2))
                    return (rv2 - rv1);
                else if (DateTime.TryParse(v1.Value.ToString(), out var rd1) && DateTime.TryParse(v2.Value.ToString(), out var rd2))
                    return (int)((rd2 - rd1).TotalSeconds);
                else
                    return v2.Value.ToString().CompareTo(v1.Value.ToString());
            }));
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
        __owner.GridView.RemoveColumn(column);
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
        CollectionChanged?.Invoke(__owner, e);
    }
}