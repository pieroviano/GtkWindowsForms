// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellContextMenuStripNeeded" /> event.
/// </summary>
public class DataGridViewCellContextMenuStripNeededEventArgs : DataGridViewCellEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    public DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex) : base(columnIndex, rowIndex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="contextMenuStrip">
    /// The context menu strip.
    /// </param>
    internal DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex, ContextMenuStrip contextMenuStrip) : base(columnIndex, rowIndex)
    {
        ContextMenuStrip = contextMenuStrip;
    }

    /// <summary>
    /// Gets or sets the context menu strip.
    /// </summary>
    /// <value>
    /// The context menu strip.
    /// </value>
    public ContextMenuStrip ContextMenuStrip { get; set; }

}