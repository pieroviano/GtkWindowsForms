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
        /// <summary>
        /// Gets or sets a flag indicating how a row or column should be sized relative to its containing table.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.SizeType" /> values that specifies how rows or columns of user interface (UI) elements should be sized relative to their container. The default is
        /// <see cref="System.Windows.Forms.SizeType.AutoSize" />
        /// </returns>
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
