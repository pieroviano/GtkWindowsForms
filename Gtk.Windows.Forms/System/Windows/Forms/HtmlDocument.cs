//
// HtmlDocument.cs
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
// The Mozilla/XPCOM DOM this originally addressed (Mono.WebBrowser.DOM.IDocument) is gone; every member
// now reads the cached DomSnapshot and writes through DomMutation. See EMULATION-GAPS.md.
//

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using Gtk.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Provides top-level programmatic access to an HTML document hosted by a
/// <see cref="T:System.Windows.Forms.WebBrowser" /> control.
/// </summary>
/// <remarks>
/// Backed by the cached
/// <see cref="Gtk.Windows.Forms.WebBrowsing.DomSnapshot" /> rather than by a live DOM: see
/// <see cref="T:System.Windows.Forms.HtmlElement" /> for what that costs and EMULATION-GAPS.md for the
/// recorded deviation.
/// </remarks>
public sealed class HtmlDocument
{
    private readonly Dictionary<int, HtmlElement> _elements = new Dictionary<int, HtmlElement>();

    private readonly Dictionary<string, EventHandler> _attachedHandlers =
        new Dictionary<string, EventHandler>(StringComparer.OrdinalIgnoreCase);

    private readonly IHtmlDomBridge _bridge;

    private readonly WebBrowser _owner;

    private EventHandlerList? _events;

    private bool _listening;

    private HtmlWindow? _window;

    internal HtmlDocument(WebBrowser owner, IHtmlDomBridge bridge)
    {
        _owner = owner;
        _bridge = bridge;
    }

    /// <summary>
    /// Gets the element that currently has focus.
    /// </summary>
    /// <value>
    /// The active element, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    public HtmlElement? ActiveElement => Wrap(Snapshot.ActiveElementHandle);

    /// <summary>
    /// Gets or sets the colour of the document's active links.
    /// </summary>
    /// <value>
    /// The active link colour.
    /// </value>
    public Color ActiveLinkColor
    {
        get => ParseColor(Snapshot.ActiveLinkColor);
        set => SetDocumentColor("alinkColor", value, snapshot => snapshot.ActiveLinkColor = ToCssColor(value));
    }

    /// <summary>
    /// Gets every element in the document.
    /// </summary>
    /// <value>
    /// The elements, in document order.
    /// </value>
    public HtmlElementCollection All =>
        new HtmlElementCollection(this, _bridge, Snapshot.InDocumentOrder().Select(node => node.Handle));

    /// <summary>
    /// Gets or sets the document's background colour.
    /// </summary>
    /// <value>
    /// The background colour.
    /// </value>
    public Color BackColor
    {
        get => ParseColor(Snapshot.BackColor);
        set => SetDocumentColor("bgColor", value, snapshot => snapshot.BackColor = ToCssColor(value));
    }

    /// <summary>
    /// Gets the document's
    /// <c>
    /// body
    /// </c> element.
    /// </summary>
    /// <value>
    /// The body, or
    /// <c>
    /// null
    /// </c> when the document has not been parsed far enough to have one.
    /// </value>
    public HtmlElement? Body => Wrap(Snapshot.BodyHandle);

    /// <summary>
    /// Gets or sets the document's cookie string.
    /// </summary>
    /// <value>
    /// The cookie string.
    /// </value>
    public string Cookie
    {
        get => Snapshot.Cookie;
        set
        {
            Snapshot.Cookie = value ?? string.Empty;
            _bridge.Post(new DomMutation(DomMutationKind.SetDocumentProperty, DomNode.NoHandle)
            {
                Name = "cookie",
                Value = value
            });
        }
    }

    /// <summary>
    /// Gets the character set the document was decoded with.
    /// </summary>
    /// <value>
    /// The character set name.
    /// </value>
    public string DefaultEncoding => Snapshot.Charset;

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
    public object DomDocument =>
        throw new NotSupportedException(
            "There is no in-process DOM object to return: WebKitGTK and WebView2 both run the page in a " +
            "separate process. Use the members of HtmlDocument, or InvokeScriptAsync from " +
            "Gtk.Windows.Extensions, to reach the live document.");

