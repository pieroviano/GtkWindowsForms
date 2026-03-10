// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.ColumnStateChanged" /> event.
/// </summary>
public class DataGridViewColumnStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewColumnStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="dataGridViewColumn">The data grid view column.</param>
    /// <param name="stateChanged">The state changed.</param>
    public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
    {
        Column = dataGridViewColumn;
        StateChanged = stateChanged;
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>The column.</value>
    public DataGridViewColumn Column { get; }

    /// <summary>
    /// Gets the state changed.
    /// </summary>
    /// <value>The state changed.</value>
    public DataGridViewElementStates StateChanged { get; }

}
