// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// The <see cref="TextBox" /> that a <see cref="DataGridViewTextBoxCell" /> hosts while editing. Absent
/// before, so a text cell had no editing-control type to name.
/// </summary>
public class DataGridViewTextBoxEditingControl : TextBox, IDataGridViewEditingControl
{
    private DataGridView? _dataGridView;
    private int _rowIndex;
    private bool _valueChanged;

    /// <inheritdoc />
    public DataGridView? EditingControlDataGridView
    {
        get => _dataGridView;
        set => _dataGridView = value;
    }

    /// <inheritdoc />
    public object? EditingControlFormattedValue
    {
        get => Text;
        set => Text = value?.ToString() ?? string.Empty;
    }

    /// <inheritdoc />
    public int EditingControlRowIndex
    {
        get => _rowIndex;
        set => _rowIndex = value;
    }

    /// <inheritdoc />
    public bool EditingControlValueChanged
    {
        get => _valueChanged;
        set => _valueChanged = value;
    }

    /// <inheritdoc />
    public Cursor EditingPanelCursor => Cursors.Default;

    /// <inheritdoc />
    public bool RepositionEditingControlOnValueChange => false;

    /// <inheritdoc />
    public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
    {
        if (dataGridViewCellStyle == null)
        {
            return;
        }

        if (dataGridViewCellStyle.Font != null)
        {
            Font = dataGridViewCellStyle.Font;
        }

        BackColor = dataGridViewCellStyle.BackColor;
        ForeColor = dataGridViewCellStyle.ForeColor;
    }

    /// <inheritdoc />
    public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
    {
        // The text box claims the caret-movement and editing keys; everything else defers to the grid.
        switch (keyData & Keys.KeyCode)
        {
            case Keys.Right:
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Home:
            case Keys.End:
            case Keys.Delete:
                return true;
            default:
                return !dataGridViewWantsInputKey;
        }
    }

    /// <inheritdoc />
    public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) =>
        EditingControlFormattedValue ?? string.Empty;

    /// <inheritdoc />
    public void PrepareEditingControlForEdit(bool selectAll)
    {
        if (selectAll)
        {
            SelectAll();
        }
    }
}
