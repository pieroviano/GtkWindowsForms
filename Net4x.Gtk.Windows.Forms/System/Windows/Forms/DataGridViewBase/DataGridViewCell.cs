using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents an individual cell in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public abstract class DataGridViewCell
    {

        internal DataGridViewRow OwningRowInternal { get; set; }

        public DataGridView DataGridView { get; set; }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> class.
        /// </summary>
        protected DataGridViewCell() { }
        /// <summary>
        /// Gets or sets the value associated with this cell.
        /// </summary>
        /// <returns>Gets or sets the data to be displayed by the cell. The default is null.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <see cref="System.Windows.Forms.DataGridViewCell.RowIndex" /> is outside the valid range of 0 to the number of rows in the control minus 1.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the ToolTip text associated with this cell.
        /// </summary>
        /// <returns>The ToolTip text associated with the cell. The default is
        /// <see cref="System.String.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ToolTipText { get; set; }
        /// <summary>
        /// Gets or sets the object that contains supplemental data about the cell.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the cell. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Bindable(true)]
        [DefaultValue(null)]
        [Localizable(false)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }
        /// <summary>
        /// Gets or sets the style for the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(true)]
        public DataGridViewCellStyle Style { get; set; }
        /// <summary>
        /// Gets the size of the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> set to the owning row's height and the owning column's width.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public Size Size { get; }

        private bool _Selected;
        /// <summary>
        /// Gets or sets a value indicating whether the cell has been selected.
        /// </summary>
        /// <returns>true if the cell has been selected; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">There is no associated
        /// <see cref="System.Windows.Forms.DataGridView" /> when setting this property. -or-The owning row is shared when setting this property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected { get => _Selected; set { _Selected = value; DataGridView?.GridView.QueueDraw(); } }
        /// <summary>
        /// Gets the index of the cell's parent row.
        /// </summary>
        /// <returns>The index of the row that contains the cell; -1 if there is no owning row.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether the cell can be resized.
        /// </summary>
        /// <returns>true if the cell can be resized; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Resizable { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the cell's data can be edited.
        /// </summary>
        /// <returns>true if the cell's data cannot be edited; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">There is no owning row when setting this property. -or-The owning row is shared when setting this property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ReadOnly { get; set; }
        /// <summary>
        /// Gets the size, in pixels, of a rectangular area into which the cell can fit.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> containing the height and width, in pixels.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Size PreferredSize { get; }
        /// <summary>
        /// Gets the row that contains this cell.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that contains the cell, or null if the cell is not in a row.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataGridViewRow OwningRow { get; }
        /// <summary>
        /// Gets the column that contains this cell.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> that contains the cell, or null if the cell is not in a column.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataGridViewColumn OwningColumn { get; }
        /// <summary>
        /// Gets a value indicating whether this cell is currently being edited.
        /// </summary>
        /// <returns>true if the cell is in edit mode; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row containing the cell is a shared row.
        /// </exception>
        public bool IsInEditMode { get; }

        internal DataGridViewCellStyle RowStyle { get; set; }
        /// <summary>
        /// Gets the style currently applied to the cell.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> currently applied to the cell.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The cell does not belong to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        public DataGridViewCellStyle InheritedStyle { get => RowStyle; }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridViewCell.DataGridViewCellAccessibleObject" /> assigned to the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCell.DataGridViewCellAccessibleObject" /> assigned to the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public AccessibleObject AccessibilityObject { get; }
        /// <summary>
        /// Gets the column index for this cell.
        /// </summary>
        /// <returns>The index of the column that contains the cell; -1 if the cell is not contained within a column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; internal set; }
        /// <summary>
        /// Gets the bounding rectangle that encloses the cell's content area.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Rectangle" /> that bounds the cell's contents.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The value of the
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> property is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Rectangle ContentBounds { get; }
        /// <summary>
        /// Gets or sets the shortcut menu associated with the cell.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> associated with the cell.
        /// </returns>
        [DefaultValue(null)]
        public virtual ContextMenuStrip ContextMenuStrip { get; set; }
        /// <summary>
        /// Gets the default value for a cell in the row for new records.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> representing the default value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual object DefaultNewRowValue { get; }
        /// <summary>
        /// Gets a value that indicates whether the cell is currently displayed on-screen.
        /// </summary>
        /// <returns>true if the cell is on-screen or partially on-screen; otherwise, false.
        /// </returns>
        public virtual bool Displayed { get; }
        /// <summary>
        /// Gets the current, formatted value of the cell, regardless of whether the cell is in edit mode and the value has not been committed.
        /// </summary>
        /// <returns>The current, formatted value of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <exception cref="System.Exception">Formatting failed and either there is no handler for the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event of the
        /// <see cref="System.Windows.Forms.DataGridView" /> control or the handler set the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.ThrowException" /> property to true. The exception object can typically be cast to type
        /// <see cref="System.FormatException" />
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object EditedFormattedValue { get; }
        /// <summary>
        /// Gets the type of the cell's hosted editing control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> representing the
        /// <see cref="System.Windows.Forms.DataGridViewTextBoxEditingControl" /> type.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual Type EditType { get; }
        /// <summary>
        /// Gets the bounds of the error icon for the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the error icon for the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The cell does not belong to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Rectangle ErrorIconBounds { get; }
        /// <summary>
        /// Gets or sets the text describing an error condition associated with the cell.
        /// </summary>
        /// <returns>The text that describes an error condition associated with the cell.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ErrorText { get; set; }
        /// <summary>
        /// Gets the value of the cell as formatted for display.
        /// </summary>
        /// <returns>The formatted value of the cell or null if the cell does not belong to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCell.ColumnIndex" /> is less than 0, indicating that the cell is a row header cell.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.
        /// </exception>
        /// <exception cref="System.Exception">Formatting failed and either there is no handler for the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event of the
        /// <see cref="System.Windows.Forms.DataGridView" /> control or the handler set the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.ThrowException" /> property to true. The exception object can typically be cast to type
        /// <see cref="System.FormatException" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public object FormattedValue { get; }
        /// <summary>
        /// Gets the type of the formatted value associated with the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> representing the type of the cell's formatted value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Type FormattedValueType { get; }
        /// <summary>
        /// Gets a value indicating whether the cell is frozen.
        /// </summary>
        /// <returns>true if the cell is frozen; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Frozen { get; }
        /// <summary>
        /// Gets a value indicating whether the cell is in a row or column that has been hidden.
        /// </summary>
        /// <returns>true if the cell is visible; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Visible { get; }
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.DataGridViewCell.Style" /> property has been set.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.DataGridViewCell.Style" /> property has been set; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool HasStyle { get; }
        /// <summary>
        /// Gets the current state of the cell as inherited from the state of its row and column.
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values representing the current state of the cell.
        /// </returns>
        /// <exception cref="System.ArgumentException">The cell is not contained within a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and the value of its
        /// <see cref="System.Windows.Forms.DataGridViewCell.RowIndex" /> property is not -1.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The cell is contained within a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and the value of its
        /// <see cref="System.Windows.Forms.DataGridViewCell.RowIndex" /> property is -1.
        /// </exception>
        public DataGridViewElementStates InheritedState { get; }

        private Type _valueType;
        /// <summary>
        /// Gets or sets the data type of the values in the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> representing the data type of the value in the cell.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Type ValueType { get { return _valueType == null ? Value?.GetType() : _valueType; } set { _valueType = value; } }

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
    public class DataGridViewRadioCell : DataGridViewCell
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
}
