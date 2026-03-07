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

        public virtual bool Expandable => false;

        public virtual bool Expanded
        {
            get;
            set;
        }

        public abstract GridItemCollection GridItems { get; }

        public abstract GridItemType GridItemType { get; }

        public abstract string Label { get; }

        public abstract GridItem Parent { get; }

        public abstract PropertyDescriptor PropertyDescriptor { get; }

        [Localizable(false)]
        [Bindable(true)]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        public abstract object Value { get; }

        public abstract bool Select();
    }
}
