// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// This is similar to ArrayList except that it also
    /// maintains a bit-flag based state element for each item in the array.
    /// The methods to enumerate, count and get data support
    /// virtualized indexes.  Indexes are virtualized according
    /// to the state mask passed in.  This allows ItemArray
    /// to be the backing store for one read-write "master"
    /// collection and several read-only collections based
    /// on masks.  ItemArray supports up to 31 masks.
    /// </summary>
    /// This is similar to ArrayList except that it also
    /// maintains a bit-flag based state element for each item in the array.
    /// The methods to enumerate, count and get data support
    /// virtualized indexes.  Indexes are virtualized according
    /// to the state mask passed in.  This allows ItemArray
    /// to be the backing store for one read-write "master"
    /// collection and several read-only collections based
    /// on masks.  ItemArray supports up to 31 masks.
    /// </summary>
    /// This is similar to ArrayList except that it also
    /// maintains a bit-flag based state element for each item in the array.
    /// The methods to enumerate, count and get data support
    /// virtualized indexes.  Indexes are virtualized according
    /// to the state mask passed in.  This allows ItemArray
    /// to be the backing store for one read-write "master"
    /// collection and several read-only collections based
    /// on masks.  ItemArray supports up to 31 masks.
    /// </summary>
    internal partial class ItemArray
    {
        /// <summary>
        /// This is a single entry in our item array.
        /// </summary>
        internal class Entry
        {
            public Entry(object item)
            {
                Item = item;
                State = 0;
            }

            public object Item { get; set; }

            // This field is used by a CheckList to store data that an item is checked or indeterminate
            public int State { get; set; }
        }
    }
}
