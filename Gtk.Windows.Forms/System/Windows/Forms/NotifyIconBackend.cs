using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// UTF-8 marshalling for the tray backends.
/// </summary>
/// <remarks>
/// Written by hand because <c>UnmanagedType.LPUTF8Str</c> does not exist on netstandard2.0, and because
/// GLib's <c>G_VARIANT_TYPE_*</c> macros are literally UTF-8 type strings - so the same buffer serves as
/// both a text argument and a <c>GVariantType*</c>.
/// </remarks>
internal static class NativeUtf8
{
    /// <summary>
    /// Converts a managed string to a newly allocated null-terminated UTF-8 buffer.
    /// </summary>
    /// <param name="value">The string, or <c>null</c>.</param>
    /// <returns>A pointer the caller must release with <see cref="Free" />.</returns>
    public static IntPtr FromString(string? value)
    {
        var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
        var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr, bytes.Length, 0);

        return ptr;
    }

    /// <summary>Releases a buffer returned by <see cref="FromString" />.</summary>
    /// <param name="ptr">The buffer, which may be <see cref="IntPtr.Zero" />.</param>
    public static void Free(IntPtr ptr)
    {
        if (ptr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    /// <summary>
    /// Reads a null-terminated UTF-8 string.
    /// </summary>
    /// <param name="ptr">The buffer, which may be <see cref="IntPtr.Zero" />.</param>
    /// <returns>The decoded string, or the empty string.</returns>
    public static string ToString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            return string.Empty;
        }

        var length = 0;
        while (Marshal.ReadByte(ptr, length) != 0)
        {
            length++;
        }

        if (length == 0)
        {
            return string.Empty;
        }

        var bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);

        return Encoding.UTF8.GetString(bytes);
    }
}

/// <summary>
/// Normalised tray pointer input, as reported by a <see cref="INotifyIconBackend" />.
/// </summary>
/// <remarks>
/// The three platform trays observe pointer input with very different fidelity, so a backend raises
/// only the events its platform can actually see and never synthesises the rest. See
/// <see cref="INotifyIconBackend" /> for the per-platform matrix.
/// </remarks>
internal sealed class TrayPointerEventArgs : EventArgs
{
    /// <summary>
    /// Initialises a new instance of the <see cref="TrayPointerEventArgs" /> class.
    /// </summary>
    /// <param name="button">The button involved, or <see cref="MouseButtons.None" />.</param>
    /// <param name="clicks">The click count; 1 unless the platform reports a double click.</param>
    /// <param name="x">The screen x coordinate, or 0 when the platform reports no position.</param>
    /// <param name="y">The screen y coordinate, or 0 when the platform reports no position.</param>
    public TrayPointerEventArgs(MouseButtons button, int clicks, int x, int y)
    {
        Button = button;
        Clicks = clicks;
        X = x;
        Y = y;
    }

    /// <summary>Gets the button involved, or <see cref="MouseButtons.None" />.</summary>
    public MouseButtons Button { get; }

    /// <summary>Gets the click count; 1 unless the platform reports a double click.</summary>
    public int Clicks { get; }

    /// <summary>Gets the screen x coordinate, or 0 when the platform reports no position.</summary>
    public int X { get; }

    /// <summary>Gets the screen y coordinate, or 0 when the platform reports no position.</summary>
    public int Y { get; }
}

