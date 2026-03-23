using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using GtkSystem.IO.Resources;

namespace GtkSystem.Resources.Extensions;

/// <summary>
/// DeserializingResourceReader is a class that provides an implementation of deserializing a resource file. It reads resources from a stream and allows access to them through the IResourceReader interface. The class supports both binary formatted resources and resources that use type converters for serialization. It also includes error handling to ensure that the resource file is properly formatted and that the resources can be deserialized correctly.
/// </summary>
/// <seealso cref="T:System.Resources.IResourceReader" />
/// <seealso cref="T:System.Collections.IEnumerable" />
/// <seealso cref="T:System.IDisposable" />
public sealed class DeserializingResourceReader : IResourceReader, IEnumerable, IDisposable
{

    internal Dictionary<string, ResourceLocator> _resCache;

    private bool _assumeBinaryFormatter;

    private long _dataSectionOffset;

    private DataContractSerializer _formatter;

    private int[] _nameHashes;

    private unsafe int* _nameHashesPtr;

    private int[] _namePositions;

    private unsafe int* _namePositionsPtr;

    private long _nameSectionOffset;

    private int _numResources;

    private BinaryReader _store;

    private int[] _typeNamePositions;

    private Type[] _typeTable;

    private UnmanagedMemoryStream _ums;

    private int _version;

