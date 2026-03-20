// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ListView.ColumnWidthChanging" /> event.
/// </summary>
public class ColumnWidthChangingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnWidthChangingEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="newWidth">
    /// The new width.
    /// </param>
    public ColumnWidthChangingEventArgs(int columnIndex, int newWidth) : this(columnIndex, newWidth, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnWidthChangingEventArgs" /> class.
    /// </summary>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="newWidth">
    /// The new width.
    /// </param>
    /// <param name="cancel">
    /// if set to
    /// <c>
    /// true
    /// </c>cancel.
    /// </param>
    public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel) : base(cancel)
    {
        ColumnIndex = columnIndex;
        NewWidth = newWidth;
    }

    /// <summary>
    /// Returns the index of the column header whose width is changing
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Returns the new width for the column header who is changing
    /// </summary>
    /// <value>
    /// The new width.
    /// </value>
    public int NewWidth
    {
        get;
        set;
    }

}