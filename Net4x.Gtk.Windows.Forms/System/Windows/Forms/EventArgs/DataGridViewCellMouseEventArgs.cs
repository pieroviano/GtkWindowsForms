// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for mouse events raised by a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// whenever the mouse is moved within a
    /// <see cref="System.Windows.Forms.DataGridViewCell" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellMouseEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellMouseEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The cell's zero-based column index.
        /// </param>
        /// <param name="rowIndex">The cell's zero-based row index.
        /// </param>
        /// <param name="localX">The x-coordinate of the mouse, in pixels.
        /// </param>
        /// <param name="localY">The y-coordinate of the mouse, in pixels.
        /// </param>
        /// <param name="e">The originating
        /// <see cref="System.Windows.Forms.MouseEventArgs" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than -1.-or-
        /// <paramref name="rowIndex" /> is less than -1.
        /// </exception>
        public DataGridViewCellMouseEventArgs(int columnIndex,
                    int rowIndex,
                    int localX,
                    int localY,
                    MouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, localX, localY, e?.Delta ?? 0)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }
        /// <summary>
        /// Gets the zero-based column index of the cell.
        /// </summary>
        /// <returns>An integer specifying the column index.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; }
        /// <summary>
        /// Gets the zero-based row index of the cell.
        /// </summary>
        /// <returns>An integer specifying the row index.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; }
    }
}
