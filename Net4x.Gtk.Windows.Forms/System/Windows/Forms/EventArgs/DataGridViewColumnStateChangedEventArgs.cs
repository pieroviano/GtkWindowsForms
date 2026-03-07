// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.ColumnStateChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewColumnStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewColumnStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewColumn">The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> whose state has changed.
        /// </param>
        /// <param name="stateChanged">One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
        {
            Column = dataGridViewColumn;
            StateChanged = stateChanged;
        }
        /// <summary>
        /// Gets the column whose state changed.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> whose state changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewColumn Column { get; }

        /// <summary>
        /// Gets the new column state.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </returns>
        public DataGridViewElementStates StateChanged { get; }
    }
}
