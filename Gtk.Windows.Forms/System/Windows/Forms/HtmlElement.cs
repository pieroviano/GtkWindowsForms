//
// HtmlElement.cs
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
// The Mozilla/XPCOM DOM this originally addressed (Mono.WebBrowser.DOM.IElement) is gone; every member
// now reads the cached DomSnapshot and writes through DomMutation. See EMULATION-GAPS.md.
//

using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Gtk.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Represents an HTML element inside a Web page.
/// </summary>
/// <remarks>
/// Every getter reads the last
/// <see cref="Gtk.Windows.Forms.WebBrowsing.DomSnapshot" /> rather than the live page, and every
/// setter updates that snapshot and posts the real change. The consequence is that a value the page
/// changes on its own - from its own script, say - is invisible here until the next snapshot. See
/// <see cref="M:System.Windows.Forms.HtmlElement.InvokeMember(System.String)" /> for the sharpest edge of
/// that design.
/// </remarks>
public sealed class HtmlElement
{
    private readonly IHtmlDomBridge _bridge;

    private readonly Dictionary<string, EventHandler> _attachedHandlers =
        new Dictionary<string, EventHandler>(StringComparer.OrdinalIgnoreCase);

    private readonly int _handle;

    private readonly HtmlDocument _owner;

    private EventHandlerList? _events;

    private bool _listening;

    internal HtmlElement(HtmlDocument owner, IHtmlDomBridge bridge, int handle)
    {
        _owner = owner;
        _bridge = bridge;
        _handle = handle;
    }

    /// <summary>
    /// Gets every element contained inside this one, at any depth.
    /// </summary>
    /// <value>
    /// The descendants, in document order.
    /// </value>
    public HtmlElementCollection All =>
        new HtmlElementCollection(_owner, _bridge, _bridge.Snapshot.Descendants(_handle).Select(node => node.Handle));

    /// <summary>
    /// Gets a value indicating whether this element can have children.
    /// </summary>
    /// <value>
    /// <c>
    /// false
    /// </c> for the void elements of the HTML specification.
    /// </value>
    public bool CanHaveChildren
    {
        get
        {
            // from http://www.w3.org/TR/html401/index/elements.html
            switch (TagName.ToLowerInvariant())
            {
                case "area":
                case "base":
                case "basefont":
                case "br":
                case "col":
                case "frame":
                case "hr":
                case "img":
                case "input":
                case "isindex":
                case "link":
                case "meta":
                case "param":
                    return false;
                default:
                    return true;
            }
        }
    }

    /// <summary>
    /// Gets this element's immediate children.
    /// </summary>
    /// <value>
    /// The children, in document order.
    /// </value>
    public HtmlElementCollection Children =>
        new HtmlElementCollection(_owner, _bridge, Node.ChildHandles);

    /// <summary>
    /// Gets the element's bounds without its border.
    /// </summary>
    /// <value>
    /// The client rectangle.
    /// </value>
    public Rectangle ClientRectangle => Node.ClientRect;

    /// <summary>
    /// Gets the document that contains this element.
    /// </summary>
    /// <value>
    /// The owning document.
    /// </value>
    public HtmlDocument Document => _owner;

    /// <summary>
    /// Always throws.
    /// </summary>
    /// <value>
    /// Never returns.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The engines behind this control run the page in a separate process, so there is no
    /// in-process DOM object to hand out.
    /// </exception>
    public object DomElement =>
        throw new NotSupportedException(
            "There is no in-process DOM object to return: WebKitGTK and WebView2 both run the page in a " +
            "separate process. Use the members of HtmlElement, or InvokeMemberAsync from " +
            "Gtk.Windows.Extensions, to reach the live node.");

    /// <summary>
    /// Gets or sets a value indicating whether the user can interact with the element.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the element is enabled.
    /// </value>
    public bool Enabled
    {
        get => Node.Enabled;
        set
        {
            Mutate(node => node.Enabled = value,
                new DomMutation(DomMutationKind.SetDisabled, _handle) { Flag = !value });
        }
    }

    /// <summary>
    /// Gets this element's first child element.
    /// </summary>
    /// <value>
    /// The first child, or
    /// <c>
    /// null
    /// </c> when the element has none.
    /// </value>
    public HtmlElement? FirstChild
    {
        get
        {
            var children = Node.ChildHandles;
            return children.Count == 0 ? null : _owner.Wrap(children[0]);
        }
    }