    /// <summary>
    /// Gets the document's domain.
    /// </summary>
    /// <value>
    /// The domain.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// On set. Relaxing the document's domain is not permitted by the DOM Level 2 HTML specification.
    /// </exception>
    public string Domain
    {
        get => Snapshot.Domain;
        set => throw new NotSupportedException(
            "Setting the domain is not supported per the DOM Level 2 HTML specification.");
    }

    /// <summary>
    /// Gets or sets the character set the document declares.
    /// </summary>
    /// <value>
    /// The character set name.
    /// </value>
    public string Encoding
    {
        get => Snapshot.Charset;
        set
        {
            Snapshot.Charset = value ?? string.Empty;
            _bridge.Post(new DomMutation(DomMutationKind.SetDocumentProperty, DomNode.NoHandle)
            {
                Name = "charset",
                Value = value
            });
        }
    }

    /// <summary>
    /// Gets a value indicating whether the document has keyboard focus.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the hosting control is focused.
    /// </value>
    public bool Focused => _owner.Focused;

    /// <summary>
    /// Gets or sets the document's foreground colour.
    /// </summary>
    /// <value>
    /// The foreground colour.
    /// </value>
    public Color ForeColor
    {
        get => ParseColor(Snapshot.ForeColor);
        set => SetDocumentColor("fgColor", value, snapshot => snapshot.ForeColor = ToCssColor(value));
    }

    /// <summary>
    /// Gets the document's
    /// <c>
    /// form
    /// </c> elements.
    /// </summary>
    /// <value>
    /// The forms.
    /// </value>
    public HtmlElementCollection Forms => ByTagName("form");

    /// <summary>
    /// Gets the document's
    /// <c>
    /// img
    /// </c> elements.
    /// </summary>
    /// <value>
    /// The images.
    /// </value>
    public HtmlElementCollection Images => ByTagName("img");

    /// <summary>
    /// Gets or sets the colour of the document's unvisited links.
    /// </summary>
    /// <value>
    /// The link colour.
    /// </value>
    public Color LinkColor
    {
        get => ParseColor(Snapshot.LinkColor);
        set => SetDocumentColor("linkColor", value, snapshot => snapshot.LinkColor = ToCssColor(value));
    }

