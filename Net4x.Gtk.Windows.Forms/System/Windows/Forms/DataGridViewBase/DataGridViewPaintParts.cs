// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Defines values for specifying the parts of a
/// <see cref="T:System.Windows.Forms.DataGridViewCell" /> that are to be painted.
/// </summary>
[Flags]
public enum DataGridViewPaintParts
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0x00,
    /// <summary>
    /// All
    /// </summary>
    All = 0x7F,
    /// <summary>
    /// The background
    /// </summary>
    Background = 0x01,
    /// <summary>
    /// The border
    /// </summary>
    Border = 0x02,
    /// <summary>
    /// The content background
    /// </summary>
    ContentBackground = 0x04,
    /// <summary>
    /// The content foreground
    /// </summary>
    ContentForeground = 0x08,
    /// <summary>
    /// The error icon
    /// </summary>
    ErrorIcon = 0x10,
    /// <summary>
    /// The focus
    /// </summary>
    Focus = 0x20,
    /// <summary>
    /// The selection background
    /// </summary>
    SelectionBackground = 0x40
}