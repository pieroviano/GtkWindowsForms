// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowsAdded" /> event.
/// </summary>
public class DataGridViewRowsAddedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowsAddedEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="rowCount">
    /// The row count.
    /// </param>
    public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
    {
        RowIndex = rowIndex;
        RowCount = rowCount;
    }

    /// <summary>
    /// Gets the row count.
    /// </summary>
    /// <value>
    /// The row count.
    /// </value>
    public int RowCount { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex { get; }
}