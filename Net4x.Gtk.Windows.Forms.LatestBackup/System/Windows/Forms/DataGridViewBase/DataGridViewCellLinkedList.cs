// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
/// Represents a linked list of
/// <see cref="DataGridViewCell" /> objects
/// </summary>
internal class DataGridViewCellLinkedList : IEnumerable
{

    private readonly int _count;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridView" /> class.
    /// Configures the underlying GTK TreeView and prepares columns/rows collections.
    /// </summary>
    public DataGridViewCellLinkedList()
    {

    }

    public int Count => _count;

    public DataGridViewCell HeadCell
    {
        get
        {
            return null;
        }
    }

    public DataGridViewCell this[int index]
    {
        get
        {
            return null;
        }
    }

    public void Add(DataGridViewCell dataGridViewCell)
    {

    }

    public void Clear()
    {

    }

    public bool Contains(DataGridViewCell dataGridViewCell)
    {

        return false;
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(DataGridViewCell dataGridViewCell)
    {

        return false;
    }

    public int RemoveAllCellsAtBand(bool column, int bandIndex)
    {
        int removedCount = 0;

        return removedCount;
    }

}