    /// <summary>
    /// Gets the document's hyperlinks - its
    /// <c>
    /// a
    /// </c> and
    /// <c>
    /// area
    /// </c> elements that carry an
    /// <c>
    /// href
    /// </c>, which is what the DOM's
    /// <c>
    /// document.links
    /// </c> means.
    /// </summary>
    /// <value>
    /// The links.
    /// </value>
    public HtmlElementCollection Links =>
        new HtmlElementCollection(this, _bridge, Snapshot.InDocumentOrder()
            .Where(node => (string.Equals(node.TagName, "a", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(node.TagName, "area", StringComparison.OrdinalIgnoreCase)) &&
                           node.HasAttribute("href"))
            .Select(node => node.Handle));

    /// <summary>
    /// Gets or sets a value indicating whether the document is laid out right to left.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the document element's
    /// <c>
    /// dir
    /// </c> attribute is
    /// <c>
    /// rtl
    /// </c>.
    /// </value>
    public bool RightToLeft
    {
        get
        {
            var root = Wrap(Snapshot.DocumentElementHandle);
            return root != null && string.Equals(root.GetAttribute("dir"), "rtl", StringComparison.OrdinalIgnoreCase);
        }
        set
        {
            var root = Wrap(Snapshot.DocumentElementHandle);
            root?.SetAttribute("dir", value ? "rtl" : "ltr");
        }
    }

    /// <summary>
    /// Gets or sets the document's title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title
    {
        get => Snapshot.Title;
        set
        {
            Snapshot.Title = value ?? string.Empty;
            _bridge.Post(new DomMutation(DomMutationKind.SetDocumentProperty, DomNode.NoHandle)
            {
                Name = "title",
                Value = value
            });
        }
    }

    /// <summary>
    /// Gets the document's URL.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c> when the snapshot carries none.
    /// </value>
    public Uri? Url =>
        Uri.TryCreate(Snapshot.Url, UriKind.Absolute, out var url) ? url : null;

    /// <summary>
    /// Gets or sets the colour of the document's visited links.
    /// </summary>
    /// <value>
    /// The visited link colour.
    /// </value>
    public Color VisitedLinkColor
    {
        get => ParseColor(Snapshot.VisitedLinkColor);
        set => SetDocumentColor("vlinkColor", value, snapshot => snapshot.VisitedLinkColor = ToCssColor(value));
    }

    /// <summary>
    /// Gets the window that contains the document.
    /// </summary>
    /// <value>
    /// The window. The same instance every time, so that a handler attached through one read is still
    /// attached when the property is read again.
    /// </value>
    public HtmlWindow Window => _window ??= new HtmlWindow(_owner, this, _bridge);

    /// <summary>
    /// Gets the document's doctype name.
    /// </summary>
    /// <value>
    /// The doctype name, or the empty string.
    /// </value>
    internal string DocType => Snapshot.DocTypeName;

    /// <summary>
    /// Gets the snapshot every member of this document reads.
    /// </summary>
    /// <value>
    /// The snapshot.
    /// </value>
    internal DomSnapshot Snapshot => _bridge.Snapshot;

    /// <summary>
    /// Determines whether two documents wrap the same page.
    /// </summary>
    /// <param name="left">
    /// The first document.
    /// </param>
    /// <param name="right">
    /// The second document.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when both read the same bridge, or both are
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static bool operator ==(HtmlDocument? left, HtmlDocument? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return ReferenceEquals(left._bridge, right._bridge);
    }

    /// <summary>
    /// Determines whether two documents wrap different pages.
    /// </summary>
    /// <param name="left">
    /// The first document.
    /// </param>
    /// <param name="right">
    /// The second document.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when they do not read the same bridge.
    /// </returns>
    public static bool operator !=(HtmlDocument? left, HtmlDocument? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Adds a handler for an arbitrary DOM event on the document.
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
        var name = NormalizeEventName(eventName);
        EnsureListening();
        _attachedHandlers.TryGetValue(name, out var existing);
        _attachedHandlers[name] = (EventHandler)Delegate.Combine(existing, eventHandler);
    }

    /// <summary>
    /// Creates a new element, detached from the document tree.
    /// </summary>
    /// <param name="elementTag">
    /// The tag name.
    /// </param>
    /// <returns>
    /// The new element, ready to be inserted with
    /// <see cref="M:System.Windows.Forms.HtmlElement.AppendChild(System.Windows.Forms.HtmlElement)" />.
    /// </returns>
    public HtmlElement CreateElement(string elementTag)
    {
        var snapshot = Snapshot;
        var handle = snapshot.AllocateDetachedHandle();
        snapshot.Add(new DomNode(handle) { TagName = (elementTag ?? string.Empty).ToUpperInvariant() });

        _bridge.Post(new DomMutation(DomMutationKind.CreateElement, handle) { Name = elementTag });

        return WrapExisting(handle);
    }

    /// <summary>
    /// Removes a handler added by
    /// <see cref="M:System.Windows.Forms.HtmlDocument.AttachEventHandler(System.String,System.EventHandler)" />.
    /// </summary>
    /// <param name="eventName">
    /// The DOM event name.
    /// </param>
    /// <param name="eventHandler">
    /// The handler to remove.
    /// </param>
    public void DetachEventHandler(string eventName, EventHandler eventHandler)
    {
        var name = NormalizeEventName(eventName);
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
    /// Determines whether an object wraps the same page as this document.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when it is an
    /// <see cref="T:System.Windows.Forms.HtmlDocument" /> over the same page.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return this == obj as HtmlDocument;
    }

    /// <summary>
    /// Executes an editing command against the document.
    /// </summary>
    /// <param name="command">
    /// The command name.
    /// </param>
    /// <param name="showUI">
    /// Whether the engine may show its own user interface.
    /// </param>
    /// <param name="value">
    /// The command argument.
    /// </param>
    public void ExecCommand(string command, bool showUI, object value)
    {
        _ = _bridge.InvokeMemberAsync(DomNode.NoHandle, "execCommand",
            new[] { command, (object)showUI, value }, CancellationToken.None);
    }

    /// <summary>
    /// Gives the document keyboard focus.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void Focus()
    {
        _owner.Focus();
    }

    /// <summary>
    /// Finds the element carrying a given
    /// <c>
    /// id
    /// </c>.
    /// </summary>
    /// <param name="id">
    /// The identifier.
    /// </param>
    /// <returns>
    /// The element, or
    /// <c>
    /// null
    /// </c> when nothing matches.
    /// </returns>
    public HtmlElement? GetElementById(string id)
    {
        var node = Snapshot.GetElementById(id);
        return node == null ? null : WrapExisting(node.Handle);
    }

    /// <summary>
    /// Finds the element at a point in the document.
    /// </summary>
    /// <param name="point">
    /// The point, in document coordinates.
    /// </param>
    /// <returns>
    /// The innermost element whose offset rectangle contains
    /// <paramref name="point" />, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    /// <remarks>
    /// Real WinForms delegates to the engine's own hit test. Here the answer is computed from the cached
    /// geometry instead, because the engine's hit test can only be asked asynchronously: the result is
    /// therefore an approximation of
    /// <c>
    /// document.elementFromPoint
    /// </c> and can be stale. Recorded in EMULATION-GAPS.md.
    /// </remarks>
    public HtmlElement? GetElementFromPoint(Point point)
    {
        DomNode? best = null;
        var bestArea = int.MaxValue;

        foreach (var node in Snapshot.InDocumentOrder())
        {
            var bounds = node.OffsetRect;
            if (bounds.Width <= 0 || bounds.Height <= 0 || !bounds.Contains(point))
            {
                continue;
            }

            var area = bounds.Width * bounds.Height;
            if (area <= bestArea)
            {
                bestArea = area;
                best = node;
            }
        }

        return best == null ? null : WrapExisting(best.Handle);
    }

    /// <summary>
    /// Finds every element with a given tag.
    /// </summary>
    /// <param name="tagName">
    /// The tag name, matched case-insensitively.
    /// </param>
    /// <returns>
    /// The matching elements, in document order.
    /// </returns>
    public HtmlElementCollection GetElementsByTagName(string tagName)
    {
        return ByTagName(tagName);
    }

    /// <summary>
    /// Serves as a hash function for the document, consistent with
    /// <see cref="M:System.Windows.Forms.HtmlDocument.Equals(System.Object)" />.
    /// </summary>
    /// <returns>
    /// The hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return _bridge.GetHashCode();
    }

