// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for mouse events raised by a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// whenever the mouse is moved within a
/// <see cref="T:System.Windows.Forms.DataGridViewCell" />
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewCellMouseEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellMouseEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="localX">
    /// The local x.
    /// </param>
    /// <param name="localY">
    /// The local y.
    /// </param>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// columnIndex
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// rowIndex
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// e
    /// </exception>
    public DataGridViewCellMouseEventArgs(int columnIndex,
                int rowIndex,
                int localX,
                int localY,
                MouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, localX, localY, e?.Delta ?? 0)
    {
        if (columnIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }
        if (rowIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
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