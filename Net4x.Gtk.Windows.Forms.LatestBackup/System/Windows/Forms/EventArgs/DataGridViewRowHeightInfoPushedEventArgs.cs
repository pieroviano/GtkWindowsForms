// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowHeightInfoPushed" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
    {
        internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight) : base(false)
        {
            Debug.Assert(rowIndex >= -1);
            RowIndex = rowIndex;
            Height = height;
            MinimumHeight = minimumHeight;
        }
        /// <summary>
        /// Gets the height of the row the event occurred for.
        /// </summary>
        /// <returns>The row height, in pixels.
        /// </returns>
        public int Height { get; }

        /// <summary>
        /// Gets the minimum height of the row the event occurred for.
        /// </summary>
        /// <returns>The minimum row height, in pixels.
        /// </returns>
        public int MinimumHeight { get; }

        /// <summary>
        /// Gets the index of the row the event occurred for.
        /// </summary>
        /// <returns>The zero-based index of the row.
        /// </returns>
        public int RowIndex { get; }
    }
}
