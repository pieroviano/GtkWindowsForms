namespace System.Windows.Forms
{
    /// <summary>
    /// Contains information that enables a
    /// <see cref="T:System.Windows.Forms.Binding" />
    /// to resolve a data binding to either the property of an object or the property of the current object in a list of objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public readonly struct BindingMemberInfo : IEquatable<BindingMemberInfo>
    {

        private readonly string _dataField;

        private readonly string _dataList;

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

        public string BindingField => _dataField ?? string.Empty;

        public string BindingMember => BindingPath.Length > 0
            ? $"{BindingPath}.{BindingField}"
            : BindingField;

        public string BindingPath => _dataList ?? string.Empty;

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

        public override int GetHashCode() => base.GetHashCode();

    }
}
