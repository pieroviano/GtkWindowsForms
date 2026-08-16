using System;
using System.Drawing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// The geometry that places a WebView2 controller over the area a GTK widget occupies.
/// </summary>
/// <remarks>
/// Pure arithmetic, deliberately separated from the controller it drives, so that it can be tested on a
/// machine with no WebView2 runtime installed. The unit mismatch it exists to resolve is the classic
/// cause of "the browser fills a quarter of the control on a 4K screen": GTK allocations are in logical
/// pixels and WebView2 bounds are in physical ones.
/// </remarks>
public static class WebView2Bounds
{
    /// <summary>
    /// Converts a widget's position and size into the controller's bounds.
    /// </summary>
    /// <param name="x">The widget's left edge, in logical pixels, relative to the toplevel.</param>
    /// <param name="y">The widget's top edge, in logical pixels, relative to the toplevel.</param>
    /// <param name="width">The widget's width, in logical pixels.</param>
    /// <param name="height">The widget's height, in logical pixels.</param>
    /// <param name="scaleFactor">
    /// The surface's scale factor. Values below one are treated as one: a zero would collapse the view to
    /// nothing, and GTK reports zero for a surface that is not yet realised.
    /// </param>
    /// <returns>The bounds, in physical pixels, never negative in size.</returns>
    public static Rectangle Compute(int x, int y, int width, int height, int scaleFactor)
    {
        var scale = scaleFactor < 1 ? 1 : scaleFactor;

        return new Rectangle(
            x * scale,
            y * scale,
            width < 0 ? 0 : width * scale,
            height < 0 ? 0 : height * scale);
    }

    /// <summary>
    /// Decides whether the controller should be visible.
    /// </summary>
    /// <param name="mapped">Whether the widget is mapped.</param>
    /// <param name="width">The widget's width, in logical pixels.</param>
    /// <param name="height">The widget's height, in logical pixels.</param>
    /// <returns>
    /// <c>true</c> when the browser should be shown. An unmapped widget or an empty allocation hides it:
    /// nothing else clips a child HWND, so a controller left visible over a zero-sized widget would keep
    /// painting its last frame on top of whatever GTK draws there.
    /// </returns>
    public static bool IsVisible(bool mapped, int width, int height)
    {
        return mapped && width > 0 && height > 0;
    }
}
