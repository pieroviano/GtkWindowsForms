// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

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
        /// EntryEnumerator is an enumerator that will enumerate over
        /// a given state mask.
        /// </summary>
        private class EntryEnumerator : IEnumerator
        {

            private readonly bool _anyBit;

            private int _current;

            private readonly ItemArray _items;

            private readonly int _state;

            private readonly int _version;

            /// <summary>
            /// Creates a new enumerator that will enumerate over the given state.
            /// </summary>
            public EntryEnumerator(ItemArray items, int state, bool anyBit)
            {
                _items = items;
                _state = state;
                _anyBit = anyBit;
                _version = items.Version;
                _current = -1;
            }

            /// <summary>
            /// Retrieves the current value in the enumerator.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_current == -1 || _current == _items.Count)
                    {
                        throw new InvalidOperationException("SR.ListEnumCurrentOutOfRange");
                    }

                    return _items._entries[_current].Item;
                }
            }

            /// <summary>
            /// Moves to the next element, or returns false if at the end.
            /// </summary>
            bool IEnumerator.MoveNext()
            {
                if (_version != _items.Version)
                {
                    throw new InvalidOperationException("SR.ListEnumVersionMismatch");
                }

                while (true)
                {
                    if (_current < _items.Count - 1)
                    {
                        _current++;

                        var entry = _items._entries[_current];

                        if (_anyBit)
                        {
                            if ((entry.State & _state) != 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if ((entry.State & _state) == _state)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        _current = _items.Count;
                        return false;
                    }
                }
            }

            /// <summary>
            /// Resets the enumeration back to the beginning.
            /// </summary>
            void IEnumerator.Reset()
            {
                if (_version != _items.Version)
                {
                    throw new InvalidOperationException("SR.ListEnumVersionMismatch");
                }

                _current = -1;
            }

        }
    }
}
