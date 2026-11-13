// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Gtk.Windows.Forms.WebBrowsing;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// The GTK widget behind
/// <see cref="T:System.Windows.Forms.WebBrowser" />.
/// </summary>
/// <remarks>
/// Named
/// <c>
/// WebViewBase
/// </c> and not
/// <c>
/// WebBrowserBase
/// </c>, although every other host widget here is named after the control it backs:
/// <see cref="T:System.Windows.Forms.WebBrowserBase" /> is a public WinForms type, so the two would be
/// distinguished only by namespace in files that already import this one.
/// </remarks>
/// <seealso cref="Gtk.Box" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class WebViewBase : Gtk.Box, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.WebViewBase" /> class, choosing the web
    /// engine through
    /// <see cref="Gtk.Windows.Forms.WebBrowsing.WebViewBackendFactory.Create" />.
    /// </summary>
    public WebViewBase() : base(Gtk.Orientation.Vertical, 0)
    {
        Override = new GtkControlOverride(this);
        StyleContext.AddClass("WebBrowser");

        Backend = WebViewBackendFactory.Create();

        var child = Backend.Widget;
        child.Hexpand = true;
        child.Vexpand = true;
        this.AddChild(child);
    }

    /// <summary>
    /// Gets the engine driving this widget.
    /// </summary>
    /// <value>
    /// The backend.
    /// </value>
    public IWebViewBackend Backend
    {
        get;
    }

    /// <inheritdoc />
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Releases the backend along with the widget, so that the engine's out-of-process resources go with
    /// the control rather than outliving it.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> when called from
    /// <see cref="M:System.IDisposable.Dispose" />.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Backend.Dispose();
        }

        base.Dispose(disposing);
    }
}
