using System.Collections;
using System.Resources;

namespace GtkSystem.Resources.Extensions;

internal sealed class RuntimeResourceSet : ResourceSet, IEnumerable
{
    internal const int Version = 2;

    private Dictionary<string, ResourceLocator> _caseInsensitiveTable;

    private DeserializingResourceReader? _defaultReader;

    private bool _haveReadFromReader;

    private Dictionary<string, ResourceLocator> _resCache;

    internal RuntimeResourceSet(IResourceReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException("reader");
        }
        _defaultReader = reader as DeserializingResourceReader ?? throw new ArgumentException(SystemResources.Format(SR.NotSupported_WrongResourceReader_Type, reader.GetType()), "reader");
        _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
        _defaultReader._resCache = _resCache;
    }

    private IResourceReader Reader => _defaultReader;

    public override IDictionaryEnumerator GetEnumerator()
    {
        return GetEnumeratorHelper();
    }

    public override object GetObject(string key)
    {
        return GetObject(key, ignoreCase: false, isString: false);
    }

    public override object GetObject(string key, bool ignoreCase)
    {
        return GetObject(key, ignoreCase, isString: false);
    }

    public override string GetString(string key)
    {
        var resourceValue = GetObject(key, ignoreCase: false, isString: true);

        return (string)resourceValue;
    }

    public override string GetString(string key, bool ignoreCase)
    {
        var resourceValue = GetObject(key, ignoreCase, isString: true);

        return (string)resourceValue;
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

    private object GetObject(string key, bool ignoreCase, bool isString)
    {
        if (key == null)
        {
            throw new ArgumentNullException("key");
        }
        if (Reader == null || _resCache == null)
        {
            throw new ObjectDisposedException(null, SR.ObjectDisposed_ResourceSet);
        }
        object resourceValue = null;
        lock (Reader)
        {
            if (Reader == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_ResourceSet);
            }
            ResourceLocator resourceLocator;
            if (_defaultReader != null)
            {
                var dataPosition = -1;
                if (_resCache.TryGetValue(key, out resourceLocator))
                {
                    resourceValue = resourceLocator.Value;
                    dataPosition = resourceLocator.DataPosition;
                }
                if (dataPosition == -1 && resourceValue == null)
                {
                    dataPosition = _defaultReader.FindPosForResource(key);
                }
                if (dataPosition != -1 && resourceValue == null)
                {
                    ResourceTypeCode typeCode;
                    if (isString)
                    {
                        resourceValue = _defaultReader.LoadString(dataPosition);
                        typeCode = ResourceTypeCode.String;
                    }
                    else
                    {
                        resourceValue = _defaultReader.LoadObject(dataPosition, out typeCode);
                    }
                    resourceLocator = new ResourceLocator(dataPosition, ResourceLocator.CanCache(typeCode) ? resourceValue : null);
                    lock (_resCache)
                    {
                        _resCache[key] = resourceLocator;
                    }
                }
                if (resourceValue != null || !ignoreCase)
                {
                    return resourceValue;
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
                        var resourceKey = (string)entry.Key;
                        var value = new ResourceLocator(-1, entry.Value);
                        _resCache.Add(resourceKey, value);
                        if (ignoreCase)
                        {
                            _caseInsensitiveTable.Add(resourceKey, value);
                        }
                    }
                    if (!ignoreCase)
                    {
                        Reader.Close();
                    }
                }
                else
                {
                    var enumeratorInternal = _defaultReader.GetEnumeratorInternal();
                    while (enumeratorInternal.MoveNext())
                    {
                        var resourceKey = (string)enumeratorInternal.Key;
                        var dataPosition = enumeratorInternal.DataPosition;
                        var locator = new ResourceLocator(dataPosition, null);
                        _caseInsensitiveTable.Add(resourceKey, locator);
                    }
                }
                _haveReadFromReader = true;
            }
            object result = null;
            var wasFound = false;
            var keyInWrongCase = false;
            if (_defaultReader != null && _resCache.TryGetValue(key, out resourceLocator))
            {
                wasFound = true;
                result = ResolveResourceLocator(resourceLocator, key, _resCache, keyInWrongCase);
            }
            if (!wasFound && ignoreCase && _caseInsensitiveTable.TryGetValue(key, out resourceLocator))
            {
                wasFound = true;
                keyInWrongCase = true;
                result = ResolveResourceLocator(resourceLocator, key, _resCache, keyInWrongCase);
            }
            return result;
        }
    }

    private object ResolveResourceLocator(ResourceLocator resLocation, string key, Dictionary<string, ResourceLocator> copyOfCache, bool keyInWrongCase)
    {
        var resourceValue = resLocation.Value;
        if (resourceValue == null)
        {
            ResourceTypeCode typeCode;
            lock (Reader)
            {
                resourceValue = _defaultReader.LoadObject(resLocation.DataPosition, out typeCode);
            }
            if (!keyInWrongCase && ResourceLocator.CanCache(typeCode))
            {
                resLocation.Value = resourceValue;
                copyOfCache[key] = resLocation;
            }
        }

        return resourceValue;
    }
}