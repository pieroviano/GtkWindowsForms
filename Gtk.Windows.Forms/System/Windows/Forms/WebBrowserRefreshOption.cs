//
// WebBrowserRefreshOption.cs
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
/// Specifies constants that define how a page is refreshed.
/// </summary>
/// <remarks>
/// The web engines behind this control offer two reload modes, not four:
/// <see cref="F:System.Windows.Forms.WebBrowserRefreshOption.Completely" /> re-fetches everything and the
/// other three revalidate. See
/// <see cref="!:System.Windows.Forms.WebBrowser.Refresh(System.Windows.Forms.WebBrowserRefreshOption)" />.
/// </remarks>
public enum WebBrowserRefreshOption
{
    /// <summary>
    /// Refreshes the page only if it has expired.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Refreshes the page only if the server indicates that it has expired.
    /// </summary>
    IfExpired = 1,

    /// <summary>
    /// Refreshes the page without checking whether it has expired.
    /// </summary>
    Continue = 2,

    /// <summary>
    /// Fully reloads the page, bypassing every cache.
    /// </summary>
    Completely = 3
}
