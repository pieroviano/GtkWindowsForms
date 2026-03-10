// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.DataGridView.UserDeletingRow" /> event of a
    /// <see cref="T:System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class DataGridViewRowCancelEventArgs : CancelEventArgs
    {
        public DataGridViewRowCancelEventArgs(DataGridViewRow dataGridViewRow)
        {
            Row = dataGridViewRow;
        }

        public DataGridViewRow Row { get; set; }

    }
}
