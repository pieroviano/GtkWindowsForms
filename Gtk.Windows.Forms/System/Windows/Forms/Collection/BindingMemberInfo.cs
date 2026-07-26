namespace System.Windows.Forms;

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

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingMemberInfo" /> struct.
    /// </summary>
    /// <param name="dataMember">
    /// The data member.
    /// </param>
    public BindingMemberInfo(string? dataMember)
    {
        dataMember ??= string.Empty;
        var lastDot = dataMember.LastIndexOf('.');
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
    /// Gets the binding field.
    /// </summary>
    /// <value>
    /// The binding field.
    /// </value>
    public string BindingField => _dataField ?? string.Empty;

    /// <summary>
    /// Gets the binding member.
    /// </summary>
    /// <value>
    /// The binding member.
    /// </value>
    public string BindingMember => BindingPath.Length > 0
                ? $"{BindingPath}.{BindingField}"
                : BindingField;

    /// <summary>
    /// Gets the binding path.
    /// </summary>
    /// <value>
    /// The binding path.
    /// </value>
    public string BindingPath => _dataList ?? string.Empty;

    /// <summary>
    /// Determines whether the specified
    /// <see cref="T:System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="otherObject">
    /// The
    /// <see cref="T:System.Object" /> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the specified
    /// <see cref="T:System.Object" /> is equal to this instance; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public override bool Equals(object otherObject)
    {
        if (otherObject is BindingMemberInfo otherMember)
        {
            return Equals(otherMember);

        }

        return false;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    /// An object to compare with this object.
    /// </param>
    /// <returns>
    /// true if the current object is equal to the
    /// <paramref name="other">
    /// other
    /// </paramref> parameter; otherwise, false.
    /// </returns>
    public bool Equals(BindingMemberInfo other)
                => string.Equals(BindingMember, other.BindingMember, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => base.GetHashCode();

}