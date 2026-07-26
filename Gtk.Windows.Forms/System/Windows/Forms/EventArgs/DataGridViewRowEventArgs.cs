// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for row-related
/// <see cref="T:System.Windows.Forms.DataGridView" /> events.
/// </summary>
public class DataGridViewRowEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewRow">
    /// The data grid view row.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// dataGridViewRow
    /// </exception>
    public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
    {
        Row = dataGridViewRow ?? throw new ArgumentNullException(nameof(dataGridViewRow));
    }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public DataGridViewRow Row { get; }
}