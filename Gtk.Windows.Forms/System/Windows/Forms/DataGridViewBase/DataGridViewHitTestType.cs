// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying the DataGridView area a point is over.
/// </summary>
public enum DataGridViewHitTestType
{
    /// <summary>A part of the DataGridView with no functionality.</summary>
    None = 0,
    /// <summary>A cell in the DataGridView.</summary>
    Cell = 1,
    /// <summary>A column header.</summary>
    ColumnHeader = 2,
    /// <summary>A row header.</summary>
    RowHeader = 3,
    /// <summary>The top-left header cell.</summary>
    TopLeftHeader = 4,
    /// <summary>The horizontal scroll bar.</summary>
    HorizontalScrollBar = 5,
    /// <summary>The vertical scroll bar.</summary>
    VerticalScrollBar = 6
}
