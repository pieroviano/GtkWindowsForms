// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for row-related
    /// <see cref="System.Windows.Forms.DataGridView" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewRowEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that the event occurred for.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRow" /> is null.
        /// </exception>
        public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
        {
            Row = dataGridViewRow ?? throw new ArgumentNullException(nameof(dataGridViewRow));
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> associated with the event.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> associated with the event.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewRow Row { get; }

    }
}
