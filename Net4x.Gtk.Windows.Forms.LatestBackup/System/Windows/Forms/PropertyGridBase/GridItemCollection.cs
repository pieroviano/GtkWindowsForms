// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
/// Contains a collection of
/// <see cref="T:System.Windows.Forms.GridItem" /> objects.
/// </summary>
public class GridItemCollection : ICollection
{
    /// <summary>
    /// The empty
    /// </summary>
    public static GridItemCollection Empty = new GridItemCollection(Array.Empty<GridItem>());

    private protected GridItem[] _entries;

    internal GridItemCollection(GridItem[]? entries)
    {
        _entries = entries ?? Array.Empty<GridItem>();
    }

    /// <summary>
    /// Retrieves the number of member attributes.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count => _entries.Length;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    /// <summary>
    /// Retrieves the member attribute with the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// GridItem.
    /// </returns>
    public GridItem this[int index] => _entries[index];

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.GridItem" /> with the specified label.
    /// </summary>
    /// <param name="label">
    /// The label.
    /// </param>
    /// <returns>
    /// GridItem.
    /// </returns>
    public GridItem this[string label]
    {
        get
        {
            foreach (var g in _entries)
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
    /// Creates and retrieves a new enumerator for this collection.
    /// </summary>
    /// <returns>
    /// An
    /// <see cref="T:System.Collections.IEnumerator">
    /// </see> object that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator() => _entries.GetEnumerator();

    void ICollection.CopyTo(Array dest, int index)
    {
        if (_entries.Length > 0)
        {
            Array.Copy(_entries, 0, dest, index, _entries.Length);
        }
    }
}