    /// <summary>
    /// Calls a function defined in the page.
    /// </summary>
    /// <param name="scriptName">
    /// The function name.
    /// </param>
    /// <returns>
    /// Always
    /// <c>
    /// null
    /// </c>: reading the answer would mean blocking the UI thread on the engine's content process. Use
    /// <c>
    /// InvokeScriptAsync
    /// </c> from the
    /// <c>
    /// Gtk.Windows.Extensions
    /// </c> package when the return value matters.
    /// </returns>
    public object? InvokeScript(string scriptName)
    {
        return InvokeScript(scriptName, Array.Empty<object>());
    }

    /// <summary>
    /// Calls a function defined in the page.
    /// </summary>
    /// <param name="scriptName">
    /// The function name.
    /// </param>
    /// <param name="args">
    /// The arguments.
    /// </param>
    /// <returns>
    /// Always
    /// <c>
    /// null
    /// </c>, for the reason given on
    /// <see cref="M:System.Windows.Forms.HtmlDocument.InvokeScript(System.String)" />.
    /// </returns>
    public object? InvokeScript(string scriptName, object[] args)
    {
        _ = _bridge.InvokeMemberAsync(DomNode.NoHandle, scriptName, args ?? Array.Empty<object>(),
            CancellationToken.None);
        return null;
    }

