// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the type of input and output stream used to load and save data in a <see cref="T:System.Windows.Forms.RichTextBox" />.</summary>
public enum RichTextBoxStreamType
{
    /// <summary>Rich Text Format (RTF) stream.</summary>
    RichText = 0,

    /// <summary>Plain text stream with a space in place of each OLE object.</summary>
    PlainText = 1,

    /// <summary>RTF stream with spaces in place of OLE objects.</summary>
    RichNoOleObjs = 2,

    /// <summary>Text stream containing spaces in place of OLE objects.</summary>
    TextTextOleObjs = 3,

    /// <summary>Text stream in the Unicode character encoding.</summary>
    UnicodePlainText = 4,
}
