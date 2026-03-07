// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for column-related events of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class DataGridViewColumnEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewColumnEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewColumn">The column that the event occurs for.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewColumn" /> is null.
        /// </exception>
        public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }

            Debug.Assert(dataGridViewColumn.Index >= -1);
            Column = dataGridViewColumn;
        }
        /// <summary>
        /// Gets the column that the event occurs for.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> that the event occurs for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewColumn Column { get; }
    }
}
