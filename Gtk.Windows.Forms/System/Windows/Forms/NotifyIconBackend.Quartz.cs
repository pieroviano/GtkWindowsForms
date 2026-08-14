using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// The macOS system tray backend, built on <c>NSStatusItem</c> in the menu bar's status area.
/// </summary>
/// <remarks>
/// <para>
/// Gtk 4's macOS backend runs a real <c>NSApplication</c>, so a status item can simply be added to
/// <c>[NSStatusBar systemStatusBar]</c> and lives alongside the Gtk windows.
/// </para>
/// <para>
/// The status item's menu is deliberately not assigned to <c>NSStatusItem.menu</c>. Doing so makes every
/// click - including a left click - open the menu and suppresses the button's action entirely, which would
/// lose the <see cref="NotifyIcon.Click" /> event that WinForms applications rely on. Instead the button
/// action is handled here and the menu is popped up explicitly for the right button, matching WinForms'
/// division of labour.
/// </para>
/// <para>
/// Objective-C messaging is done through per-signature <c>objc_msgSend</c> imports. A single variadic
/// declaration is not usable: on arm64 <c>objc_msgSend</c> has no variadic calling convention, and passing
/// arguments through the wrong prototype corrupts the call.
/// </para>
/// </remarks>
internal sealed class QuartzNotifyIconBackend : NotifyIconBackendBase
{
    private const string ObjCLibrary = "/usr/lib/libobjc.dylib";

    private const double NSVariableStatusItemLength = -1.0;

    private const ulong NSEventMaskLeftMouseUp = 1UL << 2;

    private const ulong NSEventMaskRightMouseUp = 1UL << 4;

    private const string TargetClassName = "GtkWindowsFormsTrayTarget";

    // Rooted for the process lifetime: the Objective-C runtime keeps only the raw function pointer, and a
    // collected delegate would leave the registered method pointing at a freed thunk.
    private static readonly ActionCallback s_trayCallback = OnTrayAction;

    private static readonly ActionCallback s_menuCallback = OnMenuAction;

    private static readonly Dictionary<IntPtr, QuartzNotifyIconBackend> s_targets =
        new Dictionary<IntPtr, QuartzNotifyIconBackend>();

    private static readonly object s_targetLock = new object();

    private static IntPtr s_targetClass;

    private readonly IntPtr _statusItem;

    private readonly IntPtr _target;

    private ContextMenuStrip? _menu;

    private TrayMenuModel? _model;

    private bool _visible;

    /// <summary>
    /// Initialises a new instance of the <see cref="QuartzNotifyIconBackend" /> class, creating a hidden
    /// status item.
    /// </summary>
    /// <exception cref="InvalidOperationException">The status item could not be created.</exception>
    public QuartzNotifyIconBackend()
    {
        var statusBar = SendMessage(GetClass("NSStatusBar"), GetSelector("systemStatusBar"));
        if (statusBar == IntPtr.Zero)
        {
            throw new InvalidOperationException("The macOS system status bar is not available.");
        }

        _statusItem = SendMessageDouble(statusBar, GetSelector("statusItemWithLength:"),
            NSVariableStatusItemLength);

        if (_statusItem == IntPtr.Zero)
        {
            throw new InvalidOperationException("Unable to create the macOS status item.");
        }

        // statusItemWithLength: returns an autoreleased object; without a retain it dies at the end of the
        // current autorelease pool and the icon disappears moments after it appears.
        SendMessage(_statusItem, GetSelector("retain"));

        _target = CreateTarget();

        lock (s_targetLock)
        {
            s_targets[_target] = this;
        }

        var button = SendMessage(_statusItem, GetSelector("button"));
        if (button != IntPtr.Zero)
        {
            SendMessagePtr(button, GetSelector("setTarget:"), _target);
            SendMessagePtr(button, GetSelector("setAction:"), GetSelector("trayAction:"));
            SendMessageULong(button, GetSelector("sendActionOn:"),
                NSEventMaskLeftMouseUp | NSEventMaskRightMouseUp);
        }

        SetVisibleCore(false);
    }

