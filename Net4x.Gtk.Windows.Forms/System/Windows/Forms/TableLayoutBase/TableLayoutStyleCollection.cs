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

        /// <summary>
        /// Adds a new
        /// <see cref="System.Windows.Forms.TableLayoutStyle" /> to the end of the current collection.
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="style">The
        /// <see cref="System.Windows.Forms.TableLayoutStyle" /> to add to the
        /// <see cref="System.Windows.Forms.TableLayoutStyleCollection" />
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="style" /> is already assigned to another owner. You must first remove it from its current location or clone it.
        /// </exception>
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
