using Gtk;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a row in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewRow
    {

        public int Index { get; internal set; }

        public TreeIter TreeIter { get; internal set; }

        public DataGridView DataGridView { get; set; }

        private DataGridViewCellCollection _cell;

        private DataGridViewRowCollection _children;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> class without using a template.
        /// </summary>
        public DataGridViewRow()
        {
            _cell = new DataGridViewCellCollection(this);
        }
        /// <summary>
        /// Gets the collection of cells that populate the row.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> that contains all of the cells in the row.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCellCollection Cells { get { return _cell; } }

        public DataGridViewRowCollection Children { get => _children ?? new DataGridViewRowCollection(DataGridView, this); }
        /// <summary>
        /// Gets the data-bound object that populated the row.
        /// </summary>
        /// <returns>The data-bound
        /// <see cref="System.Object" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object DataBoundItem { get; }
        /// <summary>
        /// Gets or sets the default styles for the row, which are used to render cells in the row unless the styles are overridden.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to be applied as the default style.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When setting this property, the row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle DefaultCellStyle { get; set; }
        /// <summary>
        /// Gets a value indicating whether this row is displayed on the screen.
        /// </summary>
        /// <returns>true if the row is currently displayed on the screen; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public bool Displayed { get; }
        /// <summary>
        /// Gets or sets the height, in pixels, of the row divider.
        /// </summary>
        /// <returns>The height, in pixels, of the divider (the row's bottom margin).
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When setting this property, the row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public int DividerHeight { get; set; }
        /// <summary>
        /// Gets or sets the error message text for row-level errors.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> containing the error message.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When getting the value of this property, the row is a shared row in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public string ErrorText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the row is frozen.
        /// </summary>
        /// <returns>true if the row is frozen; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Frozen { get; set; }
        /// <summary>
        /// Gets or sets the row's header cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewRowHeaderCell" /> that represents the header cell of row.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewRowHeaderCell HeaderCell { get; set; }
        /// <summary>
        /// Gets or sets the current height of the row.
        /// </summary>
        /// <returns>The height, in pixels, of the row. The default is the height of the default font plus 9 pixels.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When setting this property, the row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Height { get; set; } = 28;
        /// <summary>
        /// Gets the cell style in effect for the row.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that specifies the formatting and style information for the cells in the row.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public DataGridViewCellStyle InheritedStyle { get; }
        /// <summary>
        /// Gets a value indicating whether the row is the row for new records.
        /// </summary>
        /// <returns>true if the row is the last row in the
        /// <see cref="System.Windows.Forms.DataGridView" />, which is used for the entry of a new row of data; otherwise, false.
        /// </returns>
        public bool IsNewRow { get; }
        /// <summary>
        /// Gets or sets the minimum height of the row.
        /// </summary>
        /// <returns>The minimum row height in pixels, ranging from 2 to
        /// <see cref="System.Int32.MaxValue" /> The default is 3.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When setting this property, the row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value when setting this property is less than 2.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int MinimumHeight { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the row is read-only.
        /// </summary>
        /// <returns>true if the row is read-only; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether users can resize the row or indicating that the behavior is inherited from the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToResizeRows" /> property.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewTriState" /> value that indicates whether the row can be resized or whether it can be resized only when the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToResizeRows" /> property is set to true.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public DataGridViewTriState Resizable { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the row is selected.
        /// </summary>
        /// <returns>true if the row is selected; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public bool Selected { get => DataGridView.NativeRowGetSelected(Index); set => DataGridView.NativeRowSetSelected(Index, value); }
        /// <summary>
        /// Gets the current state of the row.
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the row state.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        //public bool Selected { get; set; }
        public DataGridViewElementStates State { get { return DataGridViewElementStates.None; } }
        /// <summary>
        /// Gets or sets the shortcut menu for the row.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> associated with the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> The default is null.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When getting the value of this property, the row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        public ContextMenuStrip ContextMenuStrip { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the row is visible.
        /// </summary>
        /// <returns>true if the row is visible; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The row is in a
        /// <see cref="System.Windows.Forms.DataGridView" /> control and is a shared row.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Visible { get; set; }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridViewRow.DataGridViewRowAccessibleObject" /> assigned to the
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewRow.DataGridViewRowAccessibleObject" /> assigned to the
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public AccessibleObject AccessibilityObject { get; }

        public int GetHeight(int index)
        {
            return Height;
        }
    }
}
