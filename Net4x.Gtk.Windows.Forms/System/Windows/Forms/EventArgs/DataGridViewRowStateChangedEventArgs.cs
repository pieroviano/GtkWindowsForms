// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowStateChanged" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
public class DataGridViewRowStateChangedEventArgs : EventArgs
{
    public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
    {
        Row = dataGridViewRow;
        StateChanged = stateChanged;
    }

    public DataGridViewRow Row { get; }

    public DataGridViewElementStates StateChanged { get; }

}
