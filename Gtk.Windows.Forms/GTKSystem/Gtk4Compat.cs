using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gtk;

// Deliberately declared in the GLOBAL namespace: some GTK-backing files reference Gtk types
// fully-qualified (e.g. Gtk.Overlay) without a `using Gtk;`, so an extension class inside
// `namespace Gtk` would not be imported there. Extension methods declared in the global
// namespace are always in scope, so these helpers resolve everywhere without adding a
// `using Gtk;` to WinForms files (which would collide with Image/Label/Button/Panel/...).

/// <summary>
/// Backend-only helpers that bridge the Gtk 3 <c>Gtk.Container</c> API this shim was
/// originally written against to Gtk 4's per-widget child model. Gtk 4 removed
/// <c>Container.Add</c>/<c>Remove</c>/<c>Children</c>; each container type now exposes its
/// own child API (<c>Append</c>, a <c>Child</c> property, <c>Put</c>, <c>Attach</c>, ...).
/// These extensions dispatch on the runtime container type so the generated GTK-backing
/// code keeps a single uniform call shape. No <c>System.Windows.Forms</c> /
/// <c>System.Drawing</c> API is affected — this is purely the GTK backend.
/// </summary>
public static class Gtk4Compat
{
    /// <summary>
    /// Materializes a widget's direct children into an array. Replaces Gtk 3's
    /// <c>Container.Children</c> (removed in Gtk 4) using the <see cref="Widget.FirstChild"/>
    /// / <see cref="Widget.NextSibling"/> sibling chain.
    /// </summary>
    /// <param name="parent">The widget whose direct children are enumerated.</param>
    /// <returns>The direct child widgets, in order; an empty array when there are none.</returns>
    public static Widget[] GetChildrenArray(this Widget parent)
    {
        var list = new List<Widget>();
        if (parent != null)
        {
            for (var child = parent.FirstChild; child != null; child = child.NextSibling)
            {
                list.Add(child);
            }
        }

        return list.ToArray();
    }

    /// <summary>
    /// Gtk 4 removed <c>gdk_pixbuf_get_from_surface</c> — the Gtk 3
    /// <c>new Gdk.Pixbuf(surface, srcX, srcY, width, height)</c> path used to snapshot a Cairo
    /// surface into a <see cref="Gdk.Pixbuf"/>. This reproduces it by hand: paint the region into
    /// an ARGB32 image surface, then convert Cairo's premultiplied native-endian ARGB (byte order
    /// B,G,R,A on little-endian) into the pixbuf's straight RGBA.
    /// </summary>
    /// <param name="surface">The source Cairo surface.</param>
    /// <param name="srcX">Left of the region to copy, in surface coordinates.</param>
    /// <param name="srcY">Top of the region to copy.</param>
    /// <param name="width">Region width in pixels.</param>
    /// <param name="height">Region height in pixels.</param>
    /// <returns>A newly allocated 8-bit-per-channel RGBA pixbuf of the requested size.</returns>
    public static Gdk.Pixbuf ToPixbuf(this Cairo.Surface surface, int srcX, int srcY, int width, int height)
    {
        using var img = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
        using (var cr = new Cairo.Context(img))
        {
            cr.SetSourceSurface(surface, -srcX, -srcY);
            cr.Paint();
        }

        img.Flush();

        var stride = img.Stride;
        var src = img.Data;                        // premultiplied B,G,R,A
        var rgba = new byte[width * height * 4];    // straight R,G,B,A

        for (var y = 0; y < height; y++)
        {
            var s = y * stride;
            var d = y * width * 4;
            for (var x = 0; x < width; x++, s += 4, d += 4)
            {
                byte b = src[s + 0], g = src[s + 1], r = src[s + 2], a = src[s + 3];
                if (a != 0 && a != 255)
                {
                    r = (byte)(r * 255 / a);
                    g = (byte)(g * 255 / a);
                    b = (byte)(b * 255 / a);
                }

                rgba[d + 0] = r;
                rgba[d + 1] = g;
                rgba[d + 2] = b;
                rgba[d + 3] = a;
            }
        }

        var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
        var rowstride = pixbuf.Rowstride;
        var dest = pixbuf.Pixels;
        for (var y = 0; y < height; y++)
        {
            Marshal.Copy(rgba, y * width * 4, dest + (y * rowstride), width * 4);
        }

        return pixbuf;
    }

