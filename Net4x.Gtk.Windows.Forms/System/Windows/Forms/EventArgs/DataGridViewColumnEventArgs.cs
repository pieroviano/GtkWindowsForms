// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for column-related events of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewColumnEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumnEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewColumn">The data grid view column.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">dataGridViewColumn
    /// </exception>
    public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
    {
        if (dataGridViewColumn == null)
        {
            throw new ArgumentNullException(nameof(dataGridViewColumn));
        }

        Debug.Assert(dataGridViewColumn.Index >= -1);
        Column = dataGridViewColumn;
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>The column.
    /// </value>
    public DataGridViewColumn Column { get; }

}