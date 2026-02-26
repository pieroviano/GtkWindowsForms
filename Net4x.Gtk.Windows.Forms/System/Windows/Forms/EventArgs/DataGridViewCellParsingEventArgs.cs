// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellParsing" />
    /// event of a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellParsingEventArgs : ConvertEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellParsingEventArgs" /> class.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell that was changed.
        /// </param>
        /// <param name="columnIndex">The column index of the cell that was changed.
        /// </param>
        /// <param name="value">The new value.
        /// </param>
        /// <param name="desiredType">The type of the new value.
        /// </param>
        /// <param name="inheritedCellStyle">The style applied to the cell that was changed.
        /// </param>
        public DataGridViewCellParsingEventArgs(int rowIndex,
                                                 int columnIndex,
                                                 object value,
                                                 Type desiredType,
                DataGridViewCellStyle inheritedCellStyle) : base(value, desiredType)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            InheritedCellStyle = inheritedCellStyle;
        }
        /// <summary>
        /// Gets the row index of the cell that requires parsing.
        /// </summary>
        /// <returns>The row index of the cell that was changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; }
        /// <summary>
        /// Gets the column index of the cell data that requires parsing.
        /// </summary>
        /// <returns>The column index of the cell that was changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; }
        /// <summary>
        /// Gets or sets the style applied to the edited cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the current style of the cell being edited. The default value is the value of the cell
        /// <see cref="System.Windows.Forms.DataGridViewCell.InheritedStyle" /> property.
        /// </returns>
        public DataGridViewCellStyle InheritedCellStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a cell's value has been successfully parsed.
        /// </summary>
        /// <returns>true if the cell's value has been successfully parsed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ParsingApplied { get; set; }
    }
}