    /// <summary>
    /// Best-effort replacement for Gtk 3's <c>widget.Window.GetOrigin(out x, out y)</c>
    /// (Gtk 4 removed <c>Gdk.Window</c> and, on Wayland, has no global screen coordinates).
    /// Returns the widget origin translated into its top-level root's coordinate space,
    /// falling back to the widget allocation. Used by the WinForms <c>PointToScreen</c> /
    /// caret / tooltip anchor code paths, whose signatures are unchanged.
    /// </summary>
    /// <param name="widget">The widget whose origin is requested.</param>
    /// <param name="x">Receives the X origin.</param>
    /// <param name="y">Receives the Y origin.</param>
    public static void GetOrigin(this Widget widget, out int x, out int y)
    {
        x = 0;
        y = 0;
        if (widget == null)
        {
            return;
        }

        var root = widget;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        if (!ReferenceEquals(root, widget) &&
            widget.TranslateCoordinates(root, 0, 0, out var dx, out var dy))
        {
            x = (int)dx;
            y = (int)dy;
            return;
        }

        var allocation = widget.Allocation;
        x = allocation.X;
        y = allocation.Y;
    }

    /// <summary>
    /// Puts text on the default display clipboard. Replaces Gtk 3's
    /// <c>Gtk.Clipboard.Get(Gdk.Selection.Clipboard).Text = ...</c> (both types removed in Gtk 4)
    /// with Gtk 4's <see cref="Gdk.Clipboard.SetContent"/> over a text
    /// <see cref="Gdk.ContentProvider"/>. Writing is synchronous, matching WinForms Copy/Cut.
    /// </summary>
    /// <param name="text">The text to place on the clipboard.</param>
    public static void SetClipboardText(string text)
    {
        try
        {
            using var value = new GLib.Value(text ?? string.Empty);
            var provider = new Gdk.ContentProvider(value);
            Gdk.Display.Default.Clipboard.SetContent(provider);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Unexpected exception setting clipboard text", ex);
        }
    }

    /// <summary>
    /// Reads text off the default display clipboard synchronously. Gtk 4's clipboard read
    /// (<see cref="Gdk.Clipboard.ReadTextAsync"/>) is asynchronous with no synchronous equivalent, so - as
    /// the modal dialog path does for async present/response - this spins a nested <see cref="GLib.MainLoop"/>
    /// and quits it from the finish callback, giving WinForms' synchronous <c>Paste</c>/<c>GetText</c> a value
    /// to return. A watchdog bounds the wait so a clipboard with no text owner cannot hang the thread.
    /// </summary>
    /// <returns>The clipboard text, or <c>null</c> when there is none (or no display).</returns>
    public static string? GetClipboardTextSync()
    {
        var clipboard = Gdk.Display.Default?.Clipboard;
        if (clipboard == null)
        {
            return null;
        }

        string? result = null;
        var loop = new GLib.MainLoop();
        var watchdog = GLib.Timeout.Add(2000, () => { if (loop.IsRunning) { loop.Quit(); } return false; });

        try
        {
            // The callback is the raw GAsyncReadyCallback shape (source, result, user_data); pass it inline so
            // the delegate type is inferred, then drive it to completion on a nested loop.
            clipboard.ReadTextAsync(null, (source, res, data) =>
            {
                try
                {
                    result = clipboard.ReadTextFinish(res);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Clipboard text read failed", ex);
                }
                finally
                {
                    if (loop.IsRunning)
                    {
                        loop.Quit();
                    }
                }
            });
            loop.Run();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Clipboard text read failed", ex);
        }
        finally
        {
            try { GLib.Source.Remove(watchdog); } catch { /* already removed */ }
        }

        return result;
    }

    /// <summary>
    /// Reads an image off the default display clipboard synchronously, using the same nested-loop trick as
    /// <see cref="GetClipboardTextSync"/> over <see cref="Gdk.Clipboard.ReadTextureAsync"/>.
    /// </summary>
    /// <returns>The clipboard image, or <c>null</c> when there is none (or no display).</returns>
    public static System.Drawing.Image? GetClipboardImageSync()
    {
        var clipboard = Gdk.Display.Default?.Clipboard;
        if (clipboard == null)
        {
            return null;
        }

        Gdk.Texture? texture = null;
        var loop = new GLib.MainLoop();
        var watchdog = GLib.Timeout.Add(2000, () => { if (loop.IsRunning) { loop.Quit(); } return false; });

        try
        {
            clipboard.ReadTextureAsync(null, (source, res, data) =>
            {
                try
                {
                    texture = clipboard.ReadTextureFinish(res);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Clipboard image read failed", ex);
                }
                finally
                {
                    if (loop.IsRunning)
                    {
                        loop.Quit();
                    }
                }
            });
            loop.Run();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Clipboard image read failed", ex);
        }
        finally
        {
            try { GLib.Source.Remove(watchdog); } catch { /* already removed */ }
        }

        return texture == null ? null : TextureToBitmap(texture);
    }

