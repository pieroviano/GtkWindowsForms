using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides a common implementation of members for the
/// <see cref="T:System.Windows.Forms.ListBox" /> and
/// <see cref="T:System.Windows.Forms.ComboBox" /> classes.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
[LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
public abstract class ListControl : ScrollableControl
{
    /// <summary>
    /// The events
    /// </summary>
    protected new EventHandlerList Events = new EventHandlerList();

    /// <summary>
    /// Occurs when the
    /// <see cref="T:System.Windows.Forms.ListControl.DisplayMember" /> property changes.
    /// </summary>
    public virtual event EventHandler DisplayMemberChanged;

    /// <summary>
    /// Occurs when formatting of list items is required.
    /// </summary>
    public virtual event ListControlConvertEventHandler Format;

    /// <summary>
    /// Occurs when the formatting provider information changes.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual event EventHandler FormatInfoChanged;

    /// <summary>
    /// Occurs when the
    /// <see cref="T:System.Windows.Forms.ListControl.FormatString" /> property changes.
    /// </summary>
    public virtual event EventHandler FormatStringChanged;

    /// <summary>
    /// Occurs when the
    /// <see cref="T:System.Windows.Forms.ListControl.FormattingEnabled" /> property changes.
    /// </summary>
    public virtual event EventHandler FormattingEnabledChanged;

    /// <summary>
    /// Occurs when the
    /// <see cref="T:System.Windows.Forms.ListControl.ValueMember" /> property changes.
    /// </summary>
    public virtual event EventHandler ValueMemberChanged;

    /// <summary>
    /// Occurs when the data source for the control changes.
    /// Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public virtual event EventHandler DataSourceChanged
    {
        add { Events.AddHandler("DataSourceChanged", value); }
        remove { Events.RemoveHandler("DataSourceChanged", value); }
    }

    /// <summary>
    /// Occurs when the selected index changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public virtual event EventHandler SelectedIndexChanged
    {
        add { Events.AddHandler("SelectedIndexChanged", value); }
        remove { Events.RemoveHandler("SelectedIndexChanged", value); }
    }

    /// <summary>
    /// Occurs when the selected item changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public virtual event EventHandler SelectedItemChanged
    {
        add { Events.AddHandler("SelectedItemChanged", value); }
        remove { Events.RemoveHandler("SelectedItemChanged", value); }
    }

    /// <summary>
    /// Occurs when the selected value changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public virtual event EventHandler SelectedValueChanged
    {
        add { Events.AddHandler("SelectedValueChanged", value); }
        remove { Events.RemoveHandler("SelectedValueChanged", value); }
    }

    /// <summary>
    /// Gets or sets the data source for the control. The data source can be any object that implements IList or IListSource.
    /// </summary>
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    public virtual object DataSource
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the property to display for each item in the control.
    /// </summary>
    [DefaultValue("")]
    public virtual string DisplayMember
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the format provider used for formatting list items.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DefaultValue(null)]
    public virtual IFormatProvider FormatInfo
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the format string used to format the item text.
    /// </summary>
    [DefaultValue("")]
    [MergableProperty(false)]
    public virtual string FormatString
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether formatting is enabled for items.
    /// </summary>
    [DefaultValue(false)]
    public virtual bool FormattingEnabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the currently selected item.
    /// Derived classes must implement selection indexing.
    /// </summary>
    public abstract int SelectedIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value of the selected item, as determined by the
    /// <see cref="T:System.Windows.Forms.ListControl.ValueMember" />.
    /// </summary>
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Bindable(true)]
    public virtual object SelectedValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the property to use as the actual value for the items in the control.
    /// </summary>
    [DefaultValue("")]
    public virtual string ValueMember
    {
        get;
        set;
    }

    /// <summary>
    /// Returns the textual representation for the given item. By default this calls
    /// <see cref="T:System.Object.ToString" /> on the item.
    /// Override to provide custom behavior.
    /// </summary>
    /// <param name="item">
    /// Item to obtain text for.
    /// </param>
    /// <returns>
    /// Display text for the item.
    /// </returns>
    public virtual string GetItemText(object item)
    {
        return item?.ToString();
    }

    /// <summary>
    /// Raises the DisplayMemberChanged event.
    /// </summary>
    protected virtual void OnDisplayMemberChanged(System.EventArgs e) { DisplayMemberChanged?.Invoke(this, e); }

    /// <summary>
    /// Raises the Format event.
    /// </summary>
    protected virtual void OnFormat(System.Windows.Forms.ListControlConvertEventArgs e) { Format?.Invoke(this, e); }

    /// <summary>
    /// Raises the FormatInfoChanged event.
    /// </summary>
    protected virtual void OnFormatInfoChanged(System.EventArgs e) { FormatInfoChanged?.Invoke(this, e); }

    /// <summary>
    /// Raises the FormatStringChanged event.
    /// </summary>
    protected virtual void OnFormatStringChanged(System.EventArgs e) { FormatStringChanged?.Invoke(this, e); }

    /// <summary>
    /// Raises the FormattingEnabledChanged event.
    /// </summary>
    protected virtual void OnFormattingEnabledChanged(System.EventArgs e) { FormattingEnabledChanged?.Invoke(this, e); }

    /// <summary>
    /// Raises the ValueMemberChanged event.
    /// </summary>
    protected virtual void OnValueMemberChanged(System.EventArgs e) { ValueMemberChanged?.Invoke(this, e); }

}