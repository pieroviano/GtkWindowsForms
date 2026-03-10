// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for
    /// <see cref="T:System.Windows.Forms.DataGridView.CellBeginEdit" /> and
    /// <see cref="T:System.Windows.Forms.DataGridView.RowValidating" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellCancelEventArgs : CancelEventArgs
    {

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

        public int ColumnIndex { get; }

        public int RowIndex { get; }

    }
}
