// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace System.Resources;

/// <summary>
/// Converter is a class that is used to convert a ResXFileRef object to and from a string. The string format is "fileName;typeName;[encoding]". The fileName is the name of the file that is being referenced, the typeName is the name of the type that is being created from the file, and the encoding is the encoding of the file (optional, only used for text files). The Converter class is used by the ResXResourceWriter class to write resources to a .resx file, and by the ResXResourceReader class to read resources from a .resx file.
/// </summary>
/// <seealso cref="T:System.ComponentModel.TypeConverter" />
public partial class ResXFileRef
{
    /// <summary>
    /// Converter is a class that is used to convert a ResXFileRef object to and from a string. The string format is "fileName;typeName;[encoding]". The fileName is the name of the file that is being referenced, the typeName is the name of the type that is being created from the file, and the encoding is the encoding of the file (optional, only used for text files). The Converter class is used by the ResXResourceWriter class to write resources to a .resx file, and by the ResXResourceReader class to read resources from a .resx file.
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.TypeConverter" />
    public class Converter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An
        /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
        /// </see> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A
        /// <see cref="T:System.Type">
        /// </see> that represents the type you want to convert from.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An
        /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
        /// </see> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A
        /// <see cref="T:System.Type">
        /// </see> that represents the type you want to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An
        /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
        /// </see> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// The
        /// <see cref="T:System.Globalization.CultureInfo">
        /// </see> to use as the current culture.
        /// </param>
        /// <param name="value">
        /// The
        /// <see cref="T:System.Object">
        /// </see> to convert.
        /// </param>
        /// <returns>
        /// An
        /// <see cref="T:System.Object">
        /// </see> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(
                        ITypeDescriptorContext context,
                        CultureInfo culture,
                        object value)
        {
            if (value is string stringValue)
            {
                string[] parts = ParseResxFileRefString(stringValue);
                string fileName = parts[0];
                Type toCreate = Type.GetType(parts[1], true);

                // special case string and byte[]
                if (toCreate == typeof(string))
                {
                    // we have a string, now we need to check the encoding
                    Encoding textFileEncoding =
                        parts.Length > 2
                            ? Encoding.GetEncoding(parts[2])
                            : Encoding.Default;
                    using (StreamReader sr = new StreamReader(fileName, textFileEncoding))
                    {
                        return sr.ReadToEnd();
                    }
                }

                // this is a regular file, we call it's constructor with a stream as a parameter
                // or if it's a byte array we just return that
                byte[] temp = null;

                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Debug.Assert(fileStream != null, "Couldn't open " + fileName);
                    temp = new byte[fileStream.Length];
                    fileStream.Read(temp, 0, (int)fileStream.Length);
                }

                if (toCreate == typeof(byte[]))
                {
                    return temp;
                }

                MemoryStream memStream = new MemoryStream(temp);
                if (toCreate == typeof(MemoryStream))
                {
                    return memStream;
                }

                if (toCreate == typeof(Bitmap) && fileName.EndsWith(".ico"))
                {
                    // we special case the .ico bitmaps because GDI+ destroy the alpha channel component and
                    // we don't want that to happen
                    Icon ico = new Icon(memStream);
                    return ico.ToBitmap();
                }

                return Activator.CreateInstance(toCreate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new object[] { memStream }, null);
            }

            return null;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An
        /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
        /// </see> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A
        /// <see cref="T:System.Globalization.CultureInfo">
        /// </see>. If null is passed, the current culture is assumed.
        /// </param>
        /// <param name="value">
        /// The
        /// <see cref="T:System.Object">
        /// </see> to convert.
        /// </param>
        /// <param name="destinationType">
        /// The
        /// <see cref="T:System.Type">
        /// </see> to convert the value parameter to.
        /// </param>
        /// <returns>
        /// An
        /// <see cref="T:System.Object">
        /// </see> that represents the converted value.
        /// </returns>
        public override object ConvertTo(
                        ITypeDescriptorContext context,
                        CultureInfo culture,
                        object value,
                        Type destinationType)
        {
            object created = null;
            if (destinationType == typeof(string))
            {
                created = ((ResXFileRef)value).ToString();
            }

            return created;
        }

        // "value" is the parameter name of ConvertFrom, which calls this method.
        internal static string[] ParseResxFileRefString(string stringValue)
        {
            string[] result = null;
            if (stringValue != null)
            {
                stringValue = stringValue.Trim();
                string fileName;
                string remainingString;
                if (stringValue.StartsWith("\""))
                {
                    int lastIndexOfQuote = stringValue.LastIndexOf('\"');
                    if (lastIndexOfQuote - 1 < 0)
                    {
                        throw new ArgumentException(nameof(stringValue));
                    }

                    fileName = stringValue.Substring(1, lastIndexOfQuote - 1); // remove the quotes in" ..... "
                    if (lastIndexOfQuote + 2 > stringValue.Length)
                    {
                        throw new ArgumentException(nameof(stringValue));
                    }

                    remainingString = stringValue.Substring(lastIndexOfQuote + 2);
                }
                else
                {
                    int nextSemiColumn = stringValue.IndexOf(';');
                    if (nextSemiColumn == -1)
                    {
                        throw new ArgumentException(nameof(stringValue));
                    }

                    fileName = stringValue.Substring(0, nextSemiColumn);
                    if (nextSemiColumn + 1 > stringValue.Length)
                    {
                        throw new ArgumentException(nameof(stringValue));
                    }

                    remainingString = stringValue.Substring(nextSemiColumn + 1);
                }

                string[] parts = remainingString.Split(';');
                if (parts.Length > 1)
                {
                    result = new string[] { fileName, parts[0], parts[1] };
                }
                else if (parts.Length > 0)
                {
                    result = new string[] { fileName, parts[0] };
                }
                else
                {
                    result = new string[] { fileName };
                }
            }

            return result;
        }

    }
}