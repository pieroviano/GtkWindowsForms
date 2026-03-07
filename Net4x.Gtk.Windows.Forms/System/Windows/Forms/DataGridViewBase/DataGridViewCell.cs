using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays a button-like user interface (UI) for use in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewButtonCell : DataGridViewCell
    {
    }

    /// <summary>
    /// Represents an individual cell in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public abstract class DataGridViewCell
    {

        private bool _Selected;

        private Type _valueType;

        protected DataGridViewCell() { }

        public AccessibleObject AccessibilityObject { get; }

        public int ColumnIndex { get; internal set; }

        public Rectangle ContentBounds { get; }

        [DefaultValue(null)]
        public virtual ContextMenuStrip ContextMenuStrip { get; set; }

        public DataGridView DataGridView { get; set; }

        public virtual object DefaultNewRowValue { get; }

        public virtual bool Displayed { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public object EditedFormattedValue { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public virtual Type EditType { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Rectangle ErrorIconBounds { get; }

        public string ErrorText { get; set; }

        public object FormattedValue { get; }

        public virtual Type FormattedValueType { get; }

        public virtual bool Frozen { get; }

        public bool HasStyle { get; }

        public DataGridViewElementStates InheritedState { get; }

        public DataGridViewCellStyle InheritedStyle { get => RowStyle; }

        public bool IsInEditMode { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataGridViewColumn OwningColumn { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public DataGridViewRow OwningRow { get; }

        public Size PreferredSize { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ReadOnly { get; set; }

        public virtual bool Resizable { get; }

        public int RowIndex { get; internal set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public virtual bool Selected { get => _Selected; set { _Selected = value; DataGridView?.GridView.QueueDraw(); } }

        public Size Size { get; }

        [Browsable(true)]
        public DataGridViewCellStyle Style { get; set; }

        [Bindable(true)]
        [DefaultValue(null)]
        [Localizable(false)]
        [TypeConverter(typeof(StringConverter))]

        public object Tag { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public string ToolTipText { get; set; }

        public object Value { get; set; }

        public virtual Type ValueType { get { return _valueType == null ? Value?.GetType() : _valueType; } set { _valueType = value; } }

        public virtual bool Visible { get; }

        internal DataGridViewRow OwningRowInternal { get; set; }

        internal DataGridViewCellStyle RowStyle { get; set; }

    }

    /// <summary>
    /// Displays a check box user interface (UI) to use in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCheckBoxCell : DataGridViewCell
    {
    }

    /// <summary>
    /// Displays a combo box in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewComboBoxCell : DataGridViewCell
    {
    }

    /// <summary>
    /// Displays a graphic in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewImageCell : DataGridViewCell
    {
    }

    /// <summary>
    /// Represents a cell that contains a link.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewLinkCell : DataGridViewCell
    {
    }

    public class DataGridViewRadioCell : DataGridViewCell
    {
    }

    /// <summary>
    /// Displays editable text information in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewTextBoxCell : DataGridViewCell
    {
        //public DataGridViewTextBoxCell(DataGridViewRow dataGridViewRow) {

        //}
    }
}
