using System.Collections;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
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

        public int Add(TableLayoutStyle style)
        {
            return base.Add(style);
        }

        internal void EnsureOwnership(IArrangedElement owner)
        {
            Owner = owner;
        }
    }
}
