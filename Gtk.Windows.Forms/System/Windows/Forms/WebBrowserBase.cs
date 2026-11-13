//
// WebBrowserBase.cs
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
// The ActiveX host this originally wrapped is gone: the engine is reached through IWebViewBackend, and
// the widget that occupies the control's area is a WebViewBase.
//

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Gtk.Windows.Forms.Controls.ControlBase;
using Gtk.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Wraps the web engine that
/// <see cref="T:System.Windows.Forms.WebBrowser" /> presents.
/// </summary>
/// <remarks>
/// In real WinForms this is the ActiveX host that sites the Internet Explorer control. Here it hosts a
/// <see cref="Gtk.Windows.Forms.Controls.ControlBase.WebViewBase" /> and translates the
/// engine's events into the WinForms ones. The ActiveX-shaped members are kept for surface parity and
/// throw, exactly as they did in mono.
/// </remarks>
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DefaultProperty("Name")]
[DefaultEvent("Enter")]
public class WebBrowserBase : Control
{
    /// <summary>
    /// The GTK widget that occupies this control's area, and through it the web engine.
    /// </summary>
    public readonly WebViewBase self = new WebViewBase();

    private bool _suppressDialogs;

    internal WebBrowserBase()
    {
        self.Override.sender = this;
        status = string.Empty;

        Backend.Navigating += OnBackendNavigating;
        Backend.Navigated += OnBackendNavigated;
        Backend.ProgressChanged += OnBackendProgressChanged;
        Backend.LoadFinished += OnBackendLoadFinished;
        Backend.TitleChanged += OnBackendTitleChanged;
        Backend.StatusChanged += OnBackendStatusChanged;
        Backend.NewWindow += OnBackendNewWindow;
    }

    /// <summary>
    /// Gets the underlying ActiveX control.
    /// </summary>
    /// <value>
    /// Never returns.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here: the engine is WebKitGTK or WebView2.
    /// </exception>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object ActiveXInstance =>
        throw new NotSupportedException(
            "There is no ActiveX control to return: this WebBrowser hosts WebKitGTK or WebView2, not the " +
            "Internet Explorer ActiveX control.");

