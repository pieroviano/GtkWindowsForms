using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Gtk.Windows.Forms.WebBrowsing;
using Microsoft.CSharp.RuntimeBinder;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>The navigation commands, routed to <c>IWebBrowser2</c> through late binding.</summary>
public sealed partial class InternetExplorerBackend
{
    private const int RefreshNormal = 1;

    private const int RefreshCompletely = 3;

    private string? _pendingHtml;

    /// <inheritdoc />
    public void GoBack() => Drive(browser => browser.GoBack());

    /// <inheritdoc />
    public void GoForward() => Drive(browser => browser.GoForward());

    /// <inheritdoc />
    public void Reload(bool bypassCache)
    {
        object level = bypassCache ? RefreshCompletely : RefreshNormal;
        Drive(browser => browser.Refresh2(ref level));
    }

    /// <inheritdoc />
    public void Stop() => Drive(browser => browser.Stop());

    /// <inheritdoc />
    public void LoadHtml(string html, Uri? baseUri)
    {
        // Trident has no "navigate to a string". Load a blank document, then write the markup into it once
        // that document completes (handled in the document-completed event).
        _pendingHtml = html ?? string.Empty;
        Navigate(new Uri("about:blank"), null, null, null);
    }

    /// <inheritdoc />
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        var target = url.ToString();
        Drive(browser =>
        {
            if (postData == null && string.IsNullOrEmpty(additionalHeaders))
            {
                browser.Navigate2(target);
                return;
            }

            object flags = 0;
            object frame = targetFrameName ?? string.Empty;
            object body = (object?)postData ?? Type.Missing;
            object headers = additionalHeaders ?? string.Empty;
            browser.Navigate2(target, ref flags, ref frame, ref body, ref headers);
        });
    }

    /// <summary>Writes the HTML queued by <see cref="LoadHtml" /> into the just-loaded blank document.</summary>
    private void FlushPendingHtml()
    {
        if (_pendingHtml == null)
        {
            return;
        }

        var literal = JavaScriptLiteral.Quote(_pendingHtml);
        _pendingHtml = null;
        _bridge.Channel.Execute("document.open();document.write(" + literal + ");document.close();");
    }

    /// <summary>Queues work until the control is ready, then runs it against the automation object, guarded.</summary>
    private void Drive(global::System.Action<dynamic> action)
    {
        Post(() =>
        {
            var browser = Browser;
            if (browser == null)
            {
                return;
            }

            try
            {
                _com.Send(() => action(browser));
            }
            catch (Exception exception) when (exception is COMException or RuntimeBinderException or
                System.Reflection.TargetInvocationException)
            {
                global::System.Diagnostics.GtkLogger.Debug("An Internet Explorer command failed.", exception);
            }
        });
    }
}
