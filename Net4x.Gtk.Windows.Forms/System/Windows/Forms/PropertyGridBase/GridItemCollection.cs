// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms
{
    /// <summary>
    /// Contains a collection of
    /// <see cref="System.Windows.Forms.GridItem" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class GridItemCollection : ICollection
    {
        public static GridItemCollection Empty = new GridItemCollection(Array.Empty<GridItem>());

        private protected GridItem[] _entries;

        internal GridItemCollection(GridItem[] entries)
        {
            _entries = entries ?? Array.Empty<GridItem>();
        }

        /// <summary>
        /// Gets the number of grid items in the collection.
        /// </summary>
        /// <returns>The number of grid items in the collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Count => _entries.Length;

        /// <summary>
        /// For a description of this member, see
        /// <see cref="System.Collections.ICollection.IsSynchronized" />
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// For a description of this member, see
        /// <see cref="System.Collections.ICollection.SyncRoot" />
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the
        /// <see cref="System.Windows.Forms.GridItemCollection" />
        /// </returns>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Retrieves the member attribute with the specified index.
        /// </summary>
        public GridItem this[int index] => _entries[index];

        public GridItem this[string label]
        {
            get
            {
                foreach (GridItem g in _entries)
                {
                    if (g.Label == label)
                    {
                        return g;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns an enumeration of all the grid items in the collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IEnumerator" /> for the
        /// <see cref="System.Windows.Forms.GridItemCollection" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IEnumerator GetEnumerator() => _entries.GetEnumerator();

        /// <summary>
        /// For a description of this member, see
        /// <see cref="System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />
        /// </summary>
        /// <param name="dest">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in the array at which copying begins.
        /// </param>
        void ICollection.CopyTo(Array dest, int index)
        {
            if (_entries.Length > 0)
            {
                Array.Copy(_entries, 0, dest, index, _entries.Length);
            }
        }

    }
}
