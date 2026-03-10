// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.ColumnStateChanged" /> event.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewColumnStateChangedEventArgs : EventArgs
{
    public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
    {
        Column = dataGridViewColumn;
        StateChanged = stateChanged;
    }

    public DataGridViewColumn Column { get; }

    public DataGridViewElementStates StateChanged { get; }

}
