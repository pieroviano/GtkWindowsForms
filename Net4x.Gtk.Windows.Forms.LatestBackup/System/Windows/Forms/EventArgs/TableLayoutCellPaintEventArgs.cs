// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
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
        /// The
        /// <see cref="T:System.Drawing.Graphics" /> used to paint the item.
        /// </param>
        /// <param name="clipRectangle">
        /// The
        /// <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle in which to paint.
        /// </param>
        /// <param name="cellBounds">
        /// The bounds of the cell.
        /// </param>
        /// <param name="column">
        /// The column of the cell.
        /// </param>
        /// <param name="row">
        /// The row of the cell.
        /// </param>
        public TableLayoutCellPaintEventArgs(Graphics g, Rectangle clipRectangle, Rectangle cellBounds, int column, int row) : base(g, clipRectangle)
        {
            CellBounds = cellBounds;
            Column = column;
            Row = row;
        }

        /// <summary>
        /// Gets the size and location of the cell.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.Drawing.Rectangle" /> representing the size and location of the cell.
        /// </returns>
        public Rectangle CellBounds { get; }

        /// <summary>
        /// Gets the column of the cell.
        /// </summary>
        /// <returns>
        /// The column position of the cell.
        /// </returns>
        public int Column { get; }

        /// <summary>
        /// Gets the row of the cell.
        /// </summary>
        /// <returns>
        /// The row position of the cell.
        /// </returns>
        public int Row { get; }

    }
}
