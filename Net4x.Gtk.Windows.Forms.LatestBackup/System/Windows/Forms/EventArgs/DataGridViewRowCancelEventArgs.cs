// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.UserDeletingRow" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class DataGridViewRowCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowCancelEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewRow">The row the user is deleting.
        /// </param>
        public DataGridViewRowCancelEventArgs(DataGridViewRow dataGridViewRow)
        {
            Row = dataGridViewRow;
        }

        /// <summary>
        /// Gets the row that the user is deleting.
        /// </summary>
        /// <returns>The row that the user deleted.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewRow Row { get; set; }

    }
}
