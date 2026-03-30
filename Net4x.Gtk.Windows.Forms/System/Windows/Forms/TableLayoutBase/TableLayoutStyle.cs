using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality that represents the appearance and behavior of a table layout.
/// </summary>
public abstract class TableLayoutStyle
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutStyle" /> class.
    /// </summary>
    protected TableLayoutStyle()
    {

    }

    /// <summary>
    /// Gets or sets the type of the size.
    /// </summary>
    /// <value>
    /// The type of the size.
    /// </value>
    [DefaultValue(SizeType.AutoSize)]
    public SizeType SizeType
    {
        get;
        set;
    }

    internal IArrangedElement? Owner
    {
        get;
        set;
    }

    internal float Size
    {
        get;
        set;
    }

    internal void SetSize(float size)
    {
        this.Size = size;
    }
}