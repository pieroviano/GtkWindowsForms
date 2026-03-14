// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellFormatting" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellFormattingEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="desiredType">
    /// Type of the desired.
    /// </param>
    /// <param name="cellStyle">
    /// The cell style.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// columnIndex
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// rowIndex
    /// </exception>
    public DataGridViewCellFormattingEventArgs(int columnIndex,
                                                       int rowIndex,
                                                       object value,
                                                       Type desiredType,
                                                       DataGridViewCellStyle cellStyle) : base(value, desiredType)
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
        CellStyle = cellStyle;
    }

    /// <summary>
    /// Gets or sets the cell style.
    /// </summary>
    /// <value>
    /// The cell style.
    /// </value>
    public DataGridViewCellStyle CellStyle { get; set; }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets or sets a value indicating whether formatting applied.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if formatting applied; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool FormattingApplied { get; set; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex { get; }

}