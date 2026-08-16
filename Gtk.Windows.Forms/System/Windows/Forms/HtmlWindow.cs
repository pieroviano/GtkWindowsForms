//
// HtmlWindow.cs
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
using System.Linq;
using GtkSystem.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Represents the window that contains an HTML document.
/// </summary>
/// <remarks>
/// One
/// <see cref="T:System.Windows.Forms.WebBrowser" /> hosts exactly one engine view, so this window is
/// always the top-level one and always open. The members that would need a second view - the
/// <c>
/// Open
/// </c> family - throw rather than silently navigate the current window somewhere the caller did not ask
/// for.
/// </remarks>
public sealed class HtmlWindow
{
    private readonly IHtmlDomBridge _bridge;

    private readonly HtmlDocument _document;

    private readonly Dictionary<string, EventHandler> _attachedHandlers =
        new Dictionary<string, EventHandler>(StringComparer.OrdinalIgnoreCase);

    private readonly WebBrowser _owner;

    private EventHandlerList? _events;

    private bool _listening;

    internal HtmlWindow(WebBrowser owner, HtmlDocument document, IHtmlDomBridge bridge)
    {
        _owner = owner;
        _document = document;
        _bridge = bridge;
    }

    /// <summary>
    /// Gets the document this window contains.
    /// </summary>
    /// <value>
    /// The document.
    /// </value>
    public HtmlDocument Document => _document;

    /// <summary>
    /// Always throws.
    /// </summary>
    /// <value>
    /// Never returns.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The engines behind this control run the page in a separate process, so there is no
    /// in-process window object to hand out.
    /// </exception>
    public object DomWindow =>
        throw new NotSupportedException(
            "There is no in-process window object to return: WebKitGTK and WebView2 both run the page in " +
            "a separate process. Use the members of HtmlWindow, or InvokeScriptAsync from " +
            "Gtk.Windows.Extensions, to reach the live window.");

    /// <summary>
    /// Gets the frames this window contains.
    /// </summary>
    /// <value>
    /// The frames. Always empty: the snapshot covers the main frame only, because a frame in another
    /// origin cannot be walked from the page's own script.
    /// </value>
    public HtmlWindowCollection Frames => new HtmlWindowCollection(Array.Empty<HtmlWindow>());

    /// <summary>
    /// Gets the window's navigation history.
    /// </summary>
    /// <value>
    /// The history.
    /// </value>
    public HtmlHistory History => new HtmlHistory(_owner);

    /// <summary>
    /// Gets a value indicating whether the window has been closed.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// false
    /// </c>: the window lives as long as the control does.
    /// </value>
    public bool IsClosed => false;

    /// <summary>
    /// Gets or sets the window's name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets the window that opened this one.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// null
    /// </c>: a control hosts one view, which nothing else opened.
    /// </value>
    public HtmlWindow? Opener => null;

    /// <summary>
    /// Gets the window that contains this one.
    /// </summary>
    /// <value>
    /// This window: it is always the top-level one.
    /// </value>
    public HtmlWindow Parent => this;

    /// <summary>
    /// Gets the window's position on screen.
    /// </summary>
    /// <value>
    /// The hosting control's location.
    /// </value>
    public Point Position => _owner.Location;

    /// <summary>
    /// Gets or sets the window's size.
    /// </summary>
    /// <value>
    /// The hosting control's size. Setting is ignored: the engine view is sized by the control's layout,
    /// not by the page.
    /// </value>
    public Size Size
    {
        get => _owner.Size;
        set
        {
        }
    }

    /// <summary>
    /// Gets or sets the text the engine offered for a status bar.
    /// </summary>
    /// <value>
    /// The status text. Setting is ignored: the control has no status bar of its own to write to.
    /// </value>
    public string StatusBarText
    {
        get => _owner.StatusText;
        set
        {
        }
    }

    /// <summary>
    /// Gets the URL of the document this window contains.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    public Uri? Url => _document.Url;

    /// <summary>
    /// Gets the element that hosts this window in its parent document.
    /// </summary>
    /// <value>
    /// The document element, since this window is never hosted in a frame.
    /// </value>
    public HtmlElement? WindowFrameElement => _document.Wrap(_document.Snapshot.DocumentElementHandle);

