//
// HtmlHistory.cs
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
/// Manages the list of documents a
/// <see cref="T:System.Windows.Forms.WebBrowser" /> has visited.
/// </summary>
/// <remarks>
/// Neither engine exposes a countable, indexable session history: both offer only "can I go back", "can
/// I go forward" and relative movement. The members that would need an index therefore step one entry at
/// a time, and
/// <see cref="P:System.Windows.Forms.HtmlHistory.Length" /> counts what this control has itself
/// navigated. Recorded in EMULATION-GAPS.md.
/// </remarks>
public sealed class HtmlHistory : IDisposable
{
    private readonly WebBrowser _owner;

    private bool _disposed;

    internal HtmlHistory(WebBrowser owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Always throws.
    /// </summary>
    /// <value>
    /// Never returns.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The engines behind this control run the page in a separate process, so there is no
    /// in-process history object to hand out.
    /// </exception>
    public object DomHistory =>
        throw new NotSupportedException(
            "There is no in-process history object to return: WebKitGTK and WebView2 both run the page in " +
            "a separate process. Use Back, Forward and Go instead.");

    /// <summary>
    /// Gets the number of entries this control has navigated.
    /// </summary>
    /// <value>
    /// The history length.
    /// </value>
    public int Length => _owner.HistoryLength;

    /// <summary>
    /// Navigates back.
    /// </summary>
    /// <param name="numberBack">
    /// How many entries to step back. Must not be negative.
    /// </param>
    public void Back(int numberBack)
    {
        if (numberBack < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberBack));
        }

        for (var step = 0; step < numberBack; step++)
        {
            _owner.GoBack();
        }
    }

    /// <summary>
    /// Releases the history object.
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Navigates forward.
    /// </summary>
    /// <param name="numberForward">
    /// How many entries to step forward. Must not be negative.
    /// </param>
    public void Forward(int numberForward)
    {
        if (numberForward < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberForward));
        }

        for (var step = 0; step < numberForward; step++)
        {
            _owner.GoForward();
        }
    }

    /// <summary>
    /// Moves through the history by a relative number of entries.
    /// </summary>
    /// <param name="relativePosition">
    /// How far to move. Negative moves back, positive moves forward.
    /// </param>
    public void Go(int relativePosition)
    {
        if (relativePosition < 0)
        {
            Back(-relativePosition);
        }
        else
        {
            Forward(relativePosition);
        }
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate.
    /// </param>
    public void Go(string urlString)
    {
        _owner.Navigate(urlString);
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    public void Go(Uri url)
    {
        _owner.Navigate(url);
    }
}
