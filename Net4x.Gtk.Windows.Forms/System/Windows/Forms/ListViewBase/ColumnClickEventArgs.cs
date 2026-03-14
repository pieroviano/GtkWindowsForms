// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.ListView.ColumnClick" /> event.
/// </summary>
public class ColumnClickEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnClickEventArgs" /> class.
    /// </summary>
    /// <param name="column">
    /// The column.
    /// </param>
    public ColumnClickEventArgs(int column)
    {
        Column = column;
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public int Column { get; }

}