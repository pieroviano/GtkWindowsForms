// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewRow" /> objects that are selected in a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
[ListBindable(false)]
public class DataGridViewSelectedRowCollection : BaseCollection, IList
{
    /// <summary>
    /// The items
    /// </summary>
    private readonly List<DataGridViewRow> _items = new List<DataGridViewRow>();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewSelectedRowCollection" /> class.
    /// </summary>
    internal DataGridViewSelectedRowCollection()
    {
    }

    /// <summary>
    /// Gets the total number of elements in a collection.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    int ICollection.Count => _items.Count;

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
    bool IList.IsFixedSize => true;

    /// <summary>
    /// Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool IList.IsReadOnly => true;

    /// <summary>
    /// Gets a value indicating whether access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see> is synchronized (thread safe).
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is synchronized; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool ICollection.IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see>.
    /// </summary>
    /// <value>
    /// The synchronize root.
    /// </value>
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
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// DataGridViewRow.
    /// </returns>
    public DataGridViewRow this[int index] => _items[index];

    /// <summary>
    /// Gets or sets the System.Nullable&lt;object&gt; at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// System.Nullable.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
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
    public void Clear() => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Checks to see if a DataGridViewCell is contained in this collection.
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
    public bool Contains(DataGridViewRow dataGridViewRow) => ((IList)_items).Contains(dataGridViewRow);

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="array">
    /// The array.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public void CopyTo(DataGridViewRow[] array, int index) => _items.CopyTo(array, index);

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Insert(int index, DataGridViewRow dataGridViewRow) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Adds a
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> to this collection.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    internal int Add(DataGridViewRow dataGridViewRow) => ((IList)_items).Add(dataGridViewRow);

    /// <summary>
    /// Adds an item to the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <param name="value">
    /// The object to add to the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <returns>
    /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    int IList.Add(object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Removes all items from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    void IList.Clear() => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Determines whether the
    /// <see cref="T:System.Collections.IList">
    /// </see> contains a specific value.
    /// </summary>
    /// <param name="value">
    /// The object to locate in the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <returns>
    /// true if the
    /// <see cref="T:System.Object">
    /// </see> is found in the
    /// <see cref="T:System.Collections.IList">
    /// </see>; otherwise, false.
    /// </returns>
    bool IList.Contains(object? value) => ((IList)_items).Contains(value);

    /// <summary>
    /// Copies the elements of the
    /// <see cref="T:System.Collections.ICollection">
    /// </see> to an
    /// <see cref="T:System.Array">
    /// </see>, starting at a particular
    /// <see cref="T:System.Array">
    /// </see> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional
    /// <see cref="T:System.Array">
    /// </see> that is the destination of the elements copied from
    /// <see cref="T:System.Collections.ICollection">
    /// </see>. The
    /// <see cref="T:System.Array">
    /// </see> must have zero-based indexing.
    /// </param>
    /// <param name="index">
    /// The zero-based index in array at which copying begins.
    /// </param>
    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

    /// <summary>
    /// Gets an IEnumerator for the collection.
    /// </summary>
    /// <returns>
    /// IEnumerator.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    /// <summary>
    /// Determines the index of a specific item in the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <param name="value">
    /// The object to locate in the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <returns>
    /// The index of
    /// <paramref name="value">
    /// value
    /// </paramref> if found in the list; otherwise, -1.
    /// </returns>
    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    /// <summary>
    /// Inserts an item to the
    /// <see cref="T:System.Collections.IList">
    /// </see> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which value should be inserted.
    /// </param>
    /// <param name="value">
    /// The object to insert into the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    void IList.Insert(int index, object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Removes the first occurrence of a specific object from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <param name="value">
    /// The object to remove from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    void IList.Remove(object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    /// <summary>
    /// Removes the
    /// <see cref="T:System.Collections.IList">
    /// </see> item at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the item to remove.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// SR.DataGridView_ReadOnlyCollection
    /// </exception>
    void IList.RemoveAt(int index) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

}