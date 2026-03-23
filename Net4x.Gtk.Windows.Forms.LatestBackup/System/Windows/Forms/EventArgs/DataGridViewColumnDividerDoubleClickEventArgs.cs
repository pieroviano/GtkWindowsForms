// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.ColumnDividerDoubleClick" />
/// event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.HandledMouseEventArgs" /> instance containing the event data.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// columnIndex
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// e
    /// </exception>
    public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs? e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
    {
        if (columnIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        ColumnIndex = columnIndex;
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; }
}