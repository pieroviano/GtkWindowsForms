// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Defines values for specifying the parts of a
/// <see cref="System.Windows.Forms.DataGridViewCell" /> that are to be painted.
/// </summary>
[Flags]
public enum DataGridViewPaintParts
{
    /// <summary>
    /// Nothing should be painted.
    /// </summary>
    None = 0x00,
    /// <summary>
    /// All parts of the cell should be painted.
    /// </summary>
    All = 0x7F,
    /// <summary>
    /// The background of the cell should be painted.
    /// </summary>
    Background = 0x01,
    /// <summary>
    /// The border of the cell should be painted.
    /// </summary>
    Border = 0x02,
    /// <summary>
    /// The background of the cell content should be painted.
    /// </summary>
    ContentBackground = 0x04,
    /// <summary>
    /// The foreground of the cell content should be painted.
    /// </summary>
    ContentForeground = 0x08,
    /// <summary>
    /// The cell error icon should be painted.
    /// </summary>
    ErrorIcon = 0x10,
    /// <summary>
    /// The focus rectangle should be painted around the cell.
    /// </summary>
    Focus = 0x20,
    /// <summary>
    /// The background of the cell should be painted when the cell is selected.
    /// </summary>
    SelectionBackground = 0x40
}
