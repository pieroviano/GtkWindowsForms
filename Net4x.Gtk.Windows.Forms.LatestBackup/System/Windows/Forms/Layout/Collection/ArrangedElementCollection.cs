// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Forms.Layout;

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

    /// <summary>
    /// Gets the number of elements contained in the
    /// <see cref="T:System.Collections.ICollection">
    /// </see>.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public virtual int Count => InnerList.Count;

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

    /// <summary>
    /// Copies the elements of the
    /// <see cref="T:System.Collections.ICollection">
    /// </see> to an
    /// <see cref="T:System.Array">
    /// </see>, starting at a particular
    /// <see cref="T:System.Array">
    /// </see> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional
    /// <see cref="T:System.Array">
    /// </see> that is the destination of the elements copied from
    /// <see cref="T:System.Collections.ICollection">
    /// </see>. The
    /// <see cref="T:System.Array">
    /// </see> must have zero-based indexing.
    /// </param>
    /// <param name="index">
    /// The zero-based index in array at which copying begins.
    /// </param>
    public void CopyTo(Array array, int index) => ((ICollection)InnerList).CopyTo(array, index);

    /// <summary>
    /// Determines whether the specified
    /// <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current object.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the specified
    /// <see cref="T:System.Object" /> is equal to this instance; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
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
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An
    /// <see cref="T:System.Collections.IEnumerator">
    /// </see> object that can be used to iterate through the collection.
    /// </returns>
    public virtual IEnumerator GetEnumerator() => InnerList.GetEnumerator();

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
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
        var delta = toIndex - fromIndex;

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