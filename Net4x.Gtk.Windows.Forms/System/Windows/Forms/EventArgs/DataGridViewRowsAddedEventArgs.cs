// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowsAdded" /> event.
    /// </summary>
    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowsAddedEventArgs" /> class.
        /// </summary>
        /// <param name="rowIndex">The index of the first added row.
        /// </param>
        /// <param name="rowCount">The number of rows that have been added.
        /// </param>
        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
        {
            RowIndex = rowIndex;
            RowCount = rowCount;
        }
        /// <summary>
        /// Gets the index of the first added row.
        /// </summary>
        /// <returns>The index of the first added row.
        /// </returns>
        public int RowIndex { get; }
        /// <summary>
        /// Gets the number of rows that have been added.
        /// </summary>
        /// <returns>The number of rows that have been added.
        /// </returns>
        public int RowCount { get; }
    }
}
