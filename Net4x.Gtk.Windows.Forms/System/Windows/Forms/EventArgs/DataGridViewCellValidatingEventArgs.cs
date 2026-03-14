// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellValidating" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
public class DataGridViewCellValidatingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellValidatingEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">Index of the column.
    /// </param>
    /// <param name="rowIndex">Index of the row.
    /// </param>
    /// <param name="formattedValue">The formatted value.
    /// </param>
    internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object formattedValue)
    {
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
        FormattedValue = formattedValue;
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>The index of the column.
    /// </value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets the formatted value.
    /// </summary>
    /// <value>The formatted value.
    /// </value>
    public object FormattedValue { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>The index of the row.
    /// </value>
    public int RowIndex { get; }

}