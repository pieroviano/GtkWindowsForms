// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellStyleContentChanged" /> event.
/// </summary>
public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
{
    internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle dataGridViewCellStyle, bool changeAffectsPreferredSize)
    {
        CellStyle = dataGridViewCellStyle;
        ChangeAffectsPreferredSize = changeAffectsPreferredSize;
    }

    /// <summary>
    /// Gets the cell style.
    /// </summary>
    /// <value>
    /// The cell style.
    /// </value>
    public DataGridViewCellStyle CellStyle { get; }

    /// <summary>
    /// Gets the cell style scope.
    /// </summary>
    /// <value>
    /// The cell style scope.
    /// </value>
    public DataGridViewCellStyleScopes CellStyleScope => CellStyle.Scope;

    internal bool ChangeAffectsPreferredSize { get; }

}