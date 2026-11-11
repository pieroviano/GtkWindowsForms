using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;
using Microsoft.CSharp.RuntimeBinder;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>State the control reports through events, and the <see cref="IWebViewBackend" /> properties
/// that read it.</summary>
public sealed partial class InternetExplorerBackend
{
    private bool _allowDrop = true;

    private bool _contextMenuEnabled = true;

    private bool _scrollBarsEnabled = true;

    private bool _shortcutsEnabled = true;

    private bool _scriptErrorsSuppressed;

    internal bool CanGoBackState;

    internal bool CanGoForwardState;

    internal bool IsBusyState;

    internal int ProgressState;

    internal WebBrowserReadyState ReadyStateValue = WebBrowserReadyState.Uninitialized;

    internal string StatusTextValue = string.Empty;

    internal string TitleValue = string.Empty;

    /// <inheritdoc />
    public bool AllowDrop
    {
        get => _allowDrop;
        set => _allowDrop = value;
    }

    /// <inheritdoc />
    public bool CanGoBack => CanGoBackState;

    /// <inheritdoc />
    public bool CanGoForward => CanGoForwardState;

    /// <inheritdoc />
    /// <remarks>Stored but not enforced: Trident has no single switch for its context menu.</remarks>
    public bool ContextMenuEnabled
    {
        get => _contextMenuEnabled;
        set => _contextMenuEnabled = value;
    }

    /// <inheritdoc />
    public string DocumentTitle => TitleValue;

    /// <inheritdoc />
    /// <remarks>
    /// Trident reports a security zone, not a cipher strength, so the Internet-Explorer-era tiers this
    /// enumeration names cannot be reproduced exactly. An <c>https</c> document is reported as
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Bit128" /> and anything else as
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Insecure" />.
    /// </remarks>
    public WebBrowserEncryptionLevel EncryptionLevel =>
        Url != null && string.Equals(Url.Scheme, "https", StringComparison.OrdinalIgnoreCase)
            ? WebBrowserEncryptionLevel.Bit128
            : WebBrowserEncryptionLevel.Insecure;

    /// <inheritdoc />
    public bool IsBusy => IsBusyState;

    /// <inheritdoc />
    public int Progress => IsBusyState ? ProgressState : 100;

    /// <inheritdoc />
    public WebBrowserReadyState ReadyState => ReadyStateValue;

    /// <inheritdoc />
    public bool ScriptErrorsSuppressed
    {
        get => _scriptErrorsSuppressed;
        set
        {
            _scriptErrorsSuppressed = value;
            ApplySilent(value);
        }
    }

    /// <inheritdoc />
    /// <remarks>Trident has no setting for this; applied as injected CSS, as the WebView2 backend does.</remarks>
    public bool ScrollBarsEnabled
    {
        get => _scrollBarsEnabled;
        set
        {
            _scrollBarsEnabled = value;
            ApplyScrollBars(value);
        }
    }

    /// <inheritdoc />
    /// <remarks>Stored but not enforced: Trident routes its own accelerators.</remarks>
    public bool ShortcutsEnabled
    {
        get => _shortcutsEnabled;
        set => _shortcutsEnabled = value;
    }

    /// <inheritdoc />
    public string StatusText => StatusTextValue;

    /// <inheritdoc />
    public Uri? Url
    {
        get
        {
            var raw = TryGetLocationUrl();
            return Uri.TryCreate(raw, UriKind.Absolute, out var url) ? url : null;
        }
    }

    private void ApplyScrollBars(bool enabled)
    {
        _bridge.Channel.Execute(enabled
            ? "(function(){var s=document.getElementById('__gtkwfNoScroll'); if (s) { s.parentNode.removeChild(s); }})()"
            : "(function(){if(document.getElementById('__gtkwfNoScroll')){return;}" +
              "var s=document.createElement('style');s.id='__gtkwfNoScroll';" +
              "s.styleSheet?(s.styleSheet.cssText='html,body{overflow:hidden}'):(s.appendChild(document.createTextNode('html,body{overflow:hidden}')));" +
              "(document.head||document.documentElement).appendChild(s);})()");
    }

    private void ApplySilent(bool suppress)
    {
        try
        {
            _com.Send(() =>
            {
                var browser = Browser;
                if (browser != null)
                {
                    browser.Silent = suppress;
                }
            });
        }
        catch (Exception exception) when (exception is COMException or RuntimeBinderException or
            System.Reflection.TargetInvocationException)
        {
            // The control is gone or does not expose the member; nothing to apply.
        }
    }

    private string? TryGetLocationUrl()
    {
        try
        {
            return _com.Send<string?>(() =>
            {
                var browser = Browser;
                return browser == null ? null : browser.LocationURL as string;
            });
        }
        catch (Exception exception) when (exception is COMException or RuntimeBinderException or
            System.Reflection.TargetInvocationException)
        {
            return null;
        }
    }
}
