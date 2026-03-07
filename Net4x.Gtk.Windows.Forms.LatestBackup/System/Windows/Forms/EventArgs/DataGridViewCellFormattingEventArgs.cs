// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellFormatting" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class DataGridViewCellFormattingEventArgs : ConvertEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellFormattingEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The column index of the cell that caused the event.
        /// </param>
        /// <param name="rowIndex">The row index of the cell that caused the event.
        /// </param>
        /// <param name="value">The cell's contents.
        /// </param>
        /// <param name="desiredType">The type to convert
        /// <paramref name="value" /> to.
        /// </param>
        /// <param name="cellStyle">The style of the cell that caused the event.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than -1-or-
        /// <paramref name="rowIndex" /> is less than -1.
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
        /// Gets or sets the style of the cell that is being formatted.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the display style of the cell being formatted. The default is the value of the cell's
        /// <see cref="System.Windows.Forms.DataGridViewCell.InheritedStyle" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCellStyle CellStyle { get; set; }

        /// <summary>
        /// Gets the column index of the cell that is being formatted.
        /// </summary>
        /// <returns>The column index of the cell that is being formatted.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the cell value has been successfully formatted.
        /// </summary>
        /// <returns>true if the formatting for the cell value has been handled; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool FormattingApplied { get; set; }

        /// <summary>
        /// Gets the row index of the cell that is being formatted.
        /// </summary>
        /// <returns>The row index of the cell that is being formatted.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; }

    }
}
