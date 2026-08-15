# Plan — Reinstating the system-tray icon (`NotifyIcon`) under Gtk4

**Status:** **implemented.** See §10 for what was verified and what is still unverified.
**Target:** `Gtk.Windows.Forms` (`netstandard2.0`), Windows + Linux + macOS.
**Driver:** Gtk4 removed `GtkStatusIcon` with no replacement, so [NotifyIcon.cs](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs) is currently inert — `statusIcon` is a permanently-`null` `object?` ([NotifyIcon.cs:27](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L27), [NotifyIcon.cs:133](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L133)) and `Visible` only round-trips a backing field ([NotifyIcon.cs:295-297](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L295-L297)).

The key insight framing this whole plan: `GtkStatusIcon` was never one API. It was three platform backends (X11 XEmbed, Win32 `Shell_NotifyIcon`, Quartz `NSStatusItem`) behind one GObject. We re-implement the same three backends in C#. **Nothing here is a GtkSharp bindings change** — there is no Gtk API to expose, so hand-written partials in `Net4x.GtkSharp` are the wrong layer.

---

## 0. Blocking verification — do these three spikes before writing any production code

Each is a throwaway spike. If any fails, the corresponding backend's design changes materially, so none of section 4 should be written until all three are answered.

### V1 — Does a message-only HWND receive dispatched messages under Gtk4's main loop? **(highest risk)**

The Win32 backend is worthless if nothing calls `DispatchMessage` for our window. Gdk's Win32 event source pumps the thread queue with `PeekMessage`/`DispatchMessage` and forwards messages it does not own — that is exactly how Gtk3's own Win32 status icon worked — but this must be *proved* against the Gtk4 build we ship, not assumed.

**Spike:** in a scratch console app that calls `Application.Init()` and runs a Gtk4 main loop, `RegisterClassExW` a private class, `CreateWindowExW` with `HWND_MESSAGE` (`-3`) as parent, `PostMessageW` a `WM_APP+1` to it from a `GLib.Timeout`, and assert the `WndProc` fires.

**If it fails:** the Win32 backend needs its own dedicated thread running a classic `GetMessage`/`TranslateMessage`/`DispatchMessage` loop, and every callback must be marshalled back to the Gtk main loop with `GLib.Idle.Add`. That is roughly +0.5 day and adds a thread-affinity constraint to `Shell_NotifyIcon` calls (the icon must be added/modified/deleted on the thread owning the HWND).

### V2 — Is the generated `Gio.DBusConnection.RegisterObject` usable from C#?

`GioSharp-api.xml` binds it as

```xml
<method name="RegisterObject" cname="g_dbus_connection_register_object">
  <return-type type="guint" />
  <parameters throws="1">
    <parameter type="const-gchar*" name="object_path" />
    <parameter type="GDBusInterfaceInfo*" name="interface_info" />
    <parameter type="const-GDBusInterfaceVTable*" name="vtable" />
    ...
```

with `DBusInterfaceVTable` bound as a struct whose `MethodCall` / `GetProperty` / `SetProperty` fields are `is_callback="1"`. `Gio.DBusNodeInfo.NewForXml` is bound, `g_bus_own_name*` is bound, and `Gio.DBusConnection.EmitSignal` is bound. On paper everything needed is present.

**Spike:** against the built `GioSharp.dll`, export a trivial interface with one method and one property on the session bus, call it with `gdbus call`, and confirm (a) the generated vtable delegate fields marshal, and (b) the delegates are not GC'd — Gtk# does not root vtable callbacks for you.

**If it fails:** declare `GDBusInterfaceVTable` and `g_dbus_connection_register_object` by hand as `DllImport("libgio-2.0-0")` (name resolved through the existing [GLibrary.cs](Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/GLibrary.cs) / [FuncLoader.cs](Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/FuncLoader.cs) loader, which already handles the per-platform library naming). Cost ≈ +0.5 day, no design change.

### V3 — macOS: `objc_msgSend` prototypes and Gtk4's Quartz backend

On arm64 `objc_msgSend` is **not** variadic-safe: every call needs its own exactly-typed `DllImport`. Also confirm the Gtk4 macOS backend genuinely runs an `NSApplication` (it does, but the packaged Net4x.GtkSharp macOS build must be confirmed to include the quartz backend rather than X11-via-XQuartz).

**Spike:** from a Gtk4 app on macOS, `objc_getClass("NSStatusBar")` → `systemStatusBar` → `statusItemWithLength:` and confirm an (empty) item appears in the menu bar.

**If it fails** (no quartz backend in our packaging): the macOS backend is deferred and `IsSystemTrayAvailable` returns `false` there — the rest of the plan is unaffected.

---

## 1. Decisions taken (and their consequences)

