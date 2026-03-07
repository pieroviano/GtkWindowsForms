// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowContextMenuStripNeeded" /> event.
    /// </summary>
    public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventArgs" /> class.
        /// </summary>
        /// <param name="rowIndex">The index of the row.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than -1.
        /// </exception>
        public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            RowIndex = rowIndex;
        }

        internal DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex, ContextMenuStrip contextMenuStrip) : this(rowIndex)
        {
            ContextMenuStrip = contextMenuStrip;
        }
        /// <summary>
        /// Gets or sets the shortcut menu for the row that raised the
        /// <see cref="System.Windows.Forms.DataGridView.RowContextMenuStripNeeded" /> event.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> in use.
        /// </returns>
        public ContextMenuStrip ContextMenuStrip { get; set; }

        /// <summary>
        /// Gets the index of the row that is requesting a shortcut menu.
        /// </summary>
        /// <returns>The zero-based index of the row that is requesting a shortcut menu.
        /// </returns>
        public int RowIndex { get; }
    }
}
