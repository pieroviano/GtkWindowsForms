// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using static System.Windows.Forms.ItemArray;

namespace System.Windows.Forms;

/// <summary>
/// Class ListBox. Implements the
/// <see cref="T:System.Windows.Forms.ListControl" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ListControl" />
public partial class ListBox
{
    /// <summary>
    /// A collection that stores objects.
    /// </summary>
    [ListBindable(false)]
    public class ObjectCollection : IList
    {
        private ItemArray _items = null!;

        private readonly ListBox _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListBox.ObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ObjectCollection(ListBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of ListBox.ObjectCollection based on another ListBox.ObjectCollection.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ObjectCollection(ListBox owner, ObjectCollection value)
            : this(owner)
        {
            AddRange(value);
        }

        /// <summary>
        /// Initializes a new instance of ListBox.ObjectCollection containing any array of objects.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ObjectCollection(ListBox owner, object[] value)
            : this(owner)
        {
            AddRange(value);
        }

        /// <summary>
        /// Retrieves the number of items.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => InnerArray.Count;

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
        public bool IsReadOnly => false;

        /// <summary>
        /// Internal access to the actual data store.
        /// </summary>
        internal ItemArray InnerArray
        {
            get
            {
                _items ??= new ItemArray(_owner);

                return _items;
            }
        }

        bool IList.IsFixedSize => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        /// <summary>
        /// Retrieves the item with the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// System.Object.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object this[int index]
        {
            get
            {
                return InnerArray.GetItem(index);
            }
            set
            {
                _owner.CheckNoDataSource();
                SetItemInternal(index, value);
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = value!;
        }

        /// <summary>
        /// Adds an item to the List box. For an unsorted List box, the item is
        /// added to the end of the existing list of items. For a sorted List box,
        /// the item is inserted into the list according to its sorted position.
        /// The item's toString() method is called to obtain the string that is
        /// displayed in the List box.
        /// A SystemException occurs if there is insufficient space available to
        /// store the new item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int Add(object item)
        {
            _owner.CheckNoDataSource();
            int index = AddInternal(item);

            return index;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddRange(ObjectCollection value)
        {
            _owner.CheckNoDataSource();
            AddRangeInternal(value);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(params object[] items)
        {
            _owner.CheckNoDataSource();
            AddRangeInternal(items);
        }

        /// <summary>
        /// Removes all items from the ListBox.
        /// </summary>
        public virtual void Clear()
        {
            _owner.CheckNoDataSource();
            ClearInternal();
        }

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
        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// Copies the ListBox Items collection to a destination array.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="arrayIndex">
        /// Index of the array.
        /// </param>
        public void CopyTo(object[] destination, int arrayIndex)
        {
            int count = InnerArray.Count;
            for (int i = 0; i < count; i++)
            {
                destination[i + arrayIndex] = InnerArray.GetItem(i);
            }
        }

        /// <summary>
        /// Returns an enumerator for the ListBox Items collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator() => InnerArray.GetEnumerator(0);

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
        public int IndexOf(object value)
        {
            return InnerArray.IndexOf(value);
        }

        /// <summary>
        /// Adds an item to the List box. For an unsorted List box, the item is
        /// added to the end of the existing list of items. For a sorted List box,
        /// the item is inserted into the list according to its sorted position.
        /// The item's toString() method is called to obtain the string that is
        /// displayed in the List box.
        /// A SystemException occurs if there is insufficient space available to
        /// store the new item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Insert(int index, object item)
        {
            _owner.CheckNoDataSource();
            // If the List box is sorted, then nust treat this like an add
            // because we are going to twiddle the index anyway.
            if (_owner._sorted)
            {
                Add(item);
            }
            else
            {
                InnerArray.Insert(index, item);
                if (_owner.IsHandleCreated)
                {
                    bool successful = false;

                    try
                    {
                        _owner.NativeInsert(index, item);
                        successful = true;
                    }
                    finally
                    {
                        if (!successful)
                        {
                            InnerArray.RemoveAt(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the given item from the ListBox, provided that it is
        /// actually in the list.
        /// </summary>
        /// <param name="value">
        /// The object to remove from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        public void Remove(object value)
        {
            _owner.CheckNoDataSource();

            int index = InnerArray.IndexOf(value);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes an item from the ListBox at the given index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            _owner.CheckNoDataSource();
            // Update InnerArray before calling NativeRemoveAt to ensure that when
            // SelectedIndexChanged is raised (by NativeRemoveAt), InnerArray's state matches wrapped LB state.
            InnerArray.RemoveAt(index);

            if (_owner.IsHandleCreated)
            {
                _owner.NativeRemoveAt(index);
            }
        }

        internal void AddRangeInternal(ICollection items)
        {
            _owner.BeginUpdate();
            try
            {
                foreach (object item in items)
                {
                    AddInternal(item);
                }
            }
            finally
            {
                _owner.EndUpdate();
            }
        }

        /// <summary>
        /// Removes all items from the ListBox.  Bypasses the data source check.
        /// </summary>
        internal void ClearInternal()
        {
            // update the width.. to reset Scrollbars..
            // Clear the selection state.
            if (_owner.IsHandleCreated)
            {
                _owner.NativeClear();
            }
            InnerArray.Clear();
        }

        internal int IndexOfIdentifier(object value)
        {
            return InnerArray.IndexOf(value);
        }

        internal void SetItemInternal(int index, object value)
        {
            InnerArray.SetItem(index, value);

            // If the native control has been created, and the display text of the new list item object
            // is different to the current text in the native list item, recreate the native list item...
            if (_owner.IsHandleCreated)
            {
                bool selected = (_owner.SelectedIndex == index);
                if (string.Compare(_owner.GetItemText(value), _owner.NativeGetItemText(index), true, CultureInfo.CurrentCulture) != 0)
                {
                    _owner.NativeRemoveAt(index);
                    _owner.SelectedItems.SetSelected(index, false);
                    _owner.NativeInsert(index, value);
                    if (selected)
                    {
                        _owner.SelectedIndex = index;
                    }
                }
                else
                {
                    // FOR COMPATIBILITY REASONS
                    if (selected)
                    {
                        _owner.OnSelectedIndexChanged(EventArgs.Empty); // will fire selectedvaluechanged

                    }
                }
            }
        }

        int IList.Add(object? item) => Add(item!);

        bool IList.Contains(object? value) => Contains(value!);

        void ICollection.CopyTo(Array destination, int index)
        {
            int count = InnerArray.Count;
            for (int i = 0; i < count; i++)
            {
                destination.SetValue(InnerArray.GetItem(i), i + index);
            }
        }

        int IList.IndexOf(object? value) => IndexOf(value!);

        void IList.Insert(int index, object? item) => Insert(index, item!);

        void IList.Remove(object? value) => Remove(value!);

        private int AddInternal(object item)
        {
            item ??= "";
            int index = -1;
            if (!_owner._sorted)
            {
                InnerArray.Add(item);
            }
            else
            {
                Entry entry = GetEntry(item);
                if (Count > 0)
                {
                    index = InnerArray.BinarySearch(entry);
                    if (index < 0)
                    {
                        // getting the index of the first element that is larger than the search value
                        // this index will be used for insert
                        index = ~index;
                    }
                }
                else
                {
                    index = 0;
                }

                InnerArray.InsertEntry(index, entry);
            }

            bool successful = false;

            try
            {
                if (_owner._sorted)
                {
                    if (_owner.IsHandleCreated)
                    {
                        _owner.NativeInsert(index, item);
                    }
                }
                else
                {
                    index = Count - 1;
                    if (_owner.IsHandleCreated)
                    {
                        _owner.NativeAdd(item);
                    }
                }

                successful = true;
            }
            finally
            {
                if (!successful)
                {
                    InnerArray.Remove(item);
                }
            }

            return index;
        }

    }
}