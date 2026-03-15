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
        private int _count;

        private int[]? _innerArray;

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

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

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
            var index = AddInternal(item);

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
            var cnt = Count;
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
            var index = -1;

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
            var index = IndexOf(item);

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

        int IList.Add(object? item)
        {
            if (!(item is int))
            {
                throw new ArgumentException(nameof(item));
            }

            return Add((int)item);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object? item)
        {
            if (item is int itemAsInt)
            {
                return Contains(itemAsInt);
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CustomTabOffsetsEnumerator(this);
        }

        int IList.IndexOf(object? item)
        {
            if (item is int itemAsInt)
            {
                return IndexOf(itemAsInt);
            }

            return -1;
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException("SR.ListBoxCantInsertIntoIntegerCollection");
        }

        void IList.Remove(object? value)
        {
            if (!(value is int))
            {
                throw new ArgumentException(nameof(value));
            }

            Remove((int)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        /// Add a unique integer to the collection in sorted order.
        /// A SystemException occurs if there is insufficient space available to
        /// store the new item.
        /// </summary>
        private int AddInternal(int item)
        {
            EnsureSpace(1);

            var index = IndexOf(item);
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
        /// Ensures that our internal array has space for
        /// the requested # of elements.
        /// </summary>
        private void EnsureSpace(int elements)
        {
            if (_innerArray is null)
            {
                _innerArray = new int[Math.Max(elements, 4)];
            }
            else if (_count + elements >= _innerArray.Length)
            {
                var newLength = Math.Max(_innerArray.Length * 2, _innerArray.Length + elements);
                var newEntries = new int[newLength];
                _innerArray.CopyTo(newEntries, 0);
                _innerArray = newEntries;
            }
        }

    }
}