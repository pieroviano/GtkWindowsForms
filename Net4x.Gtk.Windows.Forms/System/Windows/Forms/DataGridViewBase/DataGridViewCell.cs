using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// Displays a button-like user interface (UI) for use in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewButtonCell : DataGridViewCell
{
}

/// <summary>
/// Represents an individual cell in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public abstract class DataGridViewCell
{

    private bool _Selected;

    private Type _valueType;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> class.
    /// </summary>
    protected DataGridViewCell() { }

    /// <summary>
    /// Gets the accessibility object.
    /// </summary>
    /// <value>
    /// The accessibility object.
    /// </value>
    public AccessibleObject AccessibilityObject { get; }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; internal set; }

    /// <summary>
    /// Gets the content bounds.
    /// </summary>
    /// <value>
    /// The content bounds.
    /// </value>
    public Rectangle ContentBounds { get; }

    /// <summary>
    /// Gets or sets the context menu strip.
    /// </summary>
    /// <value>
    /// The context menu strip.
    /// </value>
    [DefaultValue(null)]
    public virtual ContextMenuStrip ContextMenuStrip { get; set; }

    /// <summary>
    /// Gets or sets the data grid view.
    /// </summary>
    /// <value>
    /// The data grid view.
    /// </value>
    public DataGridView DataGridView { get; set; }

    /// <summary>
    /// Gets the default new row value.
    /// </summary>
    /// <value>
    /// The default new row value.
    /// </value>
    public virtual object DefaultNewRowValue { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is displayed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if displayed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Displayed { get; }

    /// <summary>
    /// Gets the edited formatted value.
    /// </summary>
    /// <value>
    /// The edited formatted value.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object EditedFormattedValue { get; }

    /// <summary>
    /// Gets the type of the edit.
    /// </summary>
    /// <value>
    /// The type of the edit.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Type EditType { get; }

    /// <summary>
    /// Gets the error icon bounds.
    /// </summary>
    /// <value>
    /// The error icon bounds.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Rectangle ErrorIconBounds { get; }

    /// <summary>
    /// Gets or sets the error text.
    /// </summary>
    /// <value>
    /// The error text.
    /// </value>
    public string ErrorText { get; set; }

    /// <summary>
    /// Gets the formatted value.
    /// </summary>
    /// <value>
    /// The formatted value.
    /// </value>
    public object FormattedValue { get; }

    /// <summary>
    /// Gets the type of the formatted value.
    /// </summary>
    /// <value>
    /// The type of the formatted value.
    /// </value>
    public virtual Type FormattedValueType { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is frozen.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if frozen; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Frozen { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has style.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance has style; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HasStyle { get; }

    /// <summary>
    /// Gets the state of the inherited.
    /// </summary>
    /// <value>
    /// The state of the inherited.
    /// </value>
    public DataGridViewElementStates InheritedState { get; }

    /// <summary>
    /// Gets the inherited style.
    /// </summary>
    /// <value>
    /// The inherited style.
    /// </value>
    public DataGridViewCellStyle InheritedStyle { get => RowStyle; }

    /// <summary>
    /// Gets a value indicating whether this instance is in edit mode.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is in edit mode; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsInEditMode { get; }

    /// <summary>
    /// Gets the owning column.
    /// </summary>
    /// <value>
    /// The owning column.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewColumn OwningColumn { get; }

    /// <summary>
    /// Gets the owning row.
    /// </summary>
    /// <value>
    /// The owning row.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewRow OwningRow { get; }

    /// <summary>
    /// Gets the size of the preferred.
    /// </summary>
    /// <value>
    /// The size of the preferred.
    /// </value>
    public Size PreferredSize { get; }

    /// <summary>
    /// Gets or sets a value indicating whether read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool ReadOnly { get; set; }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is resizable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if resizable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Resizable { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Selected { get => _Selected; set { _Selected = value; DataGridView?.GridView.QueueDraw(); } }

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public Size Size { get; }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    [Browsable(true)]
    public DataGridViewCellStyle Style { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    [Bindable(true)]
    [DefaultValue(null)]
    [Localizable(false)]
    [TypeConverter(typeof(StringConverter))]
    public object Tag { get; set; }

    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    /// <value>
    /// The tool tip text.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ToolTipText { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public virtual Type ValueType { get { return _valueType ?? (Value?.GetType()); } set { _valueType = value; } }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewCell" /> is visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Visible { get; }

    internal DataGridViewRow OwningRowInternal { get; set; }

    internal DataGridViewCellStyle RowStyle { get; set; }

}

/// <summary>
/// Displays a check box user interface (UI) to use in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewCheckBoxCell : DataGridViewCell
{
}

/// <summary>
/// Displays a combo box in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewComboBoxCell : DataGridViewCell
{
}

/// <summary>
/// Displays a graphic in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
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

/// <summary>
/// Class DataGridViewRadioCell. Implements the
/// <see cref="T:System.Windows.Forms.DataGridViewCell" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.DataGridViewCell" />
public class DataGridViewRadioCell : DataGridViewCell
{
}

/// <summary>
/// Displays editable text information in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewTextBoxCell : DataGridViewCell
{
    //public DataGridViewTextBoxCell(DataGridViewRow dataGridViewRow) {

    //}
}