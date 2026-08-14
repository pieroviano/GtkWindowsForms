using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Exports a <see cref="ContextMenuStrip" /> as a <c>com.canonical.dbusmenu</c> object, which is how a
/// StatusNotifierItem host obtains the tray icon's context menu.
/// </summary>
/// <remarks>
/// <para>
/// The host, not the application, renders this menu: it fetches the layout, draws it with its own widgets,
/// and posts back an <c>Event</c> when the user picks something. So the whole menu has to be expressible
/// as dbusmenu properties - which is why only text, enabled, visible, separator, check state and submenu
/// nesting survive the translation, and why images, fonts, colours and owner-draw do not.
/// </para>
/// <para>
/// The layout carries a revision number. Hosts cache the layout and only re-fetch it when
/// <c>LayoutUpdated</c> announces a newer revision, so the menu is re-translated on every
/// <c>AboutToShow</c> and the revision bumped whenever the result differs from what was last published.
/// That keeps a strip mutated after assignment correct without having to observe every collection change.
/// </para>
/// </remarks>
internal sealed class DBusMenuExporter : IDisposable
{
    /// <summary>The object path this menu is exported at.</summary>
    public const string MenuPath = "/MenuBar";

    private const string MenuInterface = "com.canonical.dbusmenu";

    private const string MenuInterfaceXml = @"<node>
  <interface name='com.canonical.dbusmenu'>
    <property name='Version' type='u' access='read'/>
    <property name='TextDirection' type='s' access='read'/>
    <property name='Status' type='s' access='read'/>
    <property name='IconThemePath' type='as' access='read'/>
    <method name='GetLayout'>
      <arg type='i' name='parentId' direction='in'/>
      <arg type='i' name='recursionDepth' direction='in'/>
      <arg type='as' name='propertyNames' direction='in'/>
      <arg type='u' name='revision' direction='out'/>
      <arg type='(ia{sv}av)' name='layout' direction='out'/>
    </method>
    <method name='GetGroupProperties'>
      <arg type='ai' name='ids' direction='in'/>
      <arg type='as' name='propertyNames' direction='in'/>
      <arg type='a(ia{sv})' name='properties' direction='out'/>
    </method>
    <method name='GetProperty'>
      <arg type='i' name='id' direction='in'/>
      <arg type='s' name='name' direction='in'/>
      <arg type='v' name='value' direction='out'/>
    </method>
    <method name='Event'>
      <arg type='i' name='id' direction='in'/>
      <arg type='s' name='eventId' direction='in'/>
      <arg type='v' name='data' direction='in'/>
      <arg type='u' name='timestamp' direction='in'/>
    </method>
    <method name='EventGroup'>
      <arg type='a(isvu)' name='events' direction='in'/>
      <arg type='ai' name='idErrors' direction='out'/>
    </method>
    <method name='AboutToShow'>
      <arg type='i' name='id' direction='in'/>
      <arg type='b' name='needUpdate' direction='out'/>
    </method>
  </interface>
</node>";

    // Rooted for the object's lifetime; GLib keeps only the raw function pointers.
    private readonly GDBusNative.MethodCallFunc _methodCall;

    private readonly GDBusNative.GetPropertyFunc _getProperty;

    private readonly IntPtr _connection;

    private readonly IntPtr _vtable;

    private readonly IntPtr _nodeInfo;

    private uint _registrationId;

    private ContextMenuStrip? _menu;

    private TrayMenuModel? _published;

    private uint _revision = 1;

