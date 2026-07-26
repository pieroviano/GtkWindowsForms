using System.Collections;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality for a collection of table layout styles.
/// </summary>
public abstract class TableLayoutStyleCollection : ArrayList
{
    internal TableLayoutStyleCollection(IArrangedElement owner)
    {
        Owner = owner;

    }

    internal IArrangedElement Owner
    {
        get;

        private set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.TableLayoutStyle" /> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// TableLayoutStyle.
    /// </returns>
    public new TableLayoutStyle this[int index]
    {
        get
        {
            return (TableLayoutStyle)base[index];
        }
        set
        {
            base[index] = value;
        }
    }

    /// <summary>
    /// Adds the specified style.
    /// </summary>
    /// <param name="style">
    /// The style.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    public int Add(TableLayoutStyle style)
    {
        return base.Add(style);
    }

    internal void EnsureOwnership(IArrangedElement owner)
    {
        Owner = owner;
    }
}