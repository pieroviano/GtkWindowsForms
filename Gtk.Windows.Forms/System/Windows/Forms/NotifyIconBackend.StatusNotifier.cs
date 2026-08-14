using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// The slice of GLib, GObject and GIO needed to export D-Bus objects for the Linux system tray.
/// </summary>
/// <remarks>
/// <para>
/// These are declared here rather than taken from the generated GioSharp binding for one reason: exporting
/// an object needs <c>GDBusInterfaceVTable</c>, a struct of raw function pointers whose marshalling and
/// callback lifetime the binding does not manage. Owning the declarations makes the delegate rooting and
/// the GVariant reference counting explicit and auditable, which is worth more here than binding sugar.
/// </para>
/// <para>
/// Only ever loaded on Linux - the library names are the ones GLib installs there.
/// </para>
/// </remarks>
internal static class GDBusNative
{
    private const string GLib = "libglib-2.0.so.0";

    private const string Gio = "libgio-2.0.so.0";

    /// <summary>The session bus, as <c>G_BUS_TYPE_SESSION</c>.</summary>
    public const int BusTypeSession = 2;

    /// <summary>
    /// Converts a managed string to a newly allocated null-terminated UTF-8 buffer.
    /// </summary>
    /// <param name="value">The string, or <c>null</c>.</param>
    /// <returns>A pointer the caller must release with <see cref="FreeUtf8" />.</returns>
    public static IntPtr ToUtf8(string? value)
    {
        return NativeUtf8.FromString(value);
    }

    /// <summary>Releases a buffer returned by <see cref="ToUtf8" />.</summary>
    /// <param name="ptr">The buffer, which may be <see cref="IntPtr.Zero" />.</param>
    public static void FreeUtf8(IntPtr ptr)
    {
        NativeUtf8.Free(ptr);
    }

    /// <summary>
    /// Reads a null-terminated UTF-8 string.
    /// </summary>
    /// <param name="ptr">The buffer, which may be <see cref="IntPtr.Zero" />.</param>
    /// <returns>The decoded string, or the empty string.</returns>
    public static string FromUtf8(IntPtr ptr)
    {
        return NativeUtf8.ToString(ptr);
    }

    /// <summary>
    /// Builds a GVariant string, returning a floating reference.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    public static IntPtr NewString(string? value)
    {
        var utf8 = ToUtf8(value);

        try
        {
            return g_variant_new_string(utf8);
        }
        finally
        {
            FreeUtf8(utf8);
        }
    }

    /// <summary>
    /// Builds a GVariant object path, returning a floating reference.
    /// </summary>
    /// <param name="path">The object path, which must be syntactically valid.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    public static IntPtr NewObjectPath(string path)
    {
        var utf8 = ToUtf8(path);

        try
        {
            return g_variant_new_object_path(utf8);
        }
        finally
        {
            FreeUtf8(utf8);
        }
    }

    /// <summary>
    /// Builds a GVariant tuple from floating children, returning a floating reference.
    /// </summary>
    /// <param name="children">The child variants, whose floating references are consumed.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    public static IntPtr NewTuple(params IntPtr[] children)
    {
        return g_variant_new_tuple(children, new UIntPtr((ulong)children.Length));
    }

    /// <summary>
    /// Builds a GVariant array, returning a floating reference.
    /// </summary>
    /// <param name="elementType">The D-Bus signature of one element, e.g. <c>"(iiay)"</c>.</param>
    /// <param name="children">The child variants, whose floating references are consumed; may be empty.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    public static IntPtr NewArray(string elementType, IntPtr[] children)
    {
        var type = ToUtf8(elementType);

        try
        {
            return children.Length == 0
                ? g_variant_new_array(type, null, UIntPtr.Zero)
                : g_variant_new_array(type, children, new UIntPtr((ulong)children.Length));
        }
        finally
        {
            FreeUtf8(type);
        }
    }

