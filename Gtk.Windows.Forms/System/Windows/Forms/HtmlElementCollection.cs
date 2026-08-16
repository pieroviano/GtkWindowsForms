//
// HtmlElementCollection.cs
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
using GtkSystem.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.HtmlElement" /> objects.
/// </summary>
/// <remarks>
/// The collection is a snapshot in both senses: it is built from the cached
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomSnapshot" /> at the moment it is asked for, and it
/// does not change afterwards. Ask the owning document again to see the page's current shape.
/// </remarks>
public sealed class HtmlElementCollection : ICollection, IEnumerable
{
    private readonly IHtmlDomBridge _bridge;

    private readonly List<HtmlElement> _elements;

    private readonly HtmlDocument _owner;

    internal HtmlElementCollection(HtmlDocument owner, IHtmlDomBridge bridge, IEnumerable<int> handles)
    {
        _owner = owner;
        _bridge = bridge;
        _elements = handles.Select(owner.WrapExisting).ToList();
    }

    private HtmlElementCollection(HtmlDocument owner, IHtmlDomBridge bridge, List<HtmlElement> elements)
    {
        _owner = owner;
        _bridge = bridge;
        _elements = elements;
    }

    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    /// <value>
    /// The element count.
    /// </value>
    public int Count => _elements.Count;

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
    /// Gets the element at a position.
    /// </summary>
    /// <param name="index">
    /// The zero-based position.
    /// </param>
    /// <value>
    /// The element, or
    /// <c>
    /// null
    /// </c> when
    /// <paramref name="index" /> is out of range - which is what real WinForms returns rather than
    /// throwing.
    /// </value>
    public HtmlElement? this[int index] => index < 0 || index >= _elements.Count ? null : _elements[index];

    /// <summary>
    /// Gets the first element carrying an
    /// <c>
    /// id
    /// </c> or
    /// <c>
    /// name
    /// </c> equal to
    /// <paramref name="elementId" />.
    /// </summary>
    /// <param name="elementId">
    /// The identifier to look for.
    /// </param>
    /// <value>
    /// The element, or
    /// <c>
    /// null
    /// </c> when nothing matches.
    /// </value>
    public HtmlElement? this[string elementId]
    {
        get
        {
            if (elementId == null)
            {
                return null;
            }

            foreach (var element in _elements)
            {
                if (string.Equals(element.Id, elementId, StringComparison.Ordinal) ||
                    string.Equals(element.Name, elementId, StringComparison.Ordinal))
                {
                    return element;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Finds every element in this collection carrying a
    /// <c>
    /// name
    /// </c> attribute equal to
    /// <paramref name="name" />.
    /// </summary>
    /// <param name="name">
    /// The name to look for.
    /// </param>
    /// <returns>
    /// The matching elements.
    /// </returns>
    public HtmlElementCollection GetElementsByName(string name)
    {
        var matches = _elements
            .Where(element => element.HasAttribute("name") &&
                              string.Equals(element.GetAttribute("name"), name, StringComparison.Ordinal))
            .ToList();

        return new HtmlElementCollection(_owner, _bridge, matches);
    }

    /// <summary>
    /// Returns an enumerator over the collection.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
        return _elements.GetEnumerator();
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
        ((ICollection)_elements).CopyTo(dest, index);
    }
}
