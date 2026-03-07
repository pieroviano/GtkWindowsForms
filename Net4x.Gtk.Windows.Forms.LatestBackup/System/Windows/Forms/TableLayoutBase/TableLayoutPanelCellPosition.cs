using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a cell in a
    /// <see cref="System.Windows.Forms.TableLayoutPanel" />
    /// </summary>
    public struct TableLayoutPanelCellPosition : IEquatable<TableLayoutPanelCellPosition>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> class.
        /// </summary>
        /// <param name="column">The column position of the cell.
        /// </param>
        /// <param name="row">The row position of the cell.
        /// </param>
        public TableLayoutPanelCellPosition(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        /// <summary>
        /// Gets or sets the column number of the current
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </summary>
        /// <returns>The column number of the current
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </returns>
        public int Column
        {
            [CompilerGenerated]
            readonly get;
            [CompilerGenerated]
            set;
        }

        /// <summary>
        /// Gets or sets the row number of the current
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </summary>
        /// <returns>The row number of the current
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </returns>
        public int Row
        {
            [CompilerGenerated]
            readonly get;
            [CompilerGenerated]
            set;
        }

        /// <summary>
        /// Specifies whether this
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> contains the same row and column as the specified
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </summary>
        /// <returns>true if
        /// <paramref name="other" /> is a
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> and has the same row and column as the specified
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />; otherwise, false.
        /// </returns>
        /// <param name="other">The
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> to test.
        /// </param>
        public override readonly bool Equals(object other)
        {
            if (other is TableLayoutPanelCellPosition cell)
            {
                return Equals(cell);
            }

            return false;
        }

        public readonly bool Equals(TableLayoutPanelCellPosition other)
        {
            return this.Column == other.Column && this.Row == other.Row;
        }

        /// <summary>
        /// Returns a hash code for this
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </returns>
        public override readonly int GetHashCode()
        {
            return this.Column.GetHashCode() & this.Row.GetHashCode();
        }

        /// <summary>
        /// Converts this
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> to a human readable string.
        /// </summary>
        /// <returns>A string that represents this
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" />
        /// </returns>
        public override readonly string ToString()
        {
            return $"[{this.Column},{this.Row}]";
        }

    }
}
