using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
/// Represents a cell in a
/// <see cref="T:System.Windows.Forms.TableLayoutPanel" />
/// </summary>
public struct TableLayoutPanelCellPosition : IEquatable<TableLayoutPanelCellPosition>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutPanelCellPosition" /> struct.
    /// </summary>
    /// <param name="column">The column.
    /// </param>
    /// <param name="row">The row.
    /// </param>
    public TableLayoutPanelCellPosition(int column, int row)
    {
        this.Column = column;
        this.Row = row;
    }

    /// <summary>
    /// Sets the column.
    /// </summary>
    /// <value>The column.
    /// </value>
    public int Column
    {
        [CompilerGenerated]
        readonly get;
        [CompilerGenerated]
        set;
    }

    /// <summary>
    /// Sets the row.
    /// </summary>
    /// <value>The row.
    /// </value>
    public int Row
    {
        [CompilerGenerated]
        readonly get;
        [CompilerGenerated]
        set;
    }

    /// <summary>
    /// Determines whether the specified
    /// <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="other">
    /// The
    /// <see cref="T:System.Object" /> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the specified
    /// <see cref="T:System.Object" /> is equal to this instance; otherwise,
    /// <c>false
    /// </c>.
    /// </returns>
    public override readonly bool Equals(object other)
    {
        if (other is TableLayoutPanelCellPosition cell)
        {
            return Equals(cell);
        }

        return false;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.
    /// </param>
    /// <returns>
    /// true if the current object is equal to the
    /// <paramref name="other">other
    /// </paramref> parameter; otherwise, false.
    /// </returns>
    public readonly bool Equals(TableLayoutPanelCellPosition other)
    {
        return this.Column == other.Column && this.Row == other.Row;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override readonly int GetHashCode()
    {
        return this.Column.GetHashCode() & this.Row.GetHashCode();
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override readonly string ToString()
    {
        return $"[{this.Column},{this.Row}]";
    }

}