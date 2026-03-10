// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellFormatting" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
{
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

    public DataGridViewCellStyle CellStyle { get; set; }

    public int ColumnIndex { get; }

    public bool FormattingApplied { get; set; }

    public int RowIndex { get; }

}
