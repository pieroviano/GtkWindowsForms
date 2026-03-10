namespace System.Windows.Forms;

/// <summary>
/// Represents the look and feel of a column in a table layout.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class ColumnStyle : TableLayoutStyle
{

    public ColumnStyle()
    {

    }

    public ColumnStyle(SizeType sizeType)
    {
        this.SizeType = sizeType;
    }

    public ColumnStyle(SizeType sizeType, float width)
    {
        this.SizeType = sizeType;
        this.Width = width;
    }

    public float Width
    {
        get;
        set;
    }

}
