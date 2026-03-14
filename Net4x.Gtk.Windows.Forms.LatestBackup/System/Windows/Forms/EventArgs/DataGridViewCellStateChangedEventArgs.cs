// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellStateChanged" /> event.
/// </summary>
public class DataGridViewCellStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellStateChangedEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewCell">The data grid view cell.
    /// </param>
    /// <param name="stateChanged">The state changed.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">dataGridViewCell
    /// </exception>
    public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
    {
        Cell = dataGridViewCell ?? throw new ArgumentNullException(nameof(dataGridViewCell));
        StateChanged = stateChanged;
    }

    /// <summary>
    /// Gets the cell.
    /// </summary>
    /// <value>The cell.
    /// </value>
    public DataGridViewCell Cell { get; }

    /// <summary>
    /// Gets the state changed.
    /// </summary>
    /// <value>The state changed.
    /// </value>
    public DataGridViewElementStates StateChanged { get; }

}