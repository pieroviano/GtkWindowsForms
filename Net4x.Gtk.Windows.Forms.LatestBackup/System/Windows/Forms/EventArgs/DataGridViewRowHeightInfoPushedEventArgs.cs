// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowHeightInfoPushed" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRowHeightInfoPushedEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">Index of the row.
    /// </param>
    /// <param name="height">The height.
    /// </param>
    /// <param name="minimumHeight">The minimum height.
    /// </param>
    internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight) : base(false)
    {
        Debug.Assert(rowIndex >= -1);
        RowIndex = rowIndex;
        Height = height;
        MinimumHeight = minimumHeight;
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.
    /// </value>
    public int Height { get; }

    /// <summary>
    /// Gets the minimum height.
    /// </summary>
    /// <value>The minimum height.
    /// </value>
    public int MinimumHeight { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>The index of the row.
    /// </value>
    public int RowIndex { get; }

}