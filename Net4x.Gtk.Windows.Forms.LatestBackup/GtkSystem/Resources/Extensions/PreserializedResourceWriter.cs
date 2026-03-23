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
            throw new ArgumentNullException("fileName");
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
            throw new ArgumentNullException("stream");
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
            throw new ArgumentNullException("name");
        }
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (typeName == null)
        {
            throw new ArgumentNullException("typeName");
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
            throw new ArgumentNullException("name");
        }
        if (value == null)
        {
            throw new ArgumentNullException("value");
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
            throw new ArgumentNullException("name");
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
    public void AddResource(string name, object? value)
    {
        if (name == null)
        {
            throw new ArgumentNullException("name");
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
            throw new ArgumentNullException("name");
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
            throw new ArgumentNullException("name");
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
            throw new ArgumentNullException("name");
        }
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (typeName == null)
        {
            throw new ArgumentNullException("typeName");
        }
        if (s_primitiveTypes.TryGetValue(typeName, out var primitiveType))
        {
            if (primitiveType == typeof(string))
            {
                AddResource(name, value);
                return;
            }
            var converter = TypeDescriptor.GetConverter(primitiveType) ?? throw new TypeLoadException(SystemResources.Format(SR.TypeLoadException_CannotLoadConverter, primitiveType));
            var convertedValue = converter.ConvertFromInvariantString(value);
            AddResource(name, convertedValue);
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
            throw new ArgumentNullException("name");
        }
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        if (typeName == null)
        {
            throw new ArgumentNullException("typeName");
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
        var memoryBinaryWriter = new BinaryWriter(memoryStream);
        memoryBinaryWriter.Write(ResourceReaderTypeName);
        memoryBinaryWriter.Write(ResourceSetTypeName);
        memoryBinaryWriter.Flush();
        binaryWriter.Write((int)memoryStream.Length);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        memoryStream.CopyTo(binaryWriter.BaseStream, (int)memoryStream.Length);
        binaryWriter.Write(2);
        var totalResourceCount = _resourceList.Count;
        if (_preserializedData != null)
        {
            totalResourceCount += _preserializedData.Count;
        }
        binaryWriter.Write(totalResourceCount);
        var nameHashes = new int[totalResourceCount];
        var namePositions = new int[totalResourceCount];
        var resourceIndex = 0;
        var otherMemoryStream = new MemoryStream(totalResourceCount * 40);
        var otherMemoryBinaryWriter = new BinaryWriter(otherMemoryStream, Encoding.Unicode);
        Stream stream = new MemoryStream();
        using (stream)
        {
            var writer = new BinaryWriter(stream, Encoding.UTF8);
            if (_preserializedData != null)
            {
                foreach (var preserializedDatum in _preserializedData)
                {
                    _resourceList.Add(preserializedDatum.Key, preserializedDatum.Value);
                }
            }
            foreach (var resource in _resourceList)
            {
                nameHashes[resourceIndex] = FastResourceComparer.HashFunction(resource.Key);
                namePositions[resourceIndex++] = (int)otherMemoryBinaryWriter.Seek(0, SeekOrigin.Current);
                otherMemoryBinaryWriter.Write(resource.Key);
                otherMemoryBinaryWriter.Write((int)writer.Seek(0, SeekOrigin.Current));
                var value = resource.Value;
                var resourceTypeCode = FindTypeCode(value, list);
                Write7BitEncodedInt(writer, (int)resourceTypeCode);
                if (value is PrecannedResource precannedResource)
                {
                    WriteData(writer, precannedResource.Data);
                }
                else
                {
                    WriteValue(resourceTypeCode, value, writer);
                }
            }
            binaryWriter.Write(list.Count);
            foreach (var item in list)
            {
                binaryWriter.Write(item);
            }
            Array.Sort(nameHashes, namePositions);
            binaryWriter.Flush();
            var paddingOffset = (int)binaryWriter.BaseStream.Position & 7;
            if (paddingOffset > 0)
            {
                for (var i = 0; i < 8 - paddingOffset; i++)
                {
                    binaryWriter.Write("PAD"[i % 3]);
                }
            }
            foreach (var hashValue in nameHashes)
            {
                binaryWriter.Write(hashValue);
            }
            foreach (var positionValue in namePositions)
            {
                binaryWriter.Write(positionValue);
            }
            binaryWriter.Flush();
            otherMemoryBinaryWriter.Flush();
            writer.Flush();
            var dataSectionOffset = (int)(binaryWriter.Seek(0, SeekOrigin.Current) + otherMemoryStream.Length);
            dataSectionOffset += 4;
            binaryWriter.Write(dataSectionOffset);
            if (otherMemoryStream.Length > 0)
            {
                otherMemoryStream.Seek(0L, SeekOrigin.Begin);
                otherMemoryStream.CopyTo(binaryWriter.BaseStream, (int)otherMemoryStream.Length);
            }
            otherMemoryBinaryWriter.Dispose();
            stream.Position = 0L;
            stream.CopyTo(binaryWriter.BaseStream);
            writer.Dispose();
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

    private void AddResourceInternal(string name, Stream? value, bool closeAfterWrite)
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

    private ResourceTypeCode FindTypeCode(object? value, List<string> types)
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
            var typeIndex = types.IndexOf(typeName);
            if (typeIndex == -1)
            {
                typeIndex = types.Count;
                types.Add(typeName);
            }
            return (ResourceTypeCode)(typeIndex + 64);
        }
        throw new PlatformNotSupportedException(SR.NotSupported_BinarySerializedResources);
    }

    private static void Write7BitEncodedInt(BinaryWriter store, int value)
    {
        uint unsignedValue;
        for (unsignedValue = (uint)value; unsignedValue >= 128; unsignedValue >>= 7)
        {
            store.Write((byte)(unsignedValue | 0x80u));
        }
        store.Write((byte)unsignedValue);
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
                        var binaryData = (byte[])resourceDataRecord.Data;
                        if (_requiresDeserializingResourceReader)
                        {
                            Write7BitEncodedInt(writer, binaryData.Length);
                        }
                        writer.Write(binaryData);
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
                        var byteData = (byte[])resourceDataRecord.Data;
                        Write7BitEncodedInt(writer, byteData.Length);
                        writer.Write(byteData);
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
                    var binaryValue = ((DateTime)value).ToBinary();
                    writer.Write(binaryValue);
                    break;
                }
            case ResourceTypeCode.TimeSpan:
                writer.Write(((TimeSpan)value).Ticks);
                break;
            case ResourceTypeCode.ByteArray:
                {
                    var byteArray = (byte[])value;
                    writer.Write(byteArray.Length);
                    writer.Write(byteArray, 0, byteArray.Length);
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
                        var memoryStreamBytes = memoryStream.ToArray();
                        writer.Write(memoryStreamBytes.Length);
                        writer.Write(memoryStreamBytes, 0, memoryStreamBytes.Length);
                        break;
                    }
                    var stream = streamWrapper.Stream;
                    if (stream.Length > int.MaxValue)
                    {
                        throw new ArgumentException(SR.ArgumentOutOfRange_StreamLength);
                    }
                    stream.Position = 0L;
                    writer.Write((int)stream.Length);
                    var buffer = new byte[4096];
                    var bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
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