using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Implements one row in a
/// <see cref="T:System.Windows.Forms.PropertyGrid" />
/// </summary>
public abstract class GridItem
{
    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GridItem" /> is expandable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if expandable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Expandable => false;

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GridItem" /> is expanded.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if expanded; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Expanded
    {
        get;

        set;
    }

    /// <summary>
    /// Gets the grid items.
    /// </summary>
    /// <value>
    /// The grid items.
    /// </value>
    public abstract GridItemCollection GridItems { get; }

    /// <summary>
    /// Gets the type of the grid item.
    /// </summary>
    /// <value>
    /// The type of the grid item.
    /// </value>
    public abstract GridItemType GridItemType { get; }

    /// <summary>
    /// Gets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    public abstract string Label { get; }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    public abstract GridItem Parent { get; }

    /// <summary>
    /// Gets the property descriptor.
    /// </summary>
    /// <value>
    /// The property descriptor.
    /// </value>
    public abstract PropertyDescriptor PropertyDescriptor { get; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    [Localizable(false)]
    [Bindable(true)]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public abstract object Value { get; }

    /// <summary>
    /// Selects this instance.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    public abstract bool Select();

}