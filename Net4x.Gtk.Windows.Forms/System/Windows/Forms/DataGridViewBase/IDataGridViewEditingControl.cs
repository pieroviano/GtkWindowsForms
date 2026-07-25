// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Implemented by a control hosted in the editing cell of a <see cref="DataGridView" />. Custom editing
/// controls implement this interface so the grid can bind them to a cell, push and pull the formatted
/// value, and route input keys. It was absent, so a custom <c>DataGridView</c> editing control could not
/// be written against the real name.
/// </summary>
public interface IDataGridViewEditingControl
{
    /// <summary>Gets or sets the <see cref="DataGridView" /> that contains the cell being edited.</summary>
    DataGridView? EditingControlDataGridView { get; set; }

    /// <summary>Gets or sets the formatted value of the cell being edited.</summary>
    object? EditingControlFormattedValue { get; set; }

    /// <summary>Gets or sets the index of the hosting cell's row.</summary>
    int EditingControlRowIndex { get; set; }

    /// <summary>Gets or sets a value indicating whether the value of the editing control has changed.</summary>
    bool EditingControlValueChanged { get; set; }

    /// <summary>Gets the cursor used over the editing panel when it is larger than the editing control.</summary>
    Cursor EditingPanelCursor { get; }

    /// <summary>Gets a value indicating whether the editing control should be repositioned when its value changes.</summary>
    bool RepositionEditingControlOnValueChange { get; }

    /// <summary>Applies the given cell style to the editing control.</summary>
    /// <param name="dataGridViewCellStyle">The cell style to apply.</param>
    void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle);

    /// <summary>Determines whether the editing control wants the given input key.</summary>
    /// <param name="keyData">The key to check.</param>
    /// <param name="dataGridViewWantsInputKey">Whether the grid itself wants the key.</param>
    /// <returns><see langword="true" /> if the editing control wants the key.</returns>
    bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey);

    /// <summary>Returns the formatted value of the editing control for the given context.</summary>
    /// <param name="context">The context in which the value is needed.</param>
    /// <returns>The formatted value.</returns>
    object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context);

    /// <summary>Prepares the editing control for editing.</summary>
    /// <param name="selectAll">Whether to select all of the control's content.</param>
    void PrepareEditingControlForEdit(bool selectAll);
}
