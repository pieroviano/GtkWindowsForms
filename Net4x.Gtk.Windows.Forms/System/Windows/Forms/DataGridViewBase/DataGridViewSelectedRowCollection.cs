// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> objects that are selected in a
    /// <see cref="T:System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    [ListBindable(false)]
    public class DataGridViewSelectedRowCollection : BaseCollection, IList
    {
        private readonly List<DataGridViewRow> _items = new List<DataGridViewRow>();

        internal DataGridViewSelectedRowCollection()
        {
        }

        int ICollection.Count => _items.Count;

        bool IList.IsFixedSize => true;

        bool IList.IsReadOnly => true;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        protected override ArrayList List => ArrayList.Adapter(_items);

        public DataGridViewRow this[int index] => _items[index];

        object? IList.this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection"); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Clear() => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        /// <summary>
        /// Checks to see if a DataGridViewCell is contained in this collection.
        /// </summary>
        public bool Contains(DataGridViewRow dataGridViewRow) => ((IList)_items).Contains(dataGridViewRow);

        public void CopyTo(DataGridViewRow[] array, int index) => _items.CopyTo(array, index);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewRow dataGridViewRow) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        /// <summary>
        /// Adds a
        /// <see cref="DataGridViewCell" /> to this collection.
        /// </summary>
        internal int Add(DataGridViewRow dataGridViewRow) => ((IList)_items).Add(dataGridViewRow);

        int IList.Add(object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        void IList.Clear() => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        bool IList.Contains(object? value) => ((IList)_items).Contains(value);

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

        void IList.Insert(int index, object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        void IList.Remove(object? value) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

        void IList.RemoveAt(int index) => throw new NotSupportedException("SR.DataGridView_ReadOnlyCollection");

    }
}
