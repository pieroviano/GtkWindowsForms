// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellValueNeeded" /> and
    /// <see cref="System.Windows.Forms.DataGridView.CellValuePushed" /> events of the
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellValueEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellValueEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The index of the column containing the cell that the event occurs for.
        /// </param>
        /// <param name="rowIndex">The index of the row containing the cell that the event occurs for.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than 0.-or-
        /// <paramref name="rowIndex" /> is less than 0.
        /// </exception>
        public DataGridViewCellValueEventArgs(int columnIndex, int rowIndex)
        {
            if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        internal DataGridViewCellValueEventArgs()
        {
            ColumnIndex = -1;
            RowIndex = -1;
        }

        /// <summary>
        /// Gets a value indicating the column index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The index of the column containing the cell that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; private set; }

        /// <summary>
        /// Gets a value indicating the row index of the cell that the event occurs for.
        /// </summary>
        /// <returns>The index of the row containing the cell that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; private set; }

        /// <summary>
        /// Gets or sets the value of the cell that the event occurs for.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> representing the cell's value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Value { get; set; }

        internal void SetProperties(int columnIndex, int rowIndex, object value)
        {
            Debug.Assert(columnIndex >= -1);
            Debug.Assert(rowIndex >= -1);
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Value = value;
        }

    }
}
