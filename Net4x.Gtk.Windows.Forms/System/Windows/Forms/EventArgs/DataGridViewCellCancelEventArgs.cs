// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for
    /// <see cref="System.Windows.Forms.DataGridView.CellBeginEdit" /> and
    /// <see cref="System.Windows.Forms.DataGridView.RowValidating" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellCancelEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The index of the column containing the cell that the event occurs for.
        /// </param>
        /// <param name="rowIndex">The index of the row containing the cell that the event occurs for.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than -1.-or-
        /// <paramref name="rowIndex" /> is less than -1.
        /// </exception>
        public DataGridViewCellCancelEventArgs(int columnIndex, int rowIndex)
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
        internal DataGridViewCellCancelEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
        {
        }
        /// <summary>
        /// Gets the column index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The column index of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets the row index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The row index of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; }
    }
}