    /// <summary>
    /// Gets or sets a value indicating whether the control accepts dropped content.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when drops are allowed.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set
        {
            base.AllowDrop = value;
            Backend.AllowDrop = value;
        }
    }

    /// <summary>
    /// Gets or sets the control's background colour.
    /// </summary>
    /// <value>
    /// The background colour. The page paints the whole control, so this has no visible effect.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    /// <summary>
    /// Gets or sets the control's background image.
    /// </summary>
    /// <value>
    /// The background image. The page paints the whole control, so this has no visible effect.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    /// <summary>
    /// Gets or sets how the control's background image is laid out.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    /// <summary>
    /// Gets the control's cursor.
    /// </summary>
    /// <value>
    /// The cursor.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// On set. The page chooses its own cursor.
    /// </exception>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Cursor? Cursor
    {
        get => base.Cursor;
        set => throw new NotSupportedException(
            "The cursor over a WebBrowser is chosen by the page, not by the hosting control.");
    }

    /// <summary>
    /// Gets a value indicating whether the control is enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the control is enabled.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// On set. Disabling the host would not disable the engine view inside it.
    /// </exception>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool Enabled
    {
        get => base.Enabled;
        set => throw new NotSupportedException("A WebBrowser cannot be disabled.");
    }

    /// <summary>
    /// Gets or sets the control's font.
    /// </summary>
    /// <value>
    /// The font. The page chooses its own fonts.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    /// <summary>
    /// Gets or sets the control's foreground colour.
    /// </summary>
    /// <value>
    /// The foreground colour. The page chooses its own colours.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    /// <summary>
    /// Gets the GTK widget behind this control.
    /// </summary>
    /// <value>
    /// The
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.WebViewBase" /> hosting the engine.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the control's input method editor mode.
    /// </summary>
    /// <value>
    /// The IME mode.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    /// <summary>
    /// Gets or sets whether the control's content is laid out right to left.
    /// </summary>
    /// <value>
    /// The reading order.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Localizable(false)]
    public new virtual RightToLeft RightToLeft
    {
        get => base.RightToLeft;
        set => base.RightToLeft = value;
    }

    /// <summary>
    /// Gets or sets the control's site.
    /// </summary>
    /// <value>
    /// The site.
    /// </value>
    public override ISite? Site
    {
        get => base.Site;
        set => base.Site = value;
    }

    /// <summary>
    /// Gets the control's text.
    /// </summary>
    /// <value>
    /// Always the empty string: the control's content is a document, not text.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// On set.
    /// </exception>
    [Bindable(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get => string.Empty;
        set => throw new NotSupportedException(
            "A WebBrowser has no text of its own. Set DocumentText, or navigate to a document.");
    }

    /// <summary>
    /// Gets a value indicating whether the control shows the wait cursor.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the wait cursor is shown.
    /// </value>
    /// <exception cref="T:System.NotSupportedException">
    /// On set. The page chooses its own cursor.
    /// </exception>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool UseWaitCursor
    {
        get => base.UseWaitCursor;
        set => throw new NotSupportedException(
            "The cursor over a WebBrowser is chosen by the page, not by the hosting control.");
    }

    /// <summary>
    /// Gets the control's default size.
    /// </summary>
    /// <value>
    /// 100 by 100 pixels, as in real WinForms.
    /// </value>
    protected override Size DefaultSize => new Size(100, 100);

    /// <summary>
    /// Gets the engine driving this control.
    /// </summary>
    /// <value>
    /// The backend.
    /// </value>
    internal IWebViewBackend Backend => self.Backend;

    /// <summary>
    /// Gets or sets a value indicating whether the engine's own dialogs and script error reports are
    /// suppressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when they are suppressed.
    /// </value>
    internal bool SuppressDialogs
    {
        get => _suppressDialogs;
        set
        {
            _suppressDialogs = value;
            Backend.ScriptErrorsSuppressed = value;
        }
    }

    /// <summary>
    /// A value indicating whether the current document has finished loading.
    /// </summary>
    internal bool documentReady;

    /// <summary>
    /// The text the engine last offered for a status bar.
    /// </summary>
    protected string status = string.Empty;

    /// <summary>
    /// Draws the control onto a bitmap.
    /// </summary>
    /// <param name="bitmap">
    /// The bitmap to draw onto.
    /// </param>
    /// <param name="targetBounds">
    /// Where in the bitmap to draw.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
        base.DrawToBitmap(bitmap, targetBounds);
    }

    /// <summary>
    /// Pre-processes a message.
    /// </summary>
    /// <param name="msg">
    /// The message.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> when the message was consumed.
    /// </returns>
    public override bool PreProcessMessage(ref Message msg)
    {
        return base.PreProcessMessage(ref msg);
    }

    /// <summary>
    /// Attaches to the underlying ActiveX interfaces.
    /// </summary>
    /// <param name="nativeActiveXObject">
    /// The ActiveX object.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here.
    /// </exception>
    protected virtual void AttachInterfaces(object nativeActiveXObject)
    {
        throw new NotSupportedException(
            "There is no ActiveX control to attach to: this WebBrowser hosts WebKitGTK or WebView2.");
    }

    /// <summary>
    /// Creates the ActiveX event sink.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here.
    /// </exception>
    protected virtual void CreateSink()
    {
        throw new NotSupportedException(
            "There is no ActiveX event sink to create: this WebBrowser hosts WebKitGTK or WebView2.");
    }

    /// <summary>
    /// Creates the ActiveX host site.
    /// </summary>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here.
    /// </exception>
    protected virtual WebBrowserSiteBase CreateWebBrowserSiteBase()
    {
        throw new NotSupportedException(
            "There is no ActiveX host site to create: this WebBrowser hosts WebKitGTK or WebView2.");
    }

    /// <summary>
    /// Detaches from the underlying ActiveX interfaces.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here.
    /// </exception>
    protected virtual void DetachInterfaces()
    {
        throw new NotSupportedException(
            "There is no ActiveX control to detach from: this WebBrowser hosts WebKitGTK or WebView2.");
    }

    /// <summary>
    /// Releases the ActiveX event sink.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. There is no ActiveX control here.
    /// </exception>
    protected virtual void DetachSink()
    {
        throw new NotSupportedException(
            "There is no ActiveX event sink to release: this WebBrowser hosts WebKitGTK or WebView2.");
    }

    /// <summary>
    /// Releases the control and the engine behind it.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> when called from
    /// <see cref="M:System.ComponentModel.Component.Dispose" />.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Backend.Navigating -= OnBackendNavigating;
            Backend.Navigated -= OnBackendNavigated;
            Backend.ProgressChanged -= OnBackendProgressChanged;
            Backend.LoadFinished -= OnBackendLoadFinished;
            Backend.TitleChanged -= OnBackendTitleChanged;
            Backend.StatusChanged -= OnBackendStatusChanged;
            Backend.NewWindow -= OnBackendNewWindow;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Raised before a navigation starts.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// The navigation, which the handler may cancel.
    /// </param>
    internal virtual void OnBackendNavigating(object? sender, BackendNavigatingEventArgs e)
    {
    }

    /// <summary>
    /// Raised once a navigation has committed.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// Where the engine navigated.
    /// </param>
    internal virtual void OnBackendNavigated(object? sender, BackendNavigatedEventArgs e)
    {
    }

    /// <summary>
    /// Raised as the current load advances.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// The progress.
    /// </param>
    internal virtual void OnBackendProgressChanged(object? sender, BackendProgressEventArgs e)
    {
    }

    /// <summary>
    /// Raised when a load finishes, whether it succeeded or failed.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// The outcome.
    /// </param>
    internal virtual void OnBackendLoadFinished(object? sender, BackendLoadEventArgs e)
    {
    }

    /// <summary>
    /// Raised when the document title changes.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// Empty.
    /// </param>
    internal virtual void OnBackendTitleChanged(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Raised when the engine's status text changes.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// Empty.
    /// </param>
    internal virtual void OnBackendStatusChanged(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Raised when the page asks to open a window.
    /// </summary>
    /// <param name="sender">
    /// The backend.
    /// </param>
    /// <param name="e">
    /// The request, which the handler may cancel.
    /// </param>
    internal virtual void OnBackendNewWindow(object? sender, BackendNewWindowEventArgs e)
    {
    }

    /// <summary>
    /// Occurs when the control's background colour changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add. The page paints the whole control, so the event could never fire usefully.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackColorChanged
    {
        add => throw new NotSupportedException("Invalid event handler for BackColorChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's background image changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add => throw new NotSupportedException("Invalid event handler for BackgroundImageChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's background image layout changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new NotSupportedException("Invalid event handler for BackgroundImageLayoutChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's binding context changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BindingContextChanged
    {
        add => throw new NotSupportedException("Invalid event handler for BindingContextChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control is clicked. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler Click
    {
        add => throw new NotSupportedException("Invalid event handler for Click");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's cursor changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler CursorChanged
    {
        add => throw new NotSupportedException("Invalid event handler for CursorChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control is double-clicked. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler DoubleClick
    {
        add => throw new NotSupportedException("Invalid event handler for DoubleClick");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's enabled state changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler EnabledChanged
    {
        add => throw new NotSupportedException("Invalid event handler for EnabledChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's font changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler FontChanged
    {
        add => throw new NotSupportedException("Invalid event handler for FontChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's foreground colour changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ForeColorChanged
    {
        add => throw new NotSupportedException("Invalid event handler for ForeColorChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when a key is pressed down. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler KeyDown
    {
        add => throw new NotSupportedException("Invalid event handler for KeyDown");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when a key is pressed. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler KeyPress
    {
        add => throw new NotSupportedException("Invalid event handler for KeyPress");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when a key is released. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler KeyUp
    {
        add => throw new NotSupportedException("Invalid event handler for KeyUp");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when a mouse button is pressed. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler MouseDown
    {
        add => throw new NotSupportedException("Invalid event handler for MouseDown");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the pointer moves. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler MouseMove
    {
        add => throw new NotSupportedException("Invalid event handler for MouseMove");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when a mouse button is released. The page handles its own input.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler MouseUp
    {
        add => throw new NotSupportedException("Invalid event handler for MouseUp");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control is painted. The engine paints its own view.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => throw new NotSupportedException("Invalid event handler for Paint");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's reading order changes.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler RightToLeftChanged
    {
        add => throw new NotSupportedException("Invalid event handler for RightToLeftChanged");
        remove
        {
        }
    }

    /// <summary>
    /// Occurs when the control's text changes. A WebBrowser has no text of its own.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// On add.
    /// </exception>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add => throw new NotSupportedException("Invalid event handler for TextChanged");
        remove
        {
        }
    }
}
