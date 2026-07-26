// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Forms;

/// <summary>Represents a sorted collection of <see cref="T:System.Windows.Forms.NumericUpDownAcceleration" /> objects.</summary>
public class NumericUpDownAccelerationCollection : MarshalByRefObject, ICollection<NumericUpDownAcceleration>, IEnumerable<NumericUpDownAcceleration>, IEnumerable
{
    private readonly List<NumericUpDownAcceleration> items = new();

    /// <summary>Gets the number of items in the collection.</summary>
    public int Count => items.Count;

    /// <summary>Gets a value indicating whether the collection is read-only. Always <see langword="false" />.</summary>
    public bool IsReadOnly => false;

    /// <summary>Gets the item at the specified index.</summary>
    /// <param name="index">The zero-based index.</param>
    /// <returns>The acceleration at <paramref name="index" />.</returns>
    public NumericUpDownAcceleration this[int index] => items[index];

    /// <summary>Adds an acceleration to the collection, keeping it sorted by <see cref="P:System.Windows.Forms.NumericUpDownAcceleration.Seconds" />.</summary>
    /// <param name="acceleration">The acceleration to add.</param>
    public void Add(NumericUpDownAcceleration acceleration)
    {
        if (acceleration == null) { throw new ArgumentNullException(nameof(acceleration)); }
        items.Add(acceleration);
        items.Sort((a, b) => a.Seconds.CompareTo(b.Seconds));
    }

    /// <summary>Adds a range of accelerations to the collection.</summary>
    /// <param name="accelerations">The accelerations to add.</param>
    public void AddRange(params NumericUpDownAcceleration[] accelerations)
    {
        if (accelerations == null) { throw new ArgumentNullException(nameof(accelerations)); }
        foreach (var a in accelerations) { if (a != null) { items.Add(a); } }
        items.Sort((a, b) => a.Seconds.CompareTo(b.Seconds));
    }

    /// <summary>Removes all items from the collection.</summary>
    public void Clear() => items.Clear();

    /// <summary>Determines whether the collection contains a specific value.</summary>
    /// <param name="acceleration">The acceleration to locate.</param>
    /// <returns><see langword="true" /> if found; otherwise <see langword="false" />.</returns>
    public bool Contains(NumericUpDownAcceleration acceleration) => items.Contains(acceleration);

    /// <summary>Copies the collection to an array.</summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index in the destination at which copying begins.</param>
    public void CopyTo(NumericUpDownAcceleration[] array, int index) => items.CopyTo(array, index);

    /// <summary>Removes a specific acceleration from the collection.</summary>
    /// <param name="acceleration">The acceleration to remove.</param>
    /// <returns><see langword="true" /> if removed; otherwise <see langword="false" />.</returns>
    public bool Remove(NumericUpDownAcceleration acceleration) => items.Remove(acceleration);

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator.</returns>
    public IEnumerator<NumericUpDownAcceleration> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}