    /// <summary>
    /// Determines whether two windows wrap the same view.
    /// </summary>
    /// <param name="left">
    /// The first window.
    /// </param>
    /// <param name="right">
    /// The second window.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when both wrap the same view, or both are
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public static bool operator ==(HtmlWindow? left, HtmlWindow? right)
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
    /// Determines whether two windows wrap different views.
    /// </summary>
    /// <param name="left">
    /// The first window.
    /// </param>
    /// <param name="right">
    /// The second window.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when they do not wrap the same view.
    /// </returns>
    public static bool operator !=(HtmlWindow? left, HtmlWindow? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Shows a message box.
    /// </summary>
    /// <param name="message">
    /// The message to show.
    /// </param>
    public void Alert(string message)
    {
        MessageBox.Show(message, "Alert");
    }

    /// <summary>
    /// Adds a handler for an arbitrary DOM event on the window.
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
    /// Closes the window.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The window is the control's own engine view; closing it would leave the control hosting
    /// nothing. Dispose the control instead.
    /// </exception>
    public void Close()
    {
        throw new NotSupportedException(
            "The window cannot be closed: it is the WebBrowser control's own engine view. Dispose the " +
            "control instead.");
    }

    /// <summary>
    /// Shows a confirmation message box.
    /// </summary>
    /// <param name="message">
    /// The message to show.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when the user confirmed.
    /// </returns>
    public bool Confirm(string message)
    {
        return MessageBox.Show(message, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) ==
               DialogResult.OK;
    }

    /// <summary>
    /// Removes a handler added by
    /// <see cref="M:System.Windows.Forms.HtmlWindow.AttachEventHandler(System.String,System.EventHandler)" />.
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
    /// Determines whether an object wraps the same view as this window.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when it is an
    /// <see cref="T:System.Windows.Forms.HtmlWindow" /> over the same view.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return this == obj as HtmlWindow;
    }

    /// <summary>
    /// Gives the window keyboard focus.
    /// </summary>
    public void Focus()
    {
        _owner.Focus();
    }

