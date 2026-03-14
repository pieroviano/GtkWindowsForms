// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// events related to cell and row operations.
/// </summary>
public class DataGridViewCellEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// columnIndex
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// rowIndex
    /// </exception>
    public DataGridViewCellEventArgs(int columnIndex, int rowIndex)
    {
        if (columnIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }
        if (rowIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewCell">
    /// The data grid view cell.
    /// </param>
    internal DataGridViewCellEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
    {
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex { get; }

}