// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Forms;

/// <summary>
/// Represents a linked list of
/// <see cref="DataGridViewCell" /> objects
/// </summary>
/// <remarks>
/// Insertion is at the head, as in WinForms: the most recently added cell is
/// <see cref="HeadCell" /> and is at index 0, and enumeration walks from the head towards the tail.
/// </remarks>
internal class DataGridViewCellLinkedList : IEnumerable
{

    private readonly List<DataGridViewCell> _cells = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellLinkedList" /> class.
    /// </summary>
    public DataGridViewCellLinkedList()
    {

    }

    /// <summary>
    /// Gets the number of cells in the list.
    /// </summary>
    public int Count => _cells.Count;

    /// <summary>
    /// Gets the cell at the head of the list - the one added last - or <see langword="null" /> when the
    /// list is empty.
    /// </summary>
    public DataGridViewCell? HeadCell
    {
        get
        {
            return _cells.Count == 0 ? null : _cells[0];
        }
    }

    /// <summary>
    /// Gets the cell at <paramref name="index" />, counting from the head.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public DataGridViewCell? this[int index]
    {
        get
        {
            return index >= 0 && index < _cells.Count ? _cells[index] : null;
        }
    }

    /// <summary>
    /// Adds a cell at the head of the list.
    /// </summary>
    /// <param name="dataGridViewCell">The cell to add.</param>
    public void Add(DataGridViewCell dataGridViewCell)
    {
        if (dataGridViewCell == null)
        {
            throw new ArgumentNullException(nameof(dataGridViewCell));
        }

        _cells.Insert(0, dataGridViewCell);
    }

    /// <summary>
    /// Removes every cell from the list.
    /// </summary>
    public void Clear()
    {
        _cells.Clear();
    }

    /// <summary>
    /// Determines whether the list contains <paramref name="dataGridViewCell" />.
    /// </summary>
    /// <param name="dataGridViewCell">The cell to look for.</param>
    /// <returns><see langword="true" /> when the cell is in the list.</returns>
    public bool Contains(DataGridViewCell dataGridViewCell)
    {
        return dataGridViewCell != null && _cells.Contains(dataGridViewCell);
    }

    /// <summary>
    /// Returns an enumerator that walks the list from the head towards the tail.
    /// </summary>
    /// <returns>An enumerator over the cells.</returns>
    public IEnumerator GetEnumerator()
    {
        return _cells.GetEnumerator();
    }

    /// <summary>
    /// Removes the first occurrence of <paramref name="dataGridViewCell" />.
    /// </summary>
    /// <param name="dataGridViewCell">The cell to remove.</param>
    /// <returns><see langword="true" /> when a cell was removed.</returns>
    public bool Remove(DataGridViewCell dataGridViewCell)
    {
        return dataGridViewCell != null && _cells.Remove(dataGridViewCell);
    }

    /// <summary>
    /// Removes every cell that sits on the given band.
    /// </summary>
    /// <param name="column">
    /// <see langword="true" /> to match on <see cref="P:System.Windows.Forms.DataGridViewCell.ColumnIndex" />,
    /// <see langword="false" /> to match on <see cref="P:System.Windows.Forms.DataGridViewCell.RowIndex" />.
    /// </param>
    /// <param name="bandIndex">The index of the band being removed.</param>
    /// <returns>How many cells were removed.</returns>
    public int RemoveAllCellsAtBand(bool column, int bandIndex)
    {
        var removedCount = 0;
        for (var i = _cells.Count - 1; i >= 0; i--)
        {
            var cell = _cells[i];
            if ((column && cell.ColumnIndex == bandIndex) || (!column && cell.RowIndex == bandIndex))
            {
                _cells.RemoveAt(i);
                removedCount++;
            }
        }

        return removedCount;
    }
}
