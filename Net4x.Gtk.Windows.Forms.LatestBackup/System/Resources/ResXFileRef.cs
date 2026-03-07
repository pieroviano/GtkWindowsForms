// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using GtkSystem.Resources.Extensions;

namespace System.Resources
{
    /// <summary>
    /// ResX File Reference class. This allows the developer to represent
    /// a link to an external resource. When the resource manager asks
    /// for the value of the resource item, the external resource is loaded.
    /// </summary>
    [TypeConverter(typeof(Converter))]
    public partial class ResXFileRef
    {
        /// <summary>
        /// Creates a new instance of the
        /// <see cref="System.Resources.ResXFileRef" /> class that references the specified file.
        /// </summary>
        /// <param name="fileName">The file to reference.
        /// </param>
        /// <param name="typeName">The type of the resource that is referenced.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fileName" /> or
        /// <paramref name="typeName " />is null.
        /// </exception>
        public ResXFileRef(string fileName, string typeName)
        {
            FileName = fileName.OrThrowIfNull();
            TypeName = typeName.OrThrowIfNull();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Resources.ResXFileRef" /> class that references the specified file.
        /// </summary>
        /// <param name="fileName">The file to reference.
        /// </param>
        /// <param name="typeName">The type name of the resource that is referenced.
        /// </param>
        /// <param name="textFileEncoding">The encoding used in the referenced file.
        /// </param>
        public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding) : this(fileName, typeName)
        {
            TextFileEncoding = textFileEncoding;
        }

        /// <summary>
        /// Gets the file name specified in the current
        /// <see cref="!:Overload:System.Resources.ResXFileRef.#ctor" /> constructor.
        /// </summary>
        /// <returns>The name of the referenced file.
        /// </returns>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the encoding specified in the current
        /// <see cref="!:Overload:System.Resources.ResXFileRef.#ctor" /> constructor.
        /// </summary>
        /// <returns>The encoding used in the referenced file.
        /// </returns>
        public Encoding TextFileEncoding { get; }

        /// <summary>
        /// Gets the type name specified in the current
        /// <see cref="!:Overload:System.Resources.ResXFileRef.#ctor" /> constructor.
        /// </summary>
        /// <returns>The type name of the resource that is referenced.
        /// </returns>
        public string TypeName { get; }

        /// <summary>
        /// Gets the text representation of the current
        /// <see cref="System.Resources.ResXFileRef" /> object.
        /// </summary>
        /// <returns>A string that consists of the concatenated text representations of the parameters specified in the current
        /// <see cref="!:Overload:System.Resources.ResXFileRef.#ctor" /> constructor.
        /// </returns>
        public override string ToString()
        {
            string result = string.Empty;

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

            FileName = PathDifference(basePath, FileName, false);
        }

        /// <summary>
        /// path1+result = path2
        /// A string which is the relative path difference between path1 and
        /// path2 such that if path1 and the calculated difference are used
        /// as arguments to Combine(), path2 is returned
        /// </summary>
        private static string PathDifference(string path1, string path2, bool compareCase)
        {
            int i;
            int si = -1;

            for (i = 0; (i < path1.Length) && (i < path2.Length); ++i)
            {
                if ((path1[i] != path2[i]) && (compareCase || (char.ToLower(path1[i], CultureInfo.InvariantCulture) != char.ToLower(path2[i], CultureInfo.InvariantCulture))))
                {
                    break;
                }

                if (path1[i] == Path.DirectorySeparatorChar)
                {
                    si = i;
                }
            }

            if (i == 0)
            {
                return path2;
            }

            if ((i == path1.Length) && (i == path2.Length))
            {
                return string.Empty;
            }

            StringBuilder relPath = new StringBuilder();

            for (; i < path1.Length; ++i)
            {
                if (path1[i] == Path.DirectorySeparatorChar)
                {
                    relPath.Append(".." + Path.DirectorySeparatorChar);
                }
            }

            return string.Concat(relPath.ToString(), path2.Substring(si + 1));
        }

    }
}