    /// <summary>
    /// Initialises a new instance of the <see cref="DBusMenuExporter" /> class and exports the menu object.
    /// </summary>
    /// <param name="connection">The session bus connection to export on.</param>
    /// <exception cref="InvalidOperationException">The object could not be exported.</exception>
    public DBusMenuExporter(IntPtr connection)
    {
        _connection = connection;

        var xml = GDBusNative.ToUtf8(MenuInterfaceXml);
        var interfaceName = GDBusNative.ToUtf8(MenuInterface);
        var path = GDBusNative.ToUtf8(MenuPath);

        try
        {
            _nodeInfo = GDBusNative.g_dbus_node_info_new_for_xml(xml, out var xmlError);
            if (_nodeInfo == IntPtr.Zero)
            {
                StatusNotifierItemBackend.FreeError(xmlError);
                throw new InvalidOperationException("The dbusmenu interface description is invalid.");
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

            _registrationId = GDBusNative.g_dbus_connection_register_object(_connection, path, interfaceInfo,
                _vtable, IntPtr.Zero, IntPtr.Zero, out var registerError);

            if (_registrationId == 0)
            {
                StatusNotifierItemBackend.FreeError(registerError);
                throw new InvalidOperationException("Unable to export the dbusmenu object.");
            }
        }
        finally
        {
            GDBusNative.FreeUtf8(path);
            GDBusNative.FreeUtf8(interfaceName);
            GDBusNative.FreeUtf8(xml);
        }
    }

    /// <summary>Sets or clears the menu this object exports.</summary>
    /// <param name="menu">The menu, or <c>null</c> for an empty menu.</param>
    public void SetMenu(ContextMenuStrip? menu)
    {
        _menu = menu;
        Republish();
    }

    /// <summary>Unexports the object and releases its native resources.</summary>
    public void Dispose()
    {
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
    }

    private TrayMenuModel Current => _published ??= TrayMenuModel.From(_menu) ?? TrayMenuModel.Empty();

    /// <summary>
    /// Re-translates the strip and, if the result differs from what hosts already have, bumps the revision
    /// and announces it.
    /// </summary>
    /// <returns><c>true</c> when the layout changed.</returns>
    private bool Republish()
    {
        var rebuilt = TrayMenuModel.From(_menu) ?? TrayMenuModel.Empty();
        var changed = _published == null || !Describe(rebuilt).Equals(Describe(_published), StringComparison.Ordinal);

        _published = rebuilt;

        if (changed)
        {
            _revision++;
            EmitLayoutUpdated();
        }

        return changed;
    }

    /// <summary>
    /// Produces a comparable description of a model, used to decide whether hosts must re-fetch.
    /// </summary>
    /// <param name="model">The model to describe.</param>
    /// <returns>A string that changes whenever any published property changes.</returns>
    private static string Describe(TrayMenuModel model)
    {
        var builder = new Text.StringBuilder();
        Describe(model.Root, builder);

        return builder.ToString();
    }

    private static void Describe(TrayMenuNode node, Text.StringBuilder builder)
    {
        builder.Append(node.Id).Append('|').Append(node.Text).Append('|').Append(node.Enabled ? '1' : '0')
            .Append(node.IsSeparator ? '1' : '0').Append(node.IsCheckable ? '1' : '0')
            .Append(node.Checked ? '1' : '0').Append('{');

        foreach (var child in node.Children)
        {
            Describe(child, builder);
        }

        builder.Append('}');
    }

    private void OnMethodCall(IntPtr connection, IntPtr sender, IntPtr objectPath, IntPtr interfaceName,
        IntPtr methodName, IntPtr parameters, IntPtr invocation, IntPtr userData)
    {
        try
        {
            switch (GDBusNative.FromUtf8(methodName))
            {
                case "GetLayout":
                    HandleGetLayout(parameters, invocation);
                    break;

                case "GetGroupProperties":
                    HandleGetGroupProperties(parameters, invocation);
                    break;

                case "GetProperty":
                    HandleGetProperty(parameters, invocation);
                    break;

                case "Event":
                    HandleEvent(parameters);
                    GDBusNative.g_dbus_method_invocation_return_value(invocation, IntPtr.Zero);
                    break;

                case "EventGroup":
                    HandleEventGroup(parameters);
                    GDBusNative.g_dbus_method_invocation_return_value(invocation,
                        GDBusNative.NewTuple(GDBusNative.NewArray("i", Array.Empty<IntPtr>())));
                    break;

                case "AboutToShow":
                    // The one moment the host guarantees it is about to read the menu, and therefore the
                    // right moment to re-translate a strip that may have changed since assignment.
                    var changed = Republish();
                    GDBusNative.g_dbus_method_invocation_return_value(invocation,
                        GDBusNative.NewTuple(GDBusNative.g_variant_new_boolean(changed ? 1 : 0)));
                    break;

                default:
                    GDBusNative.g_dbus_method_invocation_return_value(invocation, IntPtr.Zero);
                    break;
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception handling a dbusmenu call", ex);
            StatusNotifierItemBackend.ReturnDBusError(invocation, ex);
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
                case "Version":
                    return GDBusNative.g_variant_new_uint32(3);

                case "TextDirection":
                    return GDBusNative.NewString("ltr");

                case "Status":
                    return GDBusNative.NewString("normal");

                case "IconThemePath":
                    return GDBusNative.NewArray("s", Array.Empty<IntPtr>());
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception reading a dbusmenu property", ex);
        }

        return IntPtr.Zero;
    }

    private void HandleGetLayout(IntPtr parameters, IntPtr invocation)
    {
        var parentId = ReadInt32(parameters, 0);
        var depth = ReadInt32(parameters, 1);

        var model = Current;
        var node = model.Find(parentId) ?? model.Root;

        var layout = BuildLayout(node, depth);
        GDBusNative.g_dbus_method_invocation_return_value(invocation,
            GDBusNative.NewTuple(GDBusNative.g_variant_new_uint32(_revision), layout));
    }

    /// <summary>
    /// Builds one <c>(ia{sv}av)</c> layout node and, subject to the requested depth, its descendants.
    /// </summary>
    /// <param name="node">The node to serialise.</param>
    /// <param name="depth">The remaining recursion depth; negative means unlimited.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    private IntPtr BuildLayout(TrayMenuNode node, int depth)
    {
        var children = new List<IntPtr>();

        if (depth != 0)
        {
            var childDepth = depth < 0 ? -1 : depth - 1;

            foreach (var child in node.Children)
            {
                children.Add(GDBusNative.g_variant_new_variant(BuildLayout(child, childDepth)));
            }
        }

        return GDBusNative.NewTuple(
            GDBusNative.g_variant_new_int32(node.Id),
            BuildProperties(node),
            GDBusNative.NewArray("v", children.ToArray()));
    }

    /// <summary>
    /// Builds the <c>a{sv}</c> property dictionary for one node.
    /// </summary>
    /// <param name="node">The node to describe.</param>
    /// <returns>A floating <c>GVariant*</c>.</returns>
    /// <remarks>
    /// Only properties that differ from the dbusmenu defaults are emitted, which is what the specification
    /// asks for and keeps the payload small.
    /// </remarks>
    private static IntPtr BuildProperties(TrayMenuNode node)
    {
        var entries = new List<IntPtr>();

        if (node.Id == 0)
        {
            entries.Add(GDBusNative.NewDictEntry("children-display", GDBusNative.NewString("submenu")));
            return GDBusNative.NewArray("{sv}", entries.ToArray());
        }

        if (node.IsSeparator)
        {
            entries.Add(GDBusNative.NewDictEntry("type", GDBusNative.NewString("separator")));
            return GDBusNative.NewArray("{sv}", entries.ToArray());
        }

        entries.Add(GDBusNative.NewDictEntry("label",
            GDBusNative.NewString(TrayMenuModel.ToDBusMenuLabel(node.Text))));

        if (!node.Enabled)
        {
            entries.Add(GDBusNative.NewDictEntry("enabled", GDBusNative.g_variant_new_boolean(0)));
        }

        if (node.IsCheckable)
        {
            entries.Add(GDBusNative.NewDictEntry("toggle-type", GDBusNative.NewString("checkmark")));
            entries.Add(GDBusNative.NewDictEntry("toggle-state",
                GDBusNative.g_variant_new_int32(node.Checked ? 1 : 0)));
        }

        if (node.HasChildren)
        {
            entries.Add(GDBusNative.NewDictEntry("children-display", GDBusNative.NewString("submenu")));
        }

        return GDBusNative.NewArray("{sv}", entries.ToArray());
    }

    private void HandleGetGroupProperties(IntPtr parameters, IntPtr invocation)
    {
        var model = Current;
        var entries = new List<IntPtr>();

        var ids = ReadInt32Array(parameters, 0);

        if (ids.Count == 0)
        {
            Collect(model.Root, entries);
        }
        else
        {
            foreach (var id in ids)
            {
                var node = model.Find(id);
                if (node != null)
                {
                    entries.Add(GDBusNative.NewTuple(GDBusNative.g_variant_new_int32(node.Id),
                        BuildProperties(node)));
                }
            }
        }

        GDBusNative.g_dbus_method_invocation_return_value(invocation,
            GDBusNative.NewTuple(GDBusNative.NewArray("(ia{sv})", entries.ToArray())));
    }

    private static void Collect(TrayMenuNode node, List<IntPtr> entries)
    {
        entries.Add(GDBusNative.NewTuple(GDBusNative.g_variant_new_int32(node.Id), BuildProperties(node)));

        foreach (var child in node.Children)
        {
            Collect(child, entries);
        }
    }

    private void HandleGetProperty(IntPtr parameters, IntPtr invocation)
    {
        var id = ReadInt32(parameters, 0);
        var name = ReadString(parameters, 1);
        var node = Current.Find(id);

        var value = node == null
            ? GDBusNative.NewString(string.Empty)
            : name switch
            {
                "label" => GDBusNative.NewString(TrayMenuModel.ToDBusMenuLabel(node.Text)),
                "enabled" => GDBusNative.g_variant_new_boolean(node.Enabled ? 1 : 0),
                "visible" => GDBusNative.g_variant_new_boolean(1),
                "type" => GDBusNative.NewString(node.IsSeparator ? "separator" : "standard"),
                "toggle-type" => GDBusNative.NewString(node.IsCheckable ? "checkmark" : string.Empty),
                "toggle-state" => GDBusNative.g_variant_new_int32(node.Checked ? 1 : 0),
                "children-display" => GDBusNative.NewString(node.HasChildren ? "submenu" : string.Empty),
                _ => GDBusNative.NewString(string.Empty),
            };

        GDBusNative.g_dbus_method_invocation_return_value(invocation,
            GDBusNative.NewTuple(GDBusNative.g_variant_new_variant(value)));
    }

    private void HandleEvent(IntPtr parameters)
    {
        var id = ReadInt32(parameters, 0);
        var eventId = ReadString(parameters, 1);

        if (!string.Equals(eventId, "clicked", StringComparison.Ordinal))
        {
            return;
        }

        var model = Current;

        // The host delivers this on the D-Bus worker context; WinForms handlers must not run there.
        NotifyIconBackendBase.InvokeOnMainLoop(() => model.Activate(id));
    }

    private void HandleEventGroup(IntPtr parameters)
    {
        if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) < 1)
        {
            return;
        }

        var events = GDBusNative.g_variant_get_child_value(parameters, UIntPtr.Zero);

        try
        {
            var count = (ulong)GDBusNative.g_variant_n_children(events);

            for (ulong i = 0; i < count; i++)
            {
                var entry = GDBusNative.g_variant_get_child_value(events, new UIntPtr(i));

                try
                {
                    HandleEvent(entry);
                }
                finally
                {
                    GDBusNative.g_variant_unref(entry);
                }
            }
        }
        finally
        {
            GDBusNative.g_variant_unref(events);
        }
    }

