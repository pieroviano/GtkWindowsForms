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

        public virtual int Count => InnerList.Count;

        public virtual bool IsReadOnly => ((IList)InnerList).IsReadOnly;

        bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;

        bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

        private protected List<IArrangedElement> InnerList { get; }

        internal virtual IArrangedElement this[int index] => InnerList[index];

        object? IList.this[int index]
        {
            get => InnerList[index];
            set => throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index) => ((ICollection)InnerList).CopyTo(array, index);

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

        public virtual IEnumerator GetEnumerator() => InnerList.GetEnumerator();

        public override int GetHashCode()
        {
            HashCode hash = default;
            foreach (object o in InnerList)
            {
                hash.Add(o);
            }

            return hash.ToHashCode();
        }

        int IList.Add(object? value) => ((IList)InnerList).Add(value);

        void IList.Clear() => ((IList)InnerList).Clear();

        bool IList.Contains(object? value) => ((IList)InnerList).Contains(value);

        int IList.IndexOf(object? value) => ((IList)InnerList).IndexOf(value);

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        void IList.Remove(object? value) => ((IList)InnerList).Remove(value);

        void IList.RemoveAt(int index) => ((IList)InnerList).RemoveAt(index);

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
    }
}
