// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Class ListBox. Implements the
/// <see cref="T:System.Windows.Forms.ListControl" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ListControl" />
public partial class ListBox
{
    /// <summary>
    /// Class IntegerCollection. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    public partial class IntegerCollection : IList
    {
        /// <summary>
        /// The count
        /// </summary>
        private int _count;

        /// <summary>
        /// The inner array
        /// </summary>
        private int[]? _innerArray;

        /// <summary>
        /// The owner
        /// </summary>
        private readonly ListBox _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListBox.IntegerCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public IntegerCollection(ListBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Number of current selected items.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Browsable(false)]
        public int Count
        {
            get
            {
                return _count;
            }
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
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> is read-only.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is read only; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

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
        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>
        /// The synchronize root.
        /// </value>
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Retrieves the specified selected item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int this[int index]
        {
            get
            {
                return _innerArray![index];
            }

            set
            {
                _innerArray![index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the System.Nullable&lt;object&gt; at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// System.Nullable.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// value
        /// </exception>
        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (!(value is int))
                {
                    throw new ArgumentException(nameof(value));
                }
                else
                {
                    this[index] = (int)value;
                }
            }
        }

        /// <summary>
        /// Adds a unique integer to the collection in sorted order.
        /// A SystemException occurs if there is insufficient space available to
        /// store the new item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int Add(int item)
        {
            int index = AddInternal(item);

            return index;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(params int[] items)
        {
            AddRangeInternal(items);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddRange(IntegerCollection value)
        {
            AddRangeInternal(value);
        }

        /// <summary>
        /// Removes all items from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _innerArray = null;
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified item]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public bool Contains(int item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public void CopyTo(Array destination, int index)
        {
            int cnt = Count;
            for (int i = 0; i < cnt; i++)
            {
                destination.SetValue(this[i], i + index);
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int IndexOf(int item)
        {
            int index = -1;

            if (_innerArray != null)
            {
                index = Array.IndexOf(_innerArray, item);

                // We initialize innerArray with more elements than needed in the method EnsureSpace,
                // and we don't actually remove element from innerArray in the method RemoveAt,
                // so there maybe some elements which are not actually in innerArray will be found
                // and we need to filter them out
                if (index >= _count)
                {
                    index = -1;
                }
            }

            return index;
        }

        /// <summary>
        /// Removes the given item from the array.  If
        /// the item is not in the array, this does nothing.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Remove(int item)
        {
            int index = IndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the item at the given index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            _count--;
            for (int i = index; i < _count; i++)
            {
                _innerArray![i] = _innerArray[i + 1];
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// item
        /// </exception>
        int IList.Add(object? item)
        {
            if (!(item is int))
            {
                throw new ArgumentException(nameof(item));
            }

            return Add((int)item);
        }

        /// <summary>
        /// Removes all items from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        void IList.Clear()
        {
            Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified item]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        bool IList.Contains(object? item)
        {
            if (item is int itemAsInt)
            {
                return Contains(itemAsInt);
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CustomTabOffsetsEnumerator(this);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        int IList.IndexOf(object? item)
        {
            if (item is int itemAsInt)
            {
                return IndexOf(itemAsInt);
            }

            return -1;
        }

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
        /// SR.ListBoxCantInsertIntoIntegerCollection
        /// </exception>
        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException("SR.ListBoxCantInsertIntoIntegerCollection");
        }

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
        /// <exception cref="T:System.ArgumentException">
        /// value
        /// </exception>
        void IList.Remove(object? value)
        {
            if (!(value is int))
            {
                throw new ArgumentException(nameof(value));
            }

            Remove((int)value);
        }

        /// <summary>
        /// Removes the
        /// <see cref="T:System.Collections.IList">
        /// </see> item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        /// Add a unique integer to the collection in sorted order.
        /// A SystemException occurs if there is insufficient space available to
        /// store the new item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        private int AddInternal(int item)
        {
            EnsureSpace(1);

            int index = IndexOf(item);
            if (index == -1)
            {
                _innerArray![_count++] = item;
                Array.Sort(_innerArray, 0, _count);
                index = IndexOf(item);
            }

            return index;
        }

        /// <summary>
        /// Add range that bypasses the data source check.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// item
        /// </exception>
        private void AddRangeInternal(ICollection items)
        {
            _owner.BeginUpdate();
            try
            {
                EnsureSpace(items.Count);
                foreach (object item in items)
                {
                    if (!(item is int))
                    {
                        throw new ArgumentException(nameof(item));
                    }
                    else
                    {
                        AddInternal((int)item);
                    }
                }

            }
            finally
            {
                _owner.EndUpdate();
            }
        }

        /// <summary>
        /// Ensures that our internal array has space for the requested # of elements.
        /// </summary>
        /// <param name="elements">
        /// The elements.
        /// </param>
        private void EnsureSpace(int elements)
        {
            if (_innerArray is null)
            {
                _innerArray = new int[Math.Max(elements, 4)];
            }
            else if (_count + elements >= _innerArray.Length)
            {
                int newLength = Math.Max(_innerArray.Length * 2, _innerArray.Length + elements);
                int[] newEntries = new int[newLength];
                _innerArray.CopyTo(newEntries, 0);
                _innerArray = newEntries;
            }
        }

    }
}