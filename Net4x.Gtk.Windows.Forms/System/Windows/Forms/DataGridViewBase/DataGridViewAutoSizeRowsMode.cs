// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how the heights of rows are adjusted.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public enum DataGridViewAutoSizeRowsMode
{
    /// <summary>
    /// All cells
    /// </summary>
    AllCells = 0,
    /// <summary>
    /// All cells except headers
    /// </summary>
    AllCellsExceptHeaders = 1,
    /// <summary>
    /// All headers
    /// </summary>
    AllHeaders = 2,
    /// <summary>
    /// The displayed cells
    /// </summary>
    DisplayedCells = 3,
    /// <summary>
    /// The displayed cells except headers
    /// </summary>
    DisplayedCellsExceptHeaders = 4,
    /// <summary>
    /// The displayed headers
    /// </summary>
    DisplayedHeaders = 5,
    /// <summary>
    /// The none
    /// </summary>
    None = 6,
}