    /// <summary>Puts an image on the default display clipboard as a Gdk.Texture.</summary>
    /// <param name="image">The image to place on the clipboard.</param>
    public static void SetClipboardImage(System.Drawing.Image image)
    {
        var pixbuf = image?.Pixbuf;
        if (pixbuf == null)
        {
            return;
        }

        try
        {
            var texture = new Gdk.Texture(pixbuf);
            // Mirror SetClipboardText: wrap the texture (a GObject) in a GLib.Value and hand it to the
            // clipboard as a ContentProvider, rather than the SetTexture convenience method.
            using var value = new GLib.Value(texture);
            var provider = new Gdk.ContentProvider(value);
            Gdk.Display.Default?.Clipboard.SetContent(provider);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Unexpected exception setting clipboard image", ex);
        }
    }

    /// <summary>Whether the default clipboard currently advertises a text format.</summary>
    public static bool ClipboardHasText()
    {
        try
        {
            var formats = Gdk.Display.Default?.Clipboard.Formats;
            return formats != null &&
                (formats.ContainMimeType("text/plain;charset=utf-8") || formats.ContainMimeType("text/plain") ||
                 formats.ContainGtype(GLib.GType.String));
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Clipboard text-format query failed", ex);
            return false;
        }
    }

    /// <summary>Whether the default clipboard currently advertises an image format.</summary>
    public static bool ClipboardHasImage()
    {
        try
        {
            var formats = Gdk.Display.Default?.Clipboard.Formats;
            return formats != null &&
                (formats.ContainGtype(Gdk.Texture.GType) || formats.ContainMimeType("image/png"));
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Clipboard image-format query failed", ex);
            return false;
        }
    }