    /// <summary>
    /// Gets or sets the element's
    /// <c>
    /// id
    /// </c> attribute.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    /// <summary>
    /// Gets or sets the HTML between the element's start and end tags.
    /// </summary>
    /// <value>
    /// The inner HTML.
    /// </value>
    public string InnerHtml
    {
        get => Node.InnerHtml;
        set
        {
            Mutate(node => node.InnerHtml = value ?? string.Empty,
                new DomMutation(DomMutationKind.SetInnerHtml, _handle) { Value = value });
        }
    }

    /// <summary>
    /// Gets or sets the text between the element's start and end tags.
    /// </summary>
    /// <value>
    /// The inner text.
    /// </value>
    public string InnerText
    {
        get => Node.InnerText;
        set
        {
            Mutate(node => node.InnerText = value ?? string.Empty,
                new DomMutation(DomMutationKind.SetInnerText, _handle) { Value = value });
        }
    }

    /// <summary>
    /// Gets or sets the element's
    /// <c>
    /// name
    /// </c> attribute.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    /// <summary>
    /// Gets the element that follows this one under the same parent.
    /// </summary>
    /// <value>
    /// The next sibling, or
    /// <c>
    /// null
    /// </c> when this is the last child.
    /// </value>
    public HtmlElement? NextSibling
    {
        get
        {
            var parent = _bridge.Snapshot.Find(Node.ParentHandle);
            if (parent == null)
            {
                return null;
            }

            var index = parent.ChildHandles.IndexOf(_handle);
            return index < 0 || index + 1 >= parent.ChildHandles.Count
                ? null
                : _owner.Wrap(parent.ChildHandles[index + 1]);
        }
    }

    /// <summary>
    /// Gets the element this one's
    /// <see cref="P:System.Windows.Forms.HtmlElement.OffsetRectangle" /> is measured against.
    /// </summary>
    /// <value>
    /// The offset parent, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    public HtmlElement? OffsetParent => _owner.Wrap(Node.OffsetParentHandle);

    /// <summary>
    /// Gets the element's position and size relative to its
    /// <see cref="P:System.Windows.Forms.HtmlElement.OffsetParent" />.
    /// </summary>
    /// <value>
    /// The offset rectangle.
    /// </value>
    public Rectangle OffsetRectangle => Node.OffsetRect;

    /// <summary>
    /// Gets or sets the element's markup, its own tags included.
    /// </summary>
    /// <value>
    /// The outer HTML.
    /// </value>
    public string OuterHtml
    {
        get => Node.OuterHtml;
        set
        {
            Mutate(node => node.OuterHtml = value ?? string.Empty,
                new DomMutation(DomMutationKind.SetOuterHtml, _handle) { Value = value });
        }
    }

    /// <summary>
    /// Gets or sets the element's text, its own tags included.
    /// </summary>
    /// <value>
    /// The outer text.
    /// </value>
    public string OuterText
    {
        get => Node.OuterText;
        set
        {
            Mutate(node => node.OuterText = value ?? string.Empty,
                new DomMutation(DomMutationKind.SetOuterText, _handle) { Value = value });
        }
    }

    /// <summary>
    /// Gets the element that contains this one.
    /// </summary>
    /// <value>
    /// The parent, or
    /// <c>
    /// null
    /// </c> for the document element.
    /// </value>
    public HtmlElement? Parent => _owner.Wrap(Node.ParentHandle);

    /// <summary>
    /// Gets or sets how far the element's content is scrolled horizontally.
    /// </summary>
    /// <value>
    /// The horizontal scroll offset.
    /// </value>
    public int ScrollLeft
    {
        get => Node.ScrollLeft;
        set
        {
            Mutate(node =>
                {
                    node.ScrollLeft = value;
                    node.ScrollRect = new Rectangle(value, node.ScrollRect.Y, node.ScrollRect.Width,
                        node.ScrollRect.Height);
                },
                new DomMutation(DomMutationKind.SetScrollLeft, _handle) { IntValue = value });
        }
    }

    /// <summary>
    /// Gets the element's scroll offset and full scrollable size.
    /// </summary>
    /// <value>
    /// The scroll rectangle.
    /// </value>
    public Rectangle ScrollRectangle => Node.ScrollRect;

