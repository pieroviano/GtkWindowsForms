// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace System.Resources;

/// <summary>
/// ResX resource reader.
/// </summary>
public partial class ResXResourceReader : IResourceReader
{
    private readonly IAliasResolver _aliasResolver;

    private readonly AssemblyName[] _assemblyNames;

    private string _basePath;

    private string _fileContents;

    private readonly string _fileName;

    private bool _isReaderDirty;

    private TextReader _reader;

    private ListDictionary _resData;

    private string _resHeaderMimeType;

    private string _resHeaderReaderType;

    private string _resHeaderVersion;

    private string _resHeaderWriterType;

    private ListDictionary _resMetadata;

    private Stream _stream;

    private readonly ITypeResolutionService _typeResolver;

    private bool _useResXDataNodes;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    public ResXResourceReader(string fileName) : this(fileName, (ITypeResolutionService)null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <param name="typeResolver">
    /// The type resolver.
    /// </param>
    public ResXResourceReader(string fileName, ITypeResolutionService typeResolver) : this(fileName, typeResolver, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    public ResXResourceReader(TextReader reader) : this(reader, (ITypeResolutionService)null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="typeResolver">
    /// The type resolver.
    /// </param>
    public ResXResourceReader(TextReader reader, ITypeResolutionService typeResolver) : this(reader, typeResolver, (IAliasResolver)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    public ResXResourceReader(Stream stream) : this(stream, (ITypeResolutionService)null, (IAliasResolver)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    /// <param name="typeResolver">
    /// The type resolver.
    /// </param>
    public ResXResourceReader(Stream stream, ITypeResolutionService typeResolver) : this(stream, typeResolver, (IAliasResolver)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="stream">
    /// The stream.
    /// </param>
    /// <param name="assemblyNames">
    /// The assembly names.
    /// </param>
    public ResXResourceReader(Stream stream, AssemblyName[] assemblyNames) : this(stream, assemblyNames, (IAliasResolver)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="assemblyNames">
    /// The assembly names.
    /// </param>
    public ResXResourceReader(TextReader reader, AssemblyName[] assemblyNames) : this(reader, assemblyNames, (IAliasResolver)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Resources.ResXResourceReader" /> class.
    /// </summary>
    /// <param name="fileName">
    /// Name of the file.
    /// </param>
    /// <param name="assemblyNames">
    /// The assembly names.
    /// </param>
    public ResXResourceReader(string fileName, AssemblyName[] assemblyNames) : this(fileName, assemblyNames, (IAliasResolver)null)
    {
    }

    internal ResXResourceReader(string fileName, ITypeResolutionService typeResolver, IAliasResolver? aliasResolver)
    {
        _fileName = fileName;
        _typeResolver = typeResolver;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    internal ResXResourceReader(TextReader reader, ITypeResolutionService typeResolver, IAliasResolver? aliasResolver)
    {
        _reader = reader;
        _typeResolver = typeResolver;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    internal ResXResourceReader(Stream stream, ITypeResolutionService typeResolver, IAliasResolver? aliasResolver)
    {
        _stream = stream;
        _typeResolver = typeResolver;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    internal ResXResourceReader(Stream stream, AssemblyName[] assemblyNames, IAliasResolver? aliasResolver)
    {
        _stream = stream;
        _assemblyNames = assemblyNames;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    internal ResXResourceReader(TextReader reader, AssemblyName[] assemblyNames, IAliasResolver? aliasResolver)
    {
        _reader = reader;
        _assemblyNames = assemblyNames;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    internal ResXResourceReader(string fileName, AssemblyName[] assemblyNames, IAliasResolver? aliasResolver)
    {
        _fileName = fileName;
        _assemblyNames = assemblyNames;
        _aliasResolver = aliasResolver ?? new ReaderAliasResolver();
    }

    private ResXResourceReader(ITypeResolutionService typeResolver)
    {
        _typeResolver = typeResolver;
        _aliasResolver = new ReaderAliasResolver();
    }

    private ResXResourceReader(AssemblyName[] assemblyNames)
    {
        _assemblyNames = assemblyNames;
        _aliasResolver = new ReaderAliasResolver();
    }

    /// <summary>
    /// BasePath for relatives filepaths with ResXFileRefs.
    /// </summary>
    public string BasePath
    {
        get
        {
            return _basePath;
        }
        set
        {
            if (_isReaderDirty)
            {
                throw new InvalidOperationException("SR.InvalidResXBasePathOperation");
            }

            _basePath = value;
        }
    }

    /// <summary>
    /// ResXFileRef's TypeConverter automatically unwraps it, creates the referenced
    /// object and returns it. This property gives the user control over whether this unwrapping should
    /// happen, or a ResXFileRef object should be returned. Default is true for backward compat and common case
    /// scenario.
    /// </summary>
    public bool UseResXDataNodes
    {
        get
        {
            return _useResXDataNodes;
        }
        set
        {
            if (_isReaderDirty)
            {
                throw new InvalidOperationException("SR.InvalidResXBasePathOperation");
            }

            _useResXDataNodes = value;
        }
    }

    /// <summary>
    /// Closes and files or streams being used by the reader.
    /// </summary>
    public void Close()
    {
        ((IDisposable)this).Dispose();
    }

    /// <summary>
    /// Creates a reader with the specified file contents.
    /// </summary>
    public static ResXResourceReader FromFileContents(string fileContents)
    {
        return FromFileContents(fileContents, (ITypeResolutionService)null);
    }

    /// <summary>
    /// Creates a reader with the specified file contents.
    /// </summary>
    public static ResXResourceReader FromFileContents(string fileContents, ITypeResolutionService typeResolver)
    {
        return new ResXResourceReader(typeResolver)
        {
            _fileContents = fileContents
        };
    }

    /// <summary>
    /// Creates a reader with the specified file contents.
    /// </summary>
    public static ResXResourceReader FromFileContents(string fileContents, AssemblyName[] assemblyNames)
    {
        return new ResXResourceReader(assemblyNames)
        {
            _fileContents = fileContents
        };
    }

    /// <summary>
    /// Returns a dictionary enumerator of the resources for this reader.
    /// </summary>
    /// <returns>
    /// A dictionary enumerator for the resources for this reader.
    /// </returns>
    public IDictionaryEnumerator GetEnumerator()
    {
        _isReaderDirty = true;
        EnsureResData();

        return _resData.GetEnumerator();
    }

    /// <summary>
    /// Returns a dictionary enumerator that can be used to enumerate the &lt;metadata&gt; elements in the .resx file.
    /// </summary>
    public IDictionaryEnumerator GetMetadataEnumerator()
    {
        EnsureResData();

        return _resMetadata.GetEnumerator();
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> to release both managed and unmanaged resources;
    /// <c>
    /// false
    /// </c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_fileName != null && _stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            _reader?.Close();
            _reader = null;
        }
    }

    /// <summary>
    /// Demand loads the resource data.
    /// </summary>
    private void EnsureResData()
    {
        if (_resData is null)
        {
            _resData = new ListDictionary();
            _resMetadata = new ListDictionary();

            XmlTextReader contentReader = null;

            try
            {
                // Read data in any which way
                if (_fileContents != null)
                {
                    contentReader = new XmlTextReader(new StringReader(_fileContents));
                }
                else if (_reader != null)
                {
                    contentReader = new XmlTextReader(_reader);
                }
                else if (_fileName != null || _stream != null)
                {
                    if (_stream is null)
                    {
                        _stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }

                    contentReader = new XmlTextReader(_stream);
                }

                SetupNameTable(contentReader);
                contentReader.WhitespaceHandling = WhitespaceHandling.None;
                ParseXml(contentReader);
            }
            finally
            {
                if (_fileName != null && _stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }
        }
    }

    private static string GetAliasFromTypeName(string typeName)
    {
        var indexStart = typeName.IndexOf(',');

        return typeName.Substring(indexStart + 2);
    }

    /// <summary>
    /// Attempts to return the line and column (Y, X) of the XML reader.
    /// </summary>
    private static Point GetPosition(XmlReader reader)
    {
        var pt = new Point(0, 0);

        if (reader is IXmlLineInfo lineInfo)
        {
            pt.Y = lineInfo.LineNumber;
            pt.X = lineInfo.LinePosition;
        }

        return pt;
    }

    private static string GetTypeFromTypeName(string typeName)
    {
        var indexStart = typeName.IndexOf(',');

        return typeName.Substring(0, indexStart);
    }

    private void ParseAssemblyNode(XmlReader reader)
    {
        var alias = reader[ResXResourceWriter.AliasStr];
        var typeName = reader[ResXResourceWriter.NameStr];

        var assemblyName = new AssemblyName(typeName);

        if (string.IsNullOrEmpty(alias))
        {
            alias = assemblyName.Name;
        }

        _aliasResolver.PushAlias(alias, assemblyName);
    }

    private void ParseDataNode(XmlTextReader reader, bool isMetaData)
    {
        var nodeInfo = new DataNodeInfo
        {
            Name = reader[ResXResourceWriter.NameStr]
        };

        var typeName = reader[ResXResourceWriter.TypeStr];

        string alias = null;
        AssemblyName assemblyName = null;

        if (!string.IsNullOrEmpty(typeName))
        {
            alias = GetAliasFromTypeName(typeName);
        }

        if (!string.IsNullOrEmpty(alias))
        {
            assemblyName = _aliasResolver.ResolveAlias(alias);
        }

        if (assemblyName != null)
        {
            nodeInfo.TypeName = GetTypeFromTypeName(typeName) + ", " + assemblyName.FullName;
        }
        else
        {
            nodeInfo.TypeName = reader[ResXResourceWriter.TypeStr];
        }

        nodeInfo.MimeType = reader[ResXResourceWriter.MimeTypeStr];

        var finishedReadingDataNode = false;
        nodeInfo.ReaderPosition = GetPosition(reader);
        while (!finishedReadingDataNode && reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement && (reader.LocalName.Equals(ResXResourceWriter.DataStr) || reader.LocalName.Equals(ResXResourceWriter.MetadataStr)))
            {
                // we just found </data>, quit or </metadata>
                finishedReadingDataNode = true;
            }
            else
            {
                // could be a <value> or a <comment>
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals(ResXResourceWriter.ValueStr))
                    {
                        var oldValue = reader.WhitespaceHandling;
                        try
                        {
                            // based on the documentation at https://docs.microsoft.com/dotnet/api/system.xml.xmltextreader.whitespacehandling
                            // this is ok because:
                            // "Because the XmlTextReader does not have DTD information available to it,
                            // SignificantWhitespace nodes are only returned within the an xml:space='preserve' scope."
                            // the xml:space would not be present for anything else than string and char (see ResXResourceWriter)
                            // so this would not cause any breaking change while reading data from Everett (we never outputted
                            // xml:space then) or from whidbey that is not specifically either a string or a char.
                            // However please note that manually editing a resx file in Everett and in Whidbey because of the addition
                            // of xml:space=preserve might have different consequences...
                            reader.WhitespaceHandling = WhitespaceHandling.Significant;
                            nodeInfo.ValueData = reader.ReadString();
                        }
                        finally
                        {
                            reader.WhitespaceHandling = oldValue;
                        }
                    }
                    else if (reader.Name.Equals(ResXResourceWriter.CommentStr))
                    {
                        nodeInfo.Comment = reader.ReadString();
                    }
                }
                else
                {
                    // weird, no <xxxx> tag, just the inside of <data> as text
                    nodeInfo.ValueData = reader.Value.Trim();
                }
            }
        }

        if (nodeInfo.Name is null)
        {
            throw new ArgumentException(string.Format("SR.InvalidResXResourceNoName,{0}", nodeInfo.ValueData));
        }

        var dataNode = new ResXDataNode(nodeInfo, BasePath);

        if (UseResXDataNodes)
        {
            _resData[nodeInfo.Name] = dataNode;
        }
        else
        {
            IDictionary data = isMetaData ? _resMetadata : _resData;
            if (_assemblyNames is null)
            {
                data[nodeInfo.Name] = dataNode.GetValue(_typeResolver);
            }
            else
            {
                data[nodeInfo.Name] = dataNode.GetValue(_assemblyNames);
            }
        }
    }

    private void ParseResHeaderNode(XmlReader reader)
    {
        var name = reader[ResXResourceWriter.NameStr];
        if (name != null)
        {
            reader.ReadStartElement();

            // The "1.1" schema requires the correct casing of the strings
            // in the resheader, however the "1.0" schema had a different
            // casing. By checking the Equals first, we should
            // see significant performance improvements.

            if (name == ResXResourceWriter.VersionStr)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    _resHeaderVersion = reader.ReadElementString();
                }
                else
                {
                    _resHeaderVersion = reader.Value.Trim();
                }
            }
            else if (name == ResXResourceWriter.ResMimeTypeStr)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    _resHeaderMimeType = reader.ReadElementString();
                }
                else
                {
                    _resHeaderMimeType = reader.Value.Trim();
                }
            }
            else if (name == ResXResourceWriter.ReaderStr)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    _resHeaderReaderType = reader.ReadElementString();
                }
                else
                {
                    _resHeaderReaderType = reader.Value.Trim();
                }
            }
            else if (name == ResXResourceWriter.WriterStr)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    _resHeaderWriterType = reader.ReadElementString();
                }
                else
                {
                    _resHeaderWriterType = reader.Value.Trim();
                }
            }
            else
            {
                switch (name.ToLower(CultureInfo.InvariantCulture))
                {
                    case ResXResourceWriter.VersionStr:
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            _resHeaderVersion = reader.ReadElementString();
                        }
                        else
                        {
                            _resHeaderVersion = reader.Value.Trim();
                        }

                        break;
                    case ResXResourceWriter.ResMimeTypeStr:
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            _resHeaderMimeType = reader.ReadElementString();
                        }
                        else
                        {
                            _resHeaderMimeType = reader.Value.Trim();
                        }

                        break;
                    case ResXResourceWriter.ReaderStr:
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            _resHeaderReaderType = reader.ReadElementString();
                        }
                        else
                        {
                            _resHeaderReaderType = reader.Value.Trim();
                        }

                        break;
                    case ResXResourceWriter.WriterStr:
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            _resHeaderWriterType = reader.ReadElementString();
                        }
                        else
                        {
                            _resHeaderWriterType = reader.Value.Trim();
                        }

                        break;
                }
            }
        }
    }

    private void ParseXml(XmlTextReader reader)
    {
        var success = false;
        try
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var s = reader.LocalName;

                        if (reader.LocalName.Equals(ResXResourceWriter.AssemblyStr))
                        {
                            ParseAssemblyNode(reader);
                        }
                        else if (reader.LocalName.Equals(ResXResourceWriter.DataStr))
                        {
                            ParseDataNode(reader, false);
                        }
                        else if (reader.LocalName.Equals(ResXResourceWriter.ResHeaderStr))
                        {
                            ParseResHeaderNode(reader);
                        }
                        else if (reader.LocalName.Equals(ResXResourceWriter.MetadataStr))
                        {
                            ParseDataNode(reader, true);
                        }
                    }
                }

                success = true;
            }
            catch (SerializationException se)
            {
                var pt = GetPosition(reader);
                var newMessage = string.Format("SR.SerializationException,{0},{1},{2},{3}", reader[ResXResourceWriter.TypeStr], pt.Y, pt.X, se.Message);
                var xml = new XmlException(newMessage, se, pt.Y, pt.X);
                var newSe = new SerializationException(newMessage, xml);

                throw newSe;
            }
            catch (TargetInvocationException tie)
            {
                var pt = GetPosition(reader);
                var newMessage = string.Format("SR.InvocationException,{0},{1},{2},{3}", reader[ResXResourceWriter.TypeStr], pt.Y, pt.X, tie.InnerException.Message);
                var xml = new XmlException(newMessage, tie.InnerException, pt.Y, pt.X);
                var newTie = new TargetInvocationException(newMessage, xml);

                throw newTie;
            }
            catch (XmlException e)
            {
                throw new ArgumentException(string.Format("SR.InvalidResXFile,{0}", e.Message), e);
            }
            catch (Exception e)
            {
                if (ClientUtils.IsCriticalException(e))
                {
                    throw;
                }
                else
                {
                    var pt = GetPosition(reader);
                    var xmlEx = new XmlException(e.Message, e, pt.Y, pt.X);
                    throw new ArgumentException(string.Format("SR.InvalidResXFile,{0}", xmlEx.Message), xmlEx);
                }
            }
        }
        finally
        {
            if (!success)
            {
                _resData = null;
                _resMetadata = null;
            }
        }

        var validFile = false;

        if (_resHeaderMimeType == ResXResourceWriter.ResMimeType)
        {
            var readerType = typeof(ResXResourceReader);
            var writerType = typeof(ResXResourceWriter);

            var readerTypeName = _resHeaderReaderType;
            var writerTypeName = _resHeaderWriterType;
            if (readerTypeName != null && readerTypeName.IndexOf(',') != -1)
            {
                readerTypeName = readerTypeName.Split(',')[0].Trim();
            }

            if (writerTypeName != null && writerTypeName.IndexOf(',') != -1)
            {
                writerTypeName = writerTypeName.Split(',')[0].Trim();
            }

            if (readerTypeName != null &&
                writerTypeName != null &&
                readerTypeName.Equals(readerType.FullName) &&
                writerTypeName.Equals(writerType.FullName))
            {
                validFile = true;
            }
        }

        if (!validFile)
        {
            _resData = null;
            _resMetadata = null;
            throw new ArgumentException("SR.InvalidResXFileReaderWriterTypes");
        }
    }

    private static void SetupNameTable(XmlReader reader)
    {
        reader.NameTable.Add(ResXResourceWriter.TypeStr);
        reader.NameTable.Add(ResXResourceWriter.NameStr);
        reader.NameTable.Add(ResXResourceWriter.DataStr);
        reader.NameTable.Add(ResXResourceWriter.MetadataStr);
        reader.NameTable.Add(ResXResourceWriter.MimeTypeStr);
        reader.NameTable.Add(ResXResourceWriter.ValueStr);
        reader.NameTable.Add(ResXResourceWriter.ResHeaderStr);
        reader.NameTable.Add(ResXResourceWriter.VersionStr);
        reader.NameTable.Add(ResXResourceWriter.ResMimeTypeStr);
        reader.NameTable.Add(ResXResourceWriter.ReaderStr);
        reader.NameTable.Add(ResXResourceWriter.WriterStr);
        reader.NameTable.Add(ResXResourceWriter.BinSerializedObjectMimeType);
        reader.NameTable.Add(ResXResourceWriter.SoapSerializedObjectMimeType);
        reader.NameTable.Add(ResXResourceWriter.AssemblyStr);
        reader.NameTable.Add(ResXResourceWriter.AliasStr);
    }
}