    private const int DefaultFileStreamBufferSize = 4096;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.Extensions.DeserializingResourceReader" /> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    public DeserializingResourceReader(string fileName)
    {
        _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
        _store = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess), Encoding.UTF8);
        try
        {
            ReadResources();
        }
        catch
        {
            _store.Close();
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.Extensions.DeserializingResourceReader" /> class.
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// stream
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// </exception>
    public DeserializingResourceReader(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException("stream");
        }
        if (!stream.CanRead)
        {
            throw new ArgumentException(SR.Argument_StreamNotReadable);
        }
        _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
        _store = new BinaryReader(stream, Encoding.UTF8);
        _ums = stream as UnmanagedMemoryStream;
        ReadResources();
    }

    /// <summary>
    /// Closes the resource reader after releasing any resources associated with it.
    /// </summary>
    public void Close()
    {
        Dispose(disposing: true);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Close();
    }

    /// <summary>
    /// Returns a dictionary enumerator of the resources for this reader.
    /// </summary>
    /// <returns>
    /// A dictionary enumerator for the resources for this reader.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public IDictionaryEnumerator GetEnumerator()
    {
        if (_resCache == null)
        {
            throw new InvalidOperationException(SR.ResourceReaderIsClosed);
        }

        return new ResourceEnumerator(this);
    }

    internal int FindPosForResource(string name)
    {
        var hashCode = FastResourceComparer.HashFunction(name);
        var left = 0;
        var right = _numResources - 1;
        var mid = -1;
        var found = false;
        while (left <= right)
        {
            mid = left + right >> 1;
            var nameHash = GetNameHash(mid);
            var comparison = nameHash != hashCode ? nameHash >= hashCode ? 1 : -1 : 0;
            if (comparison == 0)
            {
                found = true;
                break;
            }
            if (comparison < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        if (!found)
        {
            return -1;
        }
        if (left != mid)
        {
            left = mid;
            while (left > 0 && GetNameHash(left - 1) == hashCode)
            {
                left--;
            }
        }
        if (right != mid)
        {
            for (right = mid; right < _numResources - 1 && GetNameHash(right + 1) == hashCode; right++)
            {
            }
        }
        lock (this)
        {
            for (var index = left; index <= right; index++)
            {
                _store.BaseStream.Seek(_nameSectionOffset + GetNamePosition(index), SeekOrigin.Begin);
                if (CompareStringEqualsName(name))
                {
                    var dataOffset = _store.ReadInt32();
                    if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
                    {
                        throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
                    }
                    return dataOffset;
                }
            }
        }

        return -1;
    }

    internal ResourceEnumerator GetEnumeratorInternal()
    {
        return new ResourceEnumerator(this);
    }

    internal object LoadObject(int pos)
    {
        if (_version == 1)
        {
            return LoadObjectV1(pos);
        }

        return LoadObjectV2(pos, out var typeCode);
    }

    internal object LoadObject(int pos, out ResourceTypeCode typeCode)
    {
        if (_version == 1)
        {
            var obj = LoadObjectV1(pos);
            typeCode = obj is string ? ResourceTypeCode.String : ResourceTypeCode.StartOfUserTypes;
            return obj;
        }

        return LoadObjectV2(pos, out typeCode);
    }

    internal object LoadObjectV1(int pos)
    {
        try
        {
            return _LoadObjectV1(pos);
        }
        catch (EndOfStreamException inner)
        {
            throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner);
        }
        catch (ArgumentOutOfRangeException inner2)
        {
            throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner2);
        }
    }

    internal object LoadObjectV2(int pos, out ResourceTypeCode typeCode)
    {
        try
        {
            return _LoadObjectV2(pos, out typeCode);
        }
        catch (EndOfStreamException inner)
        {
            throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner);
        }
        catch (ArgumentOutOfRangeException inner2)
        {
            throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch, inner2);
        }
    }

    internal string LoadString(int pos)
    {
        _store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        string result = null;
        var typeCodeOrIndex = _store.Read7BitEncodedInt();
        if (_version == 1)
        {
            if (typeCodeOrIndex == -1)
            {
                return null;
            }
            if (FindType(typeCodeOrIndex) != typeof(string))
            {
                throw new InvalidOperationException(SystemResources.Format(SR.InvalidOperation_ResourceNotString_Type, FindType(typeCodeOrIndex).FullName));
            }
            result = _store.ReadString();
        }
        else
        {
            var resourceTypeCode = (ResourceTypeCode)typeCodeOrIndex;
            if (resourceTypeCode != ResourceTypeCode.String && resourceTypeCode != 0)
            {
                throw new InvalidOperationException(SystemResources.Format(p1: resourceTypeCode >= ResourceTypeCode.StartOfUserTypes ? FindType((int)(resourceTypeCode - 64)).FullName : resourceTypeCode.ToString(), resourceFormat: SR.InvalidOperation_ResourceNotString_Type));
            }
            if (resourceTypeCode == ResourceTypeCode.String)
            {
                result = _store.ReadString();
            }
        }

        return result;
    }

    internal static unsafe int ReadUnalignedI4(int* p)
    {
        return *(byte*)p | (((byte*)p)[1] << 8) | (((byte*)p)[2] << 16) | (((byte*)p)[3] << 24);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private object _LoadObjectV1(int pos)
    {
        _store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        var typeIndex = _store.Read7BitEncodedInt();
        if (typeIndex == -1)
        {
            return null;
        }
        var type = FindType(typeIndex);
        if (type == typeof(string))
        {
            return _store.ReadString();
        }
        if (type == typeof(int))
        {
            return _store.ReadInt32();
        }
        if (type == typeof(byte))
        {
            return _store.ReadByte();
        }
        if (type == typeof(sbyte))
        {
            return _store.ReadSByte();
        }
        if (type == typeof(short))
        {
            return _store.ReadInt16();
        }
        if (type == typeof(long))
        {
            return _store.ReadInt64();
        }
        if (type == typeof(ushort))
        {
            return _store.ReadUInt16();
        }
        if (type == typeof(uint))
        {
            return _store.ReadUInt32();
        }
        if (type == typeof(ulong))
        {
            return _store.ReadUInt64();
        }
        if (type == typeof(float))
        {
            return _store.ReadSingle();
        }
        if (type == typeof(double))
        {
            return _store.ReadDouble();
        }
        if (type == typeof(DateTime))
        {
            return new DateTime(_store.ReadInt64());
        }
        if (type == typeof(TimeSpan))
        {
            return new TimeSpan(_store.ReadInt64());
        }
        if (type == typeof(decimal))
        {
            var decimalBits = new int[4];
            for (var i = 0; i < decimalBits.Length; i++)
            {
                decimalBits[i] = _store.ReadInt32();
            }
            return new decimal(decimalBits);
        }

        return DeserializeObject(typeIndex);
    }

    private unsafe object _LoadObjectV2(int pos, out ResourceTypeCode typeCode)
    {
        _store.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        typeCode = (ResourceTypeCode)_store.Read7BitEncodedInt();
        switch (typeCode)
        {
            case ResourceTypeCode.Null:
                return null;
            case ResourceTypeCode.String:
                return _store.ReadString();
            case ResourceTypeCode.Boolean:
                return _store.ReadBoolean();
            case ResourceTypeCode.Char:
                return (char)_store.ReadUInt16();
            case ResourceTypeCode.Byte:
                return _store.ReadByte();
            case ResourceTypeCode.SByte:
                return _store.ReadSByte();
            case ResourceTypeCode.Int16:
                return _store.ReadInt16();
            case ResourceTypeCode.UInt16:
                return _store.ReadUInt16();
            case ResourceTypeCode.Int32:
                return _store.ReadInt32();
            case ResourceTypeCode.UInt32:
                return _store.ReadUInt32();
            case ResourceTypeCode.Int64:
                return _store.ReadInt64();
            case ResourceTypeCode.UInt64:
                return _store.ReadUInt64();
            case ResourceTypeCode.Single:
                return _store.ReadSingle();
            case ResourceTypeCode.Double:
                return _store.ReadDouble();
            case ResourceTypeCode.Decimal:
                return _store.ReadDecimal();
            case ResourceTypeCode.DateTime:
                {
                    var dateData = _store.ReadInt64();
                    return DateTime.FromBinary(dateData);
                }
            case ResourceTypeCode.TimeSpan:
                {
                    var ticks = _store.ReadInt64();
                    return new TimeSpan(ticks);
                }
            case ResourceTypeCode.ByteArray:
                {
                    var arrayLength = _store.ReadInt32();
                    if (arrayLength < 0)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, arrayLength));
                    }
                    if (_ums == null)
                    {
                        if (arrayLength > _store.BaseStream.Length)
                        {
                            throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, arrayLength));
                        }
                        return _store.ReadBytes(arrayLength);
                    }
                    if (arrayLength > _ums.Length - _ums.Position)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, arrayLength));
                    }
                    var byteArray = new byte[arrayLength];
                    var bytesRead = _ums.Read(byteArray, 0, arrayLength);
                    return byteArray;
                }
            case ResourceTypeCode.Stream:
                {
                    var streamLength = _store.ReadInt32();
                    if (streamLength < 0)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, streamLength));
                    }
                    if (_ums == null)
                    {
                        var buffer = _store.ReadBytes(streamLength);
                        return new PinnedBufferMemoryStream(buffer);
                    }
                    if (streamLength > _ums.Length - _ums.Position)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, streamLength));
                    }
                    return new UnmanagedMemoryStream(_ums.PositionPointer, streamLength, streamLength, FileAccess.Read);
                }
            default:
                {
                    if (typeCode < ResourceTypeCode.StartOfUserTypes)
                    {
                        throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch);
                    }
                    var typeIndex = (int)(typeCode - 64);
                    return DeserializeObject(typeIndex);
                }
        }
    }

    private unsafe void _ReadResources()
    {
        var magicNumber = _store.ReadInt32();
        if (magicNumber != ResourceManager.MagicNumber)
        {
            throw new ArgumentException(SR.Resources_StreamNotValid);
        }
        var readerVersion = _store.ReadInt32();
        var skipLength = _store.ReadInt32();
        if (skipLength < 0 || readerVersion < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
        if (readerVersion > 1)
        {
            _store.BaseStream.Seek(skipLength, SeekOrigin.Current);
        }
        else
        {
            var readerTypeName = _store.ReadString();
            if (!ValidateReaderType(readerTypeName))
            {
                throw new NotSupportedException(SystemResources.Format(SR.NotSupported_WrongResourceReader_Type, readerTypeName));
            }
            SkipString();
        }
        var resourceVersion = _store.ReadInt32();
        if (resourceVersion != 2 && resourceVersion != 1)
        {
            throw new ArgumentException(SystemResources.Format(SR.Arg_ResourceFileUnsupportedVersion, 2, resourceVersion));
        }
        _version = resourceVersion;
        _numResources = _store.ReadInt32();
        if (_numResources < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
        var typeCount = _store.ReadInt32();
        if (typeCount < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
        _typeTable = new Type[typeCount];
        _typeNamePositions = new int[typeCount];
        for (var i = 0; i < typeCount; i++)
        {
            _typeNamePositions[i] = (int)_store.BaseStream.Position;
            SkipString();
        }
        var position = _store.BaseStream.Position;
        var paddingOffset = (int)position & 7;
        if (paddingOffset != 0)
        {
            for (var j = 0; j < 8 - paddingOffset; j++)
            {
                _store.ReadByte();
            }
        }
        if (_ums == null)
        {
            _nameHashes = new int[_numResources];
            for (var k = 0; k < _numResources; k++)
            {
                _nameHashes[k] = _store.ReadInt32();
            }
        }
        else
        {
            var hashesSize = 4 * _numResources;
            if (hashesSize < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }
            _nameHashesPtr = (int*)_ums.PositionPointer;
            _ums.Seek(hashesSize, SeekOrigin.Current);
            var positionPointer = _ums.PositionPointer;
        }
        if (_ums == null)
        {
            _namePositions = new int[_numResources];
            for (var l = 0; l < _numResources; l++)
            {
                var namePosition = _store.ReadInt32();
                if (namePosition < 0)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
                }
                _namePositions[l] = namePosition;
            }
        }
        else
        {
            var positionsSize = 4 * _numResources;
            if (positionsSize < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }
            _namePositionsPtr = (int*)_ums.PositionPointer;
            _ums.Seek(positionsSize, SeekOrigin.Current);
            var positionPointer = _ums.PositionPointer;
        }
        _dataSectionOffset = _store.ReadInt32();
        if (_dataSectionOffset < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
        _nameSectionOffset = _store.BaseStream.Position;
        if (_dataSectionOffset < _nameSectionOffset)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
    }

    private unsafe string AllocateStringForNameIndex(int index, out int dataOffset)
    {
        long position = GetNamePosition(index);
        int stringLength;
        byte[] buffer;
        lock (this)
        {
            _store.BaseStream.Seek(position + _nameSectionOffset, SeekOrigin.Begin);
            stringLength = _store.Read7BitEncodedInt();
            if (stringLength < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
            }
            if (_ums != null)
            {
                if (_ums.Position > _ums.Length - stringLength)
                {
                    throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourcesIndexTooLong, index));
                }
                string text = null;
                var positionPointer = (char*)_ums.PositionPointer;
                text = new string(positionPointer, 0, stringLength / 2);
                _ums.Position += stringLength;
                dataOffset = _store.ReadInt32();
                if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
                {
                    throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
                }
                return text;
            }
            buffer = new byte[stringLength];
            var remainingBytes = stringLength;
            while (remainingBytes > 0)
            {
                var bytesRead = _store.Read(buffer, stringLength - remainingBytes, remainingBytes);
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException(SystemResources.Format(SR.BadImageFormat_ResourceNameCorrupted_NameIndex, index));
                }
                remainingBytes -= bytesRead;
            }
            dataOffset = _store.ReadInt32();
            if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
            {
                throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
            }
        }

        return Encoding.Unicode.GetString(buffer, 0, stringLength);
    }

    private unsafe bool CompareStringEqualsName(string name)
    {
        var stringLength = _store.Read7BitEncodedInt();
        if (stringLength < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
        }
        if (_ums != null)
        {
            var positionPointer = _ums.PositionPointer;
            _ums.Seek(stringLength, SeekOrigin.Current);
            if (_ums.Position > _ums.Length)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesNameTooLong);
            }
            return FastResourceComparer.CompareOrdinal(positionPointer, stringLength, name) == 0;
        }
        var buffer = new byte[stringLength];
        var remainingBytes = stringLength;
        while (remainingBytes > 0)
        {
            var bytesRead = _store.Read(buffer, stringLength - remainingBytes, remainingBytes);
            if (bytesRead == 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourceNameCorrupted);
            }
            remainingBytes -= bytesRead;
        }

        return FastResourceComparer.CompareOrdinal(buffer, stringLength / 2, name) == 0;
    }

    private unsafe object DeserializeObject(int typeIndex)
    {
        var type = FindType(typeIndex);
        if (_assumeBinaryFormatter)
        {
            return ReadBinaryFormattedObject();
        }
        object obj;
        switch (_store.Read7BitEncodedInt())
        {
            case 1:
                {
                    var size = _store.Read7BitEncodedInt();
                    if (size < 0)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                    }
                    var startPosition = _store.BaseStream.Position;
                    if (type == typeof(System.Windows.Forms.ImageListStreamer))
                    {
                        var byteData = _store.ReadBytes(size);

                        obj = new ImageListStreamer(byteData);
                    }
                    else
                    {
                        obj = ReadBinaryFormattedObject();
                    }
                    if (type == typeof(UnknownType))
                    {
                        type = obj.GetType();
                    }
                    var actualBytesRead = _store.BaseStream.Position - startPosition;
                    if (actualBytesRead != size)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                    }
                    break;
                }
            case 2:
                {
                    var size = _store.Read7BitEncodedInt();
                    if (size < 0)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                    }
                    var bytes = _store.ReadBytes(size);
                    var converter = TypeDescriptor.GetConverter(type) ?? throw new TypeLoadException(SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, type));
                    if (type == typeof(Icon))
                    {
                        obj = new System.Drawing.Icon((byte[])bytes);
                    }
                    else if (type == typeof(System.Drawing.Bitmap))
                    {
                        obj = new System.Drawing.Bitmap(bytes);
                    }
                    else
                    {
                        obj = converter.ConvertFrom(bytes);
                    }
                    break;
                }
            case 3:
                {
                    var stringValue = _store.ReadString();
                    var converter = TypeDescriptor.GetConverter(type) ?? throw new TypeLoadException(SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, type));
                    obj = converter.ConvertFromInvariantString(stringValue);
                    break;
                }
            case 4:
                {
                    var size = _store.Read7BitEncodedInt();
                    if (size < 0)
                    {
                        throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                    }
                    Stream stream;
                    if (_store.BaseStream is UnmanagedMemoryStream unmanagedMemoryStream)
                    {
                        stream = new UnmanagedMemoryStream(unmanagedMemoryStream.PositionPointer, size, size, FileAccess.Read);
                    }
                    else
                    {
                        var buffer = _store.ReadBytes(size);
                        stream = new MemoryStream(buffer, writable: false);
                    }
                    obj = Activator.CreateInstance(type, stream);
                    break;
                }
            default:
                throw new BadImageFormatException(SR.BadImageFormat_TypeMismatch);
        }
        if (obj.GetType() != type)
        {
            throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResType_SerBlobMismatch, type.FullName, obj.GetType().FullName));
        }

        return obj;
    }

    private unsafe void Dispose(bool disposing)
    {
        if (_store != null)
        {
            _resCache = null;
            if (disposing)
            {
                var store = _store;
                _store = null;
                store?.Close();
            }
            _store = null;
            _namePositions = null;
            _nameHashes = null;
            _ums = null;
            _namePositionsPtr = null;
            _nameHashesPtr = null;
        }
    }

    private Type FindType(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= _typeTable.Length)
        {
            throw new BadImageFormatException(SR.BadImageFormat_InvalidType);
        }
        if (_typeTable[typeIndex] == null)
        {
            var position = _store.BaseStream.Position;
            try
            {
                _store.BaseStream.Position = _typeNamePositions[typeIndex];
                var typeName = _store.ReadString();
                if (typeName == "System.Resources.Extensions.UnknownType")
                    _typeTable[typeIndex] = typeof(System.Windows.Forms.ImageListStreamer);
                else
                {
                    if (typeName.Contains("System.Drawing"))
                    {
                        _typeTable[typeIndex] = typeof(Bitmap).Assembly.GetType(typeName.Split(',')[0], throwOnError: true);
                    }
                    else
                    {
                        _typeTable[typeIndex] = Type.GetType(typeName.Split(',')[0], throwOnError: true);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new NotSupportedException(SR.NotSupported_ResourceObjectSerialization);
            }
            finally
            {
                _store.BaseStream.Position = position;
            }
        }

        return _typeTable[typeIndex];
    }

    private unsafe int GetNameHash(int index)
    {
        if (_ums == null)
        {
            return _nameHashes[index];
        }

        return ReadUnalignedI4(_nameHashesPtr + index);
    }

    private unsafe int GetNamePosition(int index)
    {
        var position = _ums != null ? ReadUnalignedI4(_namePositionsPtr + index) : _namePositions[index];
        if (position < 0 || position > _dataSectionOffset - _nameSectionOffset)
        {
            throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesNameInvalidOffset, position));
        }

        return position;
    }

    private object GetValueForNameIndex(int index)
    {
        long position = GetNamePosition(index);
        lock (this)
        {
            _store.BaseStream.Seek(position + _nameSectionOffset, SeekOrigin.Begin);
            SkipString();
            var dataOffset = _store.ReadInt32();
            if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
            {
                throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, dataOffset));
            }
            if (_version == 1)
            {
                return LoadObjectV1(dataOffset);
            }
            return LoadObjectV2(dataOffset, out var typeCode);
        }
    }

    private object? ReadBinaryFormattedObject()
    {
        if (_formatter == null)
        {
            _formatter = new DataContractSerializer(typeof(Bitmap));

        }

        return _formatter.ReadObject(_store.BaseStream);
    }

    private void ReadResources()
    {
        try
        {
            _ReadResources();
        }
        catch (EndOfStreamException inner)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted, inner);
        }
        catch (IndexOutOfRangeException inner2)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted, inner2);
        }
    }

    private void SkipString()
    {
        var stringLength = _store.Read7BitEncodedInt();
        if (stringLength < 0)
        {
            throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
        }
        _store.BaseStream.Seek(stringLength, SeekOrigin.Current);
    }

    private string TypeNameFromTypeCode(ResourceTypeCode typeCode)
    {
        if (typeCode < ResourceTypeCode.StartOfUserTypes)
        {
            return "ResourceTypeCode." + typeCode;
        }
        var typeIndex = (int)(typeCode - 64);
        var position = _store.BaseStream.Position;
        try
        {
            _store.BaseStream.Position = _typeNamePositions[typeIndex];
            return _store.ReadString();
        }
        finally
        {
            _store.BaseStream.Position = position;
        }
    }

    private bool ValidateReaderType(string readerType)
    {
        if (TypeNameComparer.Instance.Equals(readerType, "System.Resources.Extensions.DeserializingResourceReader, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"))
        {
            return true;
        }
        if (TypeNameComparer.Instance.Equals(readerType, "System.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"))
        {
            _assumeBinaryFormatter = true;
            return true;
        }

        return false;
    }

    internal class ImageListSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName);
        }
    }

    internal sealed class ResourceEnumerator : IDictionaryEnumerator, IEnumerator
    {

        private bool _currentIsValid;

        private int _currentName;

        private int _dataPosition;

        private readonly DeserializingResourceReader _reader;

        private const int ENUM_DONE = int.MinValue;

        private const int ENUM_NOT_STARTED = -1;

        internal ResourceEnumerator(DeserializingResourceReader reader)
        {
            _currentName = -1;
            _reader = reader;
            _dataPosition = -2;
        }

        public object Current => Entry;

        public DictionaryEntry Entry
        {
            get
            {
                if (_currentName == int.MinValue)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                }
                if (!_currentIsValid)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                }
                if (_reader._resCache == null)
                {
                    throw new InvalidOperationException(SR.ResourceReaderIsClosed);
                }
                object obj = null;
                string key;
                lock (_reader)
                {
                    lock (_reader._resCache)
                    {
                        key = _reader.AllocateStringForNameIndex(_currentName, out _dataPosition);
                        if (_reader._resCache.TryGetValue(key, out var value))
                        {
                            obj = value.Value;
                        }
                        if (obj == null)
                        {
                            obj = _dataPosition != -1 ? _reader.LoadObject(_dataPosition) : _reader.GetValueForNameIndex(_currentName);
                        }
                    }
                }

                return new DictionaryEntry(key, obj);
            }
        }

        public object Key
        {
            get
            {
                if (_currentName == int.MinValue)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                }
                if (!_currentIsValid)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                }
                if (_reader._resCache == null)
                {
                    throw new InvalidOperationException(SR.ResourceReaderIsClosed);
                }

                return _reader.AllocateStringForNameIndex(_currentName, out _dataPosition);
            }
        }

        public object Value
        {
            get
            {
                if (_currentName == int.MinValue)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                }
                if (!_currentIsValid)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                }
                if (_reader._resCache == null)
                {
                    throw new InvalidOperationException(SR.ResourceReaderIsClosed);
                }

                return _reader.GetValueForNameIndex(_currentName);
            }
        }

        internal int DataPosition => _dataPosition;

        public bool MoveNext()
        {
            if (_currentName == _reader._numResources - 1 || _currentName == int.MinValue)
            {
                _currentIsValid = false;
                _currentName = int.MinValue;
                return false;
            }
            _currentIsValid = true;
            _currentName++;

            return true;
        }

        public void Reset()
        {
            if (_reader._resCache == null)
            {
                throw new InvalidOperationException(SR.ResourceReaderIsClosed);
            }
            _currentIsValid = false;
            _currentName = -1;
        }
    }
}