// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Forms.Layout
{
    /// <summary>
    /// Represents a collection of objects.
    /// </summary>
    public class ArrangedElementCollection : IList
    {
        internal static ArrangedElementCollection Empty = new(0);

        internal ArrangedElementCollection()
            : this(4)
        {
        }

        internal ArrangedElementCollection(List<IArrangedElement> innerList)
        {
            InnerList = innerList;
        }

        private ArrangedElementCollection(int size)
        {
            InnerList = new List<IArrangedElement>(size);
        }

        private protected List<IArrangedElement> InnerList { get; }

        internal virtual IArrangedElement this[int index] => InnerList[index];
        /// <summary>
        /// Determines whether two
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" /> instances are equal.
        /// </summary>
        /// <returns>true if the specified
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" /> is equal to the current
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" /> to compare with the current
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" />
        /// </param>
        public override bool Equals(object? obj)
        {
            if (!(obj is ArrangedElementCollection other) || Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                if (InnerList[i] != other.InnerList[i])
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" />
        /// </returns>
        public override int GetHashCode()
        {
            HashCode hash = default;
            foreach (object o in InnerList)
            {
                hash.Add(o);
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Repositions a element in this list.
        /// </summary>
        private protected void MoveElement(IArrangedElement element, int fromIndex, int toIndex)
        {
            int delta = toIndex - fromIndex;

            switch (delta)
            {
                case -1:
                case 1:
                    // Simple swap
                    InnerList[fromIndex] = InnerList[toIndex];
                    break;

                default:
                    int start;
                    int dest;

                    // Which direction are we moving?
                    if (delta > 0)
                    {
                        // Shift down by the delta to open the new spot
                        start = fromIndex + 1;
                        dest = fromIndex;
                    }
                    else
                    {
                        // Shift up by the delta to open the new spot
                        start = toIndex;
                        dest = toIndex + 1;

                        // Make it positive
                        delta = -delta;
                    }

                    Copy(this, start, this, dest, delta);
                    break;
            }

            InnerList[toIndex] = element;
        }

        private static void Copy(ArrangedElementCollection sourceList, int sourceIndex, ArrangedElementCollection destinationList, int destinationIndex, int length)
        {
            if (sourceIndex < destinationIndex)
            {
                // We need to copy from the back forward to prevent overwrite if source and
                // destination lists are the same, so we need to flip the source/dest indices
                // to point at the end of the spans to be copied.
                sourceIndex += length;
                destinationIndex += length;

                for (; length > 0; length--)
                {
                    destinationList.InnerList[--destinationIndex] = sourceList.InnerList[--sourceIndex];
                }
            }
            else
            {
                for (; length > 0; length--)
                {
                    destinationList.InnerList[destinationIndex++] = sourceList.InnerList[sourceIndex++];
                }
            }
        }
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.Clear" /> method.
        /// </summary>
        void IList.Clear() => ((IList)InnerList).Clear();
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.IsFixedSize" /> property.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Collections.IList" /> has a fixed size; otherwise, false.
        /// </returns>
        bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.Contains(System.Object)" /> method.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Object" /> is found in the
        /// <see cref="System.Collections.IList" />; otherwise, false.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Object" /> to locate in the
        /// <see cref="System.Collections.IList" />
        /// </param>
        bool IList.Contains(object? value) => ((IList)InnerList).Contains(value);
        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>true if the collection is read-only; otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsReadOnly => ((IList)InnerList).IsReadOnly;
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.RemoveAt(System.Int32)" /> method.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.
        /// </param>
        void IList.RemoveAt(int index) => ((IList)InnerList).RemoveAt(index);
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.Remove(System.Object)" /> method.
        /// </summary>
        /// <param name="value">The
        /// <see cref="System.Object" /> to remove from the
        /// <see cref="System.Collections.IList" />
        /// </param>
        void IList.Remove(object? value) => ((IList)InnerList).Remove(value);
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.Add(System.Object)" /> method.
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Object" /> to add to the
        /// <see cref="System.Collections.IList" />
        /// </param>
        int IList.Add(object? value) => ((IList)InnerList).Add(value);
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.IndexOf(System.Object)" /> method.
        /// </summary>
        /// <returns>The index of
        /// <paramref name="value" /> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Object" /> to locate in the
        /// <see cref="System.Collections.IList" />
        /// </param>
        int IList.IndexOf(object? value) => ((IList)InnerList).IndexOf(value);
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.IList.Insert(System.Int32,System.Object)" /> method.
        /// </summary>
        /// <param name="index">The zero-based index at which
        /// <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Object" /> to insert into the
        /// <see cref="System.Collections.IList" />
        /// </param>
        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        object? IList.this[int index]
        {
            get => InnerList[index];
            set => throw new NotSupportedException();
        }
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>The number of elements currently contained in the collection.
        /// </returns>
        public virtual int Count => InnerList.Count;
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.ICollection.SyncRoot" /> property.
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" />
        /// </returns>
        object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;
        /// <summary>
        /// Copies the entire contents of this collection to a compatible one-dimensional
        /// <see cref="System.Array" />, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional
        /// <see cref="System.Array" /> that is the destination of the elements copied from the current collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in
        /// <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" />
        /// is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or-The number of elements in the source collection is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.InvalidCastException">The type of the source element cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        public void CopyTo(Array array, int index) => ((ICollection)InnerList).CopyTo(array, index);
        /// <summary>
        /// For a description of this member, see the
        /// <see cref="System.Collections.ICollection.IsSynchronized" /> property.
        /// </summary>
        /// <returns>true if access to the
        /// <see cref="System.Windows.Forms.Layout.ArrangedElementCollection" /> is synchronized (thread safe); otherwise, false.
        /// </returns>
        bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;
        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IEnumerator" /> for the entire collection.
        /// </returns>
        public virtual IEnumerator GetEnumerator() => InnerList.GetEnumerator();
    }
}
