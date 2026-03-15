using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
/// A collection that stores
/// <see cref="T:System.Windows.Forms.ColumnStyle" /> objects.
/// </summary>
public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
{

    internal TableLayoutColumnStyleCollection(IArrangedElement Owner) : base(Owner)
    {

    }

    internal TableLayoutColumnStyleCollection() : base(null)
    {

    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.ColumnStyle" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// ColumnStyle.
    /// </returns>
    public new ColumnStyle this[int index]
    {
        get
        {
            return (ColumnStyle)base[index];
        }
        set
        {
            base[index] = value;

        }
    }

    /// <summary>
    /// Adds the specified column style.
    /// </summary>
    /// <param name="columnStyle">
    /// The column style.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int Add(ColumnStyle columnStyle)
    {
        return base.Add(columnStyle);
    }

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="columnStyle">
    /// The column style.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if contains [the specified column style]; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public bool Contains(ColumnStyle columnStyle)
    {
        return base.Contains(columnStyle);
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="columnStyle">
    /// The column style.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int IndexOf(ColumnStyle columnStyle)
    {
        return base.IndexOf(columnStyle);
    }

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="columnStyle">
    /// The column style.
    /// </param>
    public void Insert(int index, ColumnStyle columnStyle)
    {
        base.Insert(index, columnStyle);
    }

    /// <summary>
    /// Removes the specified column style.
    /// </summary>
    /// <param name="columnStyle">
    /// The column style.
    /// </param>
    public void Remove(ColumnStyle columnStyle)
    {
        base.Remove(columnStyle);
    }

}