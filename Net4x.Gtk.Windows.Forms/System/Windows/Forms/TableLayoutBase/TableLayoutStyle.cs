using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements the basic functionality that represents the appearance and behavior of a table layout.
    /// </summary>
    public abstract class TableLayoutStyle
    {

        protected TableLayoutStyle()
        {

        }

        [DefaultValue(SizeType.AutoSize)]
        public SizeType SizeType
        {
            get;
            set;
        }

        internal IArrangedElement Owner
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
}
