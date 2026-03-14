using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
/// A collection that stores
/// <see cref="T:System.Windows.Forms.RowStyle" /> objects.
/// </summary>
public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutRowStyleCollection" /> class.
    /// </summary>
    /// <param name="Owner">
    /// The owner.
    /// </param>
    internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutRowStyleCollection" /> class.
    /// </summary>
    internal TableLayoutRowStyleCollection() : base(null)
    {

    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.RowStyle" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// RowStyle.
    /// </returns>
    public new RowStyle this[int index]
    {
        get { return (RowStyle)base[index]; }
        set { base[index] = value; }
    }

    /// <summary>
    /// Adds the specified row style.
    /// </summary>
    /// <param name="rowStyle">
    /// The row style.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int Add(RowStyle rowStyle)
    {
        return base.Add(rowStyle);
    }

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="rowStyle">
    /// The row style.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if contains [the specified row style]; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public bool Contains(RowStyle rowStyle)
    {
        return base.Contains(rowStyle);
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="rowStyle">
    /// The row style.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int IndexOf(RowStyle rowStyle)
    {
        return base.IndexOf(rowStyle);
    }

    /// <summary>
    /// Inserts the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="rowStyle">
    /// The row style.
    /// </param>
    public void Insert(int index, RowStyle rowStyle)
    {
        base.Insert(index, rowStyle);
    }

    /// <summary>
    /// Removes the specified row style.
    /// </summary>
    /// <param name="rowStyle">
    /// The row style.
    /// </param>
    public void Remove(RowStyle rowStyle)
    {
        base.Remove(rowStyle);
    }

}