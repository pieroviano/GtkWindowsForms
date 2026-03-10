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
    /// <see cref="T:System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellStyle : ICloneable
    {

        private DataGridViewCellStyleScopes scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewCellStyle"/> class.
        /// </summary>
        public DataGridViewCellStyle() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewCellStyle"/> class.
        /// </summary>
        /// <param name="dataGridViewCellStyle">The data grid view cell style.</param>
        public DataGridViewCellStyle(DataGridViewCellStyle dataGridViewCellStyle) { }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>
        /// The alignment.
        /// </value>
        public DataGridViewContentAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the data source null value.
        /// </summary>
        /// <value>
        /// The data source null value.
        /// </value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object DataSourceNullValue { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>
        /// The color of the fore.
        /// </value>
        public Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the format provider.
        /// </summary>
        /// <value>
        /// The format provider.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is data source null value default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is data source null value default; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsDataSourceNullValueDefault { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is format provider default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is format provider default; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsFormatProviderDefault { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is null value default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null value default; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsNullValueDefault { get; }

        /// <summary>
        /// Gets or sets the null value.
        /// </summary>
        /// <value>
        /// The null value.
        /// </value>
        [DefaultValue("")]
        [TypeConverter(typeof(StringConverter))]
        public object NullValue { get; set; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        public Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the color of the selection back.
        /// </summary>
        /// <value>
        /// The color of the selection back.
        /// </value>
        public Color SelectionBackColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the selection fore.
        /// </summary>
        /// <value>
        /// The color of the selection fore.
        /// </value>
        public Color SelectionForeColor { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag { get; set; }


        /// <summary>
        /// Gets or sets the wrap mode.
        /// </summary>
        /// <value>
        /// The wrap mode.
        /// </value>
        [DefaultValue(DataGridViewTriState.NotSet)]
        public DataGridViewTriState WrapMode { get; set; }

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

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="dataGridViewCellStyle">The data grid view cell style.</param>
        public virtual void ApplyStyle(DataGridViewCellStyle dataGridViewCellStyle) { }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
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
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object o) { return false; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "DataGridViewCellStyle";
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

    }
}