/// <summary>
/// The platform half of <see cref="NotifyIcon" />: one implementation per system tray technology.
/// </summary>
/// <remarks>
/// <para>
/// Gtk 4 removed <c>GtkStatusIcon</c> with no replacement, so there is no Gtk API to call. What
/// <c>GtkStatusIcon</c> actually was, however, is three platform backends behind one object - and those are
/// reimplemented here: <c>Shell_NotifyIcon</c> on Windows, the freedesktop/KDE StatusNotifierItem D-Bus
/// protocol on Linux, and <c>NSStatusItem</c> on macOS.
/// </para>
/// <para>
/// Fidelity differs by platform and the gaps are not papered over. A backend raises only the events its
/// platform can observe:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Windows sees the full pointer stream, so it raises <see cref="PointerDown" />,
/// <see cref="PointerUp" />, <see cref="PointerMove" /> and <see cref="Activated" /> (with
/// <see cref="TrayPointerEventArgs.Clicks" /> of 2 for a double click).
/// </description></item>
/// <item><description>
/// StatusNotifierItem exposes only <c>Activate</c> and <c>SecondaryActivate</c>, so Linux raises
/// <see cref="Activated" /> alone - never a down, up, move or double click.
/// </description></item>
/// <item><description>
/// <c>NSStatusItem</c> reports button-up with a click count, so macOS raises <see cref="Activated" /> and
/// <see cref="PointerUp" /> only.
/// </description></item>
/// </list>
/// <para>
/// Every event is raised on the Gtk main loop, whatever thread the platform delivered the notification on.
/// </para>
/// </remarks>
internal interface INotifyIconBackend : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this session actually provides a system tray.
    /// </summary>
    /// <value>
    /// <c>false</c> when the platform or desktop session offers no tray at all - notably GNOME Shell
    /// without the AppIndicator extension, which registers no <c>StatusNotifierWatcher</c>. Every other
    /// member is then a silent no-op and no event is ever raised.
    /// </value>
    bool IsAvailable { get; }

    /// <summary>Sets, replaces or clears the tray icon.</summary>
    /// <param name="icon">The icon to show, or <c>null</c> to clear it.</param>
    void SetIcon(Drawing.Icon? icon);

    /// <summary>Sets the tray icon's hover tooltip.</summary>
    /// <param name="text">The tooltip text; may be empty.</param>
    void SetToolTip(string text);

    /// <summary>Shows or hides the tray icon.</summary>
    /// <param name="visible"><c>true</c> to show the icon, <c>false</c> to remove it.</param>
    void SetVisible(bool visible);

    /// <summary>Sets or clears the context menu shown for the tray icon.</summary>
    /// <param name="menu">The menu, or <c>null</c> for no menu.</param>
    /// <remarks>
    /// The strip itself is retained rather than a translated snapshot, because a consumer may keep
    /// mutating it after assignment. Backends call <see cref="TrayMenuModel.From" /> at the moment the
    /// platform asks to display the menu, so what is shown always reflects the strip's current state.
    /// </remarks>
    void SetMenu(ContextMenuStrip? menu);

    /// <summary>Shows a balloon tip / desktop notification.</summary>
    /// <param name="timeout">
    /// The requested display time in milliseconds. Honoured on Linux; ignored by Windows, which has
    /// controlled notification duration systemwide since Windows Vista.
    /// </param>
    /// <param name="title">The notification title.</param>
    /// <param name="text">The notification body.</param>
    /// <param name="icon">The severity, mapped to the platform's standard notification glyph.</param>
    void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon);

    /// <summary>Occurs when the tray icon is clicked. Raised by every backend.</summary>
    event EventHandler<TrayPointerEventArgs>? Activated;

    /// <summary>Occurs on pointer press. Windows only.</summary>
    event EventHandler<TrayPointerEventArgs>? PointerDown;

    /// <summary>Occurs on pointer release. Windows and macOS only.</summary>
    event EventHandler<TrayPointerEventArgs>? PointerUp;

    /// <summary>Occurs on pointer motion over the icon. Windows only.</summary>
    event EventHandler<TrayPointerEventArgs>? PointerMove;

    /// <summary>Occurs when a balloon tip becomes visible. Windows only.</summary>
    event EventHandler? BalloonShown;

    /// <summary>Occurs when the user activates a balloon tip. Windows and Linux only.</summary>
    event EventHandler? BalloonClicked;

    /// <summary>Occurs when a balloon tip is dismissed or times out. Windows and Linux only.</summary>
    event EventHandler? BalloonClosed;
}

/// <summary>
/// Common plumbing for the platform backends: event declaration, main-loop marshalling and the
/// disposed guard.
/// </summary>
internal abstract class NotifyIconBackendBase : INotifyIconBackend
{
    private bool _disposed;

    /// <inheritdoc />
    public abstract bool IsAvailable { get; }

    /// <inheritdoc />
    public event EventHandler<TrayPointerEventArgs>? Activated;

    /// <inheritdoc />
    public event EventHandler<TrayPointerEventArgs>? PointerDown;

    /// <inheritdoc />
    public event EventHandler<TrayPointerEventArgs>? PointerUp;

    /// <inheritdoc />
    public event EventHandler<TrayPointerEventArgs>? PointerMove;

    /// <inheritdoc />
    public event EventHandler? BalloonShown;

    /// <inheritdoc />
    public event EventHandler? BalloonClicked;

    /// <inheritdoc />
    public event EventHandler? BalloonClosed;

    /// <summary>Gets a value indicating whether <see cref="Dispose()" /> has run.</summary>
    protected bool IsDisposed => _disposed;

    /// <inheritdoc />
    public abstract void SetIcon(Drawing.Icon? icon);

    /// <inheritdoc />
    public abstract void SetToolTip(string text);

    /// <inheritdoc />
    public abstract void SetVisible(bool visible);

    /// <inheritdoc />
    public abstract void SetMenu(ContextMenuStrip? menu);