    /// <summary>
    /// Builds a GVariant byte array from raw data, returning a floating reference.
    /// </summary>
    /// <param name="data">The bytes to copy.</param>
    /// <returns>A floating <c>GVariant*</c> of type <c>ay</c>.</returns>
    /// <remarks>
    /// A fixed array copies the whole buffer in one call. Building the same value out of one GVariant per
    /// byte would allocate thousands of objects for even a small icon.
    /// </remarks>
    public static IntPtr NewByteArray(byte[] data)
    {
        var type = ToUtf8("y");
        var buffer = Marshal.AllocHGlobal(Math.Max(data.Length, 1));

        try
        {
            if (data.Length > 0)
            {
                Marshal.Copy(data, 0, buffer, data.Length);
            }

            return g_variant_new_fixed_array(type, buffer, new UIntPtr((ulong)data.Length), new UIntPtr(1));
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
            FreeUtf8(type);
        }
    }

    /// <summary>
    /// Builds one <c>{sv}</c> dictionary entry, returning a floating reference.
    /// </summary>
    /// <param name="key">The entry key.</param>
    /// <param name="value">The entry value, whose floating reference is consumed.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    public static IntPtr NewDictEntry(string key, IntPtr value)
    {
        return g_variant_new_dict_entry(NewString(key), g_variant_new_variant(value));
    }

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_new_boolean(int value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_new_int32(int value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_new_uint32(uint value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_new_variant(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_string(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_object_path(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_tuple(IntPtr[] children, UIntPtr nChildren);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_array(IntPtr elementType, IntPtr[]? children, UIntPtr nChildren);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_fixed_array(IntPtr elementType, IntPtr elements,
        UIntPtr nElements, UIntPtr elementSize);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr g_variant_new_dict_entry(IntPtr key, IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_ref_sink(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_variant_unref(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr g_variant_n_children(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_get_child_value(IntPtr value, UIntPtr index);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int g_variant_get_int32(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint g_variant_get_uint32(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int g_variant_get_boolean(IntPtr value);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_variant_get_string(IntPtr value, IntPtr length);

    [DllImport(GLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_error_free(IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_bus_get_sync(int busType, IntPtr cancellable, out IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_dbus_node_info_new_for_xml(IntPtr xml, out IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_dbus_node_info_lookup_interface(IntPtr info, IntPtr name);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_dbus_node_info_unref(IntPtr info);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint g_dbus_connection_register_object(IntPtr connection, IntPtr objectPath,
        IntPtr interfaceInfo, IntPtr vtable, IntPtr userData, IntPtr userDataFreeFunc, out IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern int g_dbus_connection_unregister_object(IntPtr connection, uint registrationId);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern int g_dbus_connection_emit_signal(IntPtr connection, IntPtr destination,
        IntPtr objectPath, IntPtr interfaceName, IntPtr signalName, IntPtr parameters, out IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr g_dbus_connection_call_sync(IntPtr connection, IntPtr busName,
        IntPtr objectPath, IntPtr interfaceName, IntPtr methodName, IntPtr parameters, IntPtr replyType,
        int flags, int timeoutMsec, IntPtr cancellable, out IntPtr error);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_dbus_connection_call(IntPtr connection, IntPtr busName, IntPtr objectPath,
        IntPtr interfaceName, IntPtr methodName, IntPtr parameters, IntPtr replyType, int flags,
        int timeoutMsec, IntPtr cancellable, IntPtr callback, IntPtr userData);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint g_dbus_connection_signal_subscribe(IntPtr connection, IntPtr sender,
        IntPtr interfaceName, IntPtr member, IntPtr objectPath, IntPtr arg0, int flags, IntPtr callback,
        IntPtr userData, IntPtr userDataFreeFunc);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_dbus_connection_signal_unsubscribe(IntPtr connection, uint subscriptionId);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_dbus_method_invocation_return_value(IntPtr invocation, IntPtr parameters);

    [DllImport(Gio, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_dbus_method_invocation_return_dbus_error(IntPtr invocation, IntPtr errorName,
        IntPtr errorMessage);

    /// <summary>The layout of <c>GDBusInterfaceVTable</c>, including its reserved padding.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InterfaceVTable
    {
        /// <summary>Pointer to the <c>GDBusInterfaceMethodCallFunc</c>.</summary>
        public IntPtr MethodCall;

        /// <summary>Pointer to the <c>GDBusInterfaceGetPropertyFunc</c>.</summary>
        public IntPtr GetProperty;

        /// <summary>Pointer to the <c>GDBusInterfaceSetPropertyFunc</c>.</summary>
        public IntPtr SetProperty;

        /// <summary>Reserved padding; GLib requires the storage to exist.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public IntPtr[] Padding;
    }

    /// <summary>Signature of <c>GDBusInterfaceMethodCallFunc</c>.</summary>
    /// <param name="connection">The connection the call arrived on.</param>
    /// <param name="sender">The caller's unique bus name.</param>
    /// <param name="objectPath">The object path called.</param>
    /// <param name="interfaceName">The interface called.</param>
    /// <param name="methodName">The method called.</param>
    /// <param name="parameters">The argument tuple.</param>
    /// <param name="invocation">The invocation that must be completed.</param>
    /// <param name="userData">Unused.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MethodCallFunc(IntPtr connection, IntPtr sender, IntPtr objectPath,
        IntPtr interfaceName, IntPtr methodName, IntPtr parameters, IntPtr invocation, IntPtr userData);

    /// <summary>Signature of <c>GDBusInterfaceGetPropertyFunc</c>.</summary>
    /// <param name="connection">The connection the request arrived on.</param>
    /// <param name="sender">The caller's unique bus name.</param>
    /// <param name="objectPath">The object path queried.</param>
    /// <param name="interfaceName">The interface queried.</param>
    /// <param name="propertyName">The property queried.</param>
    /// <param name="error">Set on failure; left alone here.</param>
    /// <param name="userData">Unused.</param>
    /// <returns>A <c>GVariant*</c> GDBus takes ownership of, or <see cref="IntPtr.Zero" />.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr GetPropertyFunc(IntPtr connection, IntPtr sender, IntPtr objectPath,
        IntPtr interfaceName, IntPtr propertyName, out IntPtr error, IntPtr userData);

    /// <summary>Signature of <c>GDBusSignalCallback</c>.</summary>
    /// <param name="connection">The connection the signal arrived on.</param>
    /// <param name="senderName">The emitting bus name.</param>
    /// <param name="objectPath">The emitting object path.</param>
    /// <param name="interfaceName">The emitting interface.</param>
    /// <param name="signalName">The signal name.</param>
    /// <param name="parameters">The signal argument tuple.</param>
    /// <param name="userData">Unused.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SignalCallback(IntPtr connection, IntPtr senderName, IntPtr objectPath,
        IntPtr interfaceName, IntPtr signalName, IntPtr parameters, IntPtr userData);
}

/// <summary>
/// The Linux system tray backend, implementing the freedesktop/KDE StatusNotifierItem protocol over D-Bus.
/// </summary>
/// <remarks>
/// <para>
/// Two objects are exported on the session bus: <c>/StatusNotifierItem</c> carrying the icon, tooltip and
/// activation methods, and <c>/MenuBar</c> carrying the context menu as
/// <c>com.canonical.dbusmenu</c> (see <see cref="DBusMenuExporter" />). The item is then announced to
/// <c>org.kde.StatusNotifierWatcher</c>, and the host - Plasma, XFCE's panel, GNOME's AppIndicator
/// extension - renders it.
/// </para>
/// <para>
/// This speaks the protocol directly rather than binding <c>libayatana-appindicator</c>, so the library
/// gains no native runtime dependency and works identically under X11 and Wayland.
/// </para>
/// <para>
/// Stock GNOME Shell registers no watcher. That is not an error: <see cref="IsAvailable" /> reports
/// <c>false</c> and the component stays inert, exactly as on a headless session.
/// </para>
/// </remarks>
internal sealed class StatusNotifierItemBackend : NotifyIconBackendBase
{
    private const string ItemInterface = "org.kde.StatusNotifierItem";

    private const string ItemPath = "/StatusNotifierItem";

    private const string WatcherName = "org.kde.StatusNotifierWatcher";

    private const string WatcherPath = "/StatusNotifierWatcher";

    private const string ItemInterfaceXml = @"<node>
  <interface name='org.kde.StatusNotifierItem'>
    <property name='Category' type='s' access='read'/>
    <property name='Id' type='s' access='read'/>
    <property name='Title' type='s' access='read'/>
    <property name='Status' type='s' access='read'/>
    <property name='WindowId' type='i' access='read'/>
    <property name='IconName' type='s' access='read'/>
    <property name='IconPixmap' type='a(iiay)' access='read'/>
    <property name='OverlayIconName' type='s' access='read'/>
    <property name='AttentionIconName' type='s' access='read'/>
    <property name='ToolTip' type='(sa(iiay)ss)' access='read'/>
    <property name='ItemIsMenu' type='b' access='read'/>
    <property name='Menu' type='o' access='read'/>
    <method name='Activate'>
      <arg name='x' type='i' direction='in'/>
      <arg name='y' type='i' direction='in'/>
    </method>
    <method name='SecondaryActivate'>
      <arg name='x' type='i' direction='in'/>
      <arg name='y' type='i' direction='in'/>
    </method>
    <method name='ContextMenu'>
      <arg name='x' type='i' direction='in'/>
      <arg name='y' type='i' direction='in'/>
    </method>
    <method name='Scroll'>
      <arg name='delta' type='i' direction='in'/>
      <arg name='orientation' type='s' direction='in'/>
    </method>
    <signal name='NewIcon'/>
    <signal name='NewToolTip'/>
    <signal name='NewTitle'/>
    <signal name='NewStatus'>
      <arg name='status' type='s'/>
    </signal>
  </interface>
</node>";

    // Rooted for the object's lifetime: GLib holds only the raw function pointers, so a collected delegate
    // would leave the vtable pointing at freed thunks.
    private readonly GDBusNative.MethodCallFunc _methodCall;

    private readonly GDBusNative.GetPropertyFunc _getProperty;

    private readonly GDBusNative.SignalCallback _nameOwnerChanged;

    private readonly IntPtr _connection;

    private readonly IntPtr _vtable;

    private readonly IntPtr _nodeInfo;

    private readonly string _busName;

    private readonly string _applicationId;

    private readonly DBusMenuExporter _menuExporter;

    private uint _registrationId;

    private uint _watcherSubscription;

    private bool _nameOwned;

    private Drawing.Icon? _icon;

    private string _toolTip = string.Empty;

    private bool _visible;

    /// <summary>
    /// Initialises a new instance of the <see cref="StatusNotifierItemBackend" /> class, exporting both
    /// D-Bus objects and announcing the item if a watcher is present.
    /// </summary>
    /// <exception cref="InvalidOperationException">The session bus is unreachable or export failed.</exception>
    public StatusNotifierItemBackend()
    {
        _applicationId = NotifyIconBackendFactory.GetApplicationId();

        _connection = GDBusNative.g_bus_get_sync(GDBusNative.BusTypeSession, IntPtr.Zero, out var busError);
        if (_connection == IntPtr.Zero)
        {
            FreeError(busError);
            throw new InvalidOperationException("The D-Bus session bus is not reachable.");
        }

        // Unique per process and per item, matching the naming the specification expects hosts to see.
        _busName = ItemInterface + "-" + GetProcessId() + "-" +
                   Threading.Interlocked.Increment(ref s_itemSequence);

        var xml = GDBusNative.ToUtf8(ItemInterfaceXml);
        var interfaceName = GDBusNative.ToUtf8(ItemInterface);

        try
        {
            _nodeInfo = GDBusNative.g_dbus_node_info_new_for_xml(xml, out var xmlError);
            if (_nodeInfo == IntPtr.Zero)
            {
                FreeError(xmlError);
                throw new InvalidOperationException("The StatusNotifierItem interface description is invalid.");
            }

            var interfaceInfo = GDBusNative.g_dbus_node_info_lookup_interface(_nodeInfo, interfaceName);

            _methodCall = OnMethodCall;
            _getProperty = OnGetProperty;

            var vtable = new GDBusNative.InterfaceVTable
            {
                MethodCall = Marshal.GetFunctionPointerForDelegate(_methodCall),
                GetProperty = Marshal.GetFunctionPointerForDelegate(_getProperty),
                SetProperty = IntPtr.Zero,
                Padding = new IntPtr[8],
            };

            _vtable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GDBusNative.InterfaceVTable)));
            Marshal.StructureToPtr(vtable, _vtable, false);

            var path = GDBusNative.ToUtf8(ItemPath);

            try
            {
                _registrationId = GDBusNative.g_dbus_connection_register_object(_connection, path,
                    interfaceInfo, _vtable, IntPtr.Zero, IntPtr.Zero, out var registerError);

                if (_registrationId == 0)
                {
                    FreeError(registerError);
                    throw new InvalidOperationException("Unable to export the StatusNotifierItem object.");
                }
            }
            finally
            {
                GDBusNative.FreeUtf8(path);
            }
        }
        finally
        {
            GDBusNative.FreeUtf8(interfaceName);
            GDBusNative.FreeUtf8(xml);
        }

        _menuExporter = new DBusMenuExporter(_connection);

        _nameOwnerChanged = OnWatcherOwnerChanged;
        SubscribeToWatcherLifetime();

        IsAvailable = HasWatcher();

        if (!IsAvailable)
        {
            Diagnostics.GtkLogger.Debug(
                "No " + WatcherName + " is registered on the session bus, so this desktop shows no system " +
                "tray. NotifyIcon will stay inert. On GNOME Shell this is provided by the AppIndicator " +
                "extension.");
        }
    }

    private static int s_itemSequence;

    /// <inheritdoc />
    public override bool IsAvailable { get; }

    /// <inheritdoc />
    public override void SetIcon(Drawing.Icon? icon)
    {
        _icon = icon;

        if (_visible)
        {
            EmitSignal("NewIcon", IntPtr.Zero);
        }
    }

    /// <inheritdoc />
    public override void SetToolTip(string text)
    {
        _toolTip = text ?? string.Empty;

        if (_visible)
        {
            EmitSignal("NewToolTip", IntPtr.Zero);
            EmitSignal("NewTitle", IntPtr.Zero);
        }
    }

    /// <inheritdoc />
    public override void SetVisible(bool visible)
    {
        if (IsDisposed || visible == _visible || !IsAvailable)
        {
            _visible = visible;
            return;
        }

        _visible = visible;

        if (visible)
        {
            if (RequestName())
            {
                RegisterWithWatcher();
            }
        }
        else
        {
            // StatusNotifierItem has no hide: a "Passive" status is advisory and hosts may still render
            // it. Releasing the name is the only way to make Visible=false mean what WinForms means.
            ReleaseName();
        }
    }

    /// <inheritdoc />
    public override void SetMenu(ContextMenuStrip? menu)
    {
        _menuExporter.SetMenu(menu);
    }

    /// <inheritdoc />
    public override void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon)
    {
        if (IsDisposed)
        {
            return;
        }

        // Desktop notifications are a separate service from the tray and work even where no tray host
        // exists, so this is deliberately not gated on IsAvailable.
        var iconName = icon switch
        {
            ToolTipIcon.Info => "dialog-information",
            ToolTipIcon.Warning => "dialog-warning",
            ToolTipIcon.Error => "dialog-error",
            _ => string.Empty,
        };

        var parameters = GDBusNative.NewTuple(
            GDBusNative.NewString(_applicationId),
            GDBusNative.g_variant_new_uint32(0),
            GDBusNative.NewString(iconName),
            GDBusNative.NewString(title ?? string.Empty),
            GDBusNative.NewString(text ?? string.Empty),
            GDBusNative.NewArray("s", Array.Empty<IntPtr>()),
            GDBusNative.NewArray("{sv}", Array.Empty<IntPtr>()),
            GDBusNative.g_variant_new_int32(timeout));

        CallAsync("org.freedesktop.Notifications", "/org/freedesktop/Notifications",
            "org.freedesktop.Notifications", "Notify", parameters);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_nameOwned)
        {
            ReleaseName();
        }

        _menuExporter.Dispose();

        if (_watcherSubscription != 0)
        {
            GDBusNative.g_dbus_connection_signal_unsubscribe(_connection, _watcherSubscription);
            _watcherSubscription = 0;
        }

        if (_registrationId != 0)
        {
            GDBusNative.g_dbus_connection_unregister_object(_connection, _registrationId);
            _registrationId = 0;
        }

        if (_nodeInfo != IntPtr.Zero)
        {
            GDBusNative.g_dbus_node_info_unref(_nodeInfo);
        }

        if (_vtable != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_vtable);
        }

        base.Dispose(disposing);
    }

    private void OnMethodCall(IntPtr connection, IntPtr sender, IntPtr objectPath, IntPtr interfaceName,
        IntPtr methodName, IntPtr parameters, IntPtr invocation, IntPtr userData)
    {
        try
        {
            var method = GDBusNative.FromUtf8(methodName);
            ReadPoint(parameters, out var x, out var y);

            switch (method)
            {
                case "Activate":
                    RaiseActivated(MouseButtons.Left, 1, x, y);
                    break;

                case "SecondaryActivate":
                    RaiseActivated(MouseButtons.Middle, 1, x, y);
                    break;

                case "ContextMenu":
                    // The host owns menu display via the exported dbusmenu object; nothing to do here.
                    break;

                case "Scroll":
                    break;
            }

            GDBusNative.g_dbus_method_invocation_return_value(invocation, IntPtr.Zero);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception handling a StatusNotifierItem call", ex);
            ReturnDBusError(invocation, ex);
        }
    }

    private IntPtr OnGetProperty(IntPtr connection, IntPtr sender, IntPtr objectPath, IntPtr interfaceName,
        IntPtr propertyName, out IntPtr error, IntPtr userData)
    {
        error = IntPtr.Zero;

        try
        {
            switch (GDBusNative.FromUtf8(propertyName))
            {
                case "Category":
                    return GDBusNative.NewString("ApplicationStatus");

                case "Id":
                    return GDBusNative.NewString(_applicationId);

                case "Title":
                    return GDBusNative.NewString(_toolTip.Length > 0 ? _toolTip : _applicationId);

                case "Status":
                    return GDBusNative.NewString("Active");

                case "WindowId":
                    return GDBusNative.g_variant_new_int32(0);

                case "IconName":
                case "OverlayIconName":
                case "AttentionIconName":
                    return GDBusNative.NewString(string.Empty);

                case "IconPixmap":
                    return BuildPixmapArray(_icon);

                case "ToolTip":
                    return GDBusNative.NewTuple(
                        GDBusNative.NewString(string.Empty),
                        GDBusNative.NewArray("(iiay)", Array.Empty<IntPtr>()),
                        GDBusNative.NewString(_toolTip),
                        GDBusNative.NewString(string.Empty));

                case "ItemIsMenu":
                    return GDBusNative.g_variant_new_boolean(0);

                case "Menu":
                    return GDBusNative.NewObjectPath(DBusMenuExporter.MenuPath);
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception reading a StatusNotifierItem property", ex);
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// Builds the <c>a(iiay)</c> pixmap array a StatusNotifierItem host renders.
    /// </summary>
    /// <param name="icon">The icon to publish, or <c>null</c>.</param>
    /// <returns>A floating <c>GVariant*</c>, empty when there is no icon.</returns>
    private static IntPtr BuildPixmapArray(Drawing.Icon? icon)
    {
        var pixbuf = icon?.Pixbuf;
        if (pixbuf == null)
        {
            return GDBusNative.NewArray("(iiay)", Array.Empty<IntPtr>());
        }

        Gdk.Pixbuf? withAlpha = null;

        try
        {
            var source = pixbuf;

            if (!source.HasAlpha)
            {
                withAlpha = source.AddAlpha(false, 0, 0, 0);
                source = withAlpha;
            }

            var entry = GDBusNative.NewTuple(
                GDBusNative.g_variant_new_int32(source.Width),
                GDBusNative.g_variant_new_int32(source.Height),
                GDBusNative.NewByteArray(ToArgb32(source)));

            return GDBusNative.NewArray("(iiay)", new[] { entry });
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to convert the icon for the StatusNotifierItem host", ex);
            return GDBusNative.NewArray("(iiay)", Array.Empty<IntPtr>());
        }
        finally
        {
            withAlpha?.Dispose();
        }
    }

    /// <summary>
    /// Converts pixbuf pixels to the ARGB32 network-byte-order layout the StatusNotifierItem spec requires.
    /// </summary>
    /// <param name="pixbuf">The source pixbuf.</param>
    /// <returns>The pixel data, four bytes per pixel, top row first.</returns>
    /// <remarks>
    /// Rows are read through <c>Rowstride</c>, which is not generally <c>Width * NChannels</c>; assuming
    /// otherwise skews the image for most icon sizes.
    /// </remarks>
    internal static byte[] ToArgb32(Gdk.Pixbuf pixbuf)
    {
        var width = pixbuf.Width;
        var height = pixbuf.Height;
        var channels = pixbuf.NChannels;
        var rowstride = pixbuf.Rowstride;
        var hasAlpha = pixbuf.HasAlpha;

        var row = new byte[rowstride];
        var result = new byte[width * height * 4];
        var pixels = pixbuf.Pixels;

        for (var y = 0; y < height; y++)
        {
            Marshal.Copy(pixels + (y * rowstride), row, 0, rowstride);

            for (var x = 0; x < width; x++)
            {
                var src = x * channels;
                var dst = ((y * width) + x) * 4;

                result[dst + 0] = hasAlpha ? row[src + 3] : (byte)0xFF;
                result[dst + 1] = row[src + 0];
                result[dst + 2] = row[src + 1];
                result[dst + 3] = row[src + 2];
            }
        }

        return result;
    }

    private static void ReadPoint(IntPtr parameters, out int x, out int y)
    {
        x = 0;
        y = 0;

        if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) < 2)
        {
            return;
        }

        var first = GDBusNative.g_variant_get_child_value(parameters, UIntPtr.Zero);
        var second = GDBusNative.g_variant_get_child_value(parameters, new UIntPtr(1));

        try
        {
            x = GDBusNative.g_variant_get_int32(first);
            y = GDBusNative.g_variant_get_int32(second);
        }
        finally
        {
            GDBusNative.g_variant_unref(first);
            GDBusNative.g_variant_unref(second);
        }
    }

    private bool HasWatcher()
    {
        var reply = CallSync("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus",
            "NameHasOwner", GDBusNative.NewTuple(GDBusNative.NewString(WatcherName)));

        if (reply == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            var child = GDBusNative.g_variant_get_child_value(reply, UIntPtr.Zero);

            try
            {
                return GDBusNative.g_variant_get_boolean(child) != 0;
            }
            finally
            {
                GDBusNative.g_variant_unref(child);
            }
        }
        finally
        {
            GDBusNative.g_variant_unref(reply);
        }
    }

    private bool RequestName()
    {
        var reply = CallSync("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus",
            "RequestName",
            GDBusNative.NewTuple(GDBusNative.NewString(_busName), GDBusNative.g_variant_new_uint32(0)));

        if (reply == IntPtr.Zero)
        {
            return false;
        }

        GDBusNative.g_variant_unref(reply);
        _nameOwned = true;

        return true;
    }

    private void ReleaseName()
    {
        if (!_nameOwned)
        {
            return;
        }

        var reply = CallSync("org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus",
            "ReleaseName", GDBusNative.NewTuple(GDBusNative.NewString(_busName)));

        if (reply != IntPtr.Zero)
        {
            GDBusNative.g_variant_unref(reply);
        }

        _nameOwned = false;
    }

    private void RegisterWithWatcher()
    {
        CallAsync(WatcherName, WatcherPath, WatcherName, "RegisterStatusNotifierItem",
            GDBusNative.NewTuple(GDBusNative.NewString(_busName)));
    }

    private void SubscribeToWatcherLifetime()
    {
        var sender = GDBusNative.ToUtf8("org.freedesktop.DBus");
        var iface = GDBusNative.ToUtf8("org.freedesktop.DBus");
        var member = GDBusNative.ToUtf8("NameOwnerChanged");
        var path = GDBusNative.ToUtf8("/org/freedesktop/DBus");
        var arg0 = GDBusNative.ToUtf8(WatcherName);

        try
        {
            _watcherSubscription = GDBusNative.g_dbus_connection_signal_subscribe(_connection, sender, iface,
                member, path, arg0, 0, Marshal.GetFunctionPointerForDelegate(_nameOwnerChanged), IntPtr.Zero,
                IntPtr.Zero);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to watch for the StatusNotifierWatcher restarting", ex);
        }
        finally
        {
            GDBusNative.FreeUtf8(arg0);
            GDBusNative.FreeUtf8(path);
            GDBusNative.FreeUtf8(member);
            GDBusNative.FreeUtf8(iface);
            GDBusNative.FreeUtf8(sender);
        }
    }

    private void OnWatcherOwnerChanged(IntPtr connection, IntPtr senderName, IntPtr objectPath,
        IntPtr interfaceName, IntPtr signalName, IntPtr parameters, IntPtr userData)
    {
        try
        {
            // (name, oldOwner, newOwner): a non-empty new owner means the shell came back, so the item
            // has to announce itself again or it stays invisible until the app restarts. This is the
            // Linux counterpart of Explorer's TaskbarCreated broadcast.
            if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) < 3)
            {
                return;
            }

            var newOwner = GDBusNative.g_variant_get_child_value(parameters, new UIntPtr(2));

            try
            {
                var owner = GDBusNative.FromUtf8(GDBusNative.g_variant_get_string(newOwner, IntPtr.Zero));

                if (owner.Length > 0 && _visible && _nameOwned)
                {
                    RegisterWithWatcher();
                }
            }
            finally
            {
                GDBusNative.g_variant_unref(newOwner);
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception re-registering the StatusNotifierItem", ex);
        }
    }

    private void EmitSignal(string name, IntPtr parameters)
    {
        var path = GDBusNative.ToUtf8(ItemPath);
        var iface = GDBusNative.ToUtf8(ItemInterface);
        var signal = GDBusNative.ToUtf8(name);

        try
        {
            GDBusNative.g_dbus_connection_emit_signal(_connection, IntPtr.Zero, path, iface, signal,
                parameters, out var error);
            FreeError(error);
        }
        finally
        {
            GDBusNative.FreeUtf8(signal);
            GDBusNative.FreeUtf8(iface);
            GDBusNative.FreeUtf8(path);
        }
    }

    private IntPtr CallSync(string busName, string objectPath, string interfaceName, string method,
        IntPtr parameters)
    {
        var name = GDBusNative.ToUtf8(busName);
        var path = GDBusNative.ToUtf8(objectPath);
        var iface = GDBusNative.ToUtf8(interfaceName);
        var member = GDBusNative.ToUtf8(method);

        try
        {
            var reply = GDBusNative.g_dbus_connection_call_sync(_connection, name, path, iface, member,
                parameters, IntPtr.Zero, 0, 2000, IntPtr.Zero, out var error);

            if (reply == IntPtr.Zero)
            {
                FreeError(error);
            }

            return reply;
        }
        finally
        {
            GDBusNative.FreeUtf8(member);
            GDBusNative.FreeUtf8(iface);
            GDBusNative.FreeUtf8(path);
            GDBusNative.FreeUtf8(name);
        }
    }

    private void CallAsync(string busName, string objectPath, string interfaceName, string method,
        IntPtr parameters)
    {
        var name = GDBusNative.ToUtf8(busName);
        var path = GDBusNative.ToUtf8(objectPath);
        var iface = GDBusNative.ToUtf8(interfaceName);
        var member = GDBusNative.ToUtf8(method);

        try
        {
            GDBusNative.g_dbus_connection_call(_connection, name, path, iface, member, parameters,
                IntPtr.Zero, 0, 2000, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }
        finally
        {
            GDBusNative.FreeUtf8(member);
            GDBusNative.FreeUtf8(iface);
            GDBusNative.FreeUtf8(path);
            GDBusNative.FreeUtf8(name);
        }
    }

    internal static void ReturnDBusError(IntPtr invocation, Exception ex)
    {
        var name = GDBusNative.ToUtf8("org.freedesktop.DBus.Error.Failed");
        var message = GDBusNative.ToUtf8(ex.Message);

        try
        {
            GDBusNative.g_dbus_method_invocation_return_dbus_error(invocation, name, message);
        }
        finally
        {
            GDBusNative.FreeUtf8(message);
            GDBusNative.FreeUtf8(name);
        }
    }

    internal static void FreeError(IntPtr error)
    {
        if (error != IntPtr.Zero)
        {
            GDBusNative.g_error_free(error);
        }
    }

    private static int GetProcessId()
    {
        try
        {
            return Diagnostics.Process.GetCurrentProcess().Id;
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to read the process id for the StatusNotifierItem name", ex);
            return 0;
        }
    }
}
