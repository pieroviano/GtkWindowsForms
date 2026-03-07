// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="System.Windows.Forms.DataGridViewColumn" />
    /// objects that are selected in a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    [ListBindable(false)]
    public class DataGridViewSelectedColumnCollection : BaseCollection, IList
    {
        private readonly List<DataGridViewColumn> _items = new List<DataGridViewColumn>();

        internal DataGridViewSelectedColumnCollection()
        {
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>The number of elements in the collection.
        /// </returns>
        int ICollection.Count => _items.Count;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool IList.IsFixedSize => true;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        bool IList.IsReadOnly => true;

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the
        /// <see cref="System.Windows.Forms.DataGridViewColumnCollection" />
        /// </returns>
        object ICollection.SyncRoot => this;

        /// <returns>
        /// An
        /// <see cref="System.Collections.ArrayList" /> containing the elements of the collection. This property returns null unless overridden in a derived class.
        /// </returns>
        protected override ArrayList List => ArrayList.Adapter(_items);

        public DataGridViewColumn this[int index] => _items[index];

        object? IList.this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection"); }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Clear()
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Determines whether the specified column is contained in the collection.
        /// </summary>
        /// <returns>true if the
        /// <paramref name="dataGridViewColumn" /> parameter is in the collection; otherwise, false.
        /// </returns>
        /// <param name="dataGridViewColumn">A
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Contains(DataGridViewColumn dataGridViewColumn) => ((IList)_items).Contains(dataGridViewColumn);

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in the array at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or-The number of elements in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.InvalidCastException">The
        /// <see cref="System.Windows.Forms.DataGridViewColumnCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void CopyTo(DataGridViewColumn[] array, int index) => _items.CopyTo(array, index);

        /// <summary>
        /// Inserts a column into the collection at the specified position.
        /// </summary>
        /// <param name="index">The zero-based index at which the column should be inserted.
        /// </param>
        /// <param name="dataGridViewColumn">The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> to insert into the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewColumn dataGridViewColumn)
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Adds a
        /// <see cref="DataGridViewCell" /> to this collection.
        /// </summary>
        internal int Add(DataGridViewColumn dataGridViewColumn) => ((IList)_items).Add(dataGridViewColumn);

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.Add(System.Object)" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <returns>Not applicable. Always throws an exception.
        /// </returns>
        /// <param name="value">The item to be added to the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        int IList.Add(object? value)
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.Clear" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        void IList.Clear()
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Determines whether the specified value is contained in the collection.
        /// </summary>
        /// <returns>true if the
        /// <paramref name="value" /> parameter is in the collection; otherwise, false.
        /// </returns>
        /// <param name="value">An object to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        bool IList.Contains(object? value) => ((IList)_items).Contains(value);

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in the array at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or-The number of elements in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.InvalidCastException">The
        /// <see cref="System.Windows.Forms.DataGridViewColumnCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Returns the index of the specified element.
        /// </summary>
        /// <returns>The zero-based index of the
        /// <paramref name="value" /> parameter if it is found in the collection; otherwise, -1.
        /// </returns>
        /// <param name="value">The element to locate in the collection.
        /// </param>
        int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.Insert(System.Int32,System.Object)" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <param name="index">The index at which
        /// <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">The object to be added to the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.Remove(System.Object)" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <param name="value">The object to be removed from the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" />
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        void IList.Remove(object? value)
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.RemoveAt(System.Int32)" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");
        }

    }
}
