// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// This is the overrided PaintEventArgs for painting the cell of the table
/// It contains additional information indicating the row/column of the cell
/// as well as the bound of the cell
/// </summary>
public class TableLayoutCellPaintEventArgs : PaintEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutCellPaintEventArgs" /> class.
    /// </summary>
    /// <param name="g">
    /// The g.
    /// </param>
    /// <param name="clipRectangle">
    /// The clip rectangle.
    /// </param>
    /// <param name="cellBounds">
    /// The cell bounds.
    /// </param>
    /// <param name="column">
    /// The column.
    /// </param>
    /// <param name="row">
    /// The row.
    /// </param>
    public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row) : base(g, clipRectangle)
    {
        CellBounds = cellBounds;
        Column = column;
        Row = row;
    }

    /// <summary>
    /// Gets the cell bounds.
    /// </summary>
    /// <value>
    /// The cell bounds.
    /// </value>
    public Rectangle CellBounds { get; }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public int Column { get; }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public int Row { get; }

}