    /// <summary>
    /// Serves as a hash function for the window, consistent with
    /// <see cref="M:System.Windows.Forms.HtmlWindow.Equals(System.Object)" />.
    /// </summary>
    /// <returns>
    /// The hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return _bridge.GetHashCode();
    }

    /// <summary>
    /// Moves the window.
    /// </summary>
    /// <param name="point">
    /// The new position.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The view's position is the hosting control's; move the control.
    /// </exception>
    public void MoveTo(Point point)
    {
        MoveTo(point.X, point.Y);
    }

    /// <summary>
    /// Moves the window.
    /// </summary>
    /// <param name="x">
    /// The new horizontal position.
    /// </param>
    /// <param name="y">
    /// The new vertical position.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The view's position is the hosting control's; move the control.
    /// </exception>
    public void MoveTo(int x, int y)
    {
        throw new NotSupportedException(
            "The engine view cannot be moved independently of the WebBrowser control that hosts it. Set " +
            "the control's Location instead.");
    }

    /// <summary>
    /// Navigates the window.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate.
    /// </param>
    public void Navigate(string urlString)
    {
        _owner.Navigate(urlString);
    }

    /// <summary>
    /// Navigates the window.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    public void Navigate(Uri url)
    {
        _owner.Navigate(url);
    }

    /// <summary>
    /// Opens a new window.
    /// </summary>
    /// <param name="url">
    /// Where the new window should navigate.
    /// </param>
    /// <param name="target">
    /// The target name.
    /// </param>
    /// <param name="windowOptions">
    /// The window options.
    /// </param>
    /// <param name="replaceEntry">
    /// Whether to replace the current history entry.
    /// </param>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. See
    /// <see cref="M:System.Windows.Forms.HtmlWindow.Open(System.String,System.String,System.String,System.Boolean)" />.
    /// </exception>
    public HtmlWindow Open(Uri url, string target, string windowOptions, bool replaceEntry)
    {
        return Open(url?.ToString() ?? string.Empty, target, windowOptions, replaceEntry);
    }

    /// <summary>
    /// Opens a new window.
    /// </summary>
    /// <param name="urlString">
    /// Where the new window should navigate.
    /// </param>
    /// <param name="target">
    /// The target name.
    /// </param>
    /// <param name="windowOptions">
    /// The window options.
    /// </param>
    /// <param name="replaceEntry">
    /// Whether to replace the current history entry.
    /// </param>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. A second window would need a second engine view under one control, which neither
    /// WebKitGTK nor WebView2 provides. mono navigated the current window instead, which silently sends
    /// the user somewhere they did not ask to go; handle
    /// <see cref="E:System.Windows.Forms.WebBrowser.NewWindow" /> and open a second
    /// <see cref="T:System.Windows.Forms.WebBrowser" /> yourself.
    /// </exception>
    public HtmlWindow Open(string urlString, string target, string windowOptions, bool replaceEntry)
    {
        throw new NotSupportedException(
            "A second window cannot be opened: one WebBrowser control hosts exactly one engine view, and " +
            "neither WebKitGTK nor WebView2 offers a second one under it. Handle WebBrowser.NewWindow and " +
            "host another WebBrowser instead.");
    }

    /// <summary>
    /// Opens a new window.
    /// </summary>
    /// <param name="urlString">
    /// Where the new window should navigate.
    /// </param>
    /// <param name="windowOptions">
    /// The window options.
    /// </param>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. See
    /// <see cref="M:System.Windows.Forms.HtmlWindow.Open(System.String,System.String,System.String,System.Boolean)" />.
    /// </exception>
    public HtmlWindow OpenNew(string urlString, string windowOptions)
    {
        return Open(urlString, "_blank", windowOptions, false);
    }

    /// <summary>
    /// Opens a new window.
    /// </summary>
    /// <param name="url">
    /// Where the new window should navigate.
    /// </param>
    /// <param name="windowOptions">
    /// The window options.
    /// </param>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. See
    /// <see cref="M:System.Windows.Forms.HtmlWindow.Open(System.String,System.String,System.String,System.Boolean)" />.
    /// </exception>
    public HtmlWindow OpenNew(Uri url, string windowOptions)
    {
        return OpenNew(url?.ToString() ?? string.Empty, windowOptions);
    }

    /// <summary>
    /// Shows a prompt.
    /// </summary>
    /// <param name="message">
    /// The message to show.
    /// </param>
    /// <param name="defaultInputValue">
    /// The value the prompt starts with.
    /// </param>
    /// <returns>
    /// The value the user entered. There is no prompt dialog in this library, so the default is returned
    /// after showing the message; recorded in EMULATION-GAPS.md.
    /// </returns>
    public string Prompt(string message, string defaultInputValue)
    {
        MessageBox.Show(message, "Prompt");
        return defaultInputValue;
    }

    /// <summary>
    /// Takes keyboard focus away from the window.
    /// </summary>
    public void RemoveFocus()
    {
        _bridge.Post(new DomMutation(DomMutationKind.Blur, _document.Snapshot.ActiveElementHandle));
    }

    /// <summary>
    /// Resizes the window.
    /// </summary>
    /// <param name="size">
    /// The new size.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The view's size is the hosting control's; resize the control.
    /// </exception>
    public void ResizeTo(Size size)
    {
        ResizeTo(size.Width, size.Height);
    }

    /// <summary>
    /// Resizes the window.
    /// </summary>
    /// <param name="width">
    /// The new width.
    /// </param>
    /// <param name="height">
    /// The new height.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. The view's size is the hosting control's; resize the control.
    /// </exception>
    public void ResizeTo(int width, int height)
    {
        throw new NotSupportedException(
            "The engine view cannot be resized independently of the WebBrowser control that hosts it. Set " +
            "the control's Size instead.");
    }

    /// <summary>
    /// Scrolls the window.
    /// </summary>
    /// <param name="point">
    /// The new scroll offset.
    /// </param>
    public void ScrollTo(Point point)
    {
        ScrollTo(point.X, point.Y);
    }

    /// <summary>
    /// Scrolls the window.
    /// </summary>
    /// <param name="x">
    /// The new horizontal scroll offset.
    /// </param>
    /// <param name="y">
    /// The new vertical scroll offset.
    /// </param>
    public void ScrollTo(int x, int y)
    {
        var body = _document.Body;
        if (body == null)
        {
            return;
        }

        body.ScrollLeft = x;
        body.ScrollTop = y;
    }

    /// <summary>
    /// Raises the window's
    /// <see cref="E:System.Windows.Forms.HtmlWindow.Error" /> event.
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
    internal void RaiseError(string description, int lineNumber, Uri? url)
    {
        if (Events[ErrorEvent] is HtmlElementErrorEventHandler handler)
        {
            handler(this, new HtmlElementErrorEventArgs(description, lineNumber, url));
        }
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

        foreach (var key in HtmlDocument.KeysFor(EventKeys, e.EventType))
        {
            if (Events[key] is HtmlElementEventHandler handler)
            {
                handler(this, _document.CreateEventArgs(e));
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

    private static readonly object ErrorEvent = new object();

    private static readonly object GotFocusEvent = Key("focus");

    private static readonly object LoadEvent = Key("load");

    private static readonly object LostFocusEvent = Key("blur");

    private static readonly object ResizeEvent = Key("resize");

    private static readonly object ScrollEvent = Key("scroll");

    private static readonly object UnloadEvent = Key("unload");

    /// <summary>
    /// Occurs when a script in the page raises an error.
    /// </summary>
    public event HtmlElementErrorEventHandler Error
    {
        add
        {
            EnsureListening();
            Events.AddHandler(ErrorEvent, value);
        }
        remove => Events.RemoveHandler(ErrorEvent, value);
    }

    /// <summary>
    /// Occurs when the window receives focus.
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
    /// Occurs when the document in the window finishes loading.
    /// </summary>
    public event HtmlElementEventHandler Load
    {
        add
        {
            EnsureListening();
            Events.AddHandler(LoadEvent, value);
        }
        remove => Events.RemoveHandler(LoadEvent, value);
    }

    /// <summary>
    /// Occurs when the window loses focus.
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
    /// Occurs when the window is resized.
    /// </summary>
    public event HtmlElementEventHandler Resize
    {
        add
        {
            EnsureListening();
            Events.AddHandler(ResizeEvent, value);
        }
        remove => Events.RemoveHandler(ResizeEvent, value);
    }

    /// <summary>
    /// Occurs when the window is scrolled.
    /// </summary>
    public event HtmlElementEventHandler Scroll
    {
        add
        {
            EnsureListening();
            Events.AddHandler(ScrollEvent, value);
        }
        remove => Events.RemoveHandler(ScrollEvent, value);
    }

    /// <summary>
    /// Occurs when the document in the window is unloaded.
    /// </summary>
    public event HtmlElementEventHandler Unload
    {
        add
        {
            EnsureListening();
            Events.AddHandler(UnloadEvent, value);
        }
        remove => Events.RemoveHandler(UnloadEvent, value);
    }
}
