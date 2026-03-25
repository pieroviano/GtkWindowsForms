using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of cells in a
/// <see cref="T:System.Windows.Forms.DataGridViewRow" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
[ListBindable(false)]
public class DataGridViewCellCollection : BaseCollection, IList, ICollection, IEnumerable
{

    private readonly ArrayList items = new();

    private DataGridViewRow owner;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellCollection" /> class.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
    {
        owner = dataGridViewRow;
    }

    /// <summary>
    /// Gets the total number of elements in a collection.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public override int Count
    {
        get { return items.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the
    /// <see cref="T:System.Collections.IList">
    /// </see> has a fixed size.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is fixed size; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsFixedSize => false;

    /// <summary>
    /// Gets or sets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public DataGridViewRow Row
    {
        set => owner = value;
        get => owner;
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <value>
    /// The list.
    /// </value>
    protected override ArrayList List => items;

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// DataGridViewCell.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// value
    /// </exception>
    public DataGridViewCell this[int index]
    {
        get
        {
            return (DataGridViewCell)items[index];
        }
        set
        {
            items[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> with the specified column name.
    /// </summary>
    /// <param name="columnName">
    /// Name of the column.
    /// </param>
    /// <returns>
    /// DataGridViewCell.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// DataGridViewColumnCollection_ColumnNotFound - columnName
    /// </exception>
    public DataGridViewCell this[string columnName]
    {
        get
        {
            DataGridViewColumn dataGridViewColumn = null;
            if (owner.DataGridView != null)
            {
                dataGridViewColumn = owner.DataGridView.Columns[columnName];
            }

            if (dataGridViewColumn == null)
            {
                throw new ArgumentException("DataGridViewColumnCollection_ColumnNotFound", nameof(columnName));
            }

            return (DataGridViewCell)items[dataGridViewColumn.Index];
        }
        set
        {
            DataGridViewColumn dataGridViewColumn = null;
            if (owner.DataGridView != null)
            {
                dataGridViewColumn = owner.DataGridView.Columns[columnName];
            }

            if (dataGridViewColumn == null)
            {
                throw new ArgumentException("DataGridViewColumnCollection_ColumnNotFound", nameof(columnName));
            }

            this[dataGridViewColumn.Index] = value;
        }
    }

    object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// Adds the specified data grid view cell.
    /// </summary>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow
    /// </exception>
    //*****************
    public virtual int Add(DataGridViewCell dataGridViewCell)
    {
        if (owner.DataGridView != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
        }

        if (dataGridViewCell.OwningRow != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow");
        }
        dataGridViewCell.OwningRowInternal = owner;

        return items.Add(dataGridViewCell);
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="dataGridViewCells">
    /// The data grid view cells.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// dataGridViewCells
    /// </exception>
    public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
    {
        if (dataGridViewCells == null)
        {
            throw new ArgumentNullException(nameof(dataGridViewCells));
        }

        items.AddRange(dataGridViewCells);
        foreach (var dataGridViewCell2 in dataGridViewCells)
        {
            dataGridViewCell2.OwningRowInternal = owner;
        }
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="dataGridViewCells">
    /// The data grid view cells.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// dataGridViewCells
    /// </exception>
    public void AddRange(IEnumerable<DataGridViewCell> dataGridViewCells)
    {
        if (dataGridViewCells == null)
        {
            throw new ArgumentNullException(nameof(dataGridViewCells));
        }

        foreach (var dataGridViewCell2 in dataGridViewCells)
        {
            dataGridViewCell2.OwningRowInternal = owner;
            items.Add(dataGridViewCell2);
        }
    }

    /// <summary>
    /// Removes all items from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView
    /// </exception>
    public virtual void Clear()
    {
        if (owner.DataGridView != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
        }

        foreach (DataGridViewCell item in items)
        {
            item.OwningRowInternal = null;
        }

        items.Clear();
    }

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if contains [the specified data grid view cell]; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public virtual bool Contains(DataGridViewCell dataGridViewCell)
    {
        var num = items.IndexOf(dataGridViewCell);

        return num != -1;
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
    public void CopyTo(DataGridViewCell[] array, int index)
    {
        items.CopyTo(array, index);
    }

    /// <summary>
    /// Gets an IEnumerator for the collection.
    /// </summary>
    /// <returns>
    /// IEnumerator.
    /// </returns>
    public override IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int IndexOf(DataGridViewCell dataGridViewCell)
    {
        return items.IndexOf(dataGridViewCell);
    }

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow
    /// </exception>
    public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
    {
        if (owner.DataGridView != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
        }

        if (dataGridViewCell.OwningRow != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow");
        }

        items.Insert(index, dataGridViewCell);
        dataGridViewCell.OwningRowInternal = owner;
    }

    /// <summary>
    /// Removes the specified cell.
    /// </summary>
    /// <param name="cell">
    /// The cell.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// DataGridViewCellCollection_CellNotFound
    /// </exception>
    public virtual void Remove(DataGridViewCell cell)
    {
        if (owner.DataGridView != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
        }

        var num = -1;
        var count = items.Count;
        for (var i = 0; i < count; i++)
        {
            if (items[i] == cell)
            {
                num = i;
                break;
            }
        }

        if (num == -1)
        {
            throw new ArgumentException("DataGridViewCellCollection_CellNotFound");
        }

        RemoveAt(num);
    }

    /// <summary>
    /// Removes the
    /// <see cref="T:System.Collections.IList">
    /// </see> item at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the item to remove.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView
    /// </exception>
    public virtual void RemoveAt(int index)
    {
        if (owner.DataGridView != null)
        {
            throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
        }

        var dataGridViewCell = (DataGridViewCell)items[index];
        dataGridViewCell.OwningRowInternal = null;
        items.RemoveAt(index);
    }

    internal List<TOutput> ConvertAll<TOutput>(Converter<DataGridViewCell, TOutput> converter)
    {
        var data = new List<TOutput>();
        foreach (DataGridViewCell cell in items)
            data.Add(converter(cell));

        return data;
    }

    int IList.Add(object value)
    {
        return Add((DataGridViewCell)value);
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

    int IList.IndexOf(object value)
    {
        return items.IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
        Insert(index, (DataGridViewCell)value);
    }

    void IList.Remove(object value)
    {
        Remove((DataGridViewCell)value);
    }

    void IList.RemoveAt(int index)
    {
        RemoveAt(index);
    }
}