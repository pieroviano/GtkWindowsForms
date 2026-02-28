// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowStateChanged" /> event of a
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class DataGridViewRowStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has a changed state.
        /// </param>
        /// <param name="stateChanged">One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the state that has changed on the row.
        /// </param>
        public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
        {
            Row = dataGridViewRow;
            StateChanged = stateChanged;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has a changed state.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has a changed state.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewRow Row { get; }
        /// <summary>
        /// Gets the state that has changed on the row.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the state that has changed on the row.
        /// </returns>
        public DataGridViewElementStates StateChanged { get; }
    }
}