    /// <summary>Converts a Gdk.Texture (premultiplied BGRA download) to a System.Drawing.Bitmap.</summary>
    private static System.Drawing.Image? TextureToBitmap(Gdk.Texture texture)
    {
        try
        {
            var width = texture.Width;
            var height = texture.Height;
            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var src = texture.Download(); // premultiplied B,G,R,A, tightly packed
            var stride = width * 4;
            var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height); // straight R,G,B,A
            var rowstride = pixbuf.Rowstride;
            var dest = pixbuf.Pixels;
            var row = new byte[width * 4];

            for (var y = 0; y < height; y++)
            {
                var s = y * stride;
                for (var x = 0; x < width; x++, s += 4)
                {
                    byte b = src[s + 0], g = src[s + 1], r = src[s + 2], a = src[s + 3];
                    if (a != 0 && a != 255)
                    {
                        r = (byte)(r * 255 / a);
                        g = (byte)(g * 255 / a);
                        b = (byte)(b * 255 / a);
                    }

                    var d = x * 4;
                    row[d + 0] = r;
                    row[d + 1] = g;
                    row[d + 2] = b;
                    row[d + 3] = a;
                }

                Marshal.Copy(row, 0, dest + (y * rowstride), width * 4);
            }

            return new System.Drawing.Bitmap(1, 1) { Pixbuf = pixbuf };
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Converting clipboard texture to bitmap failed", ex);
            return null;
        }
    }

    /// <summary>
    /// Renders a widget's current appearance into a Cairo context. Replaces Gtk 3's
    /// <c>Widget.Draw(cairo)</c> (removed in Gtk 4, where rendering goes through GtkSnapshot): take a
    /// <see cref="Gtk.WidgetPaintable"/> of the widget, snapshot it to a <see cref="Gsk.RenderNode"/>, and
    /// draw that node into the target context. Used by DrawToBitmap and the UITests screenshot capture.
    /// </summary>
    /// <param name="widget">The widget to render.</param>
    /// <param name="cr">The destination Cairo context (already translated/clipped by the caller).</param>
    /// <param name="width">Render width in pixels.</param>
    /// <param name="height">Render height in pixels.</param>
    public static void RenderWidgetToCairo(this Gtk.Widget widget, Cairo.Context cr, int width, int height)
    {
        if (widget == null || cr == null || width <= 0 || height <= 0)
        {
            return;
        }

        try
        {
            var paintable = new Gtk.WidgetPaintable(widget);
            var snapshot = new Gtk.Snapshot();
            ((Gdk.IPaintable)paintable).Snapshot(snapshot, width, height);
            var node = snapshot.ToNode();
            node?.Draw(cr);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Rendering widget to Cairo (GtkSnapshot) failed", ex);
        }
    }

    /// <summary>
    /// Returns the top-level ancestor of a widget. Replaces Gtk 3's <c>Widget.Toplevel</c>
    /// (removed in Gtk 4) by walking the <see cref="Widget.Parent"/> chain to the root.
    /// </summary>
    /// <param name="widget">The widget to walk up from.</param>
    /// <returns>The root ancestor (often a <see cref="Gtk.Window"/>), or the widget itself.</returns>
    public static Widget GetToplevel(this Widget widget)
    {
        var current = widget;
        while (current != null && current.Parent != null)
        {
            current = current.Parent;
        }

        return current!;
    }

    /// <summary>
    /// Measures a widget's preferred height. Replaces Gtk 3's
    /// <c>GetPreferredHeight(out min, out natural)</c> with Gtk 4's
    /// <see cref="Widget.Measure"/> on the vertical orientation.
    /// </summary>
    /// <param name="widget">The widget to measure.</param>
    /// <param name="minimum">Receives the minimum height.</param>
    /// <param name="natural">Receives the natural height.</param>
    public static void GetPreferredHeight(this Widget widget, out int minimum, out int natural)
    {
        widget.Measure(Orientation.Vertical, -1, out minimum, out natural, out _, out _);
    }

    /// <summary>
    /// Best-effort replacement for Gtk 3's <c>Gdk.Display.GetPointer(out x, out y, out mask)</c>, removed in
    /// Gtk 4. Reads the default seat's pointer device: modifier state is always available, and the position
    /// is surface-relative (Wayland exposes no global screen coordinate, so this is the best a portable
    /// query can do). Feeds <c>Cursor.Position</c>/<c>MousePosition</c>/<c>ModifierKeys</c>/<c>MouseButtons</c>,
    /// whose signatures are unchanged.
    /// </summary>
    /// <param name="display">The display whose default seat is queried.</param>
    /// <param name="x">Receives the pointer x, surface-relative (0 when unavailable).</param>
    /// <param name="y">Receives the pointer y, surface-relative (0 when unavailable).</param>
    /// <param name="mask">Receives the active keyboard/button modifiers.</param>
    public static void GetPointer(this Gdk.Display display, out int x, out int y, out Gdk.ModifierType mask)
    {
        x = 0;
        y = 0;
        mask = 0;

        try
        {
            var device = display?.DefaultSeat?.Pointer;
            if (device == null)
            {
                return;
            }

            mask = device.ModifierState;
            var surface = device.GetSurfaceAtPosition(out var winX, out var winY);
            if (surface != null)
            {
                x = (int)winX;
                y = (int)winY;
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("GetPointer via the default seat failed", ex);
        }
    }

    /// <summary>
    /// Sets all four Gtk 4 per-edge margins at once. Gtk 3's single <c>Widget.Margin</c> shorthand
    /// (all edges) was removed in favour of <c>MarginStart/End/Top/Bottom</c>.
    /// </summary>
    /// <param name="widget">The widget to set margins on.</param>
    /// <param name="value">The margin, in pixels, applied to every edge.</param>
    public static void SetMargin(this Widget widget, int value)
    {
        widget.MarginStart = value;
        widget.MarginEnd = value;
        widget.MarginTop = value;
        widget.MarginBottom = value;
    }

    /// <summary>
    /// Converts the <see cref="GLib.DateTime"/> that Gtk 4's <c>Gtk.Calendar.Date</c> exposes into a
    /// <see cref="System.DateTime"/> (Gtk 3 used <c>System.DateTime</c> directly). Null maps to
    /// <see cref="System.DateTime.MinValue"/>.
    /// </summary>
    /// <param name="value">The GLib date-time to convert.</param>
    /// <returns>The equivalent managed <see cref="System.DateTime"/>.</returns>
    public static System.DateTime ToDateTime(this GLib.DateTime value)
    {
        if (value == null)
        {
            return System.DateTime.MinValue;
        }

        return new System.DateTime(value.Year, value.Month, value.DayOfMonth,
            value.Hour, value.Minute, value.Second);
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> into the <see cref="GLib.DateTime"/> that Gtk 4's
    /// <c>Gtk.Calendar.Date</c> now requires.
    /// </summary>
    /// <param name="value">The managed date-time to convert.</param>
    /// <returns>The equivalent <see cref="GLib.DateTime"/> in local time.</returns>
    public static GLib.DateTime ToGLibDateTime(this System.DateTime value)
    {
        return new GLib.DateTime(value.Year, value.Month, value.Day,
            value.Hour, value.Minute, value.Second);
    }

    /// <summary>
    /// Synchronous replacement for Gtk 3's <c>Gtk.Dialog.Run()</c>, which Gtk 4 removed in favour
    /// of the async present/response model. WinForms <c>ShowDialog()</c> must block and return a
    /// result, so this spins a nested <see cref="GLib.MainLoop"/> that quits on the dialog's
    /// <c>response</c> (or <c>close</c>) signal — the documented Gtk 4 port of <c>Run()</c>.
    /// </summary>
    /// <param name="dialog">The dialog to present modally and wait on.</param>
    /// <returns>The <see cref="Gtk.ResponseType"/> id (as int) the dialog closed with.</returns>
    /// <summary>
    /// Whether a GtkSharp object still wraps a live native instance (its handle is non-null and reading
    /// it does not throw for a disposed object).
    /// </summary>
    /// <param name="obj">The object to probe.</param>
    /// <returns><c>true</c> if the native instance is still alive.</returns>
    private static bool IsAlive(GLib.Object obj)
    {
        try
        {
            return obj != null && obj.Handle != System.IntPtr.Zero;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public static int RunDialog(this Gtk.Dialog dialog)
    {
        if (!IsAlive(dialog))
        {
            return (int)Gtk.ResponseType.None;
        }

        var response = (int)Gtk.ResponseType.None;
        var loop = new GLib.MainLoop();

        void Quit()
        {
            if (loop.IsRunning)
            {
                loop.Quit();
            }
        }

        void OnResponse(object o, Gtk.ResponseArgs args)
        {
            // The generated ResponseArgs.Response getter casts Args[0] to string (a binding quirk);
            // the raw signal parameter is the int response id, so read it directly.
            if (args.Args != null && args.Args.Length > 0 && args.Args[0] != null)
            {
                response = System.Convert.ToInt32(args.Args[0]);
            }

            Quit();
        }

        void OnGone(object o, System.EventArgs args) => Quit();

        dialog.Response += OnResponse;
        dialog.Close += OnGone;
        // Gtk3's Dialog.Run() quit the nested loop when the dialog was destroyed. Gtk4 has no widget
        // "destroy" signal, so also quit on "unrealize". FormBase modal dialogs additionally quit the loop
        // directly from FormBase.CloseWindow() (which then defers the destroy to after the loop unwinds —
        // see PendingModalClose), so the watchdog below is only a backstop. Net4x.GtkSharp now zeros a
        // wrapper's Handle when the native object is freed, so a liveness poll (Handle != Zero) is safe and
        // no longer risks reading freed memory.
        dialog.Unrealized += OnGone;

        // Backstop watchdog: quit the loop once the dialog's native instance is gone. The normal quit is the
        // Response/Close/Unrealize/CloseRequest signals plus FormBase.CloseWindow -> ModalLoop.Quit (which is
        // what a Load handler calling Form.Close() drives); this is only a safety net for a dialog destroyed
        // out from under the loop. It carries NO arbitrary time cap - an untouched modal stays open until it
        // is actually closed, which is the WinForms-correct behaviour.
        //
        // The IsAlive poll is now reliable: Net4x.GtkSharp zeros a wrapper's Handle when the native object is
        // freed, so IsAlive (Handle != Zero) detects an externally-destroyed dialog instead of reading freed
        // memory and spamming "GTK_IS_WIDGET failed". That handle-zeroing is what let the old fixed 5-second
        // cap - a stand-in for the then-unreliable liveness check - be removed; the FormBase Gtk.Dialog design
        // is retained (Gtk.Dialog is deprecated but functional and the modal loop is deterministic without it).
        var watchdog = GLib.Timeout.Add(100, () =>
        {
            if (!IsAlive(dialog))
            {
                Quit();
                return false;
            }

            return true;
        });

            // Let FormBase.CloseWindow() see the active modal loop so a Close() during the loop quits it and
            // DEFERS the window destroy (Gtk4 crashes on a re-entrant Window.Destroy from inside the loop).
        var formBase = dialog as GtkSystem.Windows.Forms.GtkControls.ControlBase.FormBase;
        if (formBase != null)
        {
            formBase.ModalLoop = loop;
        }

        try
        {
            dialog.Modal = true;
            // Gtk4: Window.Present() calls gdk_toplevel_present, which asserts the surface is already a
            // realized toplevel — calling it on a not-yet-realized window crashes the process
            // ("gdk_toplevel_present: GDK_IS_TOPLEVEL failed"). Show() realizes+maps safely; Present()
            // (bring-to-front) is only safe once realized.
            dialog.Show();
            if (dialog.IsRealized)
            {
                try
                {
                    dialog.Present();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Unexpected exception presenting dialog", ex);
                }
            }

            loop.Run();

            // A Close() during the loop deferred the actual dispose to here — the loop has now unwound, so
            // it is safe (not re-entrant, not on a teardown idle). Disposing the owning Form releases its
            // window; we deliberately do NOT also call formBase.Destroy() — Form.Dispose already unrefs the
            // native window, and a second destroy on the freed window crashes ("gtk_window_destroy:
            // GTK_IS_WINDOW failed").
            if (formBase != null && formBase.PendingModalClose != null)
            {
                var pending = formBase.PendingModalClose;
                formBase.PendingModalClose = null;
                pending.Dispose();
            }
        }
        finally
        {
            if (formBase != null && IsAlive(formBase))
            {
                formBase.ModalLoop = null;
            }

            // Stop the anti-hang watchdog (harmless if it already removed itself by returning false).
            try
            {
                GLib.Source.Remove(watchdog);
            }
            catch (System.Exception)
            {
                // ignore: the source may have already been removed
            }

            // The dialog may have been destroyed/disposed inside the loop; only detach handlers while
            // its native instance is still alive.
            if (IsAlive(dialog))
            {
                try
                {
                    dialog.Response -= OnResponse;
                    dialog.Close -= OnGone;
                    dialog.Unrealized -= OnGone;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Unexpected exception detaching RunDialog handlers", ex);
                }
            }
        }

        return response;
    }

    /// <summary>
    /// Returns the <see cref="Label"/> a widget displays, whether that label sits in the widget's
    /// <c>Child</c> slot or was created by Gtk 4's <c>gtk_button_set_label()</c> /
    /// <c>gtk_check_button_set_label()</c>. Those setters keep the label they create *outside* the
    /// child slot, so Gtk 3's idiom <c>Child is Gtk.Label</c> is always false for a labelled button
    /// and any styling keyed off it is silently dropped. The search is depth-first and returns the
    /// first match, so it is meant for simple caption-bearing widgets, not arbitrary containers.
    /// </summary>
    /// <param name="widget">The widget to search, including itself; may be <c>null</c>.</param>
    /// <returns>The first label found, or <c>null</c> when the widget displays no label.</returns>
    public static Label? GetLabelWidget(this Widget? widget)
    {
        if (widget == null)
            return null;
        if (widget is Label self)
            return self;

        for (var child = widget.FirstChild; child != null; child = child.NextSibling)
        {
            var found = child.GetLabelWidget();
            if (found != null)
                return found;
        }

        return null;
    }

    /// <summary>
    /// Positions a caption-bearing widget's text inside the space the widget was given, using the same
    /// 0.0-1.0 factors Gtk 3 accepted on <c>GtkButton</c>'s <c>xalign</c> / <c>yalign</c>.
    /// </summary>
    /// <param name="widget">The caption-bearing widget; may be <c>null</c>.</param>
    /// <param name="xAlign">The horizontal factor: 0.0 left, 0.5 centre, 1.0 right.</param>
    /// <param name="yAlign">The vertical factor: 0.0 top, 0.5 middle, 1.0 bottom.</param>
    /// <remarks>
    /// Gtk 4 deleted <c>gtk_button_set_alignment()</c> along with the whole <c>GtkMisc</c> alignment
    /// mechanism, so a button can no longer place its own caption. <c>GtkLabel</c> kept <c>xalign</c> and
    /// <c>yalign</c> though, and they mean exactly what the button's used to - they just apply to the
    /// space the label itself holds. Giving the label <see cref="Align.Fill"/> hands it the button's whole
    /// content box, at which point the two factors reproduce Gtk 3's behaviour. Without the fill the label
    /// is only as large as its text, both factors have nothing to move within, and every alignment
    /// collapses to centred.
    /// </remarks>
    public static void AlignCaption(this Widget? widget, float xAlign, float yAlign)
    {
        var label = widget.GetLabelWidget();
        if (label == null)
        {
            return;
        }

        label.Halign = Align.Fill;
        label.Valign = Align.Fill;
        label.Xalign = xAlign;
        label.Yalign = yAlign;
    }

    /// <summary>
    /// Adds <paramref name="child"/> to <paramref name="parent"/>, dispatching to the
    /// Gtk 4 child API appropriate to the container's runtime type. Replaces Gtk 3's
    /// universal <c>Container.Add</c>.
    /// </summary>
    /// <param name="parent">The container widget to add to.</param>
    /// <param name="child">The child widget to add.</param>
    public static void AddChild(this Widget parent, Widget child)
    {
        switch (parent)
        {
            case Box box:
                box.Append(child);
                break;
            case FlowBox flowBox:
                flowBox.Append(child);
                break;
            case ListBox listBox:
                listBox.Append(child);
                break;
            case Fixed @fixed:
                @fixed.Put(child, 0, 0);
                break;
            case Grid grid:
                grid.Attach(child, 0, 0, 1, 1);
                break;
            case Overlay overlay:
                if (overlay.Child == null)
                {
                    overlay.Child = child;
                }
                else
                {
                    overlay.AddOverlay(child);
                }

                break;
            case ScrolledWindow scrolledWindow:
                scrolledWindow.Child = child;
                break;
            case Viewport viewport:
                viewport.Child = child;
                break;
            case Frame frame:
                frame.Child = child;
                break;
            case FlowBoxChild flowBoxChild:
                flowBoxChild.Child = child;
                break;
            case ListBoxRow listBoxRow:
                listBoxRow.Child = child;
                break;
            case Notebook notebook:
                notebook.AppendPage(child, null);
                break;
            case Expander expander:
                expander.Child = child;
                break;
            case Button button:
                button.Child = child;
                break;
            case Window window:
                window.Child = child;
                break;
            case Popover popover:
                popover.Child = child;
                break;
            default:
                // Last resort: reparent by hand. Works for any single-child container
                // exposing the GtkWidget parent slot.
                if (child.Parent != null)
                {
                    child.Unparent();
                }

                child.Parent = parent;
                break;
        }
    }

    /// <summary>
    /// Removes <paramref name="child"/> from <paramref name="parent"/>, dispatching to the
    /// Gtk 4 child API appropriate to the container's runtime type. Replaces Gtk 3's
    /// universal <c>Container.Remove</c>.
    /// </summary>
    /// <param name="parent">The container widget to remove from.</param>
    /// <param name="child">The child widget to remove.</param>
    public static void RemoveChild(this Widget parent, Widget child)
    {
        switch (parent)
        {
            case Box box:
                box.Remove(child);
                break;
            case FlowBox flowBox:
                flowBox.Remove(child);
                break;
            case ListBox listBox:
                listBox.Remove(child);
                break;
            case Fixed @fixed:
                @fixed.Remove(child);
                break;
            case Grid grid:
                grid.Remove(child);
                break;
            case Overlay overlay when overlay.Child == child:
                overlay.Child = null;
                break;
            case Overlay overlay:
                overlay.RemoveOverlay(child);
                break;
            case ScrolledWindow scrolledWindow when scrolledWindow.Child == child:
                scrolledWindow.Child = null;
                break;
            case Viewport viewport when viewport.Child == child:
                viewport.Child = null;
                break;
            case Frame frame when frame.Child == child:
                frame.Child = null;
                break;
            case FlowBoxChild flowBoxChild when flowBoxChild.Child == child:
                flowBoxChild.Child = null;
                break;
            case ListBoxRow listBoxRow when listBoxRow.Child == child:
                listBoxRow.Child = null;
                break;
            case Notebook notebook:
                var pageNum = notebook.PageNum(child);
                if (pageNum >= 0)
                {
                    notebook.RemovePage(pageNum);
                }

                break;
            case Window window when window.Child == child:
                window.Child = null;
                break;
            default:
                child.Unparent();
                break;
        }
    }
}
