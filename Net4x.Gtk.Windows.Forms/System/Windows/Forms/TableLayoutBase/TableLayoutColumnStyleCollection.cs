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

        public int Add(ColumnStyle columnStyle)
        {
            return base.Add(columnStyle);
        }

        public bool Contains(ColumnStyle columnStyle)
        {
            return base.Contains(columnStyle);
        }

        public int IndexOf(ColumnStyle columnStyle)
        {
            return base.IndexOf(columnStyle);
        }

        public void Insert(int index, ColumnStyle columnStyle)
        {
            base.Insert(index, columnStyle);
        }

        public void Remove(ColumnStyle columnStyle)
        {
            base.Remove(columnStyle);
        }
    }
}
