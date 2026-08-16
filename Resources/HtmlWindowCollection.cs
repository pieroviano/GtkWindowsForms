//
// HtmlWindowCollection.cs
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

using System.Collections;
using System.Linq;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.HtmlWindow" /> objects.
/// </summary>
public class HtmlWindowCollection : ICollection, IEnumerable
{
    private readonly List<HtmlWindow> _windows;

    internal HtmlWindowCollection(IEnumerable<HtmlWindow> windows)
    {
        _windows = windows.ToList();
    }

    /// <summary>
    /// Gets the number of windows in the collection.
    /// </summary>
    /// <value>
    /// The window count.
    /// </value>
    public int Count => _windows.Count;

    /// <summary>
    /// Gets a value indicating whether access to the collection is synchronized.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// false
    /// </c>.
    /// </value>
    bool ICollection.IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    /// <value>
    /// The collection itself.
    /// </value>
    object ICollection.SyncRoot => this;

    /// <summary>
    /// Gets the window at a position.
    /// </summary>
    /// <param name="index">
    /// The zero-based position.
    /// </param>
    /// <value>
    /// The window, or
    /// <c>
    /// null
    /// </c> when
    /// <paramref name="index" /> is out of range.
    /// </value>
    public HtmlWindow? this[int index] => index < 0 || index >= _windows.Count ? null : _windows[index];

    /// <summary>
    /// Gets the window with a given name.
    /// </summary>
    /// <param name="windowId">
    /// The window name.
    /// </param>
    /// <value>
    /// The window, or
    /// <c>
    /// null
    /// </c> when nothing matches.
    /// </value>
    public HtmlWindow? this[string windowId]
    {
        get
        {
            foreach (var window in _windows)
            {
                if (string.Equals(window.Name, windowId, StringComparison.Ordinal))
                {
                    return window;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Returns an enumerator over the collection.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
        return _windows.GetEnumerator();
    }

    /// <summary>
    /// Copies the collection into an array.
    /// </summary>
    /// <param name="dest">
    /// The destination array.
    /// </param>
    /// <param name="index">
    /// Where in the destination to start writing.
    /// </param>
    void ICollection.CopyTo(Array dest, int index)
    {
        ((ICollection)_windows).CopyTo(dest, index);
    }
}
