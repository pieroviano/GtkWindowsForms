using System.ComponentModel;
using System.Resources;
using System.Text;

namespace GtkSystem.Resources.Extensions;

/// <summary>
/// PreserializedResourceWriter is a class that implements the IResourceWriter interface and is used to write resources to a stream or file in a pre-serialized format. It allows for adding resources of various types, including strings, byte arrays, and streams, and handles the serialization of these resources in a way that can be read by a corresponding resource reader. The class also manages the underlying stream and ensures that all resources are properly written and the stream is closed when done.
/// </summary>
/// <seealso cref="T:System.Resources.IResourceWriter" />
/// <seealso cref="T:System.IDisposable" />
public sealed class PreserializedResourceWriter : IResourceWriter, IDisposable
{

    internal const string DeserializingResourceReaderFullyQualifiedName = "System.Resources.Extensions.DeserializingResourceReader, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";

    internal const string ResourceReaderFullyQualifiedName = "System.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

    internal const string RuntimeResourceSetFullyQualifiedName = "System.Resources.Extensions.RuntimeResourceSet, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";

    private Dictionary<string, object> _caseInsensitiveDups;

    private Stream _output;

    private Dictionary<string, PrecannedResource> _preserializedData;

    private bool _requiresDeserializingResourceReader;

    private SortedDictionary<string, object> _resourceList;

    private const int AverageNameSize = 40;

    private const int AverageValueSize = 40;

    private const string ResSetTypeName = "System.Resources.RuntimeResourceSet";

    private const int ResSetVersion = 2;

    private static readonly IReadOnlyDictionary<string, Type> s_primitiveTypes = new Dictionary<string, Type>(16, TypeNameComparer.Instance)
    {
        {
            typeof(string).FullName,
            typeof(string)
        },
        {
            typeof(int).FullName,
            typeof(int)
        },
        {
            typeof(bool).FullName,
            typeof(bool)
        },
        {
            typeof(char).FullName,
            typeof(char)
        },
        {
            typeof(byte).FullName,
            typeof(byte)
        },
        {
            typeof(sbyte).FullName,
            typeof(sbyte)
        },
        {
            typeof(short).FullName,
            typeof(short)
        },
        {
            typeof(long).FullName,
            typeof(long)
        },
        {
            typeof(ushort).FullName,
            typeof(ushort)
        },
        {
            typeof(uint).FullName,
            typeof(uint)
        },
        {
            typeof(ulong).FullName,
            typeof(ulong)
        },
        {
            typeof(float).FullName,
            typeof(float)
        },
        {
            typeof(double).FullName,
            typeof(double)
        },
        {
            typeof(decimal).FullName,
            typeof(decimal)
        },
        {
            typeof(DateTime).FullName,
            typeof(DateTime)
        },
        {
            typeof(TimeSpan).FullName,
            typeof(TimeSpan)
        }
    };

