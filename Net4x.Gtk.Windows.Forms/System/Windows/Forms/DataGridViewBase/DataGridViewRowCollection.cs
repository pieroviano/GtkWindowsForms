/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// A collection of
/// <see cref="T:System.Windows.Forms.DataGridViewRow" /> objects.
/// </summary>
[ListBindable(false)]
public class DataGridViewRowCollection : ICollection, IEnumerable, IList
{

    private readonly DataGridView dataGridView;

    private readonly ArrayList items = new();

    private readonly DataGridViewRow parentRow;

    /// <summary>
    /// CollectionChanged event
    /// </summary>
    public virtual event CollectionChangeEventHandler CollectionChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowCollection" /> class.
    /// </summary>
    /// <param name="dataGridView">
    /// The data grid view.
    /// </param>
    public DataGridViewRowCollection(DataGridView dataGridView)
    {
        this.dataGridView = dataGridView;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowCollection" /> class.
    /// </summary>
    /// <param name="dataGridView">
    /// The data grid view.
    /// </param>
    /// <param name="parentRow">
    /// The parent row.
    /// </param>
    public DataGridViewRowCollection(DataGridView dataGridView, DataGridViewRow parentRow)
    {
        this.dataGridView = dataGridView;
        this.parentRow = parentRow;
    }

    /// <summary>
    /// Gets the number of elements contained in the
    /// <see cref="T:System.Collections.ICollection">
    /// </see>.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count => items.Count;

    internal ArrayList SharedList => items;

    bool IList.IsFixedSize => false;

    bool IList.IsReadOnly => false;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    /// <summary>
    /// Gets the data grid view.
    /// </summary>
    /// <value>
    /// The data grid view.
    /// </value>
    protected DataGridView DataGridView { get { return dataGridView; } }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <value>
    /// The list.
    /// </value>
    protected ArrayList List { get { return items; } }

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// DataGridViewRow.
    /// </returns>
    public DataGridViewRow this[int index]
    {
        get
        {
            var dataGridViewRow = SharedRow(index);

            return dataGridViewRow;
        }
    }

    object IList.this[int index] { get => items[index]; set => throw new NotSupportedException(); }

    /// <summary>
    /// Adds this instance.
    /// </summary>
    /// <returns>
    /// System.Int32.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual int Add()
    {
        var row = new DataGridViewRow();
        AddGtkStore(row);

        return row.Index;
    }

    /// <summary>
    /// Adds the specified data grid view row.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public virtual int Add(DataGridViewRow dataGridViewRow)
    {
        var rowindex = items.Count;
        if (parentRow == null)
            AddGtkStore(dataGridViewRow);
        else
            AddGtkStore(parentRow.TreeIter, ref rowindex, dataGridViewRow);

        return dataGridViewRow.Index;
    }

    /// <summary>
    /// Adds the specified values.
    /// </summary>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual int Add(params object[] values)
    {
        var newRow = new DataGridViewRow();
        var cols = dataGridView.Columns;
        var valueslength = values.Length;
        for (var i = 0; i < cols.Count; i++)
        {
            if (i >= valueslength)
                newRow.Cells.Add(dataGridView.Columns[i].NewCell());
            else if (values[i] is DataGridViewCell cellval)
                newRow.Cells.Add(cellval);
            else
                newRow.Cells.Add(dataGridView.Columns[i].NewCell(values[i]));
        }

        return Add(newRow);
    }

    /// <summary>
    /// Adds the specified count.
    /// </summary>
    /// <param name="count">
    /// The count.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual int Add(int count)
    {
        var start = items.Count;
        for (var i = 0; i < count; i++)
        {
            var row = new DataGridViewRow() { Index = start + i };
            AddGtkStore(row);
            items.Add(row);
        }

        return start + count;
    }

    /// <summary>
    /// Adds the copies.
    /// </summary>
    /// <param name="indexSource">
    /// The index source.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public virtual int AddCopies(int indexSource, int count)
    {
        var arr = new DataGridViewRow[count];
        items.CopyTo(0, arr, indexSource, count);
        this.AddRange(arr);

        return count;
    }

    /// <summary>
    /// Adds the copy.
    /// </summary>
    /// <param name="indexSource">
    /// The index source.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public virtual int AddCopy(int indexSource)
    {
        return AddCopies(indexSource, 1);
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="dataGridViewRows">
    /// The data grid view rows.
    /// </param>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
    {
        if (this.dataGridView.Store.NColumns < this.dataGridView.GridView.Columns.Length)
            this.dataGridView.Columns.Invalidate();
        if (parentRow == null)
            AddGtkStore(dataGridViewRows);
        else
        {
            var rowindex = items.Count;
            foreach (var row in dataGridViewRows)
            {
                AddGtkStore(parentRow.TreeIter, ref rowindex, row);
            }
        }
    }

    /// <summary>
    /// Removes all items from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    public virtual void Clear()
    {
        dataGridView.Store.Clear();
        items.Clear();
    }

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if contains [the specified data grid view row]; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public virtual bool Contains(DataGridViewRow dataGridViewRow)
    {
        return items.IndexOf(dataGridViewRow) != -1;
    }

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public void CopyTo(DataGridViewRow[] array, int index)
    {
        items.CopyTo(array, index);
    }

    /// <summary>
    /// Gets the first row.
    /// </summary>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetFirstRow(DataGridViewElementStates includeFilter)
    {
        var idx = -1;
        for (var i = 0; i < items.Count; i++)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the first row.
    /// </summary>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <param name="excludeFilter">
    /// The exclude filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetFirstRow(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        var idx = -1;
        for (var i = 0; i < items.Count; i++)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the last row.
    /// </summary>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetLastRow(DataGridViewElementStates includeFilter)
    {
        var idx = -1;
        for (var i = 0; i < items.Count; i++)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter)
            {
                idx = i;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the next row.
    /// </summary>
    /// <param name="indexStart">
    /// The index start.
    /// </param>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
    {
        var idx = -1;
        for (var i = indexStart - 1; i < items.Count; i++)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the next row.
    /// </summary>
    /// <param name="indexStart">
    /// The index start.
    /// </param>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <param name="excludeFilter">
    /// The exclude filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        var idx = -1;
        for (var i = indexStart - 1; i < items.Count; i++)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the previous row.
    /// </summary>
    /// <param name="indexStart">
    /// The index start.
    /// </param>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <param name="excludeFilter">
    /// The exclude filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
    {
        var idx = -1;
        for (var i = indexStart - 1; i > -1; i--)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the previous row.
    /// </summary>
    /// <param name="indexStart">
    /// The index start.
    /// </param>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
    {
        var idx = -1;
        for (var i = indexStart - 1; i > -1; i--)
        {
            if (((DataGridViewRow)items[i]).State == includeFilter)
            {
                idx = i;
                break;
            }
        }

        return idx;
    }

    /// <summary>
    /// Gets the row count.
    /// </summary>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetRowCount(DataGridViewElementStates includeFilter)
    {

        if (includeFilter == DataGridViewElementStates.None)
            return items.Count;
        else
        {
            var num = 0;
            for (var i = 0; i < items.Count; i++)
            {
                if ((GetRowState(i) & includeFilter) == includeFilter)
                {
                    num++;
                }
            }
            return num;
        }
    }

    /// <summary>
    /// Gets the height of the rows.
    /// </summary>
    /// <param name="includeFilter">
    /// The include filter.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int GetRowsHeight(DataGridViewElementStates includeFilter)
    {
        var num = 0;
        for (var i = 0; i < items.Count; i++)
        {
            if ((GetRowState(i) & includeFilter) == includeFilter)
            {
                num += ((DataGridViewRow)items[i]).GetHeight(i);
            }
        }

        return num;
    }

    /// <summary>
    /// Gets the state of the row.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <returns>
    /// DataGridViewElementStates.
    /// </returns>
    public virtual DataGridViewElementStates GetRowState(int rowIndex)
    {
        return ((DataGridViewRow)items[rowIndex]).State;
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int IndexOf(DataGridViewRow dataGridViewRow)
    {
        return items.IndexOf(dataGridViewRow);
    }

    /// <summary>
    /// Inserts the specified row index.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    public virtual void Insert(int rowIndex, params object[] values)
    {
        var newRow = new DataGridViewRow();
        var cols = dataGridView.Columns;
        var valueslength = values.Length;
        for (var i = 0; i < cols.Count; i++)
        {
            if (i >= valueslength)
                newRow.Cells.Add(dataGridView.Columns[i].NewCell());
            else if (values[i] is DataGridViewCell cellval)
                newRow.Cells.Add(cellval);
            else
                newRow.Cells.Add(dataGridView.Columns[i].NewCell(values[i]));
        }

        InsertGtkStore(rowIndex, newRow);
    }

    /// <summary>
    /// Inserts the specified row index.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
    {
        InsertGtkStore(rowIndex, dataGridViewRow);
    }

    /// <summary>
    /// Inserts the specified row index.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    public virtual void Insert(int rowIndex, int count)
    {
        var rows = new DataGridViewRow[count];
        for (var r = 0; r < count; r++)
        {
            foreach (var col in dataGridView.Columns)
            {
                var cell = col.CellTemplate;
                rows[r].Cells.Add(cell);
            }
        }
        InsertRange(rowIndex, rows);
    }

    /// <summary>
    /// Inserts the copies.
    /// </summary>
    /// <param name="indexSource">
    /// The index source.
    /// </param>
    /// <param name="indexDestination">
    /// The index destination.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    public virtual void InsertCopies(int indexSource, int indexDestination, int count) { }

    /// <summary>
    /// Inserts the copy.
    /// </summary>
    /// <param name="indexSource">
    /// The index source.
    /// </param>
    /// <param name="indexDestination">
    /// The index destination.
    /// </param>
    public virtual void InsertCopy(int indexSource, int indexDestination) { }

    /// <summary>
    /// Inserts the range.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="dataGridViewRows">
    /// The data grid view rows.
    /// </param>
    public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
    {
        InsertGtkStore(rowIndex, dataGridViewRows);
    }

    /// <summary>
    /// Removes the specified data grid view row.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    public virtual void Remove(DataGridViewRow dataGridViewRow)
    {
        RemoveAt(dataGridViewRow.Index);
    }

    /// <summary>
    /// Removes the
    /// <see cref="T:System.Collections.IList">
    /// </see> item at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the item to remove.
    /// </param>
    public virtual void RemoveAt(int index)
    {
        if (dataGridView.Store.GetIter(out var iter, new TreePath(new int[] { index })))
        {
            dataGridView.Store.Remove(ref iter);
        }
        items.RemoveAt(index);
        //reset id
        for (var i = 0; i < items.Count; i++)
        {
            ((DataGridViewRow)items[i]).Index = i;
        }
    }

    /// <summary>
    /// Shareds the row.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <returns>
    /// DataGridViewRow.
    /// </returns>
    public DataGridViewRow SharedRow(int rowIndex)
    {
        var hasiter = dataGridView.Store.GetIter(out var iter,new TreePath(new int[] { rowIndex }));
        if (hasiter)
        {
            foreach (DataGridViewRow item in items)
            {
                if (item.TreeIter.Equals(iter.UserData))
                    return item;
            }
        }

        return (DataGridViewRow)SharedList[rowIndex];
    }

    int IList.Add(object value)
    {
        return Add((DataGridViewRow)value);
    }

    void IList.Clear()
    {
        Clear();
    }

    bool IList.Contains(object value)
    {
        return items.Contains(value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        items.CopyTo(array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }

    int IList.IndexOf(object value)
    {
        return items.IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
        Insert(index, (DataGridViewRow)value);
    }

    void IList.Remove(object value)
    {
        Remove((DataGridViewRow)value);
    }

    void IList.RemoveAt(int index)
    {
        RemoveAt(index);
    }

    /// <summary>
    /// Handles the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowCollection.CollectionChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.ComponentModel.CollectionChangeEventArgs" /> instance containing the event data.
    /// </param>
    //**************************************
    protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
    {
        CollectionChanged?.Invoke(dataGridView, e);
    }

    private TreeIter AddGtkStore(DataGridViewCellCollection cells)
    {
        var ncolumns = dataGridView.Store.NColumns;
        var columnscount = dataGridView.Columns.Count;
        for (var i = cells.Count; i < columnscount; i++)
            cells.Add(dataGridView.Columns[i].NewCell());
        for (var i = cells.Count; i < ncolumns; i++)
            cells.Add(new DataGridViewTextBoxCell());
        for (var i = 0; i < ncolumns; i++)
            cells[i].ColumnIndex = i;

        if (ncolumns == cells.Count)
            return dataGridView.Store.AppendValues(cells.ConvertAll(o => o).ToArray());
        else
            return dataGridView.Store.AppendValues(cells.ConvertAll(o => o).Take(ncolumns).ToArray());
    }

    private TreeIter AddGtkStore(TreeIter parent, DataGridViewCellCollection cells)
    {
        var ncolumns = dataGridView.Store.NColumns;
        var columnscount = dataGridView.Columns.Count;
        for (var i = cells.Count; i < columnscount; i++)
            cells.Add(dataGridView.Columns[i].NewCell());
        for (var i = cells.Count; i < ncolumns; i++)
            cells.Add(new DataGridViewTextBoxCell());
        for (var i = 0; i < ncolumns; i++)
            cells[i].ColumnIndex = i;
        if (ncolumns == cells.Count)
            return dataGridView.Store.AppendValues(parent, cells.ConvertAll(o => o).ToArray());
        else
            return dataGridView.Store.AppendValues(parent, cells.ConvertAll(o => o).Take(ncolumns).ToArray());
    }

    private void AddGtkStore(params DataGridViewRow[] dataGridViewRows)
    {
        var rowindex = items.Count;
        foreach (var row in dataGridViewRows)
        {
            row.DataGridView = dataGridView;
            row.Index = rowindex;
            var _cellStyle = dataGridView.DefaultCellStyle;
            if (dataGridView.RowsDefaultCellStyle != null)
                _cellStyle = dataGridView.RowsDefaultCellStyle;
            if (rowindex % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
                _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
            if (row.DefaultCellStyle != null)
                _cellStyle = row.DefaultCellStyle;

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.RowIndex = rowindex;
                cell.RowStyle = _cellStyle;
            }
            items.Add(row);
            rowindex++;
            if (row.TreeIter.Equals(TreeIter.Zero))
            {
                row.TreeIter = AddGtkStore(row.Cells);
                foreach (DataGridViewRow child in row.Children)
                {
                    items.Add(child);
                    AddGtkStore(row.TreeIter, ref rowindex, child);
                }
            }
            else
            {
                var piter = AddGtkStore(row.TreeIter, row.Cells);
                foreach (DataGridViewRow child in row.Children)
                {
                    items.Add(child);
                    AddGtkStore(piter, ref rowindex, child);
                }
            }
        }
    }

    private void AddGtkStore(TreeIter parent, ref int rowindex, DataGridViewRow row)
    {
        row.Index = rowindex;
        row.DataGridView = dataGridView;
        var _cellStyle = dataGridView.DefaultCellStyle;
        if (dataGridView.RowsDefaultCellStyle != null)
            _cellStyle = dataGridView.RowsDefaultCellStyle;
        if (rowindex % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
            _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
        if (row.DefaultCellStyle != null)
            _cellStyle = row.DefaultCellStyle;

        foreach (DataGridViewCell cell in row.Cells)
            cell.RowStyle = _cellStyle;

        items.Add(row);
        row.TreeIter = AddGtkStore(parent, row.Cells);
        rowindex++;
        foreach (DataGridViewRow child in row.Children)
        {
            items.Add(child);
            AddGtkStore(row.TreeIter, ref rowindex, child);
        }
    }

    private TreeIter InsertGtkStore(int rowIndex, DataGridViewCellCollection cells)
    {
        var ncolumns = dataGridView.Store.NColumns;
        var columnscount = dataGridView.Columns.Count;
        for (var i = cells.Count; i < columnscount; i++)
            cells.Add(dataGridView.Columns[i].NewCell());
        for (var i = cells.Count; i < ncolumns; i++)
            cells.Add(new DataGridViewTextBoxCell());
        for (var i = 0; i < ncolumns; i++)
            cells[i].ColumnIndex = i;
        if (ncolumns == cells.Count)
            return dataGridView.Store.InsertWithValues(rowIndex, cells.ConvertAll(o => o).ToArray());
        else
            return dataGridView.Store.InsertWithValues(rowIndex, cells.ConvertAll(o => o).Take(ncolumns).ToArray());
    }

    private void InsertGtkStore(int rowIndex, params DataGridViewRow[] dataGridViewRows)
    {
        var idx = rowIndex;
        foreach (var row in dataGridViewRows)
        {
            var _cellStyle = dataGridView.DefaultCellStyle;
            if (dataGridView.RowsDefaultCellStyle != null)
                _cellStyle = dataGridView.RowsDefaultCellStyle;
            if (idx % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
                _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
            if (row.DefaultCellStyle != null)
                _cellStyle = row.DefaultCellStyle;

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.RowIndex = idx;
                cell.RowStyle = _cellStyle;
            }

            items.Insert(idx, row);
            row.TreeIter = InsertGtkStore(idx, row.Cells);
            idx++;
        }
        var newindex = idx;
        for (var i = idx; i < items.Count; i++)
        {
            ((DataGridViewRow)items[i]).Index = idx;
        }
    }
}