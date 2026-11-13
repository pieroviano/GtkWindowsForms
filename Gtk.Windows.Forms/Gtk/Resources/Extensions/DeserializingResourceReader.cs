using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Gtk.IO;

namespace Gtk.Resources.Extensions;

/// <summary>
/// DeserializingResourceReader is a class that provides an implementation of deserializing a resource file. It reads resources from a stream and allows access to them through the IResourceReader interface. The class supports both binary formatted resources and resources that use type converters for serialization. It also includes error handling to ensure that the resource file is properly formatted and that the resources can be deserialized correctly.
/// </summary>
/// <seealso cref="T:System.Resources.IResourceReader" />
/// <seealso cref="T:System.Collections.IEnumerable" />
/// <seealso cref="T:System.IDisposable" />
public class DeserializingResourceReader : IResourceReader, IEnumerable, IDisposable
{

    internal Dictionary<string, ResourceLocator>? _resCache;

    private bool _assumeBinaryFormatter;

    private long _dataSectionOffset;

    private DataContractSerializer? _formatter;

    private int[]? _nameHashes;

    private unsafe int* _nameHashesPtr;

    private int[]? _namePositions;

    private unsafe int* _namePositionsPtr;

    private long _nameSectionOffset;

    private int _numResources;

    private BinaryReader? _store;

    private int[]? _typeNamePositions;

    private Type[]? _typeTable;

    private UnmanagedMemoryStream? _ums;

    private int _version;

