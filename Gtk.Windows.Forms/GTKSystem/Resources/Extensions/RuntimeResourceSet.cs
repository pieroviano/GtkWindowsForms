using System.Collections;
using System.Resources;

namespace GtkSystem.Resources.Extensions;

public class RuntimeResourceSet : ResourceSet, IEnumerable
{
    internal const int Version = 2;

    private Dictionary<string, ResourceLocator>? _caseInsensitiveTable;

    private DeserializingResourceReader? _defaultReader;

    private bool _haveReadFromReader;

    private Dictionary<string, ResourceLocator>? _resCache;

    public RuntimeResourceSet(IResourceReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }
        _defaultReader = (reader as DeserializingResourceReader) ?? throw new ArgumentException(SystemResources.Format(SR.NotSupported_WrongResourceReader_Type, reader.GetType()), nameof(reader));
        _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
        _defaultReader._resCache = _resCache;
    }

    private IResourceReader? Reader => _defaultReader;

    public override IDictionaryEnumerator GetEnumerator()
    {
        return GetEnumeratorHelper();
    }

    public override object? GetObject(string key)
    {
        return GetObject(key, ignoreCase: false, isString: false);
    }

    public override object? GetObject(string key, bool ignoreCase)
    {
        return GetObject(key, ignoreCase, isString: false);
    }

    public override string? GetString(string key)
    {
        var @object = GetObject(key, ignoreCase: false, isString: true);

        return (string?)@object;
    }

    public override string? GetString(string key, bool ignoreCase)
    {
        var @object = GetObject(key, ignoreCase, isString: true);

        return (string?)@object;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumeratorHelper();
    }

    protected override void Dispose(bool disposing)
    {
        if (Reader == null)
        {
            return;
        }
        if (disposing)
        {
            lock (Reader)
            {
                _resCache = null;
                _defaultReader?.Close();
                _defaultReader = null;
                _caseInsensitiveTable = null;
                base.Dispose(disposing);
            }
        }
        else
        {
            _resCache = null;
            _caseInsensitiveTable = null;
            _defaultReader = null;
            base.Dispose(disposing);
        }
    }

    private IDictionaryEnumerator GetEnumeratorHelper()
    {
        var reader = Reader;
        if (reader == null || _resCache == null)
        {
            throw new ObjectDisposedException(null, SR.ObjectDisposed_ResourceSet);
        }

        return reader.GetEnumerator();
    }

    private object? GetObject(string key, bool ignoreCase, bool isString)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (Reader == null || _resCache == null)
        {
            throw new ObjectDisposedException(null, SR.ObjectDisposed_ResourceSet);
        }
        object? obj = null;
        lock (Reader)
        {
            if (Reader == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_ResourceSet);
            }
            ResourceLocator value;
            if (_defaultReader != null)
            {
                var num = -1;
                if (_resCache.TryGetValue(key, out value))
                {
                    obj = value.Value;
                    num = value.DataPosition;
                }
                if (num == -1 && obj == null)
                {
                    num = _defaultReader.FindPosForResource(key);
                }
                if (num != -1 && obj == null)
                {
                    ResourceTypeCode typeCode;
                    if (isString)
                    {
                        obj = _defaultReader.LoadString(num);
                        typeCode = ResourceTypeCode.String;
                    }
                    else
                    {
                        obj = _defaultReader.LoadObject(num, out typeCode);
                    }
                    value = new ResourceLocator(num, ResourceLocator.CanCache(typeCode) ? obj : null);
                    lock (_resCache)
                    {
                        _resCache[key] = value;
                    }
                }
                if (obj != null || !ignoreCase)
                {
                    return obj;
                }
            }
            if (!_haveReadFromReader)
            {
                if (ignoreCase && _caseInsensitiveTable == null)
                {
                    _caseInsensitiveTable = new Dictionary<string, ResourceLocator>(StringComparer.OrdinalIgnoreCase);
                }
                if (_defaultReader == null)
                {
                    var enumerator = Reader.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var entry = enumerator.Entry;
                        var key2 = (string)entry.Key;
                        var value2 = new ResourceLocator(-1, entry.Value);
                        _resCache.Add(key2, value2);
                        if (ignoreCase)
                        {
                            _caseInsensitiveTable?.Add(key2, value2);
                        }
                    }
                    if (!ignoreCase)
                    {
                        Reader.Close();
                    }
                }
                else
                {
                    var enumerator = _defaultReader.GetEnumeratorInternal();
                    while (enumerator.MoveNext())
                    {
                        var enumeratorKey = (string)enumerator.Key;
                        var dataPosition = enumerator.DataPosition;
                        var resourceLocator = new ResourceLocator(dataPosition, null);
                        _caseInsensitiveTable?.Add(enumeratorKey, resourceLocator);
                    }
                }
                _haveReadFromReader = true;
            }
            object? result = null;
            var flag = false;
            var keyInWrongCase = false;
            if (_defaultReader != null && _resCache.TryGetValue(key, out value))
            {
                flag = true;
                result = ResolveResourceLocator(value, key, _resCache, keyInWrongCase);
            }
            if (!flag && ignoreCase && _caseInsensitiveTable != null && _caseInsensitiveTable.TryGetValue(key, out value))
            {
                flag = true;
                keyInWrongCase = true;
                result = ResolveResourceLocator(value, key, _resCache, keyInWrongCase);
            }
            return result;
        }
    }

    private object? ResolveResourceLocator(ResourceLocator resLocation, string key, Dictionary<string, ResourceLocator> copyOfCache, bool keyInWrongCase)
    {
        var obj = resLocation.Value;
        if (obj == null)
        {
            ResourceTypeCode typeCode;
            if (Reader != null)
            {
                var b = false;
                typeCode = default;
                lock (Reader)
                {
                    if (_defaultReader != null)
                    {
                        obj = _defaultReader.LoadObject(resLocation.DataPosition, out typeCode);
                        b = true;
                    }
                }

                if (b && !keyInWrongCase && ResourceLocator.CanCache(typeCode))
                {
                    resLocation.Value = obj;
                    copyOfCache[key] = resLocation;
                }
            }
        }

        return obj;
    }
}