    private void EmitLayoutUpdated()
    {
        var path = GDBusNative.ToUtf8(MenuPath);
        var iface = GDBusNative.ToUtf8(MenuInterface);
        var signal = GDBusNative.ToUtf8("LayoutUpdated");

        try
        {
            var parameters = GDBusNative.NewTuple(
                GDBusNative.g_variant_new_uint32(_revision),
                GDBusNative.g_variant_new_int32(0));

            GDBusNative.g_dbus_connection_emit_signal(_connection, IntPtr.Zero, path, iface, signal,
                parameters, out var error);
            StatusNotifierItemBackend.FreeError(error);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to announce the dbusmenu layout change", ex);
        }
        finally
        {
            GDBusNative.FreeUtf8(signal);
            GDBusNative.FreeUtf8(iface);
            GDBusNative.FreeUtf8(path);
        }
    }

    private static int ReadInt32(IntPtr parameters, ulong index)
    {
        if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) <= index)
        {
            return 0;
        }

        var child = GDBusNative.g_variant_get_child_value(parameters, new UIntPtr(index));

        try
        {
            return GDBusNative.g_variant_get_int32(child);
        }
        finally
        {
            GDBusNative.g_variant_unref(child);
        }
    }

    private static string ReadString(IntPtr parameters, ulong index)
    {
        if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) <= index)
        {
            return string.Empty;
        }

        var child = GDBusNative.g_variant_get_child_value(parameters, new UIntPtr(index));

        try
        {
            return GDBusNative.FromUtf8(GDBusNative.g_variant_get_string(child, IntPtr.Zero));
        }
        finally
        {
            GDBusNative.g_variant_unref(child);
        }
    }

    private static List<int> ReadInt32Array(IntPtr parameters, ulong index)
    {
        var result = new List<int>();

        if (parameters == IntPtr.Zero || (ulong)GDBusNative.g_variant_n_children(parameters) <= index)
        {
            return result;
        }

        var array = GDBusNative.g_variant_get_child_value(parameters, new UIntPtr(index));

        try
        {
            var count = (ulong)GDBusNative.g_variant_n_children(array);

            for (ulong i = 0; i < count; i++)
            {
                var child = GDBusNative.g_variant_get_child_value(array, new UIntPtr(i));

                try
                {
                    result.Add(GDBusNative.g_variant_get_int32(child));
                }
                finally
                {
                    GDBusNative.g_variant_unref(child);
                }
            }
        }
        finally
        {
            GDBusNative.g_variant_unref(array);
        }

        return result;
    }
}
