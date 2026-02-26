using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a common implementation of members for the
    /// <see cref="System.Windows.Forms.ListBox" /> and
    /// <see cref="System.Windows.Forms.ComboBox" /> classes.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    public abstract class ListControl : ScrollableControl
    {

        protected new EventHandlerList Events = new EventHandlerList();
        /// <summary>
        /// Gets or sets the data source for this
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </summary>
        /// <returns>An object that implements the
        /// <see cref="System.Collections.IList" /> or
        /// <see cref="System.ComponentModel.IListSource" /> interfaces, such as a
        /// <see cref="System.Data.DataSet" /> or an
        /// <see cref="System.Array" /> The default is null.
        /// </returns>
        /// <exception cref="System.ArgumentException">The assigned value does not implement the
        /// <see cref="System.Collections.IList" /> or
        /// <see cref="System.ComponentModel.IListSource" /> interfaces.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        public virtual object DataSource
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the property to display for this
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> specifying the name of an object property that is contained in the collection specified by the
        /// <see cref="System.Windows.Forms.ListControl.DataSource" /> property. The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue("")]
        public virtual string DisplayMember
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.IFormatProvider" /> that provides custom formatting behavior.
        /// </summary>
        /// <returns>The
        /// <see cref="System.IFormatProvider" /> implementation that provides custom formatting behavior.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DefaultValue(null)]
        public virtual IFormatProvider FormatInfo
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the format-specifier characters that indicate how a value is to be displayed.
        /// </summary>
        /// <returns>The string of format-specifier characters that indicates how a value is to be displayed.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue("")]
        [MergableProperty(false)]
        public virtual string FormatString
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether formatting is applied to the
        /// <see cref="System.Windows.Forms.ListControl.DisplayMember" /> property of the
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </summary>
        /// <returns>true if formatting of the
        /// <see cref="System.Windows.Forms.ListControl.DisplayMember" /> property is enabled; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool FormattingEnabled
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the property to use as the actual value for the items in the
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> representing the name of an object property that is contained in the collection specified by the
        /// <see cref="System.Windows.Forms.ListControl.DataSource" /> property. The default is an empty string ("").
        /// </returns>
        /// <exception cref="System.ArgumentException">The specified property cannot be found on the object specified by the
        /// <see cref="System.Windows.Forms.ListControl.DataSource" /> property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue("")]
        public virtual string ValueMember
        {
            get; set;
        }
        /// <summary>
        /// When overridden in a derived class, gets or sets the zero-based index of the currently selected item.
        /// </summary>
        /// <returns>A zero-based index of the currently selected item. A value of negative one (-1) is returned if no item is selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract int SelectedIndex { get; set; }
        /// <summary>
        /// Gets or sets the value of the member property specified by the
        /// <see cref="System.Windows.Forms.ListControl.ValueMember" /> property.
        /// </summary>
        /// <returns>An object containing the value of the member of the data source specified by the
        /// <see cref="System.Windows.Forms.ListControl.ValueMember" /> property.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The assigned value is null or the empty string ("").
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(null)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(true)]
        public virtual object SelectedValue
        {
            get; set;
        }

        public event EventHandler DataSourceChanged
        {
            add { Events.AddHandler("DataSourceChanged", value); }
            remove { Events.RemoveHandler("DataSourceChanged", value); }
        }

        public event EventHandler DisplayMemberChanged;


        public event ListControlConvertEventHandler Format;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler FormatInfoChanged;

        public event EventHandler FormatStringChanged;

        public event EventHandler FormattingEnabledChanged;


        public event EventHandler ValueMemberChanged;
        public event EventHandler SelectedItemChanged
        {
            add { Events.AddHandler("SelectedItemChanged", value); }
            remove { Events.RemoveHandler("SelectedItemChanged", value); }
        }
        public event EventHandler SelectedValueChanged
        {
            add { Events.AddHandler("SelectedValueChanged", value); }
            remove { Events.RemoveHandler("SelectedValueChanged", value); }
        }
        public event EventHandler SelectedIndexChanged
        {
            add { Events.AddHandler("SelectedIndexChanged", value); }
            remove { Events.RemoveHandler("SelectedIndexChanged", value); }
        }
        /// <summary>
        /// Returns the text representation of the specified item.
        /// </summary>
        /// <returns>If the
        /// <see cref="System.Windows.Forms.ListControl.DisplayMember" /> property is not specified, the value returned by
        /// <see cref="System.Windows.Forms.ListControl.GetItemText(System.Object)" /> is the value of the item's ToString method. Otherwise, the method returns the string value of the member specified in the
        /// <see cref="System.Windows.Forms.ListControl.DisplayMember" /> property for the object specified in the
        /// <paramref name="item" /> parameter.
        /// </returns>
        /// <param name="item">The object from which to get the contents to display.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string GetItemText(object item)
        {
            return item?.ToString();
        }

    }
}
