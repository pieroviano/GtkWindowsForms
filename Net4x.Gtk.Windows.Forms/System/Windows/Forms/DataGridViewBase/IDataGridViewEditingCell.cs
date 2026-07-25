// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines common functionality for cells that host editing controls in a
/// <see cref="T:System.Windows.Forms.DataGridView" />.</summary>
public interface IDataGridViewEditingCell
{
    /// <summary>Gets or sets the formatted value of the cell being edited.</summary>
    object? EditingCellFormattedValue { get; set; }

    /// <summary>Gets or sets a value indicating whether the value of the cell has changed.</summary>
    bool EditingCellValueChanged { get; set; }

    /// <summary>Returns the formatted value of the cell being edited.</summary>
    /// <param name="context">A bitwise combination of context values.</param>
    /// <returns>The formatted value.</returns>
    object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context);

    /// <summary>Prepares the cell for editing.</summary>
    /// <param name="selectAll">Whether all the cell content should be selected.</param>
    void PrepareEditingCellForEdit(bool selectAll);
}
