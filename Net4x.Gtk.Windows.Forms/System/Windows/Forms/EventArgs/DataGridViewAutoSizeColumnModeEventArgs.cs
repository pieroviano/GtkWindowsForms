// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.AutoSizeColumnModeChanged" /> event.
    /// </summary>
    public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnModeEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewColumn">The column with the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property that changed.
        /// </param>
        /// <param name="previousMode">The previous
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value of the column's
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property.
        /// </param>
        public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            Column = dataGridViewColumn;
            PreviousMode = previousMode;
        }

        /// <summary>
        /// Gets the column with the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property that changed.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> with the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property that changed.
        /// </returns>
        public DataGridViewColumn Column { get; }

        /// <summary>
        /// Gets the previous value of the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property of the column.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value representing the previous value of the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property of the
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnModeEventArgs.Column" />
        /// </returns>
        public DataGridViewAutoSizeColumnMode PreviousMode { get; }

    }
}