    /// <inheritdoc />
    public abstract void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon);

    /// <summary>
    /// Runs <paramref name="action" /> on the Gtk main loop.
    /// </summary>
    /// <param name="action">The work to run.</param>
    /// <remarks>
    /// D-Bus method calls and Objective-C actions can arrive on a thread that is not the one driving Gtk,
    /// and every backend ultimately calls back into WinForms handlers that expect the UI thread. Routing
    /// through an idle callback makes that safe uniformly, at the cost of one main-loop turn.
    /// </remarks>
    internal static void InvokeOnMainLoop(Action action)
    {
        GLib.Idle.Add(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug("Unhandled exception in a system tray callback", ex);
            }

            return false;
        });
    }

    /// <summary>Raises <see cref="Activated" /> on the main loop.</summary>
    /// <param name="button">The button involved.</param>
    /// <param name="clicks">The click count.</param>
    /// <param name="x">The screen x coordinate.</param>
    /// <param name="y">The screen y coordinate.</param>
    protected void RaiseActivated(MouseButtons button, int clicks, int x, int y)
    {
        var handler = Activated;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, new TrayPointerEventArgs(button, clicks, x, y)));
        }
    }

    /// <summary>Raises <see cref="PointerDown" /> on the main loop.</summary>
    /// <param name="button">The button involved.</param>
    /// <param name="x">The screen x coordinate.</param>
    /// <param name="y">The screen y coordinate.</param>
    protected void RaisePointerDown(MouseButtons button, int x, int y)
    {
        var handler = PointerDown;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, new TrayPointerEventArgs(button, 1, x, y)));
        }
    }

    /// <summary>Raises <see cref="PointerUp" /> on the main loop.</summary>
    /// <param name="button">The button involved.</param>
    /// <param name="x">The screen x coordinate.</param>
    /// <param name="y">The screen y coordinate.</param>
    protected void RaisePointerUp(MouseButtons button, int x, int y)
    {
        var handler = PointerUp;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, new TrayPointerEventArgs(button, 1, x, y)));
        }
    }

    /// <summary>Raises <see cref="PointerMove" /> on the main loop.</summary>
    /// <param name="x">The screen x coordinate.</param>
    /// <param name="y">The screen y coordinate.</param>
    protected void RaisePointerMove(int x, int y)
    {
        var handler = PointerMove;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, new TrayPointerEventArgs(MouseButtons.None, 0, x, y)));
        }
    }

    /// <summary>Raises <see cref="BalloonShown" /> on the main loop.</summary>
    protected void RaiseBalloonShown()
    {
        var handler = BalloonShown;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, EventArgs.Empty));
        }
    }

    /// <summary>Raises <see cref="BalloonClicked" /> on the main loop.</summary>
    protected void RaiseBalloonClicked()
    {
        var handler = BalloonClicked;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, EventArgs.Empty));
        }
    }

    /// <summary>Raises <see cref="BalloonClosed" /> on the main loop.</summary>
    protected void RaiseBalloonClosed()
    {
        var handler = BalloonClosed;
        if (handler != null)
        {
            InvokeOnMainLoop(() => handler(this, EventArgs.Empty));
        }
    }

    /// <summary>Releases the platform resources held by this backend.</summary>
    /// <param name="disposing"><c>true</c> when called from <see cref="Dispose()" />.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            Dispose(true);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception disposing the system tray backend", ex);
        }

        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// The backend used where no system tray exists: every member is a no-op and no event is ever raised.
/// </summary>
/// <remarks>
/// This reproduces the behaviour <see cref="NotifyIcon" /> had while Gtk 4 left it inert, which is what
/// keeps a headless or trayless session working rather than throwing.
/// </remarks>
internal sealed class NullNotifyIconBackend : NotifyIconBackendBase
{
    /// <inheritdoc />
    public override bool IsAvailable => false;

    /// <inheritdoc />
    public override void SetIcon(Drawing.Icon? icon)
    {
    }

    /// <inheritdoc />
    public override void SetToolTip(string text)
    {
    }

    /// <inheritdoc />
    public override void SetVisible(bool visible)
    {
    }

    /// <inheritdoc />
    public override void SetMenu(ContextMenuStrip? menu)
    {
    }

    /// <inheritdoc />
    public override void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon)
    {
    }
}

/// <summary>
/// Chooses and constructs the system tray backend for the running platform.
/// </summary>
internal static class NotifyIconBackendFactory
{
    /// <summary>
    /// Creates the backend for this platform.
    /// </summary>
    /// <returns>
    /// The platform backend, or <see cref="NullNotifyIconBackend" /> when the platform is unsupported or
    /// its backend failed to initialise. Never <c>null</c> and never throws: a missing tray is a normal
    /// runtime condition, not an error, so it degrades to an inert component that still round-trips every
    /// property.
    /// </returns>
    public static INotifyIconBackend Create()
    {
        try
        {
            if (Gtk.Windows.Forms.Controls.ControlBase.FuncLoader.IsWindows)
            {
                return new Win32NotifyIconBackend();
            }

            if (Gtk.Windows.Forms.Controls.ControlBase.FuncLoader.IsOSX)
            {
                return new QuartzNotifyIconBackend();
            }

            if (Gtk.Windows.Forms.Controls.ControlBase.FuncLoader.IsLinux)
            {
                return new StatusNotifierItemBackend();
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug(
                "No system tray backend could be initialised; NotifyIcon will be inert.", ex);
        }

        return new NullNotifyIconBackend();
    }

    /// <summary>
    /// Gets a stable, human-meaningful identifier for the hosting application.
    /// </summary>
    /// <returns>The entry or calling assembly's simple name, falling back to <c>"Application"</c>.</returns>
    /// <remarks>
    /// Used as the StatusNotifierItem <c>Id</c> and as the Windows tray icon's class hint, so it must not
    /// change between runs of the same application.
    /// </remarks>
    internal static string GetApplicationId()
    {
        try
        {
            var name = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name!;
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to resolve the application identity for the system tray", ex);
        }

        return "Application";
    }
}
