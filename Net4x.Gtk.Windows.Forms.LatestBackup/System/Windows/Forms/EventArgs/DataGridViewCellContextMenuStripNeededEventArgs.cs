// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellContextMenuStripNeeded" /> event.
    /// </summary>
    public class DataGridViewCellContextMenuStripNeededEventArgs : DataGridViewCellEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The column index of cell that the event occurred for.
        /// </param>
        /// <param name="rowIndex">The row index of the cell that the event occurred for.
        /// </param>
        public DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex) : base(columnIndex, rowIndex)
        {
        }

        internal DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex, ContextMenuStrip contextMenuStrip) : base(columnIndex, rowIndex)
        {
            ContextMenuStrip = contextMenuStrip;
        }

        /// <summary>
        /// Gets or sets the shortcut menu for the cell that raised the
        /// <see cref="System.Windows.Forms.DataGridView.CellContextMenuStripNeeded" /> event.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> for the cell.
        /// </returns>
        public ContextMenuStrip ContextMenuStrip { get; set; }

    }
}
