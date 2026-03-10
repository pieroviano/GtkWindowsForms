using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a cell in a
    /// <see cref="T:System.Windows.Forms.TableLayoutPanel" />
    /// </summary>
    public struct TableLayoutPanelCellPosition : IEquatable<TableLayoutPanelCellPosition>
    {

        public TableLayoutPanelCellPosition(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        public int Column
        {
            [CompilerGenerated]
            readonly get;
            [CompilerGenerated]
            set;
        }

        public int Row
        {
            [CompilerGenerated]
            readonly get;
            [CompilerGenerated]
            set;
        }

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

        public override readonly int GetHashCode()
        {
            return this.Column.GetHashCode() & this.Row.GetHashCode();
        }

        public override readonly string ToString()
        {
            return $"[{this.Column},{this.Row}]";
        }

    }
}
