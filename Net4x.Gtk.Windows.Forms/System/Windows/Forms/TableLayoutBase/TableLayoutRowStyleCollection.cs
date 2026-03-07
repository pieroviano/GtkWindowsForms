using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// A collection that stores
    /// <see cref="System.Windows.Forms.RowStyle" /> objects.
    /// </summary>
    public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
    {

        internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner)
        {

        }

        internal TableLayoutRowStyleCollection() : base(null)
        {

        }

        public new RowStyle this[int index]
        {
            get { return (RowStyle)base[index]; }
            set { base[index] = value; }
        }

        public int Add(RowStyle rowStyle)
        {
            return base.Add(rowStyle);
        }

        public bool Contains(RowStyle rowStyle)
        {
            return base.Contains(rowStyle);
        }

        public int IndexOf(RowStyle rowStyle)
        {
            return base.IndexOf(rowStyle);
        }

        public void Insert(int index, RowStyle rowStyle)
        {
            base.Insert(index, rowStyle);
        }

        public void Remove(RowStyle rowStyle)
        {
            base.Remove(rowStyle);
        }
    }
}
