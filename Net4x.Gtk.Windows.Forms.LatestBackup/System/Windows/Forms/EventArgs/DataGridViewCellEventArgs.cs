// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// events related to cell and row operations.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellEventArgs : EventArgs
    {
        internal DataGridViewCellEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The index of the column containing the cell that the event occurs for.
        /// </param>
        /// <param name="rowIndex">The index of the row containing the cell that the event occurs for.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than -1.-or-
        /// <paramref name="rowIndex" /> is less than -1.
        /// </exception>
        public DataGridViewCellEventArgs(int columnIndex, int rowIndex)
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
        }
        /// <summary>
        /// Gets a value indicating the column index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The index of the column containing the cell that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; }
        /// <summary>
        /// Gets a value indicating the row index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The index of the row containing the cell that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; }
    }
}
