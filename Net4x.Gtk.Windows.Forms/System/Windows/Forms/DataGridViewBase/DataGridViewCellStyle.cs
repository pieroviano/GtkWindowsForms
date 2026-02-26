using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the formatting and style information applied to individual cells within a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellStyle : ICloneable
    {

        private DataGridViewCellStyleScopes scope;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> class using default property values.
        /// </summary>
        public DataGridViewCellStyle() { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> class using the property values of the specified
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <param name="dataGridViewCellStyle">The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> used as a template to provide initial property values.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewCellStyle" /> is null.
        /// </exception>
        public DataGridViewCellStyle(DataGridViewCellStyle dataGridViewCellStyle) { }
        /// <summary>
        /// Gets or sets the foreground color used by a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell when it is selected.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of a selected cell. The default is
        /// <see cref="System.Drawing.Color.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color SelectionForeColor { get; set; }
        /// <summary>
        /// Gets or sets the background color used by a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell when it is selected.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the background color of a selected cell. The default is
        /// <see cref="System.Drawing.Color.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color SelectionBackColor { get; set; }
        /// <summary>
        /// Gets or sets the space between the edge of a
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> and its content.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> that represents the space between the edge of a
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> and its content.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Padding Padding { get; set; }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.DataGridView" /> cell display value corresponding to a cell value of
        /// <see cref="System.DBNull.Value" /> or null.
        /// </summary>
        /// <returns>The object used to indicate a null value in a cell. The default is
        /// <see cref="System.String.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue("")]
        [TypeConverter(typeof(StringConverter))]
        public object NullValue { get; set; }
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.NullValue" /> property has been set.
        /// </summary>
        /// <returns>true if the value of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.NullValue" /> property is the default value; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsNullValueDefault { get; }
        /// <summary>
        /// Gets a value that indicates whether the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.FormatProvider" /> property has been set.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.FormatProvider" /> property is the default value; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsFormatProviderDefault { get; }
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.DataSourceNullValue" /> property has been set.
        /// </summary>
        /// <returns>true if the value of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.DataSourceNullValue" /> property is the default value; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsDataSourceNullValueDefault { get; }
        /// <summary>
        /// Gets or sets the object used to provide culture-specific formatting of
        /// <see cref="System.Windows.Forms.DataGridView" /> cell values.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IFormatProvider" /> used for cell formatting. The default is
        /// <see cref="System.Globalization.CultureInfo.CurrentUICulture" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IFormatProvider FormatProvider { get; set; }
        /// <summary>
        /// Gets or sets the format string applied to the textual content of a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell.
        /// </summary>
        /// <returns>A string that indicates the format of the cell value. The default is
        /// <see cref="System.String.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Format { get; set; }
        /// <summary>
        /// Gets or sets the font applied to the textual content of a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Font" /> applied to the cell text. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Font Font { get; set; }
        /// <summary>
        /// Gets or sets the foreground color of a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of a cell. The default is
        /// <see cref="System.Drawing.Color.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color ForeColor { get; set; }
        /// <summary>
        /// Gets or sets the value saved to the data source when the user enters a null value into a cell.
        /// </summary>
        /// <returns>The value saved to the data source when the user specifies a null cell value. The default is
        /// <see cref="System.DBNull.Value" />
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object DataSourceNullValue { get; set; }
        /// <summary>
        /// Gets or sets the background color of a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the background color of a cell. The default is
        /// <see cref="System.Drawing.Color.Empty" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color BackColor { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the position of the cell content within a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewContentAlignment" /> values. The default is
        /// <see cref="System.Windows.Forms.DataGridViewContentAlignment.NotSet" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property value is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewContentAlignment" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewContentAlignment Alignment { get; set; }
        /// <summary>
        /// Gets or sets an object that contains additional data related to the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <returns>An object that contains additional data. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether textual content in a
        /// <see cref="System.Windows.Forms.DataGridView" /> cell is wrapped to subsequent lines or truncated when it is too long to fit on a single line.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewTriState" /> values. The default is
        /// <see cref="System.Windows.Forms.DataGridViewTriState.NotSet" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property value is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewTriState" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(DataGridViewTriState.NotSet)]
        public DataGridViewTriState WrapMode { get; set; }
        /// <summary>
        /// Applies the specified
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to the current
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <param name="dataGridViewCellStyle">The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to apply to the current
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewCellStyle" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ApplyStyle(DataGridViewCellStyle dataGridViewCellStyle) { }
        /// <summary>
        /// Creates an exact copy of this
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents an exact copy of this cell style.
        /// </returns>
        public virtual DataGridViewCellStyle Clone()
        {
            Type celltype = this.GetType();
            DataGridViewCellStyle newobj = new DataGridViewCellStyle();
            var propertys = celltype.GetProperties(Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (PropertyInfo property in propertys)
                if (property.CanRead && property.CanWrite)
                    property.SetValue(newobj, property.GetValue(this));
            return newobj;
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equivalent to the specified object.
        /// </summary>
        /// <returns>true if
        /// <paramref name="o" /> is a
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> and has the same property values as this instance; otherwise, false.
        /// </returns>
        /// <param name="o">An object to compare with this instance, or null.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool Equals(object o) { return false; }
        /// <returns>
        /// A hash code for the current
        /// <see cref="System.Object" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override int GetHashCode()
        {
            return 0;
        }
        /// <summary>
        /// Returns a string indicating the current property settings of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <returns>A string indicating the current property settings of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override string ToString()
        {
            return "DataGridViewCellStyle";
        }
        /// <summary>
        /// Creates an exact copy of this
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents an exact copy of this cell style.
        /// </returns>
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        internal DataGridViewCellStyleScopes Scope
        {
            get
            {
                return scope;
            }
            set
            {
                scope = value;
            }
        }
    }
}
