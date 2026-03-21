// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.AutoSizeColumnModeChanged" /> event.
/// </summary>
public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewAutoSizeColumnModeEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewColumn">
    /// The data grid view column.
    /// </param>
    /// <param name="previousMode">
    /// The previous mode.
    /// </param>
    public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
    {
        Column = dataGridViewColumn;
        PreviousMode = previousMode;
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public DataGridViewColumn Column { get; }

    /// <summary>
    /// Gets the previous mode.
    /// </summary>
    /// <value>
    /// The previous mode.
    /// </value>
    public DataGridViewAutoSizeColumnMode PreviousMode { get; }
}