    /// <summary>
    /// Replaces the document with a blank one.
    /// </summary>
    /// <param name="replaceInHistory">
    /// <c>
    /// true
    /// </c> to replace the current history entry rather than add one. Neither engine exposes that choice
    /// through its navigation API, so the entry is always added; recorded in EMULATION-GAPS.md.
    /// </param>
    /// <returns>
    /// This document.
    /// </returns>
    public HtmlDocument OpenNew(bool replaceInHistory)
    {
        _owner.Navigate(WebViewBackendFactory.BlankUri);
        return this;
    }

    /// <summary>
    /// Writes markup into the document.
    /// </summary>
    /// <param name="text">
    /// The markup to write.
    /// </param>
    public void Write(string text)
    {
        _bridge.Post(new DomMutation(DomMutationKind.Write, DomNode.NoHandle) { Value = text });
    }

    /// <summary>
    /// Strips the
    /// <c>
    /// on
    /// </c> prefix WinForms callers habitually pass, and lower-cases the rest, so that
    /// <c>
    /// onclick
    /// </c>,
    /// <c>
    /// OnClick
    /// </c> and
    /// <c>
    /// click
    /// </c> all name the same DOM event.
    /// </summary>
    /// <param name="eventName">
    /// The name as the caller wrote it.
    /// </param>
    /// <returns>
    /// The DOM event type.
    /// </returns>
    internal static string NormalizeEventName(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            return string.Empty;
        }

        var name = eventName.Trim();
        if (name.Length > 2 && name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring(2);
        }

