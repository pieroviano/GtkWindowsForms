// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellValidating" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    public class DataGridViewCellValidatingEventArgs : CancelEventArgs
    {
        internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object formattedValue)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            FormattedValue = formattedValue;
        }
        /// <summary>
        /// Gets the column index of the cell that needs to be validated.
        /// </summary>
        /// <returns>A zero-based integer that specifies the column index of the cell that needs to be validated.
        /// </returns>
        public int ColumnIndex { get; }
        /// <summary>
        /// Gets the row index of the cell that needs to be validated.
        /// </summary>
        /// <returns>A zero-based integer that specifies the row index of the cell that needs to be validated.
        /// </returns>
        public int RowIndex { get; }
        /// <summary>
        /// Gets the formatted contents of the cell that needs to be validated.
        /// </summary>
        /// <returns>A reference to the formatted value.
        /// </returns>
        public object FormattedValue { get; }
    }
}
