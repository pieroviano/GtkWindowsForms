namespace System.Windows.Forms
{
    /// <summary>
    /// Contains information that enables a
    /// <see cref="System.Windows.Forms.Binding" />
    /// to resolve a data binding to either the property of an object or the property of the current object in a list of objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public readonly struct BindingMemberInfo : IEquatable<BindingMemberInfo>
    {
        private readonly string _dataList;

        private readonly string _dataField;
        /// <summary>
        /// Gets the property name, or the period-delimited hierarchy of property names, that comes before the property name of the data-bound object.
        /// </summary>
        /// <returns>The property name, or the period-delimited hierarchy of property names, that comes before the data-bound object property name.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string BindingPath => _dataList ?? string.Empty;
        /// <summary>
        /// Gets the property name of the data-bound object.
        /// </summary>
        /// <returns>The property name of the data-bound object. This can be an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string BindingField => _dataField ?? string.Empty;
        /// <summary>
        /// Gets the information that is used to specify the property name of the data-bound object.
        /// </summary>
        /// <returns>An empty string (""), a single property name, or a hierarchy of period-delimited property names that resolves to the property name of the final data-bound object.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string BindingMember => BindingPath.Length > 0
                    ? $"{BindingPath}.{BindingField}"
                    : BindingField;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingMemberInfo" /> class.
        /// </summary>
        /// <param name="dataMember">A navigation path that resolves to either the property of an object or the property of the current object in a list of objects.
        /// </param>
        public BindingMemberInfo(string dataMember)
        {
            dataMember ??= string.Empty;
            int lastDot = dataMember.LastIndexOf('.');
            if (lastDot != -1)
            {
                _dataList = dataMember.Substring(0, lastDot);
                _dataField = dataMember.Substring(lastDot + 1);
            }
            else
            {
                _dataList = string.Empty;
                _dataField = dataMember;
            }
        }
        /// <summary>
        /// Determines whether the specified object is equal to this
        /// <see cref="System.Windows.Forms.BindingMemberInfo" />
        /// </summary>
        /// <returns>true if
        /// <paramref name="otherObject" /> is a
        /// <see cref="System.Windows.Forms.BindingMemberInfo" /> and both
        /// <see cref="System.Windows.Forms.BindingMemberInfo.BindingMember" /> strings are equal; otherwise false.
        /// </returns>
        /// <param name="otherObject">The object to compare for equality.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool Equals(object otherObject)
        {
            if (otherObject is BindingMemberInfo otherMember)
            {
                return Equals(otherMember);

            }
            return false;
        }

        public bool Equals(BindingMemberInfo other)
            => string.Equals(BindingMember, other.BindingMember, StringComparison.OrdinalIgnoreCase);

        public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b) => a.Equals(b);

        public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b) => !a.Equals(b);
        /// <summary>
        /// Returns the hash code for this
        /// <see cref="System.Windows.Forms.BindingMemberInfo" />
        /// </summary>
        /// <returns>The hash code for this
        /// <see cref="System.Windows.Forms.BindingMemberInfo" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override int GetHashCode() => base.GetHashCode();
    }
}
