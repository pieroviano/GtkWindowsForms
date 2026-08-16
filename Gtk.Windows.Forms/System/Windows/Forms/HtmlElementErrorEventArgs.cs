//
// HtmlElementErrorEventArgs.cs
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

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="E:System.Windows.Forms.HtmlWindow.Error" /> event.
/// </summary>
public sealed class HtmlElementErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.HtmlElementErrorEventArgs" /> class.
    /// </summary>
    /// <param name="description">
    /// What went wrong.
    /// </param>
    /// <param name="lineNumber">
    /// The line the error was reported on.
    /// </param>
    /// <param name="url">
    /// The document the error was reported in.
    /// </param>
    internal HtmlElementErrorEventArgs(string description, int lineNumber, Uri? url)
    {
        Description = description;
        LineNumber = lineNumber;
        Url = url;
    }

    /// <summary>
    /// Gets a description of the error.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the error has been handled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the error has been handled and should not be reported further.
    /// </value>
    public bool Handled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the line of the document on which the error occurred.
    /// </summary>
    /// <value>
    /// The line number.
    /// </value>
    public int LineNumber
    {
        get;
    }

    /// <summary>
    /// Gets the document in which the error occurred.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c> when the engine did not report one.
    /// </value>
    public Uri? Url
    {
        get;
    }
}
