// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowDividerDoubleClick" />
    /// event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowDividerDoubleClickEventArgs" /> class.
        /// </summary>
        /// <param name="rowIndex">The index of the row above the row divider that was double-clicked.
        /// </param>
        /// <param name="e">A new
        /// <see cref="System.Windows.Forms.HandledMouseEventArgs" /> containing the inherited event data.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than -1.
        /// </exception>
        public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            RowIndex = rowIndex;
        }
        /// <summary>
        /// The index of the row above the row divider that was double-clicked.
        /// </summary>
        /// <returns>The index of the row above the divider.
        /// </returns>
        public int RowIndex { get; }
    }
}
