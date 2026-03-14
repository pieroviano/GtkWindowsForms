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
    /// <summary>
    /// The empty
    /// </summary>
    internal static ArrangedElementCollection Empty = new(0);

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Layout.ArrangedElementCollection" /> class.
    /// </summary>
    internal ArrangedElementCollection()
                : this(4)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Layout.ArrangedElementCollection" /> class.
    /// </summary>
    /// <param name="innerList">
    /// The inner list.
    /// </param>
    internal ArrangedElementCollection(List<IArrangedElement> innerList)
    {
        InnerList = innerList;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Layout.ArrangedElementCollection" /> class.
    /// </summary>
    /// <param name="size">
    /// The size.
    /// </param>
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

    /// <summary>
    /// Gets a value indicating whether the
    /// <see cref="T:System.Collections.IList">
    /// </see> has a fixed size.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is fixed size; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;

    /// <summary>
    /// Gets a value indicating whether access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see> is synchronized (thread safe).
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is synchronized; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the
    /// <see cref="T:System.Collections.ICollection">
    /// </see>.
    /// </summary>
    /// <value>
    /// The synchronize root.
    /// </value>
    object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

    /// <summary>
    /// Gets the inner list.
    /// </summary>
    /// <value>
    /// The inner list.
    /// </value>
    private protected List<IArrangedElement> InnerList { get; }

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.Layout.IArrangedElement" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// IArrangedElement.
    /// </returns>
    internal virtual IArrangedElement this[int index] => InnerList[index];

    /// <summary>
    /// Gets or sets the System.Nullable&lt;object&gt; at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// System.Nullable.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// </exception>
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

    /// <summary>
    /// Adds an item to the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    /// <param name="value">
    /// The object to add to the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <returns>
    /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
    /// </returns>
    int IList.Add(object? value) => ((IList)InnerList).Add(value);

    /// <summary>
    /// Removes all items from the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </summary>
    void IList.Clear() => ((IList)InnerList).Clear();

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
    bool IList.Contains(object? value) => ((IList)InnerList).Contains(value);

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
    int IList.IndexOf(object? value) => ((IList)InnerList).IndexOf(value);

    /// <summary>
    /// Inserts an item to the
    /// <see cref="T:System.Collections.IList">
    /// </see> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which value should be inserted.
    /// </param>
    /// <param name="value">
    /// The object to insert into the
    /// <see cref="T:System.Collections.IList">
    /// </see>.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// </exception>
    void IList.Insert(int index, object? value) => throw new NotSupportedException();

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
    void IList.Remove(object? value) => ((IList)InnerList).Remove(value);

    /// <summary>
    /// Removes the
    /// <see cref="T:System.Collections.IList">
    /// </see> item at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the item to remove.
    /// </param>
    void IList.RemoveAt(int index) => ((IList)InnerList).RemoveAt(index);

    /// <summary>
    /// Repositions a element in this list.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="fromIndex">
    /// From index.
    /// </param>
    /// <param name="toIndex">
    /// To index.
    /// </param>
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

    /// <summary>
    /// Copies the specified source list.
    /// </summary>
    /// <param name="sourceList">
    /// The source list.
    /// </param>
    /// <param name="sourceIndex">
    /// Index of the source.
    /// </param>
    /// <param name="destinationList">
    /// The destination list.
    /// </param>
    /// <param name="destinationIndex">
    /// Index of the destination.
    /// </param>
    /// <param name="length">
    /// The length.
    /// </param>
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