    /// <summary>
    /// Gets or sets how far the element's content is scrolled vertically.
    /// </summary>
    /// <value>
    /// The vertical scroll offset.
    /// </value>
    public int ScrollTop
    {
        get => Node.ScrollTop;
        set
        {
            Mutate(node =>
                {
                    node.ScrollTop = value;
                    node.ScrollRect = new Rectangle(node.ScrollRect.X, value, node.ScrollRect.Width,
                        node.ScrollRect.Height);
                },
                new DomMutation(DomMutationKind.SetScrollTop, _handle) { IntValue = value });
        }
    }

    /// <summary>
    /// Gets or sets the element's inline style.
    /// </summary>
    /// <value>
    /// The style declaration.
    /// </value>
    public string Style
    {
        get => Node.Style;
        set
        {
            Mutate(node =>
                {
                    node.Style = value ?? string.Empty;
                    node.Attributes["style"] = value ?? string.Empty;
                },
                new DomMutation(DomMutationKind.SetStyle, _handle) { Value = value });
        }
    }

    /// <summary>
    /// Gets or sets the element's position in the tab order.
    /// </summary>
    /// <value>
    /// The tab index.
    /// </value>
    public short TabIndex
    {
        get => (short)Node.TabIndex;
        set
        {
            Mutate(node => node.TabIndex = value,
                new DomMutation(DomMutationKind.SetTabIndex, _handle) { IntValue = value });
        }
    }

    /// <summary>
    /// Gets the element's tag name.
    /// </summary>
    /// <value>
    /// The tag name, upper-cased as the DOM reports it for HTML documents.
    /// </value>
    public string TagName => Node.TagName;

    /// <summary>
    /// Gets the bridge this element reads and writes through.
    /// </summary>
    /// <value>
    /// The DOM bridge.
    /// </value>
    internal IHtmlDomBridge Bridge => _bridge;

    /// <summary>
    /// Gets the handle addressing this element in the page.
    /// </summary>
    /// <value>
    /// The node handle.
    /// </value>
    internal int Handle => _handle;

    /// <summary>
    /// Gets the element's cached state, or an empty stand-in when the snapshot no longer holds it -
    /// which happens when the page navigated away underneath a retained
    /// <see cref="T:System.Windows.Forms.HtmlElement" />. Reading empty values beats throwing from a
    /// property getter.
    /// </summary>
    /// <value>
    /// The cached node.
    /// </value>
    internal DomNode Node => _bridge.Snapshot.Find(_handle) ?? new DomNode(_handle);

    /// <summary>
    /// Determines whether two elements address the same node.
    /// </summary>
    /// <param name="left">
    /// The first element.
    /// </param>
    /// <param name="right">
    /// The second element.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when both wrap the same node, or both are
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static bool operator ==(HtmlElement? left, HtmlElement? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left._handle == right._handle && ReferenceEquals(left._bridge, right._bridge);
    }

    /// <summary>
    /// Determines whether two elements address different nodes.
    /// </summary>
    /// <param name="left">
    /// The first element.
    /// </param>
    /// <param name="right">
    /// The second element.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when they do not wrap the same node.
    /// </returns>
    public static bool operator !=(HtmlElement? left, HtmlElement? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Adds an element as this element's last child.
    /// </summary>
    /// <param name="newElement">
    /// The element to add.
    /// </param>
    /// <returns>
    /// The element that was added.
    /// </returns>
    public HtmlElement AppendChild(HtmlElement newElement)
    {
        if (newElement == null)
        {
            throw new ArgumentNullException(nameof(newElement));
        }

        var snapshot = _bridge.Snapshot;
        var parent = snapshot.Find(_handle);
        var child = snapshot.Find(newElement._handle);

        if (parent != null && child != null)
        {
            Detach(snapshot, child);
            parent.ChildHandles.Add(child.Handle);
            child.ParentHandle = parent.Handle;
        }

        _bridge.Post(new DomMutation(DomMutationKind.AppendChild, _handle)
        {
            RelatedHandle = newElement._handle
        });

        return newElement;
    }

    /// <summary>
    /// Adds a handler for an arbitrary DOM event on this element.
    /// </summary>
    /// <param name="eventName">
    /// The DOM event name, with or without the
    /// <c>
    /// on
    /// </c> prefix.
    /// </param>
    /// <param name="eventHandler">
    /// The handler to add.
    /// </param>
    public void AttachEventHandler(string eventName, EventHandler eventHandler)
    {
        var name = HtmlDocument.NormalizeEventName(eventName);
        EnsureListening();
        _attachedHandlers.TryGetValue(name, out var existing);
        _attachedHandlers[name] = (EventHandler)Delegate.Combine(existing, eventHandler);
    }

    /// <summary>
    /// Removes a handler added by
    /// <see cref="M:System.Windows.Forms.HtmlElement.AttachEventHandler(System.String,System.EventHandler)" />.
    /// </summary>
    /// <param name="eventName">
    /// The DOM event name.
    /// </param>
    /// <param name="eventHandler">
    /// The handler to remove.
    /// </param>
    public void DetachEventHandler(string eventName, EventHandler eventHandler)
    {
        var name = HtmlDocument.NormalizeEventName(eventName);
        if (!_attachedHandlers.TryGetValue(name, out var existing))
        {
            return;
        }

        var remaining = (EventHandler?)Delegate.Remove(existing, eventHandler);
        if (remaining == null)
        {
            _attachedHandlers.Remove(name);
        }
        else
        {
            _attachedHandlers[name] = remaining;
        }
    }

    /// <summary>
    /// Determines whether an object addresses the same node as this element.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when it is an
    /// <see cref="T:System.Windows.Forms.HtmlElement" /> over the same node.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return this == obj as HtmlElement;
    }

