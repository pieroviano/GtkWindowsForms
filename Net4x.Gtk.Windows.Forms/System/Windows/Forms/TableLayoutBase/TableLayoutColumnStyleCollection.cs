using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// A collection that stores
    /// <see cref="System.Windows.Forms.ColumnStyle" /> objects.
    /// </summary>
    public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
    {

        internal TableLayoutColumnStyleCollection(IArrangedElement Owner) : base(Owner)
        {

        }

        internal TableLayoutColumnStyleCollection() : base(null)
        {

        }

        public new ColumnStyle this[int index]
        {
            get
            {
                return (ColumnStyle)base[index];
            }
            set
            {
                base[index] = value;

            }
        }

        /// <summary>
        /// Adds an item to the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="columnStyle">The
        /// <see cref="System.Windows.Forms.ColumnStyle" /> to add to the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </param>
        public int Add(ColumnStyle columnStyle)
        {
            return base.Add(columnStyle);
        }

        /// <summary>
        /// Determines whether the specified
        /// <see cref="System.Windows.Forms.ColumnStyle" /> is in the collection.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ColumnStyle" /> is found in the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />; otherwise, false.
        /// </returns>
        /// <param name="columnStyle">The
        /// <see cref="System.Windows.Forms.ColumnStyle" /> to locate in the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" /> The value can be null.
        /// </param>
        public bool Contains(ColumnStyle columnStyle)
        {
            return base.Contains(columnStyle);
        }

        /// <summary>
        /// Determines the index of a specific item in the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </summary>
        /// <returns>The index of
        /// <paramref name="columnStyle" /> if found in the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />; otherwise, -1.
        /// </returns>
        /// <param name="columnStyle">The
        /// <see cref="System.Windows.Forms.ColumnStyle" /> to locate in the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </param>
        public int IndexOf(ColumnStyle columnStyle)
        {
            return base.IndexOf(columnStyle);
        }

        /// <summary>
        /// Inserts a
        /// <see cref="System.Windows.Forms.ColumnStyle" /> into the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" /> at the specified position.
        /// </summary>
        /// <param name="index">The zero-based index at which
        /// <see cref="System.Windows.Forms.ColumnStyle" /> should be inserted.
        /// </param>
        /// <param name="columnStyle">The
        /// <see cref="System.Windows.Forms.ColumnStyle" /> to insert into the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </param>
        public void Insert(int index, ColumnStyle columnStyle)
        {
            base.Insert(index, columnStyle);
        }

        /// <summary>
        /// Removes the first occurrence of a specific
        /// <see cref="System.Windows.Forms.ColumnStyle" /> from the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" />
        /// </summary>
        /// <param name="columnStyle">The
        /// <see cref="System.Windows.Forms.ColumnStyle" /> to remove from the
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" /> The value can be null.
        /// </param>
        public void Remove(ColumnStyle columnStyle)
        {
            base.Remove(columnStyle);
        }

    }
}
