// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewColumn" />
/// objects that are selected in a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
[ListBindable(false)]
public class DataGridViewSelectedColumnCollection : BaseCollection, IList
{
    private readonly List<DataGridViewColumn> _items = new List<DataGridViewColumn>();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridView" /> class.
    /// Configures the underlying GTK TreeView and prepares columns/rows collections.
    /// </summary>
    internal DataGridViewSelectedColumnCollection()
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
    protected override ArrayList List => ArrayList.Adapter(_items);

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// DataGridViewColumn.
    /// </returns>
    public DataGridViewColumn this[int index] => _items[index];

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
    public bool Contains(DataGridViewColumn dataGridViewColumn) => ((IList)_items).Contains(dataGridViewColumn);

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public void CopyTo(DataGridViewColumn[] array, int index) => _items.CopyTo(array, index);

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="dataGridViewColumn">
    /// The data grid view column.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Insert(int index, DataGridViewColumn dataGridViewColumn)
    {
        throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
    }

    /// <summary>
    /// Adds a
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> to this collection.
    /// </summary>
    internal int Add(DataGridViewColumn dataGridViewColumn) => ((IList)_items).Add(dataGridViewColumn);

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