    private static readonly string UnknownObjectTypeName = typeof(UnknownType).FullName;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.Extensions.PreserializedResourceWriter" /> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// fileName
    /// </exception>
    public PreserializedResourceWriter(string fileName)
    {
        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }
        _output = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        _resourceList = new SortedDictionary<string, object>(FastResourceComparer.Default);
        _caseInsensitiveDups = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.Extensions.PreserializedResourceWriter" /> class.
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// stream
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// </exception>
    public PreserializedResourceWriter(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        if (!stream.CanWrite)
        {
            throw new ArgumentException(SR.Argument_StreamNotWritable);
        }
        _output = stream;
        _resourceList = new SortedDictionary<string, object>(FastResourceComparer.Default);
        _caseInsensitiveDups = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    private string ResourceReaderTypeName
    {
        get
        {
            if (!_requiresDeserializingResourceReader)
            {
                return "System.Resources.ResourceReader, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            }

            return "System.Resources.Extensions.DeserializingResourceReader, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";
        }
    }

    private string ResourceSetTypeName
    {
        get
        {
            if (!_requiresDeserializingResourceReader)
            {
                return "System.Resources.RuntimeResourceSet";
            }

            return "System.Resources.Extensions.RuntimeResourceSet, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";
        }
    }

    /// <summary>
    /// Adds the activator resource.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="typeName">
    /// Name of the type.
    /// </param>
    /// <param name="closeAfterWrite">
    /// if set to
    /// <c>
    /// true
    /// </c>close after write.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// or
    /// value
    /// or
    /// typeName
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// </exception>
    public void AddActivatorResource(string name, Stream value, string typeName, bool closeAfterWrite = false)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (typeName == null)
        {
            throw new ArgumentNullException(nameof(typeName));
        }
        if (!value.CanSeek)
        {
            throw new ArgumentException(SR.NotSupported_UnseekableStream);
        }
        AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.ActivatorStream, value, closeAfterWrite));
        _requiresDeserializingResourceReader = true;
    }

    /// <summary>
    /// Adds the binary formatted resource.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="typeName">
    /// Name of the type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// or
    /// value
    /// </exception>
    public void AddBinaryFormattedResource(string name, byte[] value, string typeName = null)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (typeName == null)
        {
            typeName = UnknownObjectTypeName;
            _requiresDeserializingResourceReader = true;
        }
        AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.BinaryFormatter, value));
    }

    /// <summary>
    /// Adds a named resource of type
    /// <see cref="T:System.String">
    /// </see> to the list of resources to be written.
    /// </summary>
    /// <param name="name">
    /// The name of the resource.
    /// </param>
    /// <param name="value">
    /// The value of the resource.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public void AddResource(string name, string value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        _caseInsensitiveDups.Add(name, null);
        _resourceList.Add(name, value);
    }

    /// <summary>
    /// Adds a named resource of type
    /// <see cref="T:System.Object">
    /// </see> to the list of resources to be written.
    /// </summary>
    /// <param name="name">
    /// The name of the resource.
    /// </param>
    /// <param name="value">
    /// The value of the resource.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public void AddResource(string name, object value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        if (value != null && value is Stream)
        {
            AddResourceInternal(name, (Stream)value, closeAfterWrite: false);
            return;
        }
        _caseInsensitiveDups.Add(name, null);
        _resourceList.Add(name, value);
    }

    /// <summary>
    /// Adds the resource.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="closeAfterWrite">
    /// if set to
    /// <c>
    /// true
    /// </c>close after write.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public void AddResource(string name, Stream value, bool closeAfterWrite = false)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        AddResourceInternal(name, value, closeAfterWrite);
    }

    /// <summary>
    /// Adds an 8-bit unsigned integer array as a named resource to the list of resources to be written.
    /// </summary>
    /// <param name="name">
    /// Name of a resource.
    /// </param>
    /// <param name="value">
    /// Value of a resource as an 8-bit unsigned integer array.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public void AddResource(string name, byte[] value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        _caseInsensitiveDups.Add(name, null);
        _resourceList.Add(name, value);
    }

    /// <summary>
    /// Adds the resource.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="typeName">
    /// Name of the type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// or
    /// value
    /// or
    /// typeName
    /// </exception>
    /// <exception cref="T:System.TypeLoadException">
    /// </exception>
    public void AddResource(string name, string value, string typeName)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (typeName == null)
        {
            throw new ArgumentNullException(nameof(typeName));
        }
        if (s_primitiveTypes.TryGetValue(typeName, out var value2))
        {
            if (value2 == typeof(string))
            {
                AddResource(name, value);
                return;
            }
            var converter = TypeDescriptor.GetConverter(value2) ?? throw new TypeLoadException(SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, value2));
            var value3 = converter.ConvertFromInvariantString(value);
            AddResource(name, value3);
        }
        else
        {
            AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.TypeConverterString, value));
            _requiresDeserializingResourceReader = true;
        }
    }

    /// <summary>
    /// Adds the type converter resource.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="typeName">
    /// Name of the type.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// name
    /// or
    /// value
    /// or
    /// typeName
    /// </exception>
    public void AddTypeConverterResource(string name, byte[] value, string typeName)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (typeName == null)
        {
            throw new ArgumentNullException(nameof(typeName));
        }
        AddResourceData(name, typeName, new ResourceDataRecord(SerializationFormat.TypeConverterByteArray, value));
        _requiresDeserializingResourceReader = true;
    }

    /// <summary>
    /// Closes the underlying resource file or stream, ensuring all the data has been written to the file.
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
        Dispose(disposing: true);
    }

    /// <summary>
    /// Writes all the resources added by the
    /// <see cref="T:System.Resources.IResourceWriter.AddResource(System.String,System.String)">
    /// </see> method to the output file or stream.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// </exception>
    public void Generate()
    {
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        var binaryWriter = new BinaryWriter(_output, Encoding.UTF8);
        var list = new List<string>();
        binaryWriter.Write(ResourceManager.MagicNumber);
        binaryWriter.Write(ResourceManager.HeaderVersionNumber);
        var memoryStream = new MemoryStream(240);
        var writer = new BinaryWriter(memoryStream);
        writer.Write(ResourceReaderTypeName);
        writer.Write(ResourceSetTypeName);
        writer.Flush();
        binaryWriter.Write((int)memoryStream.Length);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        memoryStream.CopyTo(binaryWriter.BaseStream, (int)memoryStream.Length);
        binaryWriter.Write(2);
        var num = _resourceList.Count;
        if (_preserializedData != null)
        {
            num += _preserializedData.Count;
        }
        binaryWriter.Write(num);
        var array = new int[num];
        var items = new int[num];
        var j = 0;
        var otherMemoryStream = new MemoryStream(num * 40);
        var otherWriter = new BinaryWriter(otherMemoryStream, Encoding.Unicode);
        Stream stream = new MemoryStream();
        using (stream)
        {
            var yetOtherWriter = new BinaryWriter(stream, Encoding.UTF8);
            if (_preserializedData != null)
            {
                foreach (var preserializedDatum in _preserializedData)
                {
                    _resourceList.Add(preserializedDatum.Key, preserializedDatum.Value);
                }
            }
            foreach (var resource in _resourceList)
            {
                array[j] = FastResourceComparer.HashFunction(resource.Key);
                items[j++] = (int)otherWriter.Seek(0, SeekOrigin.Current);
                otherWriter.Write(resource.Key);
                otherWriter.Write((int)yetOtherWriter.Seek(0, SeekOrigin.Current));
                var value = resource.Value;
                var resourceTypeCode = FindTypeCode(value, list);
                Write7BitEncodedInt(yetOtherWriter, (int)resourceTypeCode);
                if (value is PrecannedResource precannedResource)
                {
                    WriteData(yetOtherWriter, precannedResource.Data);
                }
                else
                {
                    WriteValue(resourceTypeCode, value, yetOtherWriter);
                }
            }
            binaryWriter.Write(list.Count);
            foreach (var item in list)
            {
                binaryWriter.Write(item);
            }
            Array.Sort(array, items);
            binaryWriter.Flush();
            var position = (int)binaryWriter.BaseStream.Position & 7;
            if (position > 0)
            {
                for (var i = 0; i < 8 - position; i++)
                {
                    binaryWriter.Write("PAD"[i % 3]);
                }
            }
            var ints = array;
            foreach (var value in ints)
            {
                binaryWriter.Write(value);
            }
            var values = items;
            foreach (var value in values)
            {
                binaryWriter.Write(value);
            }
            binaryWriter.Flush();
            otherWriter.Flush();
            yetOtherWriter.Flush();
            var length = (int)(binaryWriter.Seek(0, SeekOrigin.Current) + otherMemoryStream.Length);
            length += 4;
            binaryWriter.Write(length);
            if (otherMemoryStream.Length > 0)
            {
                otherMemoryStream.Seek(0L, SeekOrigin.Begin);
                otherMemoryStream.CopyTo(binaryWriter.BaseStream, (int)otherMemoryStream.Length);
            }
            otherWriter.Dispose();
            stream.Position = 0L;
            stream.CopyTo(binaryWriter.BaseStream);
            yetOtherWriter.Dispose();
        }
        binaryWriter.Flush();
        _resourceList = null;
    }

    private void AddResourceData(string name, string typeName, object data)
    {
        if (_resourceList == null)
        {
            throw new InvalidOperationException(SR.InvalidOperation_ResourceWriterSaved);
        }
        _caseInsensitiveDups.Add(name, null);
        if (_preserializedData == null)
        {
            _preserializedData = new Dictionary<string, PrecannedResource>(FastResourceComparer.Default);
        }
        _preserializedData.Add(name, new PrecannedResource(typeName, data));
    }

    private void AddResourceInternal(string name, Stream value, bool closeAfterWrite)
    {
        if (value == null)
        {
            _caseInsensitiveDups.Add(name, null);
            _resourceList.Add(name, value);
            return;
        }
        if (!value.CanSeek)
        {
            throw new ArgumentException(SR.NotSupported_UnseekableStream);
        }
        _caseInsensitiveDups.Add(name, null);
        _resourceList.Add(name, new StreamWrapper(value, closeAfterWrite));
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_resourceList != null)
            {
                Generate();
            }
            _output?.Dispose();
        }
        _output = null;
        _caseInsensitiveDups = null;
    }

    private ResourceTypeCode FindTypeCode(object value, List<string> types)
    {
        if (value == null)
        {
            return ResourceTypeCode.Null;
        }
        var type = value.GetType();
        if (type == typeof(string))
        {
            return ResourceTypeCode.String;
        }
        if (type == typeof(int))
        {
            return ResourceTypeCode.Int32;
        }
        if (type == typeof(bool))
        {
            return ResourceTypeCode.Boolean;
        }
        if (type == typeof(char))
        {
            return ResourceTypeCode.Char;
        }
        if (type == typeof(byte))
        {
            return ResourceTypeCode.Byte;
        }
        if (type == typeof(sbyte))
        {
            return ResourceTypeCode.SByte;
        }
        if (type == typeof(short))
        {
            return ResourceTypeCode.Int16;
        }
        if (type == typeof(long))
        {
            return ResourceTypeCode.Int64;
        }
        if (type == typeof(ushort))
        {
            return ResourceTypeCode.UInt16;
        }
        if (type == typeof(uint))
        {
            return ResourceTypeCode.UInt32;
        }
        if (type == typeof(ulong))
        {
            return ResourceTypeCode.UInt64;
        }
        if (type == typeof(float))
        {
            return ResourceTypeCode.Single;
        }
        if (type == typeof(double))
        {
            return ResourceTypeCode.Double;
        }
        if (type == typeof(decimal))
        {
            return ResourceTypeCode.Decimal;
        }
        if (type == typeof(DateTime))
        {
            return ResourceTypeCode.DateTime;
        }
        if (type == typeof(TimeSpan))
        {
            return ResourceTypeCode.TimeSpan;
        }
        if (type == typeof(byte[]))
        {
            return ResourceTypeCode.ByteArray;
        }
        if (type == typeof(StreamWrapper))
        {
            return ResourceTypeCode.Stream;
        }
        if (type == typeof(PrecannedResource))
        {
            var typeName = ((PrecannedResource)value).TypeName;
            if (typeName.StartsWith("ResourceTypeCode.", StringComparison.Ordinal))
            {
                typeName = typeName.Substring(17);
                return (ResourceTypeCode)Enum.Parse(typeof(ResourceTypeCode), typeName);
            }
            var num = types.IndexOf(typeName);
            if (num == -1)
            {
                num = types.Count;
                types.Add(typeName);
            }
            return (ResourceTypeCode)(num + 64);
        }
        throw new PlatformNotSupportedException(SR.NotSupported_BinarySerializedResources);
    }

    private static void Write7BitEncodedInt(BinaryWriter store, int value)
    {
        uint num;
        for (num = (uint)value; num >= 128; num >>= 7)
        {
            store.Write((byte)(num | 0x80u));
        }
        store.Write((byte)num);
    }

    private void WriteData(BinaryWriter writer, object dataContext)
    {
        var resourceDataRecord = dataContext as ResourceDataRecord;
        if (_requiresDeserializingResourceReader)
        {
            Write7BitEncodedInt(writer, (int)resourceDataRecord.Format);
        }
        try
        {
            switch (resourceDataRecord.Format)
            {
                case SerializationFormat.BinaryFormatter:
                    {
                        var data = (byte[])resourceDataRecord.Data;
                        if (_requiresDeserializingResourceReader)
                        {
                            Write7BitEncodedInt(writer, data.Length);
                        }
                        writer.Write(data);
                        break;
                    }
                case SerializationFormat.ActivatorStream:
                    {
                        var stream = (Stream)resourceDataRecord.Data;
                        if (stream.Length > int.MaxValue)
                        {
                            throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);
                        }
                        stream.Position = 0L;
                        Write7BitEncodedInt(writer, (int)stream.Length);
                        stream.CopyTo(writer.BaseStream);
                        break;
                    }
                case SerializationFormat.TypeConverterByteArray:
                    {
                        var array = (byte[])resourceDataRecord.Data;
                        Write7BitEncodedInt(writer, array.Length);
                        writer.Write(array);
                        break;
                    }
                case SerializationFormat.TypeConverterString:
                    {
                        var value = (string)resourceDataRecord.Data;
                        writer.Write(value);
                        break;
                    }
                default:
                    throw new ArgumentException("Format");
            }
        }
        finally
        {
            if (resourceDataRecord.Data is IDisposable disposable && resourceDataRecord.CloseAfterWrite)
            {
                disposable.Dispose();
            }
        }
    }

    private void WriteValue(ResourceTypeCode typeCode, object value, BinaryWriter writer)
    {
        switch (typeCode)
        {
            case ResourceTypeCode.String:
                writer.Write((string)value);
                break;
            case ResourceTypeCode.Boolean:
                writer.Write((bool)value);
                break;
            case ResourceTypeCode.Char:
                writer.Write((ushort)(char)value);
                break;
            case ResourceTypeCode.Byte:
                writer.Write((byte)value);
                break;
            case ResourceTypeCode.SByte:
                writer.Write((sbyte)value);
                break;
            case ResourceTypeCode.Int16:
                writer.Write((short)value);
                break;
            case ResourceTypeCode.UInt16:
                writer.Write((ushort)value);
                break;
            case ResourceTypeCode.Int32:
                writer.Write((int)value);
                break;
            case ResourceTypeCode.UInt32:
                writer.Write((uint)value);
                break;
            case ResourceTypeCode.Int64:
                writer.Write((long)value);
                break;
            case ResourceTypeCode.UInt64:
                writer.Write((ulong)value);
                break;
            case ResourceTypeCode.Single:
                writer.Write((float)value);
                break;
            case ResourceTypeCode.Double:
                writer.Write((double)value);
                break;
            case ResourceTypeCode.Decimal:
                writer.Write((decimal)value);
                break;
            case ResourceTypeCode.DateTime:
                {
                    var binary = ((DateTime)value).ToBinary();
                    writer.Write(binary);
                    break;
                }
            case ResourceTypeCode.TimeSpan:
                writer.Write(((TimeSpan)value).Ticks);
                break;
            case ResourceTypeCode.ByteArray:
                {
                    var bytes = (byte[])value;
                    writer.Write(bytes.Length);
                    writer.Write(bytes, 0, bytes.Length);
                    break;
                }
            case ResourceTypeCode.Stream:
                {
                    var streamWrapper = (StreamWrapper)value;
                    if (streamWrapper.Stream.GetType() == typeof(MemoryStream))
                    {
                        var memoryStream = (MemoryStream)streamWrapper.Stream;
                        if (memoryStream.Length > int.MaxValue)
                        {
                            throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);
                        }
                        var array = memoryStream.ToArray();
                        writer.Write(array.Length);
                        writer.Write(array, 0, array.Length);
                        break;
                    }
                    var stream = streamWrapper.Stream;
                    if (stream.Length > int.MaxValue)
                    {
                        throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);
                    }
                    stream.Position = 0L;
                    writer.Write((int)stream.Length);
                    var bytes = new byte[4096];
                    var num = 0;
                    while ((num = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        writer.Write(bytes, 0, num);
                    }
                    if (streamWrapper.CloseAfterWrite)
                    {
                        stream.Close();
                    }
                    break;
                }
            default:
                throw new PlatformNotSupportedException(SR.NotSupported_BinarySerializedResources);
            case ResourceTypeCode.Null:
                break;
        }
    }

    private class PrecannedResource
    {

        internal readonly object Data;

        internal readonly string TypeName;

        internal PrecannedResource(string typeName, object data)
        {
            TypeName = typeName;
            Data = data;
        }
    }

    private class ResourceDataRecord
    {

        internal readonly bool CloseAfterWrite;

        internal readonly object Data;

        internal readonly SerializationFormat Format;

        internal ResourceDataRecord(SerializationFormat format, object data, bool closeAfterWrite = false)
        {
            Format = format;
            Data = data;
            CloseAfterWrite = closeAfterWrite;
        }
    }

    private class StreamWrapper
    {

        internal readonly bool CloseAfterWrite;

        internal readonly Stream Stream;

        internal StreamWrapper(Stream s, bool closeAfterWrite)
        {
            Stream = s;
            CloseAfterWrite = closeAfterWrite;
        }
    }
}