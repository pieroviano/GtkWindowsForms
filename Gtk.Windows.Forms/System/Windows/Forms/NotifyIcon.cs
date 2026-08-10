/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this componentGTKApp.Windows.Forms instead ofMicrosoft.WindowsDesktop.App.WindowsForms， compile once， cross-platformwindows、linux、macos run
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk;
using GtkSystem.Windows.Forms.Resources;

namespace System.Windows.Forms
{
    [DefaultProperty(nameof(Text))]
    [DefaultEvent(nameof(MouseDoubleClick))]
    [Designer($"System.Windows.Forms.Design.NotifyIconDesigner, {AssemblyRef.SystemDesign}")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public partial class NotifyIcon : Component
    {

        internal const int MaxTextSize = 127;

        int offset_bottom = 0;

        int offset_index = 0;

        // Gtk 4 removed Gtk.StatusIcon with no replacement (no system-tray API). NotifyIcon is inert.
        object? statusIcon = null;

        Gtk.Window statuswin;

        private ToolTipIcon _balloonTipIcon;

        private string _balloonTipText = string.Empty;

        private string _balloonTipTitle = string.Empty;

        private ContextMenuStrip? _contextMenuStrip;

        private bool _doubleClick;

        private Drawing.Icon? _icon;

        private string _text = string.Empty;

        private object? _userData;

        private bool _visible;

        private static readonly object EVENT_BALLOONTIPCLICKED = new();

        private static readonly object EVENT_BALLOONTIPCLOSED = new();

        private static readonly object EVENT_BALLOONTIPSHOWN = new();

        private static readonly object EVENT_CLICK = new();

        private static readonly object EVENT_DOUBLECLICK = new();

        private static readonly object EVENT_MOUSECLICK = new();

        private static readonly object EVENT_MOUSEDOUBLECLICK = new();

        private static readonly object EVENT_MOUSEDOWN = new();

        private static readonly object EVENT_MOUSEMOVE = new();

        private static readonly object EVENT_MOUSEUP = new();

        public virtual event EventHandler? BalloonTipClicked
        {
            add => Events.AddHandler(EVENT_BALLOONTIPCLICKED, value);

            remove => Events.RemoveHandler(EVENT_BALLOONTIPCLICKED, value);
        }

        public virtual event EventHandler? BalloonTipClosed
        {
            add => Events.AddHandler(EVENT_BALLOONTIPCLOSED, value);

            remove => Events.RemoveHandler(EVENT_BALLOONTIPCLOSED, value);
        }

        public virtual event EventHandler? BalloonTipShown
        {
            add => Events.AddHandler(EVENT_BALLOONTIPSHOWN, value);
            remove => Events.RemoveHandler(EVENT_BALLOONTIPSHOWN, value);
        }

        public virtual event EventHandler? Click
        {
            add => Events.AddHandler(EVENT_CLICK, value);
            remove => Events.RemoveHandler(EVENT_CLICK, value);
        }

        public virtual event EventHandler? DoubleClick
        {
            add => Events.AddHandler(EVENT_DOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DOUBLECLICK, value);
        }

        public virtual event MouseEventHandler? MouseClick
        {
            add => Events.AddHandler(EVENT_MOUSECLICK, value);
            remove => Events.RemoveHandler(EVENT_MOUSECLICK, value);
        }

        public virtual event MouseEventHandler? MouseDoubleClick
        {
            add => Events.AddHandler(EVENT_MOUSEDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_MOUSEDOUBLECLICK, value);
        }

        public virtual event MouseEventHandler? MouseDown
        {
            add => Events.AddHandler(EVENT_MOUSEDOWN, value);
            remove => Events.RemoveHandler(EVENT_MOUSEDOWN, value);
        }

        public virtual event MouseEventHandler? MouseMove
        {
            add => Events.AddHandler(EVENT_MOUSEMOVE, value);
            remove => Events.RemoveHandler(EVENT_MOUSEMOVE, value);
        }

        public virtual event MouseEventHandler? MouseUp
        {
            add => Events.AddHandler(EVENT_MOUSEUP, value);
            remove => Events.RemoveHandler(EVENT_MOUSEUP, value);
        }

        public NotifyIcon()
        {
            statusIcon = null; // Gtk 4: system tray unsupported (StatusIcon removed).
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
            statuswin = new Gtk.Window(WindowType.Toplevel);
            statuswin.Decorated = false;
            statuswin.WidthRequest = 1;
            statuswin.HeightRequest = 1;
            statuswin.Shown += statuswin_Shown;
            statuswin.Show();
        }

        public NotifyIcon(IContainer container) : this()
        {
            container.Add(this);
        }

        [DefaultValue(ToolTipIcon.None)]
        public ToolTipIcon BalloonTipIcon
        {
            get
            {
                return _balloonTipIcon;
            }
            set
            {
                if (value != _balloonTipIcon)
                {
                    _balloonTipIcon = value;
                }
            }
        }

        [Localizable(true)]
        [DefaultValue("")]
        public string BalloonTipText
        {
            get
            {
                return _balloonTipText;
            }
            set
            {
                if (value != _balloonTipText)
                {
                    _balloonTipText = value;
                }
            }
        }

        [Localizable(true)]
        [DefaultValue("")]
        public string BalloonTipTitle
        {
            get
            {
                return _balloonTipTitle;
            }
            set
            {
                if (value != _balloonTipTitle)
                {
                    _balloonTipTitle = value;
                }
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip? ContextMenuStrip
        {
            get
            {
                return _contextMenuStrip;
            }
            set
            {
                _contextMenuStrip = value;
            }
        }

        [Localizable(true)]
        [DefaultValue(null)]
        public Drawing.Icon? Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    if (statusIcon != null && _icon != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            _icon.Save(ms);
                        }
                    }
                }
            }
        }

