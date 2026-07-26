// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Specifies the
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// entity that owns the cell style that was changed.
/// </summary>
[Flags]
public enum DataGridViewCellStyleScopes
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0x00,
    /// <summary>
    /// The cell
    /// </summary>
    Cell = 0x01,
    /// <summary>
    /// The column
    /// </summary>
    Column = 0x02,
    /// <summary>
    /// The row
    /// </summary>
    Row = 0x04,
    /// <summary>
    /// The data grid view
    /// </summary>
    DataGridView = 0x08,
    /// <summary>
    /// The column headers
    /// </summary>
    ColumnHeaders = 0x10,
    /// <summary>
    /// The row headers
    /// </summary>
    RowHeaders = 0x20,
    /// <summary>
    /// The rows
    /// </summary>
    Rows = 0x40,
    /// <summary>
    /// The alternating rows
    /// </summary>
    AlternatingRows = 0x80
}