    /// <summary>
    /// Gives the element keyboard focus.
    /// </summary>
    public void Focus()
    {
        _bridge.Post(new DomMutation(DomMutationKind.Focus, _handle));
    }

    /// <summary>
    /// Reads one of the element's attributes.
    /// </summary>
    /// <param name="attributeName">
    /// The attribute name.
    /// </param>
    /// <returns>
    /// The attribute value, or the empty string when the element does not carry it.
    /// </returns>
    public string GetAttribute(string attributeName)
    {
        return Node.GetAttribute(attributeName);
    }

    /// <summary>
    /// Finds every descendant with the given tag.
    /// </summary>
    /// <param name="tagName">
    /// The tag name, matched case-insensitively.
    /// </param>
    /// <returns>
    /// The matching elements, in document order.
    /// </returns>
    public HtmlElementCollection GetElementsByTagName(string tagName)
    {
        return new HtmlElementCollection(_owner, _bridge,
            _bridge.Snapshot.GetElementsByTagName(_handle, tagName).Select(node => node.Handle));
    }

    /// <summary>
    /// Serves as a hash function for the element, consistent with
    /// <see cref="M:System.Windows.Forms.HtmlElement.Equals(System.Object)" />.
    /// </summary>
    /// <returns>
    /// The hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return _handle;
    }

    /// <summary>
    /// Inserts an element relative to this one.
    /// </summary>
    /// <param name="orient">
    /// Where the new element goes.
    /// </param>
    /// <param name="newElement">
    /// The element to insert.
    /// </param>
    /// <returns>
    /// The inserted element, or
    /// <c>
    /// null
    /// </c> when
    /// <paramref name="orient" /> names a position that does not exist - a sibling position for an
    /// element with no parent.
    /// </returns>
    public HtmlElement? InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
    {
        if (newElement == null)
        {
            throw new ArgumentNullException(nameof(newElement));
        }

        switch (orient)
        {
            case HtmlElementInsertionOrientation.BeforeBegin:
                return InsertSibling(newElement, 0);
            case HtmlElementInsertionOrientation.AfterBegin:
                return InsertFirstChild(newElement);
            case HtmlElementInsertionOrientation.BeforeEnd:
                return AppendChild(newElement);
            case HtmlElementInsertionOrientation.AfterEnd:
                return InsertSibling(newElement, 1);
            default:
                return null;
        }
    }

    /// <summary>
    /// Calls a method on the underlying DOM node.
    /// </summary>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <returns>
    /// Always
    /// <c>
    /// null
    /// </c>. Reading the answer would mean blocking the UI thread on the engine's content process; use
    /// <c>
    /// InvokeMemberAsync
    /// </c> from the
    /// <c>
    /// Gtk.Windows.Extensions
    /// </c> package when the return value matters. This is the most visible deviation from real WinForms
    /// and is recorded in EMULATION-GAPS.md.
    /// </returns>
    public object? InvokeMember(string methodName)
    {
        return InvokeMember(methodName, Array.Empty<object>());
    }

    /// <summary>
    /// Calls a method on the underlying DOM node.
    /// </summary>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <param name="parameter">
    /// The arguments.
    /// </param>
    /// <returns>
    /// Always
    /// <c>
    /// null
    /// </c>, for the reason given on
    /// <see cref="M:System.Windows.Forms.HtmlElement.InvokeMember(System.String)" />.
    /// </returns>
    public object? InvokeMember(string methodName, params object[] parameter)
    {
        // Fire-and-forget: the returned Task is deliberately not awaited, because this member is
        // synchronous and awaiting it here would deadlock the GTK main loop.
        _ = _bridge.InvokeMemberAsync(_handle, methodName, parameter ?? Array.Empty<object>(),
            CancellationToken.None);
        return null;
    }

    /// <summary>
    /// Dispatches an event at the element.
    /// </summary>
    /// <param name="eventName">
    /// The DOM event name.
    /// </param>
    public void RaiseEvent(string eventName)
    {
        _bridge.Post(new DomMutation(DomMutationKind.RaiseEvent, _handle)
        {
            Name = HtmlDocument.NormalizeEventName(eventName)
        });
    }

    /// <summary>
    /// Takes keyboard focus away from the element.
    /// </summary>
    public void RemoveFocus()
    {
        _bridge.Post(new DomMutation(DomMutationKind.Blur, _handle));
    }

    /// <summary>
    /// Scrolls the element into view.
    /// </summary>
    /// <param name="alignWithTop">
    /// <c>
    /// true
    /// </c> to align the element's top edge with the top of the viewport.
    /// </param>
    public void ScrollIntoView(bool alignWithTop)
    {
        _bridge.Post(new DomMutation(DomMutationKind.ScrollIntoView, _handle) { Flag = alignWithTop });
    }

    /// <summary>
    /// Writes one of the element's attributes.
    /// </summary>
    /// <param name="attributeName">
    /// The attribute name.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    public void SetAttribute(string attributeName, string value)
    {
        if (attributeName == null)
        {
            throw new ArgumentNullException(nameof(attributeName));
        }

        Mutate(node =>
            {
                node.Attributes[attributeName] = value ?? string.Empty;
                if (string.Equals(attributeName, "style", StringComparison.OrdinalIgnoreCase))
                {
                    node.Style = value ?? string.Empty;
                }
            },
            new DomMutation(DomMutationKind.SetAttribute, _handle)
            {
                Name = attributeName,
                Value = value
            });
    }

    /// <summary>
    /// Determines whether the element carries an attribute.
    /// </summary>
    /// <param name="name">
    /// The attribute name.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when the attribute is present.
    /// </returns>
    internal bool HasAttribute(string name)
    {
        return Node.HasAttribute(name);
    }

    private static void Detach(DomSnapshot snapshot, DomNode child)
    {
        var currentParent = snapshot.Find(child.ParentHandle);
        currentParent?.ChildHandles.Remove(child.Handle);
        child.ParentHandle = DomNode.NoHandle;
    }

    private void EnsureListening()
    {
        if (_listening)
        {
            return;
        }

        _listening = true;
        _bridge.DomEvent += OnBridgeDomEvent;
    }

    private HtmlElement? InsertFirstChild(HtmlElement newElement)
    {
        var snapshot = _bridge.Snapshot;
        var parent = snapshot.Find(_handle);
        var child = snapshot.Find(newElement._handle);

        if (parent != null && child != null)
        {
            Detach(snapshot, child);
            parent.ChildHandles.Insert(0, child.Handle);
            child.ParentHandle = parent.Handle;
        }

        _bridge.Post(new DomMutation(DomMutationKind.InsertBefore, _handle)
        {
            RelatedHandle = newElement._handle,
            ReferenceHandle = parent != null && parent.ChildHandles.Count > 1 ? parent.ChildHandles[1] : DomNode.NoHandle
        });

        return newElement;
    }

    private HtmlElement? InsertSibling(HtmlElement newElement, int offset)
    {
        var snapshot = _bridge.Snapshot;
        var self = snapshot.Find(_handle);
        var parent = self == null ? null : snapshot.Find(self.ParentHandle);
        if (parent == null)
        {
            return null;
        }

        var child = snapshot.Find(newElement._handle);
        var index = parent.ChildHandles.IndexOf(_handle);
        if (index < 0)
        {
            return null;
        }

        var referenceHandle = DomNode.NoHandle;
        if (child != null)
        {
            Detach(snapshot, child);
            index = parent.ChildHandles.IndexOf(_handle) + offset;
            parent.ChildHandles.Insert(index, child.Handle);
            child.ParentHandle = parent.Handle;
            referenceHandle = index + 1 < parent.ChildHandles.Count
                ? parent.ChildHandles[index + 1]
                : DomNode.NoHandle;
        }

        _bridge.Post(new DomMutation(DomMutationKind.InsertBefore, parent.Handle)
        {
            RelatedHandle = newElement._handle,
            ReferenceHandle = referenceHandle
        });

        return newElement;
    }

    private void Mutate(Action<DomNode> applyToSnapshot, DomMutation mutation)
    {
        var node = _bridge.Snapshot.Find(_handle);
        if (node != null)
        {
            applyToSnapshot(node);
        }

        _bridge.Post(mutation);
    }

    private void OnBridgeDomEvent(object? sender, DomEventArgs e)
    {
        if (e.NodeHandle != _handle)
        {
            return;
        }

        if (_attachedHandlers.TryGetValue(e.EventType, out var attached))
        {
            attached(this, EventArgs.Empty);
        }

        foreach (var key in HtmlDocument.KeysFor(EventKeys, e.EventType))
        {
            if (Events[key] is HtmlElementEventHandler handler)
            {
                handler(this, _owner.CreateEventArgs(e));
            }
        }
    }

    private EventHandlerList Events => _events ??= new EventHandlerList();

    private static readonly Dictionary<object, string> EventKeys = new Dictionary<object, string>();

    private static object Key(string domEventName)
    {
        var key = new object();
        EventKeys[key] = domEventName;
        return key;
    }

    private static readonly object ClickEvent = Key("click");

    private static readonly object DoubleClickEvent = Key("dblclick");

    private static readonly object DragEvent = Key("drag");

    private static readonly object DragEndEvent = Key("dragend");

    private static readonly object DragLeaveEvent = Key("dragleave");

    private static readonly object DragOverEvent = Key("dragover");

    private static readonly object FocusingEvent = Key("focus");

    private static readonly object GotFocusEvent = Key("focus");

    private static readonly object KeyDownEvent = Key("keydown");

    private static readonly object KeyPressEvent = Key("keypress");

    private static readonly object KeyUpEvent = Key("keyup");

    private static readonly object LosingFocusEvent = Key("blur");

    private static readonly object LostFocusEvent = Key("blur");

    private static readonly object MouseDownEvent = Key("mousedown");

    private static readonly object MouseEnterEvent = Key("mouseenter");

    private static readonly object MouseLeaveEvent = Key("mouseleave");

    private static readonly object MouseMoveEvent = Key("mousemove");

    private static readonly object MouseOverEvent = Key("mouseover");

    private static readonly object MouseUpEvent = Key("mouseup");

    /// <summary>
    /// Occurs when the element is clicked.
    /// </summary>
    public event HtmlElementEventHandler Click
    {
        add
        {
            EnsureListening();
            Events.AddHandler(ClickEvent, value);
        }
        remove => Events.RemoveHandler(ClickEvent, value);
    }

    /// <summary>
    /// Occurs when the element is double-clicked.
    /// </summary>
    public event HtmlElementEventHandler DoubleClick
    {
        add
        {
            EnsureListening();
            Events.AddHandler(DoubleClickEvent, value);
        }
        remove => Events.RemoveHandler(DoubleClickEvent, value);
    }

    /// <summary>
    /// Occurs while the element is being dragged.
    /// </summary>
    public event HtmlElementEventHandler Drag
    {
        add
        {
            EnsureListening();
            Events.AddHandler(DragEvent, value);
        }
        remove => Events.RemoveHandler(DragEvent, value);
    }

    /// <summary>
    /// Occurs when a drag of the element finishes.
    /// </summary>
    public event HtmlElementEventHandler DragEnd
    {
        add
        {
            EnsureListening();
            Events.AddHandler(DragEndEvent, value);
        }
        remove => Events.RemoveHandler(DragEndEvent, value);
    }

    /// <summary>
    /// Occurs when a drag leaves the element.
    /// </summary>
    public event HtmlElementEventHandler DragLeave
    {
        add
        {
            EnsureListening();
            Events.AddHandler(DragLeaveEvent, value);
        }
        remove => Events.RemoveHandler(DragLeaveEvent, value);
    }

    /// <summary>
    /// Occurs while a drag is over the element.
    /// </summary>
    public event HtmlElementEventHandler DragOver
    {
        add
        {
            EnsureListening();
            Events.AddHandler(DragOverEvent, value);
        }
        remove => Events.RemoveHandler(DragOverEvent, value);
    }

    /// <summary>
    /// Occurs as the element is about to receive focus.
    /// </summary>
    public event HtmlElementEventHandler Focusing
    {
        add
        {
            EnsureListening();
            Events.AddHandler(FocusingEvent, value);
        }
        remove => Events.RemoveHandler(FocusingEvent, value);
    }

    /// <summary>
    /// Occurs when the element receives focus.
    /// </summary>
    public event HtmlElementEventHandler GotFocus
    {
        add
        {
            EnsureListening();
            Events.AddHandler(GotFocusEvent, value);
        }
        remove => Events.RemoveHandler(GotFocusEvent, value);
    }

    /// <summary>
    /// Occurs when a key is pressed down over the element.
    /// </summary>
    public event HtmlElementEventHandler KeyDown
    {
        add
        {
            EnsureListening();
            Events.AddHandler(KeyDownEvent, value);
        }
        remove => Events.RemoveHandler(KeyDownEvent, value);
    }

    /// <summary>
    /// Occurs when a key is pressed over the element.
    /// </summary>
    public event HtmlElementEventHandler KeyPress
    {
        add
        {
            EnsureListening();
            Events.AddHandler(KeyPressEvent, value);
        }
        remove => Events.RemoveHandler(KeyPressEvent, value);
    }

    /// <summary>
    /// Occurs when a key is released over the element.
    /// </summary>
    public event HtmlElementEventHandler KeyUp
    {
        add
        {
            EnsureListening();
            Events.AddHandler(KeyUpEvent, value);
        }
        remove => Events.RemoveHandler(KeyUpEvent, value);
    }

    /// <summary>
    /// Occurs as the element is about to lose focus.
    /// </summary>
    public event HtmlElementEventHandler LosingFocus
    {
        add
        {
            EnsureListening();
            Events.AddHandler(LosingFocusEvent, value);
        }
        remove => Events.RemoveHandler(LosingFocusEvent, value);
    }

    /// <summary>
    /// Occurs when the element loses focus.
    /// </summary>
    public event HtmlElementEventHandler LostFocus
    {
        add
        {
            EnsureListening();
            Events.AddHandler(LostFocusEvent, value);
        }
        remove => Events.RemoveHandler(LostFocusEvent, value);
    }

    /// <summary>
    /// Occurs when a mouse button is pressed over the element.
    /// </summary>
    public event HtmlElementEventHandler MouseDown
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseDownEvent, value);
        }
        remove => Events.RemoveHandler(MouseDownEvent, value);
    }

    /// <summary>
    /// Occurs when the pointer enters the element.
    /// </summary>
    public event HtmlElementEventHandler MouseEnter
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseEnterEvent, value);
        }
        remove => Events.RemoveHandler(MouseEnterEvent, value);
    }

    /// <summary>
    /// Occurs when the pointer leaves the element.
    /// </summary>
    public event HtmlElementEventHandler MouseLeave
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseLeaveEvent, value);
        }
        remove => Events.RemoveHandler(MouseLeaveEvent, value);
    }

    /// <summary>
    /// Occurs when the pointer moves over the element.
    /// </summary>
    public event HtmlElementEventHandler MouseMove
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseMoveEvent, value);
        }
        remove => Events.RemoveHandler(MouseMoveEvent, value);
    }

    /// <summary>
    /// Occurs when the pointer moves onto the element.
    /// </summary>
    public event HtmlElementEventHandler MouseOver
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseOverEvent, value);
        }
        remove => Events.RemoveHandler(MouseOverEvent, value);
    }

    /// <summary>
    /// Occurs when a mouse button is released over the element.
    /// </summary>
    public event HtmlElementEventHandler MouseUp
    {
        add
        {
            EnsureListening();
            Events.AddHandler(MouseUpEvent, value);
        }
        remove => Events.RemoveHandler(MouseUpEvent, value);
    }
}