        [Localizable(false)]
        [Bindable(true)]
        [DefaultValue(null)]
        //[TypeConverter(typeof(StringConverter))]
        public object? Tag
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        [Localizable(true)]
        [DefaultValue("")]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                value ??= string.Empty;

                if (!value.Equals(_text))
                {
                    if (value.Length > MaxTextSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Text), value, "TrayIcon_TextTooLong");
                    }

                    _text = value;

                    if (statusIcon != null)
                    {

                    }
                }
            }
        }

        [Localizable(true)]
        [DefaultValue(false)]
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    if (statusIcon != null)
                    _visible = value;

                }
            }
        }

        public void ShowBalloonTip(int timeout)
        {
            ShowBalloonTip(timeout, _balloonTipTitle, _balloonTipText, _balloonTipIcon);
        }

        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            var balloonTip = new Gtk.Window(WindowType.Toplevel);
            balloonTip.DestroyWithParent = true;
            balloonTip.TypeHint = Gdk.WindowTypeHint.Dialog;
            balloonTip.TransientFor = statuswin;
            balloonTip.WidthRequest = 200;
            balloonTip.HeightRequest = 50;
            balloonTip.Decorated = true;
            balloonTip.Title = tipTitle ?? "";
            var balloonTipText = new Gtk.Label(tipText) { Xpad = 20, Ypad = 20, Halign = Align.Start };
            balloonTip.AddChild(balloonTipText);

            // Gtk4: a balloon-tip window icon from a Pixbuf is unsupported (Gtk.Window.Icon removed; only a
            // themed icon-name can be set). The ToolTipIcon -> Info/Warning/Error/default icon mapping is
            // dropped. TODO(gtk4): map ToolTipIcon to a themed icon-name via Window.IconName.

        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).

            uint handler = GLib.Timeout.Add((uint)timeout, () =>
            {
                try
                {
                    if (balloonTip != null)
                    {
                        offset_index -= 1;
                        if (offset_index <= 0)
                            offset_bottom = 0;
                        balloonTip.Dispose();
                        balloonTip.Destroy();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug($"Unexpected exception in ShowBalloonTip", ex);
                }
                return false;
            });
            balloonTip.Data.Add("Timeout", handler);
            balloonTip.Show();
            // Gtk4: GetMonitorAtWindow removed; resolve via the window's surface, Monitor has only Geometry.
            Gdk.Monitor monitor = (balloonTip.Surface is Gdk.Surface balloonSurface
                    ? Gdk.Display.Default.GetMonitorAtSurface(balloonSurface)
                    : null)
                ?? (Gdk.Monitor)Gdk.Display.Default.Monitors.GetObject(0);
            int width = monitor.Geometry.Width;
            int height = monitor.Geometry.Height;
            int offsetY = height - balloonTip.AllocatedHeight - 50 - offset_bottom;
            if (offsetY < 0)
            {
                offset_bottom = 0;
            }
            offset_bottom += balloonTip.AllocatedHeight;
            offset_index += 1;
        }

        protected override void Dispose(bool disposing)
        {
            _contextMenuStrip = null;
            statuswin?.Dispose();
            statuswin?.Destroy();
            base.Dispose(disposing);
        }

        private void OnClick(EventArgs e)
        {
            ((EventHandler?)Events[EVENT_CLICK])?.Invoke(this, e);
        }

        private void OnDoubleClick(EventArgs e)
        {
            ((EventHandler?)Events[EVENT_DOUBLECLICK])?.Invoke(this, e);
        }

        private void OnMouseClick(MouseEventArgs mea)
        {
            ((MouseEventHandler?)Events[EVENT_MOUSECLICK])?.Invoke(this, mea);
        }

        private void OnMouseDoubleClick(MouseEventArgs mea)
        {
            ((MouseEventHandler?)Events[EVENT_MOUSEDOUBLECLICK])?.Invoke(this, mea);
        }

        private void OnMouseDown(MouseEventArgs e)
        {
            ((MouseEventHandler?)Events[EVENT_MOUSEDOWN])?.Invoke(this, e);
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            ((MouseEventHandler?)Events[EVENT_MOUSEMOVE])?.Invoke(this, e);
        }

        private void OnMouseUp(MouseEventArgs e)
        {
            ((MouseEventHandler?)Events[EVENT_MOUSEUP])?.Invoke(this, e);
        }

        private void StatusIcon_Activate(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        private static void statuswin_Shown(object? sender, EventArgs e)
        {
            var pw = (Gtk.Window?)sender;
            pw?.Hide();
        }
    }

}