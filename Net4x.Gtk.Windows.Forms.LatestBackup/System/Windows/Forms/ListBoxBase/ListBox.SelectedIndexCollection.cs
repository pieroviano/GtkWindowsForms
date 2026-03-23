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
    /// Class SelectedIndexCollection. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    public partial class SelectedIndexCollection : IList
    {
        private readonly ListBox _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListBox.SelectedIndexCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public SelectedIndexCollection(ListBox owner)
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
                return _owner.SelectedItems.Count;
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
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return true;
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
        /// This is the item array that stores our data.  We share this backing store
        /// with the main object collection.
        /// </summary>
        private ItemArray InnerArray
        {
            get
            {
                _owner.SelectedItems.EnsureUpToDate();

                return ((ObjectCollection)_owner.Items).InnerArray;
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
                object identifier = InnerArray.GetEntryObject(index, SelectedObjectCollection.SelectedObjectMask);

                return InnerArray.IndexOf(identifier);
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
                throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
            }
        }

        /// <summary>
        /// Adds the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void Add(int index)
        {
            if (_owner != null)
            {
                ObjectCollection items = _owner.Items;
                if (items != null)
                {
                    if (index != -1 && !Contains(index))
                    {
                        _owner.SetSelected(index, true);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all items from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        public void Clear()
        {
            _owner?.ClearSelected();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="selectedIndex">
        /// Index of the selected.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified selected index]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public bool Contains(int selectedIndex)
        {
            return IndexOf(selectedIndex) != -1;
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
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new SelectedIndexEnumerator(this);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="selectedIndex">
        /// Index of the selected.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int IndexOf(int selectedIndex)
        {
            // Just what does this do?  The selectedIndex parameter above is the index into the
            // main object collection.  We look at the state of that item, and if the state indicates
            // that it is selected, we get back the virtualized index into this collection.  Indexes on
            // this collection match those on the SelectedObjectCollection.
            if (selectedIndex >= 0 &&
                selectedIndex < InnerArray.Count &&
                InnerArray.GetState(selectedIndex, SelectedObjectCollection.SelectedObjectMask))
            {
                return InnerArray.IndexOf(InnerArray.GetItem(selectedIndex), SelectedObjectCollection.SelectedObjectMask);
            }

            return -1;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void Remove(int index)
        {
            if (_owner != null)
            {
                ObjectCollection items = _owner.Items;
                if (items != null)
                {
                    if (index != -1 && Contains(index))
                    {
                        _owner.SetSelected(index, false);
                    }
                }
            }
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
        }

        void IList.Clear()
        {
            throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
        }

        bool IList.Contains(object? selectedIndex)
        {
            if (selectedIndex is int selectedIndexAsInt)
            {
                return Contains(selectedIndexAsInt);
            }
            else
            {
                return false;
            }
        }

        int IList.IndexOf(object? selectedIndex)
        {
            if (selectedIndex is int selectedIndexAsInt)
            {
                return IndexOf(selectedIndexAsInt);
            }
            else
            {
                return -1;
            }
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
        }

        void IList.Remove(object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException("SR.ListBoxSelectedIndexCollectionIsReadOnly");
        }
    }
}