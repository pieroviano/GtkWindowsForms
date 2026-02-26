// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.ColumnDividerDoubleClick" />
    /// event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventArgs" /> class.
        /// </summary>
        /// <param name="columnIndex">The index of the column next to the column divider that was double-clicked.
        /// </param>
        /// <param name="e">A new
        /// <see cref="System.Windows.Forms.HandledMouseEventArgs" /> containing the inherited event data.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="columnIndex" /> is less than -1.
        /// </exception>
        public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            ColumnIndex = columnIndex;
        }
        /// <summary>
        /// The index of the column next to the column divider that was double-clicked.
        /// </summary>
        /// <returns>The index of the column next to the divider.
        /// </returns>
        public int ColumnIndex { get; }
    }
}
