// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.Text;
using GtkSystem.Resources.Extensions;

namespace System.Resources;

/// <summary>
/// Converter is a class that is used to convert a ResXFileRef object to and from a string. The string format is "fileName;typeName;[encoding]". The fileName is the name of the file that is being referenced, the typeName is the name of the type that is being created from the file, and the encoding is the encoding of the file (optional, only used for text files). The Converter class is used by the ResXResourceWriter class to write resources to a .resx file, and by the ResXResourceReader class to read resources from a .resx file.
/// </summary>
/// <seealso cref="T:System.ComponentModel.TypeConverter" />
[TypeConverter(typeof(Converter))]
public partial class ResXFileRef
{
    /// <summary>
    /// Creates a new ResXFileRef that points to the specified file.
    /// The type refered to by typeName must support a constructor
    /// that accepts a System.IO.Stream as a parameter.
    /// </summary>
    public ResXFileRef(string fileName, string typeName)
    {
        FileName = fileName.OrThrowIfNull();
        TypeName = typeName.OrThrowIfNull();
    }

    /// <summary>
    /// Creates a new ResXFileRef that points to the specified file.
    /// The type refered to by typeName must support a constructor
    /// that accepts a System.IO.Stream as a parameter.
    /// </summary>
    public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding) : this(fileName, typeName)
    {
        TextFileEncoding = textFileEncoding;
    }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>
    /// The name of the file.
    /// </value>
    public string FileName
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the text file encoding.
    /// </summary>
    /// <value>
    /// The text file encoding.
    /// </value>
    public Encoding TextFileEncoding { get; }

    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    /// <value>
    /// The name of the type.
    /// </value>
    public string TypeName { get; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var result = string.Empty;

        if (FileName.IndexOf(';') != -1 || FileName.IndexOf('\"') != -1)
        {
            result += ("\"" + FileName + "\";");
        }
        else
        {
            result += (FileName + ";");
        }

        result += TypeName;
        if (TextFileEncoding != null)
        {
            result += (";" + TextFileEncoding.WebName);
        }

        return result;
    }

    internal ResXFileRef Clone()
    {
        return new ResXFileRef(FileName, TypeName, TextFileEncoding);
    }

    internal void MakeFilePathRelative(string basePath)
    {
        if (string.IsNullOrEmpty(basePath))
        {
            return;
        }

        // Normalize both paths
        var baseFull = Path.GetFullPath(basePath);
        var fileFull = Path.GetFullPath(FileName);

        // If paths are identical, set to empty string
        if (string.Equals(baseFull.TrimEnd(Path.DirectorySeparatorChar), fileFull.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
        {
            FileName = string.Empty;
            return;
        }

        string relPath;
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        relPath = Path.GetRelativePath(baseFull, fileFull);
#else
        // Fallback for older .NET: use Uri
        var baseUri = new Uri(baseFull.EndsWith(Path.DirectorySeparatorChar.ToString()) ? baseFull : baseFull + Path.DirectorySeparatorChar);
        var fileUri = new Uri(fileFull);
        relPath = Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
#endif
        FileName = relPath == "." ? string.Empty : relPath;
    }
}