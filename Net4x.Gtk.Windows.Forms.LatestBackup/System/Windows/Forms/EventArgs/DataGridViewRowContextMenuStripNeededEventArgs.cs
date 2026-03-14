// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowContextMenuStripNeeded" /> event.
/// </summary>
public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">Index of the row.
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">rowIndex
    /// </exception>
    public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
    {
        if (rowIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        RowIndex = rowIndex;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">Index of the row.
    /// </param>
    /// <param name="contextMenuStrip">The context menu strip.
    /// </param>
    internal DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex, ContextMenuStrip contextMenuStrip) : this(rowIndex)
    {
        ContextMenuStrip = contextMenuStrip;
    }

    /// <summary>
    /// Gets or sets the context menu strip.
    /// </summary>
    /// <value>The context menu strip.
    /// </value>
    public ContextMenuStrip ContextMenuStrip { get; set; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>The index of the row.
    /// </value>
    public int RowIndex { get; }

}