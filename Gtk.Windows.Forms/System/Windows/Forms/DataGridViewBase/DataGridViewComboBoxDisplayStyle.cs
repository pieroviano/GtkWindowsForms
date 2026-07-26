// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying the appearance of a DataGridViewComboBoxCell.
/// </summary>
public enum DataGridViewComboBoxDisplayStyle
{
    /// <summary>The cell appears as a combo box.</summary>
    ComboBox = 0,
    /// <summary>The cell appears with a drop-down button when in editing mode.</summary>
    DropDownButton = 1,
    /// <summary>The cell appears as a static text cell.</summary>
    Nothing = 2
}
