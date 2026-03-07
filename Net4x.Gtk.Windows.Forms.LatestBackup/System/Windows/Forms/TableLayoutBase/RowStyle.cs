namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the look and feel of a row in a table layout.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class RowStyle : TableLayoutStyle
    {

        public RowStyle()
        {

        }

        public RowStyle(SizeType sizeType)
        {
            this.SizeType = sizeType;
        }

        public RowStyle(SizeType sizeType, float height)
        {
            this.SizeType = sizeType;
            this.Height = height;
        }
        public float Height
        {
            get;
            set;
        }
    }
}
