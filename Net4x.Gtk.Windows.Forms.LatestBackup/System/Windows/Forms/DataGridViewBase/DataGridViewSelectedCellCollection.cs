// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of selected
/// <see cref="T:System.Windows.Forms.DataGridViewCell" /> objects in the
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
[ListBindable(false)]
public class DataGridViewSelectedCellCollection : BaseCollection, IList
{
    private readonly List<DataGridViewCell> _items = new List<DataGridViewCell>();

    internal DataGridViewSelectedCellCollection()
    {
    }

    int ICollection.Count => _items.Count;

    bool IList.IsFixedSize => true;

    bool IList.IsReadOnly => true;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <value>
    /// The list.
    /// </value>
    protected override ArrayList List
    {
        get
        {
            return ArrayList.Adapter(_items);
        }
    }

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// DataGridViewCell.
    /// </returns>
    public DataGridViewCell this[int index]
    {
        get
        {
            return _items[index];
        }
    }

    object? IList.this[int index]
    {
        get { return _items[index]; }
        set { throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection"); }
    }

    /// <summary>
    /// Removes all items from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Clear()
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    /// <summary>
    /// Checks to see if a DataGridViewCell is contained in this collection.
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
    public bool Contains(DataGridViewCell dataGridViewCell) => ((IList)_items).Contains(dataGridViewCell);

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public void CopyTo(DataGridViewCell[] array, int index) => _items.CopyTo(array, index);

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Insert(int index, DataGridViewCell dataGridViewCell)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    /// <summary>
    /// Adds a
    /// <see cref="DataGridViewCell" /> to this collection.
    /// </summary>
    internal int Add(DataGridViewCell dataGridViewCell)
    {
        return ((IList)_items).Add(dataGridViewCell);
    }

    /// <summary>
    /// Adds all the
    /// <see cref="DataGridViewCell" /> objects from the provided linked list to this collection.
    /// </summary>
    internal void AddCellLinkedList(DataGridViewCellLinkedList dataGridViewCells)
    {
        foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
        {
            _items.Add(dataGridViewCell);
        }
    }

    int IList.Add(object? value)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    void IList.Clear()
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    bool IList.Contains(object? value) => ((IList)_items).Contains(value);

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    void IList.Insert(int index, object? value)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    void IList.Remove(object? value)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }
}