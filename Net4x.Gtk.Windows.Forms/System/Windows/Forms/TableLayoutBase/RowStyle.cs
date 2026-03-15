namespace System.Windows.Forms;

/// <summary>
/// Represents the look and feel of a row in a table layout.
/// </summary>
public class RowStyle : TableLayoutStyle
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RowStyle" /> class.
    /// </summary>
    public RowStyle()
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RowStyle" /> class.
    /// </summary>
    /// <param name="sizeType">
    /// Type of the size.
    /// </param>
    public RowStyle(SizeType sizeType)
    {
        this.SizeType = sizeType;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RowStyle" /> class.
    /// </summary>
    /// <param name="sizeType">
    /// Type of the size.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    public RowStyle(SizeType sizeType, float height)
    {
        this.SizeType = sizeType;
        this.Height = height;
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public float Height
    {
        get;
        set;
    }

}