// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellValueNeeded" /> and
/// <see cref="T:System.Windows.Forms.DataGridView.CellValuePushed" /> events of the
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
public class DataGridViewCellValueEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellValueEventArgs" /> class.
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
    public DataGridViewCellValueEventArgs(int columnIndex, int rowIndex)
    {
        if (columnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }
        if (rowIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    internal DataGridViewCellValueEventArgs()
    {
        ColumnIndex = -1;
        RowIndex = -1;
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value
    {
        get;
        set;
    }

    internal void SetProperties(int columnIndex, int rowIndex, object value)
    {
        Debug.Assert(columnIndex >= -1);
        Debug.Assert(rowIndex >= -1);
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
        Value = value;
    }
}