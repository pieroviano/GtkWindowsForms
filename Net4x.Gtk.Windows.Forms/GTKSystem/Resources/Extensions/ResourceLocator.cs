namespace GtkSystem.Resources.Extensions;

internal struct ResourceLocator
{

    internal int _dataPos;

    internal object _value;

    internal ResourceLocator(int dataPos, object value)
    {
        _dataPos = dataPos;
        _value = value;
    }

    internal int DataPosition => _dataPos;

    internal object Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    internal static bool CanCache(ResourceTypeCode value)
    {
        return value <= ResourceTypeCode.TimeSpan;
    }

}