    /// <inheritdoc />
    public override bool IsAvailable => true;

    /// <inheritdoc />
    public override void SetIcon(Drawing.Icon? icon)
    {
        if (IsDisposed)
        {
            return;
        }

        var button = SendMessage(_statusItem, GetSelector("button"));
        if (button == IntPtr.Zero)
        {
            return;
        }

        var image = CreateImage(icon);

        try
        {
            SendMessagePtr(button, GetSelector("setImage:"), image);
        }
        finally
        {
            if (image != IntPtr.Zero)
            {
                SendMessage(image, GetSelector("release"));
            }
        }
    }

    /// <inheritdoc />
    public override void SetToolTip(string text)
    {
        if (IsDisposed)
        {
            return;
        }

        var button = SendMessage(_statusItem, GetSelector("button"));
        if (button == IntPtr.Zero)
        {
            return;
        }

        var value = CreateString(text ?? string.Empty);

        try
        {
            SendMessagePtr(button, GetSelector("setToolTip:"), value);
        }
        finally
        {
            if (value != IntPtr.Zero)
            {
                SendMessage(value, GetSelector("release"));
            }
        }
    }

    /// <inheritdoc />
    public override void SetVisible(bool visible)
    {
        if (IsDisposed || visible == _visible)
        {
            return;
        }

        SetVisibleCore(visible);
    }

    /// <inheritdoc />
    public override void SetMenu(ContextMenuStrip? menu)
    {
        _menu = menu;
    }

