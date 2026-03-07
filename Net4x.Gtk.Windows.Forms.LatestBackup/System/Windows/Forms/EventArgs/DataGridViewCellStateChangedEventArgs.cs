// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellStateChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewCell">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that has a changed state.
        /// </param>
        /// <param name="stateChanged">One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the state that has changed on the cell.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewCell" /> is null.
        /// </exception>
        public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
        {
            Cell = dataGridViewCell ?? throw new ArgumentNullException(nameof(dataGridViewCell));
            StateChanged = stateChanged;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that has a changed state.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> whose state has changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCell Cell { get; }

        /// <summary>
        /// Gets the state that has changed on the cell.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the state that has changed on the cell.
        /// </returns>
        public DataGridViewElementStates StateChanged { get; }

    }
}
