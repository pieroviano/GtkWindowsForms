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
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellStyleContentChangedEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewCellStyle">
    /// The data grid view cell style.
    /// </param>
    /// <param name="changeAffectsPreferredSize">
    /// if set to
    /// <c>
    /// true
    /// </c>change affects preferred size.
    /// </param>
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

    /// <summary>
    /// Gets a value indicating whether change affects preferred size.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if change affects preferred size; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    internal bool ChangeAffectsPreferredSize { get; }

}