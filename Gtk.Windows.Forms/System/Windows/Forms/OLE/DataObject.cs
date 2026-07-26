// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Windows.Forms;

/// <summary>
/// Implements a format-independent, in-memory mechanism for transferring data. This is the top-level
/// <c>System.Windows.Forms.DataObject</c> — the type designer code and drag-and-drop operations construct
/// directly (<c>new DataObject(...)</c>) — as distinct from the clipboard-backed
/// <see cref="T:System.Windows.Forms.Clipboard.DataObject" /> used internally by
/// <see cref="T:System.Windows.Forms.Clipboard" />.
/// </summary>
public class DataObject : IDataObject
{
    private readonly Dictionary<string, object?> _store = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Initializes a new, empty instance of the <see cref="T:System.Windows.Forms.DataObject" /> class.</summary>
    public DataObject()
    {
    }

    /// <summary>Initializes a new instance storing <paramref name="data" /> under its own type name.</summary>
    /// <param name="data">The data to store.</param>
    public DataObject(object data)
    {
        SetData(data);
    }

    /// <summary>Initializes a new instance storing <paramref name="data" /> under <paramref name="format" />.</summary>
    /// <param name="format">The format under which to store the data.</param>
    /// <param name="data">The data to store.</param>
    public DataObject(string format, object data)
    {
        SetData(format, data);
    }

    /// <inheritdoc />
    public object? GetData(string format, bool autoConvert)
    {
        if (format == null)
        {
            return null;
        }

        if (_store.TryGetValue(format, out var value))
        {
            return value;
        }

