// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Specifies the border styles that can be applied to the cells of a
/// <see cref="System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public enum DataGridViewAdvancedCellBorderStyle
{
    /// <summary>
    /// The border is not set.
    /// </summary>
    NotSet = 0,
    /// <summary>
    /// No borders.
    /// </summary>
    None = 1,
    /// <summary>
    /// A single-line border.
    /// </summary>
    Single = 2,
    /// <summary>
    /// A single-line sunken border.
    /// </summary>
    Inset = 3,
    /// <summary>
    /// A double-line sunken border.
    /// </summary>
    InsetDouble = 4,
    /// <summary>
    /// A single-line raised border.
    /// </summary>
    Outset = 5,
    /// <summary>
    /// A double-line raised border.
    /// </summary>
    OutsetDouble = 6,
    /// <summary>
    /// A single-line border containing a raised portion.
    /// </summary>
    OutsetPartial = 7
}
