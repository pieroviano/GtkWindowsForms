// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{

    /// <summary>
    /// Represents a collection of selected
    /// <see cref="System.Windows.Forms.DataGridViewCell" /> objects in the
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    [ListBindable(false)]
    public class DataGridViewSelectedCellCollection : BaseCollection, IList
    {
        private readonly List<DataGridViewCell> _items = new List<DataGridViewCell>();

        internal DataGridViewSelectedCellCollection()
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
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" />
        /// </returns>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets a list of elements in the collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.ArrayList" /> containing the elements of the collection.
        /// </returns>
        protected override ArrayList List
        {
            get
            {
                return ArrayList.Adapter(_items);
            }
        }

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
        /// Determines whether the specified cell is contained in the collection.
        /// </summary>
        /// <returns>true if
        /// <paramref name="dataGridViewCell" /> is in the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />; otherwise, false.
        /// </returns>
        /// <param name="dataGridViewCell">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Contains(DataGridViewCell dataGridViewCell) => ((IList)_items).Contains(dataGridViewCell);

        /// <summary>
        /// Copies the elements of the collection to the specified
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of type
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in
        /// <paramref name="array" /> at which copying begins.
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
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void CopyTo(DataGridViewCell[] array, int index) => _items.CopyTo(array, index);

        /// <summary>
        /// Inserts a cell into the collection.
        /// </summary>
        /// <param name="index">The index at which
        /// <paramref name="dataGridViewCell" /> should be inserted.
        /// </param>
        /// <param name="dataGridViewCell">The object to be added to the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
        /// </param>
        /// <exception cref="System.NotSupportedException">Always thrown.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
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

        /// <summary>
        /// Implements the
        /// <see cref="System.Collections.IList.Add(System.Object)" /> method. Always throws
        /// <see cref="System.NotSupportedException" />
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="value">The item to be added to the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
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
        /// Determines whether the specified cell is contained in the collection.
        /// </summary>
        /// <returns>true if
        /// <paramref name="value" /> is in the collection; otherwise, false.
        /// </returns>
        /// <param name="value">A
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
        /// </param>
        bool IList.Contains(object? value) => ((IList)_items).Contains(value);

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in
        /// <paramref name="array" /> at which copying begins.
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
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Returns the index of the specified cell.
        /// </summary>
        /// <returns>The zero-based index of the
        /// <paramref name="value" /> parameter if it is found in the collection; otherwise, -1.
        /// </returns>
        /// <param name="value">The cell to locate in the collection.
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
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
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
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" />
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
