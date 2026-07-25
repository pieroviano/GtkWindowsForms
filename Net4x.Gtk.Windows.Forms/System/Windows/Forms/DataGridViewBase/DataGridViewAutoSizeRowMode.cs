// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how the height of a row is adjusted.
/// </summary>
public enum DataGridViewAutoSizeRowMode
{
    /// <summary>The row height adjusts to fit the content of all cells including headers.</summary>
    AllCells = 3,
    /// <summary>The row height adjusts to fit the content of all cells excluding the header.</summary>
    AllCellsExceptHeader = 2,
    /// <summary>The row height adjusts to fit the content of the row header cell.</summary>
    RowHeader = 1
}