| # | Decision | Consequence |
|---|---|---|
| D1 | **Linux = hand-rolled StatusNotifierItem over GDBus** (via GioSharp), not `libayatana-appindicator`, not Tmds.DBus | No new NuGet or native runtime dependency; `netstandard2.0` stays clean; works on X11 *and* Wayland. Cost: we own the `com.canonical.dbusmenu` implementation, which is the single largest work item in this plan. |
| D2 | **Menu fidelity = text, enabled, visible, checked, separators, submenus** | Covers what tray menus actually use. Item **images, fonts, colours and owner-draw are explicitly not supported** and are silently ignored on all three backends. `ToolStripItem.Image` set by a consumer will simply not render. |
| D3 | **No tray host ⇒ degrade silently + expose a probe** | All existing property round-tripping is preserved byte-for-byte (so [ToolStripNotifyIconFunctionalTests.cs:216-261](Gtk.Windows.Forms.Tests/System/Windows/Forms/ToolStripNotifyIconFunctionalTests.cs#L216-L261) keeps passing unchanged). A new `NotifyIcon.IsSystemTrayAvailable` lets apps detect it. Balloons still route to desktop notifications where those work without a tray. |
| D4 | **Backends ship as `Resources/` overlay files; the `NotifyIcon` rewrite ships as one `patches/` patch** | New files stay readable and re-rollable; only the genuine edit to generated code is a patch. Upstream changes to `NotifyIcon.cs` still 3-way merge instead of being silently shadowed. |

Two further decisions implied by D3 and worth stating explicitly, because both are visible behaviour changes:

- **D5 — the fake-window balloon is removed.** [NotifyIcon.cs:309-372](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L309-L372) currently fabricates an undecorated `Gtk.Window` as a "balloon", and the constructor at [NotifyIcon.cs:138-143](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L138-L143) creates and shows a 1×1 decorationless window purely to parent it. Both go. Balloons become real desktop notifications (`NIF_INFO` on Windows, `org.freedesktop.Notifications` on Linux); on macOS see R6.
- **D6 — `Visible = false` truly hides.** On SNI this means releasing the bus name, not merely setting `Status = "Passive"` (hosts may still render Passive items). Re-showing re-owns the name.

---

## 2. Behaviour fidelity matrix — what each backend can and cannot deliver

This table is the honest contract and should be reproduced in the XML docs of the affected members. WinForms' `NotifyIcon` exposes a richer input surface than two of the three native trays can observe. **Nothing is synthesised to paper over the gaps** — an event that a platform cannot observe is simply never raised.

| WinForms member | Windows | Linux (SNI) | macOS |
|---|---|---|---|
| `Icon` | ✅ HICON | ✅ `IconPixmap` ARGB32 | ✅ `NSImage` |
| `Text` (tooltip) | ✅ (127 char cap) | ✅ `ToolTip` property | ✅ `setToolTip:` |
| `Visible` | ✅ add/delete | ✅ own/unown bus name (D6) | ✅ `setVisible:` |
| `ContextMenuStrip` | ✅ `TrackPopupMenuEx` | ✅ dbusmenu (host renders) | ✅ `NSMenu` |
| `Click` / `MouseClick` | ✅ | ✅ from `Activate` | ✅ from button action |
| `DoubleClick` / `MouseDoubleClick` | ✅ `WM_LBUTTONDBLCLK` | ❌ no such concept in SNI | ⚠️ via `NSEvent.clickCount` |
| `MouseDown` / `MouseUp` | ✅ | ❌ | ⚠️ up only |
| `MouseMove` | ✅ | ❌ | ❌ |
| `ShowBalloonTip` | ✅ `NIF_INFO` | ✅ `org.freedesktop.Notifications` | ⚠️ needs signed bundle (R6) |
| `BalloonTipShown/Clicked/Closed` | ✅ `NIN_BALLOON*` | ⚠️ `ActionInvoked`/`NotificationClosed` | ❌ |
| Middle-click | ✅ `WM_MBUTTONUP` | ✅ `SecondaryActivate` | ❌ |
| `ShowBalloonTip(timeout)` honoured | ❌ system-controlled since Vista | ✅ `expire_timeout` | n/a |

---

## 3. File layout

New overlay files, all declaring `namespace System.Windows.Forms`. The pipeline copies from `Resources/` at [TransformPipeline.cs:61](PrepareProjects/TransformPipeline.cs#L61) and *then* reorganises by namespace at [TransformPipeline.cs:74-80](PrepareProjects/TransformPipeline.cs#L74-L80), so — exactly as [Resources/NonStaticMessageBox.cs](Resources/NonStaticMessageBox.cs) already demonstrates — copying into `GTKControls\` lands the file at `System\Windows\Forms\`.

| New file in `Resources/` | Copied to (pre-reorg) | Final location |
|---|---|---|
| `NotifyIconBackend.cs` | `GTKControls\NotifyIconBackend.cs` | `System\Windows\Forms\NotifyIconBackend.cs` |
| `TrayMenuModel.cs` | `GTKControls\TrayMenuModel.cs` | `System\Windows\Forms\TrayMenuModel.cs` |
| `NotifyIconBackend.Win32.cs` | `GTKControls\NotifyIconBackend.Win32.cs` | `System\Windows\Forms\…` |
| `NotifyIconBackend.StatusNotifier.cs` | idem | idem |
| `NotifyIconBackend.DBusMenu.cs` | idem | idem |
| `NotifyIconBackend.Quartz.cs` | idem | idem |

Modified: [Resources/../ResourcesCopyService.cs](PrepareProjects/Preparation/Source/ResourcesCopyService.cs) and one new `patches/0127-Reinstate-NotifyIcon-system-tray.patch`.

---

## 4. Design

### 4.1 The backend contract — `Resources/NotifyIconBackend.cs`

```csharp
namespace System.Windows.Forms;

/// <summary>Normalised tray pointer input. <see cref="Clicks"/> is 1 unless the platform
/// reports a double-click. <see cref="X"/>/<see cref="Y"/> are screen coordinates, or 0
/// where the platform does not report a position.</summary>
internal sealed class TrayPointerEventArgs : EventArgs
{
    public TrayPointerEventArgs(MouseButtons button, int clicks, int x, int y) { … }
    public MouseButtons Button { get; }
    public int Clicks { get; }
    public int X { get; }
    public int Y { get; }
}

internal interface INotifyIconBackend : IDisposable
{
    /// <summary>False when the platform has no tray host at all (see D3). Every other member
    /// is then a no-op, and no event is ever raised.</summary>
    bool IsAvailable { get; }

    void SetIcon(Drawing.Icon? icon);
    void SetToolTip(string text);
    void SetVisible(bool visible);
    void SetMenu(TrayMenuModel? menu);
    void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon);

    // Raised only by backends that can observe the corresponding input — see the fidelity
    // matrix. Consumers must not assume a Down precedes every Activated.
    event EventHandler<TrayPointerEventArgs>? Activated;
    event EventHandler<TrayPointerEventArgs>? PointerDown;
    event EventHandler<TrayPointerEventArgs>? PointerUp;
    event EventHandler<TrayPointerEventArgs>? PointerMove;

    event EventHandler? BalloonShown;
    event EventHandler? BalloonClicked;
    event EventHandler? BalloonClosed;
}
```

Selection, mirroring the existing platform detection in [FuncLoader.cs:8](Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/FuncLoader.cs#L8) rather than introducing a second mechanism:

```csharp
internal static class NotifyIconBackendFactory
{
    /// <summary>Never throws and never returns null: a platform whose backend fails to
    /// initialise gets <see cref="NullNotifyIconBackend"/>, which reports IsAvailable=false.</summary>
    public static INotifyIconBackend Create()
    {
        try
        {
            if (FuncLoader.IsWindows) return new Win32NotifyIconBackend();
            if (FuncLoader.IsOSX)     return new QuartzNotifyIconBackend();
            if (FuncLoader.IsLinux)   return new StatusNotifierItemBackend();
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("System tray backend unavailable; NotifyIcon will be inert.", ex);
        }

        return new NullNotifyIconBackend();
    }
}
```

`NullNotifyIconBackend` reproduces today's inert behaviour exactly, which is what keeps D3 cheap and keeps headless CI green.

### 4.2 The menu model — `Resources/TrayMenuModel.cs`

`ContextMenuStrip` is translated once into a backend-neutral tree, so the three backends share one interpretation of WinForms semantics rather than each re-deriving it.

```csharp
internal sealed class TrayMenuNode
{
    public int Id { get; internal set; }        // DFS-assigned, root = 0; stable per model instance
    public string Text { get; init; } = "";     // WinForms text, '&' mnemonic preserved verbatim
    public bool Enabled { get; init; } = true;
    public bool Visible { get; init; } = true;
    public bool IsSeparator { get; init; }
    public bool IsCheckable { get; init; }
    public bool Checked { get; init; }
    public IReadOnlyList<TrayMenuNode> Children { get; init; } = Array.Empty<TrayMenuNode>();
    public ToolStripItem? Source { get; init; } // null for the synthetic root
}
```

Built by walking `ContextMenuStrip.Items` and `ToolStripDropDownItem.DropDownItems` ([ToolStripDropDownItem.cs:166](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripDropDownItem.cs#L166)), reading the members the shim already exposes: `Text` ([ToolStripItem.cs:901](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs#L901)), `Enabled` ([:468](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs#L468)), `Visible` ([:1014](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs#L1014)), `Checked`/`CheckState` ([ToolStripMenuItem.cs:86](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripMenuItem.cs#L86), [:106](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripMenuItem.cs#L106)).

Rules, applied centrally:

- `IsSeparator` ⇔ `item is ToolStripSeparator`.
- `IsCheckable` ⇔ `item is ToolStripMenuItem m && (m.CheckOnClick || m.CheckState != CheckState.Unchecked)`.
- Invisible items are dropped from the emitted tree, not emitted-and-hidden (only dbusmenu has a `visible` property; Win32 and NSMenu do not).
- Mnemonic conversion is **per backend**, done at emit time, never in the model: Win32 keeps `&`, dbusmenu converts `&`→`_` (and escapes literal `_` as `__`), NSMenu strips `&`.
- Activation always routes through `node.Source?.PerformClick()` ([ToolStripItem.cs:1096](Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs#L1096)), marshalled onto the Gtk main loop with `GLib.Idle.Add`, because D-Bus and Obj-C callbacks may arrive off it.

### 4.3 Windows — `Resources/NotifyIconBackend.Win32.cs`

Owns a private message-only window (subject to V1); it never touches Gdk's HWND, so there is no `WNDPROC` subclassing and no interference with Gtk's own message handling.

```csharp
private const int  HWND_MESSAGE   = -3;
private const uint WM_TRAYCALLBACK = 0x8001;   // WM_APP + 1

[DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
private static extern bool Shell_NotifyIconW(uint dwMessage, ref NOTIFYICONDATAW lpData);

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
private struct NOTIFYICONDATAW
{
    public uint cbSize;  public IntPtr hWnd;  public uint uID;  public uint uFlags;
    public uint uCallbackMessage;  public IntPtr hIcon;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szTip;
    public uint dwState;  public uint dwStateMask;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string szInfo;
    public uint uTimeoutOrVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst =  64)] public string szInfoTitle;
    public uint dwInfoFlags;  public Guid guidItem;  public IntPtr hBalloonIcon;
}
```

Points that must not be got wrong:

- **The `WndProc` delegate must be held in a static field.** A local or instance delegate passed to `RegisterClassExW` is collected and the process dies with a hard crash at the first tray message.
- **`TaskbarCreated`.** `RegisterWindowMessageW("TaskbarCreated")` at startup; on receipt, re-`NIM_ADD` the icon. Explorer restarts are routine and without this the icon vanishes permanently. This is the Windows analogue of the SNI watcher re-registration in 4.4.
- **Icon conversion.** `Drawing.Icon.Pixbuf` (public, `Gdk.Pixbuf`) → `ScaleSimple(GetSystemMetrics(SM_CXSMICON), …, InterpType.Bilinear)` → `CreateDIBSection` 32bpp top-down BGRA (**not** premultiplied — `CreateIconIndirect` wants straight alpha) + 1bpp mask → `CreateIconIndirect`. `DestroyIcon` the previous HICON on every replace; leaking these is a classic GDI exhaustion bug.
- **Context menu.** On `WM_RBUTTONUP`: `CreatePopupMenu` + `InsertMenuW` from the `TrayMenuModel` (`MF_STRING`/`MF_SEPARATOR`/`MF_POPUP`/`MF_GRAYED`/`MF_CHECKED`), then `SetForegroundWindow(hwnd)`, `TrackPopupMenuEx(TPM_RETURNCMD | TPM_RIGHTBUTTON, …)`, then `PostMessageW(hwnd, WM_NULL, 0, 0)` — the documented workaround without which the menu will not dismiss correctly. Map the returned command id back to the node and `PerformClick()`.
- **Balloons** are `NIM_MODIFY` with `NIF_INFO` and `dwInfoFlags` from `ToolTipIcon`. `uTimeoutOrVersion` is ignored by Windows ≥ Vista — the `timeout` argument of `ShowBalloonTip` therefore has no effect, which the XML doc must state.
- `szTip` is capped at 128 chars including NUL, which is exactly why WinForms caps `Text` at 127 — the existing throw at [NotifyIcon.cs:268-271](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L268-L271) stays as-is.

### 4.4 Linux — `Resources/NotifyIconBackend.StatusNotifier.cs` + `.DBusMenu.cs`

Two exported objects on the session bus, both registered with `Gio.DBusConnection.RegisterObject` using `Gio.DBusNodeInfo.NewForXml(Xml).Interfaces[0]` (subject to V2).

**`/StatusNotifierItem` implementing `org.kde.StatusNotifierItem`:**

- Properties: `Category` = `"ApplicationStatus"`, `Id` (assembly name), `Title` (the `Text`), `Status` = `"Active"`, `IconPixmap`, `ToolTip`, `Menu` = `/MenuBar`, `ItemIsMenu` = `false`.
- Methods: `Activate(ii)` → `Activated(Left, 1)`; `SecondaryActivate(ii)` → `Activated(Middle, 1)`; `ContextMenu(ii)` → no-op, the host owns menu display; `Scroll(is)` → ignored.
- Signals: `NewIcon`, `NewToolTip`, `NewStatus`, emitted via `DBusConnection.EmitSignal` on the corresponding setter.

**Lifecycle:**

1. Own `org.kde.StatusNotifierItem-<pid>-<seq>` via `g_bus_own_name`.
2. Call `org.kde.StatusNotifierWatcher.RegisterStatusNotifierItem`. If the watcher name has no owner ⇒ `IsAvailable = false` (D3) and log once naming the missing watcher.
3. Subscribe to `NameOwnerChanged` for the watcher and re-register when it reappears — the shell-restart analogue of `TaskbarCreated`.
4. `SetVisible(false)` releases the owned name; `true` re-owns and re-registers (D6).

**`IconPixmap`** is `a(iiay)`: width, height, ARGB32 **big-endian**. From `Icon.Pixbuf`: force `HasAlpha`, walk `Pixels` honouring `Rowstride` (do **not** assume `width*4`), emit `[A,R,G,B]` per pixel. Build the byte array with a direct `g_variant_new_fixed_array` P/Invoke rather than `Variant.NewArray` over per-byte `Variant`s — a 22×22 icon is 1936 bytes and 1936 managed `Variant` allocations per icon update is needless.

**`/MenuBar` implementing `com.canonical.dbusmenu`** is the largest single work item:

- Properties `Version` = 3, `Status` = `"normal"`, `TextDirection`, `IconThemePath`.
- Methods `GetLayout(i,i,as)` → `(u(ia{sv}av))`, `GetGroupProperties`, `GetProperty`, `Event(i,s,v,u)`, `EventGroup`, `AboutToShow(i)`.
- Signals `LayoutUpdated(u,i)`, `ItemsPropertiesUpdated`.
- Item properties emitted: `label`, `enabled`, `visible`, `type` (`"separator"`), `toggle-type` (`"checkmark"`), `toggle-state` (0/1), `children-display` (`"submenu"`).
- Ids come from `TrayMenuNode.Id`; root is 0.
- Rebuild the model on `AboutToShow(0)`; if the tree changed, bump the revision and emit `LayoutUpdated`. This keeps a mutated `ContextMenuStrip` correct without observing every `ToolStripItemCollection` mutation.
- `Event(id, "clicked", …)` → `PerformClick()` on the node's `Source`.

**Balloons** go to `org.freedesktop.Notifications.Notify` with `expire_timeout = timeout` (honoured here, unlike Windows), mapping `ToolTipIcon` to `dialog-information`/`dialog-warning`/`dialog-error` — the same mapping the current code already uses for its fake window at [NotifyIcon.cs:323-329](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L323-L329). `ActionInvoked` → `BalloonClicked`, `NotificationClosed` → `BalloonClosed`.

### 4.5 macOS — `Resources/NotifyIconBackend.Quartz.cs`

Per-signature `objc_msgSend` imports (V3), never a variadic one:

```csharp
[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
private static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);
[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
private static extern IntPtr IntPtr_objc_msgSend_double(IntPtr receiver, IntPtr selector, double arg);
[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
private static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg);
// … one per distinct signature used
```

- `[[NSStatusBar systemStatusBar] statusItemWithLength:-1.0]` (`NSVariableStatusItemLength`), retained for the object's lifetime.
- Icon: `NSImage` from `Icon.PixbufData` (already PNG bytes, [Icon.cs:346-365](Gtk.Drawing/System/Drawing/Icon.cs#L346-L365)) via `NSData dataWithBytes:length:`. `setTemplate:` stays **NO** by default — a coloured application icon rendered as a template becomes a monochrome silhouette.
- Menu: an `NSMenu`/`NSMenuItem` tree with `setSubmenu:`, `separatorItem`, `setEnabled:`, `setState:`, and `setTag:` carrying `TrayMenuNode.Id`. The action target is an Obj-C class created at runtime: `objc_allocateClassPair(NSObject, "GtkWinFormsTrayTarget", 0)` + `class_addMethod(cls, sel("trayAction:"), fnPtr, "v@:@")` where `fnPtr` comes from `Marshal.GetFunctionPointerForDelegate` on a **static** delegate held in a static field.
- Do **not** assign `statusItem.menu`: that makes *every* click open the menu and suppresses the action, losing `Click`. Instead handle the button action and call `popUpMenuPositioningItem:atLocation:inView:` for the right-button case, so left-click raises `Click` as WinForms does.
- `sendActionOn:` with `NSEventMaskLeftMouseUp | NSEventMaskRightMouseUp`; read `[NSApp currentEvent]` for `buttonNumber` and `clickCount`.

### 4.6 The rewrite of `NotifyIcon.cs` (the patch)

Constructor — [NotifyIcon.cs:131-144](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L131-L144), replacing the null `statusIcon` and the 1×1 `statuswin`:

```csharp
public NotifyIcon()
{
    _backend = NotifyIconBackendFactory.Create();
    _backend.Activated    += (_, e) => { OnClick(EventArgs.Empty);
                                         OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0));
                                         if (e.Clicks > 1) { OnDoubleClick(EventArgs.Empty);
                                                             OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0)); } };
    _backend.PointerDown  += (_, e) => OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0));
    _backend.PointerUp    += (_, e) => OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0));
    _backend.PointerMove  += (_, e) => OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0));
    _backend.BalloonShown  += (_, _) => ((EventHandler?)Events[EVENT_BALLOONTIPSHOWN])?.Invoke(this, EventArgs.Empty);
    _backend.BalloonClicked+= (_, _) => ((EventHandler?)Events[EVENT_BALLOONTIPCLICKED])?.Invoke(this, EventArgs.Empty);
    _backend.BalloonClosed += (_, _) => ((EventHandler?)Events[EVENT_BALLOONTIPCLOSED])?.Invoke(this, EventArgs.Empty);
}
```

New public member (D3):

```csharp
/// <summary>Gets a value indicating whether this platform and desktop session actually provide a
/// system tray. When false every visual member of this component is inert — properties still
/// round-trip, no icon is shown and no input event is raised. Notably false on GNOME Shell without
/// the AppIndicator extension, which provides no StatusNotifierWatcher.</summary>
[Browsable(false)]
[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
public bool IsSystemTrayAvailable => _backend.IsAvailable;
```

`Icon` ([:216-236](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L216-L236)) — the existing body opens a `MemoryStream`, writes the icon into it and discards it, which is pure waste; it becomes `_backend.SetIcon(value)`. `Text` ([:256-281](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L256-L281)) keeps the 127-char throw and gains `_backend.SetToolTip(_text)` in place of the empty `if (statusIcon != null) { }`. `Visible` ([:285-300](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L285-L300)) gains `_backend.SetVisible(value)`. `ContextMenuStrip` ([:202-212](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L202-L212)) gains `_backend.SetMenu(value is null ? null : TrayMenuModel.From(value))`.

`ShowBalloonTip` ([:307-373](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L307-L373)) collapses to one line delegating to the backend — the entire fake-window body, the `Gdk.Monitor` geometry maths and the `GLib.Timeout` teardown go (D5). `Dispose` ([:375-381](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L375-L381)) disposes `_backend` instead of `statuswin`.

Deleted as dead: `statusIcon` ([:27](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L27)), `statuswin` ([:29](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L29)), `offset_bottom`/`offset_index` ([:22-24](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L22-L24)), `statuswin_Shown` ([:423-427](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L423-L427)), `StatusIcon_Activate` ([:418-421](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L418-L421), currently dead), and the three duplicated stale Gtk4 migration comments at [:134-137](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L134-L137) and [:335-336](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs#L335-L336).

---

## 5. Pipeline wiring

Append to `ResourcesCopyService.CopyCodeFromResources`, after [ResourcesCopyService.cs:33](PrepareProjects/Preparation/Source/ResourcesCopyService.cs#L33):

```csharp
foreach (var trayFile in new[]
         {
             "NotifyIconBackend.cs", "TrayMenuModel.cs", "NotifyIconBackend.Win32.cs",
             "NotifyIconBackend.StatusNotifier.cs", "NotifyIconBackend.DBusMenu.cs",
             "NotifyIconBackend.Quartz.cs"
         })
{
    LogUtility.FileCopyAndLog(RepoPaths.At(Path.Combine("Resources", trayFile)),
        Path.Combine(targetDir, "GTKControls", trayFile));
}
```

The `NotifyIcon.cs` rewrite is then captured per [Patches.txt](Patches.txt):

```sh
git format-patch -o patches/ -1 HEAD -- :(exclude)*.csproj
```

renamed to `patches/0127-Reinstate-NotifyIcon-system-tray.patch`, applied by `GitPatchApplier.CommitAndApplyPatches()` at [Program.cs:31](PrepareProjects/Program.cs#L31).

---

## 6. Verification

**Automated** (must run green on headless CI, where every backend reports `IsAvailable = false`):

- The existing `NotifyIcon` tests at [ToolStripNotifyIconFunctionalTests.cs:216-261](Gtk.Windows.Forms.Tests/System/Windows/Forms/ToolStripNotifyIconFunctionalTests.cs#L216-L261) must pass **unmodified**. `NotifyIcon_VisibleRoundTrips` in particular is the regression guard for D3.
- New `Gtk.Windows.Forms.Tests/System/Windows/Forms/NotifyIconTrayBackendTests.cs` (the backends are `internal`; `InternalsVisibleTo` is already declared for the test assembly). Per [tests-must-verify-behavior], these assert real oracles, not line coverage:
  - `TrayMenuModel.From` — ordering, nesting depth, separator detection, checkable detection, invisible items dropped, DFS id assignment stable across two builds of the same menu.
  - Mnemonic conversion per backend: `"&File"` → `"&File"` (Win32) / `"_File"` (dbusmenu) / `"File"` (NSMenu); `"a_b"` → `"a__b"` for dbusmenu.
  - `Marshal.SizeOf<NOTIFYICONDATAW>()` equals the documented `NOTIFYICONDATAW` size for the current bitness, guarding the struct layout.
  - Pixbuf→ARGB32 conversion against a synthetic 2×2 `Gdk.Pixbuf` with a deliberately non-`4*width` rowstride, asserting exact bytes — this is the rowstride bug caught before it ships.
  - `NotifyIcon.IsSystemTrayAvailable` returns without throwing, and construction/disposal is clean, in a headless session.
- Test classes constructing widgets call `Application.Init()` in the constructor, matching [ToolStripNotifyIconFunctionalTests.cs:15](Gtk.Windows.Forms.Tests/System/Windows/Forms/ToolStripNotifyIconFunctionalTests.cs#L15) — see [gtk-tests-need-application-init]. Run the suite serially (`-m:1`) per [solution-tests-need-serial-run].

**Manual**, per platform, using the existing `NotifyIcon` sample in [GtkWinFormsApp/Form3.cs](GtkWinFormsApp/Form3.cs) — used to *observe* behaviour only; nothing in the implementation may be shaped by that sample's specifics.

| Platform | Sessions to cover | Checks |
|---|---|---|
| Windows 11 | — | icon appears; tooltip on hover; left-click `Click`; double-click `DoubleClick`; right-click menu with a checked item and a submenu; balloon appears; **kill and restart `explorer.exe`, icon returns**; no GDI handle growth over 100 icon swaps |
| Linux | KDE Plasma (Wayland), GNOME **with** AppIndicator ext., GNOME **without** it, XFCE | icon; tooltip; menu structure incl. checkmarks and submenus; `Activate` → `Click`; middle-click → middle `MouseClick`; notification; **restart the shell, item re-registers**; on GNOME-without-ext. `IsSystemTrayAvailable == false` and no crash |
| macOS 13+ | — | item in the menu bar; icon legible in light *and* dark menu bar; tooltip; left-click `Click`; right-click menu; `Visible=false` removes it |

---

## 7. Risks

| # | Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|---|
| R1 | Gdk's Win32 loop does not dispatch to our message-only window | Low | Whole Win32 backend | **V1 blocks coding.** Fallback: dedicated message thread + `GLib.Idle.Add` marshalling |
| R2 | Generated `RegisterObject` vtable unusable / delegates GC'd | Medium | Whole Linux backend | **V2 blocks coding.** Fallback: hand-declared `DllImport` to `libgio`; root all delegates in static fields regardless |
| R3 | `com.canonical.dbusmenu` is under-specified and host-dependent; Plasma, GNOME-ext. and XFCE disagree in corners | High | Menu correctness on some desktops | Test on all four sessions listed above; keep the emitted property set minimal and standard |
| R4 | GNOME Shell ships no `StatusNotifierWatcher` by default | Certain | No tray on stock GNOME | Accepted and designed for (D3); documented in `IsSystemTrayAvailable`'s XML doc and the README |
| R5 | Gtk4 macOS backend absent from our packaged Net4x.GtkSharp | Medium | Whole macOS backend | **V3.** Degrades to `IsSystemTrayAvailable == false`, no other code affected |
| R6 | macOS notifications need a signed, bundled app; `NSUserNotification` is deprecated and `UNUserNotificationCenter` refuses to work unbundled | High | `ShowBalloonTip` on macOS | Attempt, detect failure, no-op; document. Do **not** resurrect a fake window to fake it |
| R7 | HICON leak on repeated `Icon` assignment exhausts GDI handles | Medium | Long-running Windows apps | `DestroyIcon` on replace and in `Dispose`; the 100-swap handle-count check in §6 |
| R8 | Off-main-thread callbacks (D-Bus, Obj-C) invoking WinForms handlers | Medium | Intermittent hard crashes | Every backend→`NotifyIcon` hop goes through `GLib.Idle.Add`; assert this in code review of each backend |
| R9 | Overlay files relocated unexpectedly by `FolderFixer` | Low | Broken build after pipeline run | Namespace-driven placement is already proven by `NonStaticMessageBox.cs`; verify with one full `PrepareProjects` run before finalising the patch |
| R10 | Rowstride-vs-width assumption in pixel conversion | Medium | Corrupted/skewed icons | Explicit unit test with a non-trivial rowstride (§6) |

---

## 8. Out of scope

Menu item images, fonts, colours and owner-draw (D2). `NotifyIcon.BalloonTipIcon` custom icons (only the three standard severities are mapped). The legacy XEmbed freedesktop system-tray protocol — X11-only and Gtk4 has no `GtkPlug`, so it is not implementable and SNI covers modern X11 sessions anyway. Any change to `Net4x.GtkSharp` itself.

## 9. Effort

Assuming the three spikes pass: Win32 ≈ 1 day, Quartz ≈ 1.5 days, SNI+dbusmenu ≈ 2.5–3 days, shared model + `NotifyIcon` rewrite + pipeline wiring + tests ≈ 1.5 days, cross-platform manual verification ≈ 1 day. **≈ 8–9 days**, of which the Linux dbusmenu implementation and the four-desktop test matrix dominate.

---

## 10. Implementation record

### Delivered

| Artefact | Location |
|---|---|
| Backend contract, main-loop marshalling, null backend, factory, UTF-8 helper | [Resources/NotifyIconBackend.cs](Resources/NotifyIconBackend.cs) |
| Shared menu translation | [Resources/TrayMenuModel.cs](Resources/TrayMenuModel.cs) |
| Windows backend | [Resources/NotifyIconBackend.Win32.cs](Resources/NotifyIconBackend.Win32.cs) |
| Linux GDBus interop + StatusNotifierItem | [Resources/NotifyIconBackend.StatusNotifier.cs](Resources/NotifyIconBackend.StatusNotifier.cs) |
| Linux `com.canonical.dbusmenu` exporter | [Resources/NotifyIconBackend.DBusMenu.cs](Resources/NotifyIconBackend.DBusMenu.cs) |
| macOS backend | [Resources/NotifyIconBackend.Quartz.cs](Resources/NotifyIconBackend.Quartz.cs) |
| `NotifyIcon` rewired onto the backend, plus `IsSystemTrayAvailable` | [Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs](Gtk.Windows.Forms/System/Windows/Forms/NotifyIcon.cs) |
| Overlay copy step | [ResourcesCopyService.cs:35-53](PrepareProjects/Preparation/Source/ResourcesCopyService.cs#L35-L53) |
| Pipeline patch | [patches/0127-Reinstate-the-NotifyIcon-system-tray-on-Gtk4.patch](patches/0127-Reinstate-the-NotifyIcon-system-tray-on-Gtk4.patch) |
| Tests | [NotifyIconTrayBackendTests.cs](Gtk.Windows.Forms.Tests/System/Windows/Forms/NotifyIconTrayBackendTests.cs) |

### Blocking verification — outcomes

- **V1 (Win32 message dispatch) — PASSED, and it is the load-bearing result.** A throwaway Gtk 4 console
  app created a `HWND_MESSAGE` window and posted to it from a `GLib.Timeout`; the window procedure ran,
  so Gdk's Win32 event source does dispatch messages it does not own. `Shell_NotifyIcon` `NIM_ADD` and
  `NIM_DELETE` both returned true against that window, and `Marshal.SizeOf<NOTIFYICONDATAW>()` came back
  as 976 on x64, matching `NOTIFYICONDATA_V4_SIZE`. The dedicated-thread fallback is therefore not needed.
  Incidental finding: Gtk on Windows requires an STA main thread (`OleInitialize failed` otherwise).
- **V2 (GDBus object export) — VERIFIED on Linux (WSL, Ubuntu 22.04).** The export path was written against
  hand-declared `DllImport`s for `g_dbus_connection_register_object` and `GDBusInterfaceVTable` - V2's own
  documented fallback - and a harness has now run it end to end under `dbus-run-session`: both objects
  export, the item announces itself to a `StatusNotifierWatcher`, every `org.kde.StatusNotifierItem`
  property reads back with the signature GDBus expects (including `IconPixmap` as `a(iiay)` and `ToolTip`
  as `(sa(iiay)ss)`), and `com.canonical.dbusmenu`'s `GetLayout`, `AboutToShow` and `Version` all answer -
  `GetLayout` returning a well-formed `(u(ia{sv}av))`. **All checks passed.**

  It also found a real defect, since fixed: the `GetPropertyFunc` delegate declared the `GError**` argument
  as `out IntPtr`, which makes the runtime dereference it to store a value - that faulted with a
  `NullReferenceException` the first time any property was read, taking the process down. The pointer is
  now taken opaquely and never written through; returning `IntPtr.Zero` is how a getter declines anyway.

  Not covered: translating a real `ContextMenuStrip` and the icon pixmap, both of which need GTK widgets
  rather than just the D-Bus layer.
- **V3 (macOS `objc_msgSend` and the quartz backend) — NOT RUN.** Cannot be answered from Windows. The
  code follows the per-signature rule V3 exists to protect (no variadic `objc_msgSend`), and a failure to
  initialise degrades to `NullNotifyIconBackend`, so the risk is confined to macOS.

### Deviations from the plan as written

- **`SetMenu` takes the `ContextMenuStrip`, not a `TrayMenuModel`.** A snapshot taken at assignment time
  goes stale when a consumer mutates the strip afterwards, which is normal WinForms usage. Each backend
  now calls `TrayMenuModel.From` at display time instead.
- **Checkability is derived from `CheckState` alone.** §4.2 assumed `ToolStripMenuItem.CheckOnClick`; the
  shim does not expose it. An item that is checkable but currently unchecked therefore renders without a
  check box. This is a real fidelity gap, noted in the code, and closing it means adding `CheckOnClick`
  to `ToolStripMenuItem` — deliberately out of scope here.
- **`macOS ShowBalloonTip` is a logged no-op**, as R6 anticipated. `NSUserNotification` is gone and
  `UNUserNotificationCenter` requires a signed bundle, so there is nothing to call.

### Verification actually performed

- Full solution suite, run serially (`-m:1`): **3,858 passed, 12 skipped, 0 failed.**
- 30 new tests, of which these assert real oracles rather than absence of exceptions: menu translation
  (order, nesting, separators, check state, enabled state, invisible items dropped, id stability across
  re-translation, `Activate` reaching the original item, unknown ids ignored); mnemonic conversion for
  each of the three platform conventions; `NOTIFYICONDATAW` size against the documented native value;
  and pixel conversion for both ARGB32 and BGRA against a pixbuf whose rowstride is deliberately wider
  than `width * channels` (R10).
- On Windows the full native path is exercised end to end: `CreateHIcon` returns a real `HICON` from a
  `Gdk.Pixbuf`, and the backend's `IsIconAdded` — which reflects the shell's own answer to `NIM_ADD`,
  not the requested state — confirms the shell accepted and then released the icon.
- The pre-existing `NotifyIcon` tests at
  [ToolStripNotifyIconFunctionalTests.cs:216-261](Gtk.Windows.Forms.Tests/System/Windows/Forms/ToolStripNotifyIconFunctionalTests.cs#L216-L261)
  pass **unmodified**, which is the regression guard for D3.
- The patch was checked against the tree with `git apply --check --reverse`.

### Still unverified

Everything in §6's manual matrix for Linux and macOS. Specifically: whether Plasma, XFCE and
GNOME-with-AppIndicator render the item and its menu; whether the dbusmenu layout is accepted as written
(R3 remains the largest open risk, since the specification is under-defined and hosts disagree); whether
the item re-registers after a shell restart; and whether the macOS status item appears at all. None of
this is reachable from Windows.