        if (autoConvert)
        {
            // Text and UnicodeText are interchangeable in WinForms' conversion set.
            if (format == DataFormats.Text && _store.TryGetValue(DataFormats.UnicodeText, out var u))
            {
                return u;
            }

            if (format == DataFormats.UnicodeText && _store.TryGetValue(DataFormats.Text, out var t))
            {
                return t;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public object? GetData(string format) => GetData(format, true);

    /// <inheritdoc />
    public object? GetData(Type format) => format == null ? null : GetData(format.FullName!, true);

    /// <inheritdoc />
    public bool GetDataPresent(string format, bool autoConvert)
    {
        if (format == null)
        {
            return false;
        }

        if (_store.ContainsKey(format))
        {
            return true;
        }

        if (autoConvert)
        {
            if (format == DataFormats.Text && _store.ContainsKey(DataFormats.UnicodeText))
            {
                return true;
            }

            if (format == DataFormats.UnicodeText && _store.ContainsKey(DataFormats.Text))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool GetDataPresent(string format) => GetDataPresent(format, true);

    /// <inheritdoc />
    public bool GetDataPresent(Type format) => format != null && GetDataPresent(format.FullName!, true);

    /// <inheritdoc />
    public string[] GetFormats(bool autoConvert) => _store.Keys.ToArray();

    /// <inheritdoc />
    public string[] GetFormats() => GetFormats(true);

    /// <inheritdoc />
    public void SetData(string format, bool autoConvert, object data)
    {
        if (format != null)
        {
            _store[format] = data;
        }
    }

    /// <inheritdoc />
    public void SetData(string format, object data) => SetData(format, true, data);

    /// <inheritdoc />
    public void SetData(Type format, object data)
    {
        if (format != null)
        {
            _store[format.FullName!] = data;
        }
    }

    /// <inheritdoc />
    public void SetData(object data)
    {
        if (data != null)
        {
            _store[data.GetType().FullName!] = data;
        }
    }

    /// <summary>Indicates whether the data object contains data in the <see cref="F:System.Windows.Forms.DataFormats.Text" /> or <see cref="F:System.Windows.Forms.DataFormats.UnicodeText" /> format.</summary>
    /// <returns><see langword="true" /> if text data is present; otherwise <see langword="false" />.</returns>
    public virtual bool ContainsText() => GetDataPresent(DataFormats.UnicodeText, true);

    /// <summary>Indicates whether the data object contains text data in the specified format.</summary>
    /// <param name="format">One of the <see cref="T:System.Windows.Forms.TextDataFormat" /> values.</param>
    /// <returns><see langword="true" /> if text in <paramref name="format" /> is present; otherwise <see langword="false" />.</returns>
    public virtual bool ContainsText(TextDataFormat format) => GetDataPresent(ToFormat(format), false);

    /// <summary>Returns the <see cref="F:System.Windows.Forms.DataFormats.UnicodeText" /> data, or the empty string if none is present.</summary>
    /// <returns>The stored text, or <see cref="F:System.String.Empty" />.</returns>
    public virtual string GetText() => GetText(TextDataFormat.UnicodeText);

    /// <summary>Returns the text data in the specified format, or the empty string if none is present.</summary>
    /// <param name="format">One of the <see cref="T:System.Windows.Forms.TextDataFormat" /> values.</param>
    /// <returns>The stored text, or <see cref="F:System.String.Empty" />.</returns>
    public virtual string GetText(TextDataFormat format) => GetData(ToFormat(format), false) as string ?? string.Empty;

    /// <summary>Stores <paramref name="textData" /> in the <see cref="F:System.Windows.Forms.DataFormats.UnicodeText" /> format.</summary>
    /// <param name="textData">The text to store.</param>
    public virtual void SetText(string textData) => SetText(textData, TextDataFormat.UnicodeText);

    /// <summary>Stores <paramref name="textData" /> in the specified text format.</summary>
    /// <param name="textData">The text to store.</param>
    /// <param name="format">One of the <see cref="T:System.Windows.Forms.TextDataFormat" /> values.</param>
    public virtual void SetText(string textData, TextDataFormat format)
    {
        if (string.IsNullOrEmpty(textData))
        {
            throw new ArgumentNullException(nameof(textData));
        }

        SetData(ToFormat(format), false, textData);
    }

    /// <summary>Indicates whether the data object contains data in the <see cref="F:System.Windows.Forms.DataFormats.Bitmap" /> format.</summary>
    /// <returns><see langword="true" /> if image data is present; otherwise <see langword="false" />.</returns>
    public virtual bool ContainsImage() => GetDataPresent(DataFormats.Bitmap, false);

    /// <summary>Returns the <see cref="F:System.Windows.Forms.DataFormats.Bitmap" /> data, or <see langword="null" /> if none is present.</summary>
    /// <returns>The stored image, or <see langword="null" />.</returns>
    public virtual System.Drawing.Image? GetImage() => GetData(DataFormats.Bitmap, false) as System.Drawing.Image;

    /// <summary>Stores <paramref name="image" /> in the <see cref="F:System.Windows.Forms.DataFormats.Bitmap" /> format.</summary>
    /// <param name="image">The image to store.</param>
    public virtual void SetImage(System.Drawing.Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        SetData(DataFormats.Bitmap, true, image);
    }

    /// <summary>Indicates whether the data object contains a file-drop list.</summary>
    /// <returns><see langword="true" /> if a file-drop list is present; otherwise <see langword="false" />.</returns>
    public virtual bool ContainsFileDropList() => GetDataPresent(DataFormats.FileDrop, true);

    /// <summary>Returns the file-drop list, or an empty collection if none is present.</summary>
    /// <returns>A <see cref="T:System.Collections.Specialized.StringCollection" /> of file names.</returns>
    public virtual StringCollection GetFileDropList()
    {
        var result = new StringCollection();
        if (GetData(DataFormats.FileDrop, true) is string[] files)
        {
            result.AddRange(files);
        }

        return result;
    }

    /// <summary>Stores a file-drop list.</summary>
    /// <param name="filePaths">The file names to store.</param>
    public virtual void SetFileDropList(StringCollection filePaths)
    {
        if (filePaths == null)
        {
            throw new ArgumentNullException(nameof(filePaths));
        }

        var files = new string[filePaths.Count];
        filePaths.CopyTo(files, 0);
        SetData(DataFormats.FileDrop, true, files);
    }

    private static string ToFormat(TextDataFormat format) => format switch
    {
        TextDataFormat.UnicodeText => DataFormats.UnicodeText,
        TextDataFormat.Rtf => DataFormats.Rtf,
        TextDataFormat.Html => DataFormats.Html,
        TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
        _ => DataFormats.Text,
    };
}
