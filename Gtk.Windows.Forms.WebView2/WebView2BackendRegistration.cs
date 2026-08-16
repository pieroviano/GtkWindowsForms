using GtkSystem.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// Puts the WebView2 backend into <see cref="WebViewBackendFactory" />.
/// </summary>
/// <remarks>
/// Referencing this package is the whole opt-in: a module initializer registers the backend the first
/// time any code in the assembly runs, so an application gets the Windows engine by adding a package
/// reference and nothing else. Priority 100 puts it above the WebKitGTK backend's 50, so it wins on
/// Windows where both could in principle load.
/// </remarks>
public static class WebView2BackendRegistration
{
    /// <summary>The name the backend registers under.</summary>
    public const string Name = "WebView2";

    /// <summary>Priority above the WebKitGTK backend, which registers at 50.</summary>
    public const int Priority = 100;

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
        WebViewBackendFactory.Register(Name, Priority, WebView2Backend.TryCreate);
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
