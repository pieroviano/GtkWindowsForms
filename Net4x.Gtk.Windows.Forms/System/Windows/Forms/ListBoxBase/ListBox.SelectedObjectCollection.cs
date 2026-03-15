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
    /// Class SelectedObjectCollection. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    public class SelectedObjectCollection : IList
    {
        // This is the bitmask used within ItemArray to identify selected objects.
        internal static int SelectedObjectMask = ItemArray.CreateMask();

        private int _count;

        private int _lastVersion;

        private readonly ListBox _owner;

        private bool _stateDirty;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListBox.SelectedObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public SelectedObjectCollection(ListBox owner)
        {
            _owner = owner;
            _stateDirty = true;
            _lastVersion = -1;
        }

        /// <summary>
        /// Number of current selected items.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                if (_owner.IsHandleCreated)
                {
                    SelectionMode current = _owner.SelectionMode;
                    switch (current)
                    {
                        case SelectionMode.None:
                            return 0;

                        case SelectionMode.One:
                            int index = _owner.SelectedIndex;
                            if (index >= 0)
                            {
                                return 1;
                            }

                            return 0;

                        case SelectionMode.MultiSimple:
                        case SelectionMode.MultiExtended:
                            return InnerArray.GetCount(SelectedObjectMask);
                    }

                    return 0;
                }

                if (_lastVersion != InnerArray.Version)
                {
                    _lastVersion = InnerArray.Version;
                    _count = InnerArray.GetCount(SelectedObjectMask);
                }

                return _count;
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
                return false;
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
                EnsureUpToDate();

                return _owner.Items.InnerArray;
            }
        }

        /// <summary>
        /// Retrieves the specified selected item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// System.Nullable.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// SR.ListBoxSelectedObjectCollectionIsReadOnly
        /// </exception>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? this[int index]
        {
            get
            {
                return InnerArray.GetItem(index, SelectedObjectMask);
            }
            set
            {
                throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
            }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(object value)
        {
            if (_owner != null)
            {
                ObjectCollection items = _owner.Items;
                if (items != null && value != null)
                {
                    int index = items.IndexOf(value);
                    if (index != -1 && !GetSelected(index))
                    {
                        _owner.SelectedIndex = index;
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
        /// <param name="selectedObject">
        /// The selected object.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified selected object]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public bool Contains(object? selectedObject)
        {
            return IndexOf(selectedObject) != -1;
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
            int cnt = InnerArray.GetCount(SelectedObjectMask);
            for (int i = 0; i < cnt; i++)
            {
                destination.SetValue(InnerArray.GetItem(i, SelectedObjectMask), i + index);
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
            return InnerArray.GetEnumerator(SelectedObjectMask);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="selectedObject">
        /// The selected object.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int IndexOf(object? selectedObject)
        {
            return selectedObject is null ? -1 : InnerArray.IndexOf(selectedObject, SelectedObjectMask);
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
        public void Remove(object value)
        {
            if (_owner != null)
            {
                ObjectCollection items = _owner.Items;
                if (items != null && value != null)
                {
                    int index = items.IndexOf(value);
                    if (index != -1 && GetSelected(index))
                    {
                        _owner.SetSelected(index, false);
                    }
                }
            }
        }

        /// <summary>
        /// Called by the list box to dirty the selected item state.
        /// </summary>
        internal void Dirty()
        {
            _stateDirty = true;
        }

        /// <summary>
        /// This is the function that Ensures that the selections are uptodate with
        /// current listbox handle selections.
        /// </summary>
        internal void EnsureUpToDate()
        {
        }

        // A new internal method used in SelectedIndex getter...
        // For a Multi select ListBox there can be two items with the same name ...
        // and hence a object comparison is required...
        // This method returns the "object" at the passed index rather than the "item" ...
        // this "object" is then compared in the IndexOf( ) method of the itemsCollection.
        internal object GetObjectAt(int index)
        {
            return InnerArray.GetEntryObject(index, SelectedObjectMask);
        }

        internal bool GetSelected(int index)
        {
            return InnerArray.GetState(index, SelectedObjectMask);
        }

        internal void PushSelectionIntoNativeListBox(int index)
        {
        }

        /// <summary>
        /// Same thing for GetSelected.
        /// </summary>
        internal void SetSelected(int index, bool value)
        {
            if (index > -1 && index < InnerArray.Count)
            {
                InnerArray.SetState(index, SelectedObjectMask, value);
                _owner.SetSelected(index, value);
            }
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
        }

        void IList.Clear()
        {
            throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
        }

        void IList.Remove(object? value)
        {
            throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException("SR.ListBoxSelectedObjectCollectionIsReadOnly");
        }

    }
}