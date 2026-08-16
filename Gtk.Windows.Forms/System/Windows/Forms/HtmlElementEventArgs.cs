//
// HtmlElementEventArgs.cs
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

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the events defined on
/// <see cref="T:System.Windows.Forms.HtmlDocument" />,
/// <see cref="T:System.Windows.Forms.HtmlElement" /> and
/// <see cref="T:System.Windows.Forms.HtmlWindow" />.
/// </summary>
public sealed class HtmlElementEventArgs : EventArgs
{
    /// <summary>
    /// Initializes an empty instance of the
    /// <see cref="T:System.Windows.Forms.HtmlElementEventArgs" /> class.
    /// </summary>
    internal HtmlElementEventArgs()
    {
        EventType = string.Empty;
        MouseButtonsPressed = MouseButtons.None;
    }

    /// <summary>
    /// Initializes an instance of the
    /// <see cref="T:System.Windows.Forms.HtmlElementEventArgs" /> class from the event the page
    /// reported.
    /// </summary>
    /// <param name="eventType">
    /// The DOM event type, without the
    /// <c>
    /// on
    /// </c> prefix.
    /// </param>
    /// <param name="fromElement">
    /// The element the pointer came from.
    /// </param>
    /// <param name="toElement">
    /// The element the pointer moved to.
    /// </param>
    internal HtmlElementEventArgs(string eventType, HtmlElement? fromElement, HtmlElement? toElement)
    {
        EventType = eventType ?? string.Empty;
        FromElement = fromElement;
        ToElement = toElement;
        MouseButtonsPressed = MouseButtons.None;
    }

    /// <summary>
    /// Gets a value indicating whether the ALT key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when ALT was down.
    /// </value>
    public bool AltKeyPressed
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the event bubbles up the document tree.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> to let the event bubble.
    /// </value>
    public bool BubbleEvent
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the position of the mouse in the document's client area.
    /// </summary>
    /// <value>
    /// The client mouse position.
    /// </value>
    public Point ClientMousePosition
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets a value indicating whether the CTRL key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when CTRL was down.
    /// </value>
    public bool CtrlKeyPressed
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the name of the event that was raised.
    /// </summary>
    /// <value>
    /// The event type, without the
    /// <c>
    /// on
    /// </c> prefix.
    /// </value>
    public string EventType
    {
        get;
    }

    /// <summary>
    /// Gets the element the mouse pointer moved away from.
    /// </summary>
    /// <value>
    /// The source element, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public HtmlElement? FromElement
    {
        get;
    }

    /// <summary>
    /// Gets the ASCII value of the key that was pressed.
    /// </summary>
    /// <value>
    /// The key code.
    /// </value>
    public int KeyPressedCode
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets which mouse buttons were pressed.
    /// </summary>
    /// <value>
    /// The buttons.
    /// </value>
    public MouseButtons MouseButtonsPressed
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the position of the mouse relative to the document.
    /// </summary>
    /// <value>
    /// The mouse position.
    /// </value>
    public Point MousePosition
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the position of the mouse relative to the element that raised the event.
    /// </summary>
    /// <value>
    /// The offset mouse position.
    /// </value>
    public Point OffsetMousePosition
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets the value the event handler returns to the page.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public bool ReturnValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the SHIFT key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when SHIFT was down.
    /// </value>
    public bool ShiftKeyPressed
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the element the mouse pointer moved onto.
    /// </summary>
    /// <value>
    /// The target element, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public HtmlElement? ToElement
    {
        get;
    }
}
