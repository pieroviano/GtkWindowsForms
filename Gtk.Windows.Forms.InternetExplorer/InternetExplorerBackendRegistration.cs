using Gtk.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Puts the Internet Explorer backend into <see cref="WebViewBackendFactory" />.
/// </summary>
/// <remarks>
/// Referencing this package is the whole opt-in: a module initializer registers the backend the first time
/// any code in the assembly runs. Priority 75 sits below the WebView2 backend's 100 and above the
/// WebKitGTK backend's 50, so WebView2 is preferred when installed and Internet Explorer is the fallback
/// before WebKitGTK. An application that wants Trident even when WebView2 is present calls
/// <see cref="PreferOverWebView2" />.
/// </remarks>
public static class InternetExplorerBackendRegistration
{
    /// <summary>The name the backend registers under.</summary>
    public const string Name = "InternetExplorer";

    /// <summary>Default priority: below WebView2 (100), above WebKitGTK (50).</summary>
    public const int FallbackPriority = 75;

    /// <summary>Priority above the WebView2 backend, used by <see cref="PreferOverWebView2" />.</summary>
    public const int PreferredPriority = 150;

    private static bool _registered;

    /// <summary>
    /// Registers the backend. Called automatically when this assembly loads; safe to call again.
    /// </summary>
    [global::System.Runtime.CompilerServices.ModuleInitializer]
    public static void Register()
    {
        if (_registered)
        {
            return;
        }

        _registered = true;
        WebViewBackendFactory.Register(Name, FallbackPriority, InternetExplorerBackend.TryCreate);
    }

    /// <summary>
    /// Re-registers the backend above WebView2, so Trident wins on Windows even when the WebView2 runtime
    /// is installed. Call it once at startup, before the first <see cref="T:System.Windows.Forms.WebBrowser" />
    /// is constructed.
    /// </summary>
    public static void PreferOverWebView2()
    {
        _registered = true;
        WebViewBackendFactory.Register(Name, PreferredPriority, InternetExplorerBackend.TryCreate);
    }

    /// <summary>
    /// Removes the backend, so a process can fall back to whatever else is registered.
    /// </summary>
    /// <returns><c>true</c> when it was registered.</returns>
    public static bool Unregister()
    {
        _registered = false;
        return WebViewBackendFactory.Unregister(Name);
    }
}
