using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// A collection that stores
    /// <see cref="System.Windows.Forms.RowStyle" /> objects.
    /// </summary>
    public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
    {

        public new RowStyle this[int index]
        {
            get { return (RowStyle)base[index]; }
            set { base[index] = value; }
        }

        internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner)
        {

        }

        internal TableLayoutRowStyleCollection() : base(null)
        {

        }
        /// <summary>
        /// Adds a new
        /// <see cref="System.Windows.Forms.RowStyle" /> to the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="rowStyle">The
        /// <see cref="System.Windows.Forms.RowStyle" /> to add to the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </param>
        public int Add(RowStyle rowStyle)
        {
            return base.Add(rowStyle);
        }
        /// <summary>
        /// Inserts a
        /// <see cref="System.Windows.Forms.RowStyle" /> into the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" /> at the specified position.
        /// </summary>
        /// <param name="index">The zero-based index at which the
        /// <see cref="System.Windows.Forms.RowStyle" /> should be inserted.
        /// </param>
        /// <param name="rowStyle">The
        /// <see cref="System.Windows.Forms.RowStyle" /> to insert into the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" /> The value can be null.
        /// </param>
        public void Insert(int index, RowStyle rowStyle)
        {
            base.Insert(index, rowStyle);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </summary>
        /// <param name="rowStyle">The
        /// <see cref="System.Windows.Forms.RowStyle" /> to remove from the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" /> The value can be null.
        /// </param>
        public void Remove(RowStyle rowStyle)
        {
            base.Remove(rowStyle);
        }
        /// <summary>
        /// Determines whether the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" /> contains a specific style.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.RowStyle" /> is found in the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />; otherwise, false.
        /// </returns>
        /// <param name="rowStyle">The
        /// <see cref="System.Windows.Forms.RowStyle" /> to locate in the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </param>
        public bool Contains(RowStyle rowStyle)
        {
            return base.Contains(rowStyle);
        }
        /// <summary>
        /// Determines the index of a specific item in the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </summary>
        /// <returns>The index of
        /// <paramref name="rowStyle" /> if found in the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />; otherwise, -1.
        /// </returns>
        /// <param name="rowStyle">The
        /// <see cref="System.Windows.Forms.RowStyle" /> to locate in the
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" />
        /// </param>
        public int IndexOf(RowStyle rowStyle)
        {
            return base.IndexOf(rowStyle);
        }
    }
}
