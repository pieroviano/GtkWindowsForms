using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Contains functionality common to row header cells and column header cells.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewHeaderCell : DataGridViewCell
    {

        public DataGridViewHeaderCell()
        {

        }

        [Browsable(false)]
        public override bool Displayed
        {
            get;
        }

        public override Type FormattedValueType
        {
            get;
        }

        [Browsable(false)]
        public override bool Frozen
        {
            get;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool ReadOnly
        {
            get;
            set;
        }

        [Browsable(false)]
        public override bool Resizable
        {
            get;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Selected
        {
            get;
            set;
        }

        public override Type ValueType
        {
            get;
            set;
        }

        [Browsable(false)]
        public override bool Visible
        {
            get
            {
                return true;
            }
        }

        internal Bitmap FlipXPThemesBitmap
        {
            get;
            set;
        }

        protected ButtonState ButtonState
        {
            get;
        }

    }
}
