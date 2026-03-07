// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowErrorTextNeeded" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
    {
        internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
        {
            Debug.Assert(rowIndex >= -1);
            RowIndex = rowIndex;
            ErrorText = errorText;
        }
        /// <summary>
        /// Gets or sets the error text for the row.
        /// </summary>
        /// <returns>A string that represents the error text for the row.
        /// </returns>
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets the row that raised the
        /// <see cref="System.Windows.Forms.DataGridView.RowErrorTextNeeded" /> event.
        /// </summary>
        /// <returns>The zero based row index for the row.
        /// </returns>
        public int RowIndex { get; }
    }
}
