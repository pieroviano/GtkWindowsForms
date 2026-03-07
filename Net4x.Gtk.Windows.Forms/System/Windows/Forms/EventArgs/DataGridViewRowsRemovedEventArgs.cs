// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowsRemoved" /> event.
    /// </summary>
    public class DataGridViewRowsRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowsRemovedEventArgs" /> class.
        /// </summary>
        /// <param name="rowIndex">The zero-based index of the row that was deleted, or the first deleted row if multiple rows were deleted.
        /// </param>
        /// <param name="rowCount">The number of rows that were deleted.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than 0.-or-
        /// <paramref name="rowCount" /> is less than 1.
        /// </exception>
        public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), "string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowIndex), rowIndex.ToString(CultureInfo.CurrentCulture), 0)");
            }
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "string.Format(SR.InvalidLowBoundArgumentEx, nameof(rowCount), rowCount.ToString(CultureInfo.CurrentCulture), 1)");
            }

            RowIndex = rowIndex;
            RowCount = rowCount;
        }
        /// <summary>
        /// Gets the number of rows that were deleted.
        /// </summary>
        /// <returns>The number of deleted rows.
        /// </returns>
        public int RowCount { get; }

        /// <summary>
        /// Gets the zero-based index of the row deleted, or the first deleted row if multiple rows were deleted.
        /// </summary>
        /// <returns>The zero-based index of the row that was deleted, or the first deleted row if multiple rows were deleted.
        /// </returns>
        public int RowIndex { get; }
    }
}
