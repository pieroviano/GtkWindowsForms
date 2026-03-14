// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowsRemoved" /> event.
/// </summary>
public class DataGridViewRowsRemovedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowsRemovedEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="rowCount">
    /// The row count.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// rowIndex - string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowIndex), rowIndex.ToString(CultureInfo.CurrentCulture), 0)
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// rowCount - string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowCount), rowCount.ToString(CultureInfo.CurrentCulture), 1)
    /// </exception>
    public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
    {
        if (rowIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowIndex), rowIndex.ToString(CultureInfo.CurrentCulture), 0)");
        }
        if (rowCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowCount), "string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowCount), rowCount.ToString(CultureInfo.CurrentCulture), 1)");
        }

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