//
// WebBrowserEncryptionLevel.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Novell, Inc.
//
// Authors:
//  Jonathan Pobst (monkey@jpobst.com)
//

namespace System.Windows.Forms;

/// <summary>
/// Specifies constants that define the encryption methods used by documents displayed in the
/// <see cref="T:System.Windows.Forms.WebBrowser" /> control.
/// </summary>
/// <remarks>
/// This enumeration names Internet Explorer's cipher tiers, which no modern engine reports. The mapping
/// is therefore coarse: a plain HTTP document is
/// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Insecure" />, a document served over TLS
/// with a valid certificate is
/// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Bit128" />, and one whose certificate the
/// engine flagged is
/// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Mixed" />.
/// </remarks>
public enum WebBrowserEncryptionLevel
{
    /// <summary>
    /// The document is not encrypted.
    /// </summary>
    Insecure = 0,

    /// <summary>
    /// The document contains a mixture of encrypted and unencrypted content.
    /// </summary>
    Mixed = 1,

    /// <summary>
    /// The document uses an unknown encryption method.
    /// </summary>
    Unknown = 2,

    /// <summary>
    /// The document uses 40-bit encryption.
    /// </summary>
    Bit40 = 3,

    /// <summary>
    /// The document uses 56-bit encryption.
    /// </summary>
    Bit56 = 4,

    /// <summary>
    /// The document uses Fortezza encryption.
    /// </summary>
    Fortezza = 5,

    /// <summary>
    /// The document uses 128-bit encryption.
    /// </summary>
    Bit128 = 6
}
