// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of strings used for the automatic-completion feature on certain Windows Forms
/// controls (such as <see cref="T:System.Windows.Forms.TextBox" /> and
/// <see cref="T:System.Windows.Forms.ComboBox" />).
/// </summary>
public class AutoCompleteStringCollection : IList, ICollection, IEnumerable
{
    private readonly List<string> _items = new();

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.AutoCompleteStringCollection" /> class.</summary>
    public AutoCompleteStringCollection()
    {
    }

    /// <summary>Occurs when the collection changes.</summary>
    public event CollectionChangeEventHandler? CollectionChanged;

    /// <summary>Gets the number of strings in the collection.</summary>
    public int Count => _items.Count;

    /// <summary>Gets a value indicating whether the collection is read-only. Always <see langword="false" />.</summary>
    public bool IsReadOnly => false;

    /// <summary>Gets a value indicating whether access to the collection is synchronized. Always <see langword="false" />.</summary>
    public bool IsSynchronized => false;

    /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
    public object SyncRoot => this;

    bool IList.IsFixedSize => false;

    /// <summary>Gets or sets the string at the specified index.</summary>
    /// <param name="index">The zero-based index of the string.</param>
    /// <returns>The string at <paramref name="index" />.</returns>
    public string this[int index]
    {
        get => _items[index];
        set
        {
            _items[index] = value;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (string)value!;
    }

    /// <summary>Adds a string to the collection.</summary>
    /// <param name="value">The string to add.</param>
    /// <returns>The zero-based index at which the string was added.</returns>
    public int Add(string value)
    {
        _items.Add(value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        return _items.Count - 1;
    }

    /// <summary>Adds an array of strings to the collection.</summary>
    /// <param name="value">The strings to add.</param>
    public void AddRange(string[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        _items.AddRange(value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    /// <summary>Removes all strings from the collection.</summary>
    public void Clear()
    {
        _items.Clear();
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
    }

    /// <summary>Determines whether the collection contains the specified string.</summary>
    /// <param name="value">The string to locate.</param>
    /// <returns><see langword="true" /> if the string is found; otherwise <see langword="false" />.</returns>
    public bool Contains(string value) => _items.Contains(value);

    /// <summary>Copies the collection to a string array, starting at the specified index.</summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The zero-based index in the destination at which copying begins.</param>
    public void CopyTo(string[] array, int index) => _items.CopyTo(array, index);

    /// <summary>Returns the index of the specified string in the collection.</summary>
    /// <param name="value">The string to locate.</param>
    /// <returns>The zero-based index, or -1 if not found.</returns>
    public int IndexOf(string value) => _items.IndexOf(value);

    /// <summary>Inserts a string into the collection at the specified index.</summary>
    /// <param name="index">The zero-based index at which to insert.</param>
    /// <param name="value">The string to insert.</param>
    public void Insert(int index, string value)
    {
        _items.Insert(index, value);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
    }

    /// <summary>Removes the specified string from the collection.</summary>
    /// <param name="value">The string to remove.</param>
    public void Remove(string value)
    {
        if (_items.Remove(value))
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }
    }

    /// <summary>Removes the string at the specified index.</summary>
    /// <param name="index">The zero-based index of the string to remove.</param>
    public void RemoveAt(int index)
    {
        var value = _items[index];
        _items.RemoveAt(index);
        OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator GetEnumerator() => _items.GetEnumerator();

    /// <summary>Raises the <see cref="E:System.Windows.Forms.AutoCompleteStringCollection.CollectionChanged" /> event.</summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.CollectionChangeEventArgs" /> that contains the event data.</param>
    protected void OnCollectionChanged(CollectionChangeEventArgs e) => CollectionChanged?.Invoke(this, e);

    int IList.Add(object? value) => Add((string)value!);

    bool IList.Contains(object? value) => value is string s && Contains(s);

    int IList.IndexOf(object? value) => value is string s ? IndexOf(s) : -1;

    void IList.Insert(int index, object? value) => Insert(index, (string)value!);

    void IList.Remove(object? value)
    {
        if (value is string s)
        {
            Remove(s);
        }
    }

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);
}
