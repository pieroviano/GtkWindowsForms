// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines values for setting the automatic sizing behavior of the columns in a <see cref="T:System.Windows.Forms.DataGridView" />.</summary>
public enum DataGridViewAutoSizeColumnsMode
{
    /// <summary>The column widths do not automatically adjust.</summary>
    None = 1,

    /// <summary>The column widths adjust to fit the column header cell content.</summary>
    ColumnHeader = 2,

    /// <summary>The column widths adjust to fit the content of all cells excluding header cells.</summary>
    AllCellsExceptHeader = 4,

    /// <summary>The column widths adjust to fit the content of all cells including header cells.</summary>
    AllCells = 6,

    /// <summary>The column widths adjust to fit the content of displayed cells excluding header cells.</summary>
    DisplayedCellsExceptHeader = 8,

    /// <summary>The column widths adjust to fit the content of displayed cells including header cells.</summary>
    DisplayedCells = 10,

    /// <summary>The column widths adjust so the total widths fill the display area of the control.</summary>
    Fill = 16,
}
