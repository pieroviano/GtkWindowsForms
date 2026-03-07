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

        /// <returns>
        /// true if the cell is on-screen or partially on-screen; otherwise, false.
        /// </returns>
        [Browsable(false)]
        public override bool Displayed
        {
            get;
        }

        /// <summary>
        /// Gets the type of the formatted value of the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> object representing the
        /// <see cref="System.String" /> type.
        /// </returns>
        public override Type FormattedValueType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the cell is frozen.
        /// </summary>
        /// <returns>true if the cell is frozen; otherwise, false. The default is false if the cell is detached from a
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public override bool Frozen
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the header cell is read-only.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">An operation tries to set this property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the cell is resizable.
        /// </summary>
        /// <returns>true if this cell can be resized; otherwise, false. The default is false if the cell is not attached to a
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public override bool Resizable
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell is selected.
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">This property is being set.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the value stored in the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> object representing the
        /// <see cref="System.String" /> type.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override Type ValueType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not the cell is visible.
        /// </summary>
        /// <returns>true if the cell is visible; otherwise, false. The default is false if the cell is detached from a
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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

        /// <summary>
        /// Gets the buttonlike visual state of the header cell.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ButtonState" /> values; the default is
        /// <see cref="System.Windows.Forms.ButtonState.Normal" />
        /// </returns>
        protected ButtonState ButtonState
        {
            get;
        }

    }
}