    /// <inheritdoc />
    public override void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon)
    {
        // macOS delivers notifications only through UNUserNotificationCenter, which refuses to work unless
        // the process is a signed application bundle. A Gtk application launched from a plain executable
        // therefore cannot post one, and there is no lesser API left: NSUserNotification was removed.
        // Failing silently is the honest outcome - faking a floating window here would misrepresent a
        // system notification.
        Diagnostics.GtkLogger.Debug(
            "Balloon tips are unavailable on macOS outside a signed application bundle; the request was ignored.");
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_statusItem != IntPtr.Zero)
        {
            var statusBar = SendMessage(GetClass("NSStatusBar"), GetSelector("systemStatusBar"));
            if (statusBar != IntPtr.Zero)
            {
                SendMessagePtr(statusBar, GetSelector("removeStatusItem:"), _statusItem);
            }

            SendMessage(_statusItem, GetSelector("release"));
        }

        lock (s_targetLock)
        {
            s_targets.Remove(_target);
        }

        if (_target != IntPtr.Zero)
        {
            SendMessage(_target, GetSelector("release"));
        }

        base.Dispose(disposing);
    }

    private void SetVisibleCore(bool visible)
    {
        _visible = visible;
        SendMessageBool(_statusItem, GetSelector("setVisible:"), visible ? (byte)1 : (byte)0);
    }

    private static IntPtr CreateTarget()
    {
        lock (s_targetLock)
        {
            if (s_targetClass == IntPtr.Zero)
            {
                var nsObject = GetClass("NSObject");
                s_targetClass = objc_allocateClassPair(nsObject, TargetClassName, IntPtr.Zero);

                if (s_targetClass == IntPtr.Zero)
                {
                    // Already registered by an earlier instance in this process.
                    s_targetClass = GetClass(TargetClassName);
                }
                else
                {
                    // "v@:@" - returns void, takes self, _cmd and the sender. Two distinct selectors,
                    // because a status item click and a menu item choice mean different things: the first
                    // is tray input, the second identifies an item by its tag.
                    class_addMethod(s_targetClass, GetSelector("trayAction:"),
                        Marshal.GetFunctionPointerForDelegate(s_trayCallback), "v@:@");
                    class_addMethod(s_targetClass, GetSelector("menuAction:"),
                        Marshal.GetFunctionPointerForDelegate(s_menuCallback), "v@:@");
                    objc_registerClassPair(s_targetClass);
                }
            }
        }

        if (s_targetClass == IntPtr.Zero)
        {
            throw new InvalidOperationException("Unable to register the macOS status item action target.");
        }

        var instance = SendMessage(s_targetClass, GetSelector("alloc"));

        return SendMessage(instance, GetSelector("init"));
    }

    private static QuartzNotifyIconBackend? FindTarget(IntPtr self)
    {
        lock (s_targetLock)
        {
            return s_targets.TryGetValue(self, out var backend) ? backend : null;
        }
    }

    private static void OnTrayAction(IntPtr self, IntPtr selector, IntPtr sender)
    {
        try
        {
            FindTarget(self)?.HandleAction();
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception handling a macOS status item click", ex);
        }
    }

    private static void OnMenuAction(IntPtr self, IntPtr selector, IntPtr sender)
    {
        try
        {
            var backend = FindTarget(self);
            var model = backend?._model;

            if (model == null || sender == IntPtr.Zero)
            {
                return;
            }

            var id = (int)SendMessageReturnLong(sender, GetSelector("tag"));

            // AppKit delivers this on the main thread, but every other backend routes through the Gtk
            // loop before touching WinForms handlers and consistency is worth more than a saved turn.
            InvokeOnMainLoop(() => model.Activate(id));
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception handling a macOS menu selection", ex);
        }
    }

    private void HandleAction()
    {
        var app = SendMessage(GetClass("NSApplication"), GetSelector("sharedApplication"));
        var currentEvent = app == IntPtr.Zero ? IntPtr.Zero : SendMessage(app, GetSelector("currentEvent"));

        var button = MouseButtons.Left;
        var clicks = 1;

        if (currentEvent != IntPtr.Zero)
        {
            var buttonNumber = SendMessageReturnLong(currentEvent, GetSelector("buttonNumber"));
            var clickCount = SendMessageReturnLong(currentEvent, GetSelector("clickCount"));

            if (buttonNumber == 1)
            {
                button = MouseButtons.Right;
            }

            if (clickCount > 1)
            {
                clicks = (int)clickCount;
            }
        }

        RaisePointerUp(button, 0, 0);
        RaiseActivated(button, clicks, 0, 0);

        if (button == MouseButtons.Right)
        {
            ShowContextMenu();
        }
    }

    private void ShowContextMenu()
    {
        // Translated now rather than at assignment time, so a strip mutated since then still renders.
        var model = TrayMenuModel.From(_menu);
        if (model == null || !model.Root.HasChildren)
        {
            return;
        }

        _model = model;

        var menu = BuildMenu(model.Root);
        if (menu == IntPtr.Zero)
        {
            return;
        }

        try
        {
            SendMessagePtr(_statusItem, GetSelector("popUpStatusItemMenu:"), menu);
        }
        finally
        {
            SendMessage(menu, GetSelector("release"));
        }
    }

    private IntPtr BuildMenu(TrayMenuNode parent)
    {
        var menu = SendMessage(SendMessage(GetClass("NSMenu"), GetSelector("alloc")), GetSelector("init"));
        if (menu == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        // Items are enabled explicitly per item; without this AppKit disables everything that has no
        // responder in the chain, which is every item here.
        SendMessageBool(menu, GetSelector("setAutoenablesItems:"), 0);

        foreach (var child in parent.Children)
        {
            IntPtr item;

            if (child.IsSeparator)
            {
                item = SendMessage(GetClass("NSMenuItem"), GetSelector("separatorItem"));
                SendMessagePtr(menu, GetSelector("addItem:"), item);
                continue;
            }

            var title = CreateString(TrayMenuModel.StripMnemonics(child.Text));
            var empty = CreateString(string.Empty);

            try
            {
                item = SendMessage(GetClass("NSMenuItem"), GetSelector("alloc"));
                item = SendMessageInitMenuItem(item, GetSelector("initWithTitle:action:keyEquivalent:"),
                    title, GetSelector("menuAction:"), empty);

                if (item == IntPtr.Zero)
                {
                    continue;
                }

                SendMessagePtr(item, GetSelector("setTarget:"), _target);
                SendMessageBool(item, GetSelector("setEnabled:"), child.Enabled ? (byte)1 : (byte)0);
                SendMessageLong(item, GetSelector("setTag:"), child.Id);

                if (child.IsCheckable)
                {
                    SendMessageLong(item, GetSelector("setState:"), child.Checked ? 1 : 0);
                }

                if (child.HasChildren)
                {
                    var submenu = BuildMenu(child);
                    if (submenu != IntPtr.Zero)
                    {
                        SendMessagePtr(item, GetSelector("setSubmenu:"), submenu);
                        SendMessage(submenu, GetSelector("release"));
                    }
                }

                SendMessagePtr(menu, GetSelector("addItem:"), item);
                SendMessage(item, GetSelector("release"));
            }
            finally
            {
                if (empty != IntPtr.Zero)
                {
                    SendMessage(empty, GetSelector("release"));
                }

                if (title != IntPtr.Zero)
                {
                    SendMessage(title, GetSelector("release"));
                }
            }
        }

        return menu;
    }

    /// <summary>
    /// Creates an autoreleased-free <c>NSImage</c> from a WinForms icon.
    /// </summary>
    /// <param name="icon">The icon to convert, or <c>null</c>.</param>
    /// <returns>A retained <c>NSImage</c> the caller must release, or <see cref="IntPtr.Zero" />.</returns>
    /// <remarks>
    /// The icon is not marked as a template image. A template renders as a monochrome silhouette that
    /// adapts to the menu bar's appearance, which is right for a symbol and wrong for the coloured
    /// application icon a WinForms program normally supplies.
    /// </remarks>
    private static IntPtr CreateImage(Drawing.Icon? icon)
    {
        var png = icon?.PixbufData;
        if (png == null || png.Length == 0)
        {
            return IntPtr.Zero;
        }

        var buffer = Marshal.AllocHGlobal(png.Length);

        try
        {
            Marshal.Copy(png, 0, buffer, png.Length);

            var data = SendMessageBytes(GetClass("NSData"), GetSelector("dataWithBytes:length:"), buffer,
                new UIntPtr((ulong)png.Length));

            if (data == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var image = SendMessage(GetClass("NSImage"), GetSelector("alloc"));

            return SendMessagePtr(image, GetSelector("initWithData:"), data);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to convert the icon for the macOS status item", ex);
            return IntPtr.Zero;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static IntPtr CreateString(string value)
    {
        var utf8 = NativeUtf8.FromString(value);

        try
        {
            var instance = SendMessage(GetClass("NSString"), GetSelector("alloc"));

            return SendMessagePtr(instance, GetSelector("initWithUTF8String:"), utf8);
        }
        finally
        {
            NativeUtf8.Free(utf8);
        }
    }

    private static IntPtr GetClass(string name)
    {
        return objc_getClass(name);
    }

    private static IntPtr GetSelector(string name)
    {
        return sel_registerName(name);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void ActionCallback(IntPtr self, IntPtr selector, IntPtr sender);

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern IntPtr objc_allocateClassPair(IntPtr superclass, string name, IntPtr extraBytes);

    [DllImport(ObjCLibrary)]
    private static extern void objc_registerClassPair(IntPtr cls);

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern bool class_addMethod(IntPtr cls, IntPtr name, IntPtr imp, string types);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessagePtr(IntPtr receiver, IntPtr selector, IntPtr arg);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageDouble(IntPtr receiver, IntPtr selector, double arg);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageBool(IntPtr receiver, IntPtr selector, byte arg);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageLong(IntPtr receiver, IntPtr selector, long arg);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageULong(IntPtr receiver, IntPtr selector, ulong arg);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern long SendMessageReturnLong(IntPtr receiver, IntPtr selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageBytes(IntPtr receiver, IntPtr selector, IntPtr bytes,
        UIntPtr length);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageInitMenuItem(IntPtr receiver, IntPtr selector, IntPtr title,
        IntPtr action, IntPtr keyEquivalent);
}