    private const int DefaultFileStreamBufferSize = 4096;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Resources.Extensions.DeserializingResourceReader" /> class.
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
    /// <see cref="Gtk.Resources.Extensions.DeserializingResourceReader" /> class.
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
            throw new ArgumentNullException(nameof(stream));
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
        var result = FastResourceComparer.HashFunction(name);
        var bitPosition = 0;
        var i = _numResources - 1;
        var position = -1;
        var flag = false;
        while (bitPosition <= i)
        {
            position = bitPosition + i >> 1;
            var nameHash = GetNameHash(position);
            var j = ((nameHash != result) ? ((nameHash >= result) ? 1 : (-1)) : 0);
            if (j == 0)
            {
                flag = true;
                break;
            }
            if (j < 0)
            {
                bitPosition = position + 1;
            }
            else
            {
                i = position - 1;
            }
        }
        if (!flag)
        {
            return -1;
        }
        if (bitPosition != position)
        {
            bitPosition = position;
            while (bitPosition > 0 && GetNameHash(bitPosition - 1) == result)
            {
                bitPosition--;
            }
        }
        if (i != position)
        {
            for (i = position; i < _numResources - 1 && GetNameHash(i + 1) == result; i++)
            {
            }
        }
        lock (this)
        {
            for (var j = bitPosition; j <= i; j++)
            {
                _store?.BaseStream.Seek(_nameSectionOffset + GetNamePosition(j), SeekOrigin.Begin);
                if (CompareStringEqualsName(name))
                {
                    if (_store != null)
                    {
                        var findPosForResource = _store.ReadInt32();
                        if (findPosForResource < 0 || findPosForResource >= _store.BaseStream.Length - _dataSectionOffset)
                        {
                            throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, findPosForResource));
                        }
                        return findPosForResource;
                    }
                }
            }
        }

        return -1;
    }

    internal ResourceEnumerator GetEnumeratorInternal()
    {
        return new ResourceEnumerator(this);
    }

    internal object? LoadObject(int pos)
    {
        if (_version == 1)
        {
            return LoadObjectV1(pos);
        }

        return LoadObjectV2(pos, out var typeCode);
    }

    internal object? LoadObject(int pos, out ResourceTypeCode typeCode)
    {
        if (_version == 1)
        {
            var obj = LoadObjectV1(pos);
            typeCode = ((obj is string) ? ResourceTypeCode.String : ResourceTypeCode.StartOfUserTypes);
            return obj;
        }

        return LoadObjectV2(pos, out typeCode);
    }

    internal object? LoadObjectV1(int pos)
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

    internal object? LoadObjectV2(int pos, out ResourceTypeCode typeCode)
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

    internal string? LoadString(int pos)
    {
        _store?.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        string? result = null;
        if (_store != null)
        {
            var num = _store.Read7BitEncodedInt();
            if (_version == 1)
            {
                if (num == -1)
                {
                    return null;
                }
                if (FindType(num) != typeof(string))
                {
                    throw new InvalidOperationException(SystemResources.Format(SR.InvalidOperation_ResourceNotString_Type, FindType(num).FullName));
                }
                result = _store.ReadString();
            }
            else
            {
                var resourceTypeCode = (ResourceTypeCode)num;
                if (resourceTypeCode != ResourceTypeCode.String && resourceTypeCode != 0)
                {
                    throw new InvalidOperationException(SystemResources.Format(p1: (resourceTypeCode >= ResourceTypeCode.StartOfUserTypes) ? FindType((int)(resourceTypeCode - 64)).FullName : resourceTypeCode.ToString(), resourceFormat: SR.InvalidOperation_ResourceNotString_Type));
                }
                if (resourceTypeCode == ResourceTypeCode.String)
                {
                    result = _store.ReadString();
                }
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

    private object? _LoadObjectV1(int pos)
    {
        _store?.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        if (_store != null)
        {
            var num = _store.Read7BitEncodedInt();
            if (num == -1)
            {
                return null;
            }
            var type = FindType(num);
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
                var array = new int[4];
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = _store.ReadInt32();
                }
                return new decimal(array);
            }

            return DeserializeObject(num);
        }
        return null;
    }

    private unsafe object? _LoadObjectV2(int pos, out ResourceTypeCode typeCode)
    {
        _store?.BaseStream.Seek(_dataSectionOffset + pos, SeekOrigin.Begin);
        if (_store != null)
        {
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
                        var byteArrayLength = _store.ReadInt32();
                        if (byteArrayLength < 0)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, byteArrayLength));
                        }

                        if (_ums == null)
                        {
                            if (byteArrayLength > _store.BaseStream.Length)
                            {
                                throw new BadImageFormatException(
                                    SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid,
                                        byteArrayLength));
                            }

                            return _store.ReadBytes(byteArrayLength);
                        }

                        if (byteArrayLength > _ums.Length - _ums.Position)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, byteArrayLength));
                        }

                        var array2 = new byte[byteArrayLength];
                        var num3 = _ums.Read(array2, 0, byteArrayLength);
                        return array2;
                    }
                case ResourceTypeCode.Stream:
                    {
                        var num = _store.ReadInt32();
                        if (num < 0)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num));
                        }

                        if (_ums == null)
                        {
                            var array = _store.ReadBytes(num);
                            return new PinnedBufferMemoryStream(array);
                        }

                        if (num > _ums.Length - _ums.Position)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, num));
                        }

                        return new UnmanagedMemoryStream(_ums.PositionPointer, num, num, FileAccess.Read);
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
        typeCode = ResourceTypeCode.Null;
        return null;
    }

    private unsafe void _ReadResources()
    {
        if (_store != null)
        {
            var num = _store.ReadInt32();
            if (num != ResourceManager.MagicNumber)
            {
                throw new ArgumentException(SR.Resources_StreamNotValid);
            }
        }

        if (_store != null)
        {
            var x = _store.ReadInt32();
            var y = _store.ReadInt32();
            if (y < 0 || x < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }
            if (x > 1)
            {
                _store.BaseStream.Seek(y, SeekOrigin.Current);
            }
            else
            {
                var text = _store.ReadString();
                if (!ValidateReaderType(text))
                {
                    throw new NotSupportedException(SystemResources.Format(SR.NotSupported_WrongResourceReader_Type, text));
                }
                SkipString();
            }
        }

        if (_store != null)
        {
            var w = _store.ReadInt32();
            if (w != 2 && w != 1)
            {
                throw new ArgumentException(SystemResources.Format(SR.Arg_ResourceFileUnsupportedVersion, 2, w));
            }
            _version = w;
        }

        if (_store != null)
        {
            _numResources = _store.ReadInt32();
            if (_numResources < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }

            var z = _store.ReadInt32();
            if (z < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }

            _typeTable = new Type[z];
            _typeNamePositions = new int[z];
            for (var i = 0; i < z; i++)
            {
                _typeNamePositions[i] = (int)_store.BaseStream.Position;
                SkipString();
            }

            var position = _store.BaseStream.Position;
            var a = (int)position & 7;
            if (a != 0)
            {
                for (var j = 0; j < 8 - a; j++)
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
                var numResources = 4 * _numResources;
                if (numResources < 0)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
                }

                _nameHashesPtr = (int*)_ums.PositionPointer;
                _ums.Seek(numResources, SeekOrigin.Current);
                var positionPointer = _ums.PositionPointer;
            }

            if (_ums == null)
            {
                _namePositions = new int[_numResources];
                for (var l = 0; l < _numResources; l++)
                {
                    var c = _store.ReadInt32();
                    if (c < 0)
                    {
                        throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
                    }

                    _namePositions[l] = c;
                }
            }
            else
            {
                var d = 4 * _numResources;
                if (d < 0)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
                }

                _namePositionsPtr = (int*)_ums.PositionPointer;
                _ums.Seek(d, SeekOrigin.Current);
                var positionPointer2 = _ums.PositionPointer;
            }

            _dataSectionOffset = _store.ReadInt32();
            if (_dataSectionOffset < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
            }

            _nameSectionOffset = _store.BaseStream.Position;
        }

        if (_dataSectionOffset < _nameSectionOffset)
        {
            throw new BadImageFormatException(SR.BadImageFormat_ResourcesHeaderCorrupted);
        }
    }

    private unsafe string AllocateStringForNameIndex(int index, out int dataOffset)
    {
        long namePosition = GetNamePosition(index);
        int encodedInt;
        byte[] array;
        lock (this)
        {
            if (_store != null)
            {
                _store.BaseStream.Seek(namePosition + _nameSectionOffset, SeekOrigin.Begin);
                encodedInt = _store.Read7BitEncodedInt();
                if (encodedInt < 0)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
                }

                if (_ums != null)
                {
                    if (_ums.Position > _ums.Length - encodedInt)
                    {
                        throw new BadImageFormatException(
                            SystemResources.Format(SR.BadImageFormat_ResourcesIndexTooLong, index));
                    }

                    string? text = null;
                    var positionPointer = (char*)_ums.PositionPointer;
                    text = new string(positionPointer, 0, encodedInt / 2);
                    _ums.Position += encodedInt;
                    dataOffset = _store.ReadInt32();
                    if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
                    {
                        throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset,
                            dataOffset));
                    }

                    return text;
                }

                array = new byte[encodedInt];
                var count = encodedInt;
                while (count > 0)
                {
                    var read = _store.Read(array, encodedInt - count, count);
                    if (read == 0)
                    {
                        throw new EndOfStreamException(
                            SystemResources.Format(SR.BadImageFormat_ResourceNameCorrupted_NameIndex, index));
                    }

                    count -= read;
                }

                dataOffset = _store.ReadInt32();
                if (dataOffset < 0 || dataOffset >= _store.BaseStream.Length - _dataSectionOffset)
                {
                    throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset,
                        dataOffset));
                }
            }
            else
            {
                throw new InvalidOperationException(SR.ResourceReaderIsClosed);
            }
        }

        return Encoding.Unicode.GetString(array, 0, encodedInt);
    }

    private unsafe bool CompareStringEqualsName(string name)
    {
        if (_store != null)
        {
            var num = _store.Read7BitEncodedInt();
            if (num < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
            }
            if (_ums != null)
            {
                var positionPointer = _ums.PositionPointer;
                _ums.Seek(num, SeekOrigin.Current);
                if (_ums.Position > _ums.Length)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_ResourcesNameTooLong);
                }
                return FastResourceComparer.CompareOrdinal(positionPointer, num, name) == 0;
            }
            var array = new byte[num];
            var count = num;
            while (count > 0)
            {
                var read = _store.Read(array, num - count, count);
                if (read == 0)
                {
                    throw new BadImageFormatException(SR.BadImageFormat_ResourceNameCorrupted);
                }
                count -= read;
            }

            return FastResourceComparer.CompareOrdinal(array, num / 2, name) == 0;
        }
        else
        {
            throw new InvalidOperationException(SR.ResourceReaderIsClosed);
        }
    }

    private unsafe object? DeserializeObject(int typeIndex)
    {
        var type = FindType(typeIndex);
        if (_assumeBinaryFormatter)
        {
            return ReadBinaryFormattedObject();
        }
        object? obj;
        if (_store != null)
            switch (_store.Read7BitEncodedInt())
            {
                case 1:
                    {
                        var size = _store.Read7BitEncodedInt();
                        if (size < 0)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                        }

                        var position = _store.BaseStream.Position;
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
                            type = obj?.GetType() ?? typeof(UnknownType);
                        }

                        var streamPosition = _store.BaseStream.Position - position;
                        if (streamPosition != size)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                        }

                        break;
                    }
                case 2:
                    {
                        var size = _store.Read7BitEncodedInt();
                        if (size < 0)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                        }

                        var value = _store.ReadBytes(size);
                        var converter = TypeDescriptor.GetConverter(type) ??
                                        throw new TypeLoadException(
                                            SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, type));
                        if (type == typeof(Icon))
                        {
                            obj = new System.Drawing.Icon((byte[])value);
                        }
                        else if (type == typeof(System.Drawing.Bitmap))
                        {
                            obj = new System.Drawing.Bitmap(value);
                        }
                        else
                        {
                            obj = converter.ConvertFrom(value);
                        }

                        break;
                    }
                case 3:
                    {
                        var text = _store.ReadString();
                        var converter = TypeDescriptor.GetConverter(type) ??
                                        throw new TypeLoadException(
                                            SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, type));
                        obj = converter.ConvertFromInvariantString(text);
                        break;
                    }
                case 4:
                    {
                        var size = _store.Read7BitEncodedInt();
                        if (size < 0)
                        {
                            throw new BadImageFormatException(
                                SystemResources.Format(SR.BadImageFormat_ResourceDataLengthInvalid, size));
                        }

                        Stream stream;
                        if (_store.BaseStream is UnmanagedMemoryStream unmanagedMemoryStream)
                        {
                            stream = new UnmanagedMemoryStream(unmanagedMemoryStream.PositionPointer, size, size,
                                FileAccess.Read);
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
        else
        {
            throw new InvalidOperationException(SR.ResourceReaderIsClosed);
        }

        if (obj?.GetType() != type)
        {
            throw new BadImageFormatException(SystemResources.Format(SR.BadImageFormat_ResType_SerBlobMismatch, type.FullName, obj?.GetType().FullName));
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
        if (typeIndex < 0 || typeIndex >= _typeTable?.Length || _typeTable == null)
        {
            throw new BadImageFormatException(SR.BadImageFormat_InvalidType);
        }
        if (_typeTable[typeIndex] == null)
        {
            if (_store != null)
            {
                var position = _store.BaseStream.Position;
                try
                {
                    if (_typeNamePositions != null) _store.BaseStream.Position = _typeNamePositions[typeIndex];
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
        }

        return _typeTable[typeIndex];
    }

    private unsafe int GetNameHash(int index)
    {
        if (_ums == null)
        {
            return _nameHashes?[index] ?? throw new InvalidOperationException(nameof(_nameHashes));
        }

        return ReadUnalignedI4(_nameHashesPtr + index);
    }

    private unsafe int GetNamePosition(int index)
    {
        var num = ((_ums != null) ? ReadUnalignedI4(_namePositionsPtr + index) : _namePositions?[index] ?? throw new InvalidOperationException(nameof(_namePositions)));
        if (num < 0 || num > _dataSectionOffset - _nameSectionOffset)
        {
            throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesNameInvalidOffset, num));
        }

        return num;
    }

    private object? GetValueForNameIndex(int index)
    {
        long num = GetNamePosition(index);
        lock (this)
        {
            _store?.BaseStream.Seek(num + _nameSectionOffset, SeekOrigin.Begin);
            SkipString();
            if (_store != null)
            {
                var i = _store.ReadInt32();
                if (i < 0 || i >= _store.BaseStream.Length - _dataSectionOffset)
                {
                    throw new FormatException(SystemResources.Format(SR.BadImageFormat_ResourcesDataInvalidOffset, i));
                }
                if (_version == 1)
                {
                    return LoadObjectV1(i);
                }
                return LoadObjectV2(i, out var typeCode);
            }
        }
        throw new InvalidOperationException(SR.ResourceReaderIsClosed);
    }

    private object? ReadBinaryFormattedObject()
    {
        if (_formatter == null)
        {
            _formatter = new DataContractSerializer(typeof(Bitmap));

        }

        if (_store != null) return _formatter.ReadObject(_store.BaseStream);
        throw new InvalidOperationException(SR.ResourceReaderIsClosed);
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
        if (_store != null)
        {
            var num = _store.Read7BitEncodedInt();
            if (num < 0)
            {
                throw new BadImageFormatException(SR.BadImageFormat_NegativeStringLength);
            }
            _store.BaseStream.Seek(num, SeekOrigin.Current);
        }
    }

    private string TypeNameFromTypeCode(ResourceTypeCode typeCode)
    {
        if (typeCode < ResourceTypeCode.StartOfUserTypes)
        {
            return "ResourceTypeCode." + typeCode;
        }
        var num = (int)(typeCode - 64);
        if (_store != null)
        {
            var position = _store.BaseStream.Position;
            try
            {
                _store.BaseStream.Position = _typeNamePositions?[num] ?? throw new ArgumentException(nameof(_typeNamePositions));
                return _store.ReadString();
            }
            finally
            {
                _store.BaseStream.Position = position;
            }
        }
        throw new InvalidOperationException(SR.ResourceReaderIsClosed);
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
                object? obj = null;
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
                            obj = ((_dataPosition != -1) ? _reader.LoadObject(_dataPosition) : _reader.GetValueForNameIndex(_currentName));
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

        public object? Value
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