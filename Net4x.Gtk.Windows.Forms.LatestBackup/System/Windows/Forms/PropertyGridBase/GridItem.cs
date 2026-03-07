using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements one row in a
    /// <see cref="System.Windows.Forms.PropertyGrid" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public abstract class GridItem
    {
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the specified property is expandable to show nested properties.
        /// </summary>
        /// <returns>true if the specified property can be expanded; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Expandable => false;

        /// <summary>
        /// When overridden in a derived class, gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.GridItem" /> is in an expanded state.
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The
        /// <see cref="System.Windows.Forms.GridItem.Expanded" /> property was set to true, but a
        /// <see cref="System.Windows.Forms.GridItem" /> is not expandable.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Expanded
        {
            get;
            set;
        }

        /// <summary>
        /// When overridden in a derived class, gets the collection of
        /// <see cref="System.Windows.Forms.GridItem" /> objects, if any, associated as a child of this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.GridItemCollection" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract GridItemCollection GridItems { get; }

        /// <summary>
        /// When overridden in a derived class, gets the type of this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.GridItemType" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract GridItemType GridItemType { get; }

        /// <summary>
        /// When overridden in a derived class, gets the text of this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> representing the text associated with this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract string Label { get; }

        /// <summary>
        /// When overridden in a derived class, gets the parent
        /// <see cref="System.Windows.Forms.GridItem" /> of this
        /// <see cref="System.Windows.Forms.GridItem" />, if any.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.GridItem" /> representing the parent of the
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract GridItem Parent { get; }

        /// <summary>
        /// When overridden in a derived class, gets the
        /// <see cref="System.ComponentModel.PropertyDescriptor" /> that is associated with this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.ComponentModel.PropertyDescriptor" /> associated with this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract PropertyDescriptor PropertyDescriptor { get; }

        /// <summary>
        /// Gets or sets user-defined data about the
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Localizable(false)]
        [Bindable(true)]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        /// When overridden in a derived class, gets the current value of this
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>The current value of this
        /// <see cref="System.Windows.Forms.GridItem" /> This can be null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract object Value { get; }

        /// <summary>
        /// When overridden in a derived class, selects this
        /// <see cref="System.Windows.Forms.GridItem" /> in the
        /// <see cref="System.Windows.Forms.PropertyGrid" />
        /// </summary>
        /// <returns>true if the selection is successful; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract bool Select();

    }
}