        return name.ToLowerInvariant();
    }

    /// <summary>
    /// Lists the event-list keys registered for a DOM event type.
    /// </summary>
    /// <param name="keys">
    /// The key-to-event-type map the caller maintains.
    /// </param>
    /// <param name="eventType">
    /// The DOM event type.
    /// </param>
    /// <returns>
    /// The matching keys.
    /// </returns>
    internal static IEnumerable<object> KeysFor(Dictionary<object, string> keys, string eventType)
    {
        foreach (var pair in keys)
        {
            if (string.Equals(pair.Value, eventType, StringComparison.OrdinalIgnoreCase))
            {
                yield return pair.Key;
            }
        }
    }

    /// <summary>
    /// Builds the WinForms event data for one DOM event, resolving the node handles it carries against
    /// this document.
    /// </summary>
    /// <param name="source">
    /// The event the bridge reported.
    /// </param>
    /// <returns>
    /// The event data.
    /// </returns>
    internal HtmlElementEventArgs CreateEventArgs(DomEventArgs source)
    {
        return new HtmlElementEventArgs(source.EventType, Wrap(source.FromHandle), Wrap(source.ToHandle))
        {
            AltKeyPressed = source.AltKeyPressed,
            CtrlKeyPressed = source.CtrlKeyPressed,
            ShiftKeyPressed = source.ShiftKeyPressed,
            KeyPressedCode = source.KeyPressedCode,
            MouseButtonsPressed = source.Buttons,
            MousePosition = source.MousePosition,
            ClientMousePosition = source.ClientMousePosition,
            OffsetMousePosition = source.OffsetMousePosition
        };
    }

    /// <summary>
    /// Wraps a node handle, returning
    /// <c>
    /// null
    /// </c> when it names no node in the current snapshot.
    /// </summary>
    /// <param name="handle">
    /// The node handle.
    /// </param>
    /// <returns>
    /// The element, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    internal HtmlElement? Wrap(int handle)
    {
        return handle == DomNode.NoHandle || Snapshot.Find(handle) == null ? null : WrapExisting(handle);
    }

    /// <summary>
    /// Wraps a node handle, reusing the wrapper already handed out for it so that two reads of the same
    /// node give reference-equal elements as well as value-equal ones.
    /// </summary>
    /// <param name="handle">
    /// The node handle.
    /// </param>
    /// <returns>
    /// The element.
    /// </returns>
    internal HtmlElement WrapExisting(int handle)
    {
        if (_elements.TryGetValue(handle, out var existing))
        {
            return existing;
        }

        var element = new HtmlElement(this, _bridge, handle);
        _elements[handle] = element;
        return element;
    }

    private static Color ParseColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return Color.Empty;
        }

        var text = color.Trim();

        if (text[0] == '#')
        {
            var digits = text.Substring(1);
            if (digits.Length == 3)
            {
                digits = new string(new[] { digits[0], digits[0], digits[1], digits[1], digits[2], digits[2] });
            }

            if (digits.Length == 6 &&
                int.TryParse(digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
            {
                return Color.FromArgb(255, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
            }

            return Color.Empty;
        }

        if (text.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
        {
            var open = text.IndexOf('(');
            var close = text.LastIndexOf(')');
            if (open >= 0 && close > open)
            {
                var parts = text.Substring(open + 1, close - open - 1).Split(',');
                if (parts.Length >= 3 &&
                    int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var red) &&
                    int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var green) &&
                    int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var blue))
                {
                    return Color.FromArgb(255, red, green, blue);
                }
            }

            return Color.Empty;
        }

        return Color.FromName(text);
    }

    private static string ToCssColor(Color color)
    {
        return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
    }

    private HtmlElementCollection ByTagName(string tagName)
    {
        return new HtmlElementCollection(this, _bridge,
            Snapshot.GetElementsByTagName(Snapshot.DocumentElementHandle, tagName).Select(node => node.Handle));
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

    private void OnBridgeDomEvent(object? sender, DomEventArgs e)
    {
        if (_attachedHandlers.TryGetValue(e.EventType, out var attached))
        {
            attached(this, EventArgs.Empty);
        }

        foreach (var key in KeysFor(EventKeys, e.EventType))
        {
            if (Events[key] is HtmlElementEventHandler handler)
            {
                handler(this, CreateEventArgs(e));
            }
        }
    }

    private void SetDocumentColor(string property, Color value, Action<DomSnapshot> applyToSnapshot)
    {
        applyToSnapshot(Snapshot);
        _bridge.Post(new DomMutation(DomMutationKind.SetDocumentProperty, DomNode.NoHandle)
        {
            Name = property,
            Value = ToCssColor(value)
        });
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

    private static readonly object ContextMenuShowingEvent = Key("contextmenu");

    private static readonly object FocusingEvent = Key("focus");

    private static readonly object LosingFocusEvent = Key("blur");

    private static readonly object MouseDownEvent = Key("mousedown");

    private static readonly object MouseLeaveEvent = Key("mouseleave");

    private static readonly object MouseMoveEvent = Key("mousemove");

    private static readonly object MouseOverEvent = Key("mouseover");

    private static readonly object MouseUpEvent = Key("mouseup");

    private static readonly object StopEvent = Key("stop");

    /// <summary>
    /// Occurs when the document is clicked.
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
    /// Occurs when the engine is about to show its context menu.
    /// </summary>
    public event HtmlElementEventHandler ContextMenuShowing
    {
        add
        {
            EnsureListening();
            Events.AddHandler(ContextMenuShowingEvent, value);
        }
        remove => Events.RemoveHandler(ContextMenuShowingEvent, value);
    }

    /// <summary>
    /// Occurs as the document is about to receive focus.
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
    /// Occurs as the document is about to lose focus.
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
    /// Occurs when a mouse button is pressed over the document.
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
    /// Occurs when the pointer leaves the document.
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
    /// Occurs when the pointer moves over the document.
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
    /// Occurs when the pointer moves onto the document.
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
    /// Occurs when a mouse button is released over the document.
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

    /// <summary>
    /// Occurs when the document's loading is stopped.
    /// </summary>
    public event HtmlElementEventHandler Stop
    {
        add
        {
            EnsureListening();
            Events.AddHandler(StopEvent, value);
        }
        remove => Events.RemoveHandler(StopEvent, value);
    }
}
