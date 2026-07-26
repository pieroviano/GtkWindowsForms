namespace System.Windows.Forms;

/// <summary>
/// Represents the look and feel of a column in a table layout.
/// </summary>
public class ColumnStyle : TableLayoutStyle
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnStyle" /> class.
    /// </summary>
    public ColumnStyle()
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnStyle" /> class.
    /// </summary>
    /// <param name="sizeType">
    /// Type of the size.
    /// </param>
    public ColumnStyle(SizeType sizeType)
    {
        this.SizeType = sizeType;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColumnStyle" /> class.
    /// </summary>
    /// <param name="sizeType">
    /// Type of the size.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    public ColumnStyle(SizeType sizeType, float width)
    {
        this.SizeType = sizeType;
        this.Width = width;
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public float Width
    {
        get;
        set;
    }
}