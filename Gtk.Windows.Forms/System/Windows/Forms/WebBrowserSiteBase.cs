//
// WebBrowserSiteBase.cs
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
// Copyright (c) 2007, 2008 Novell, Inc.
//
// Authors:
//  Andreia Gaita <avidigal@novell.com>
//

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Implements the ActiveX host interfaces that real WinForms uses to site the Internet Explorer control.
/// </summary>
/// <remarks>
/// Inert here, and kept only so that code written against real WinForms still compiles: this library
/// hosts WebKitGTK or WebView2, neither of which is an ActiveX control, so there is no site to
/// implement. Nothing calls the members.
/// </remarks>
[ComVisible(true)]
public class WebBrowserSiteBase : IDisposable
{
    internal WebBrowserSiteBase()
    {
    }

    /// <summary>
    /// Releases the site.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the site.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> when called from
    /// <see cref="M:System.Windows.Forms.WebBrowserSiteBase.Dispose" />.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
