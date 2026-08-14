/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this componentGTKApp.Windows.Forms instead ofMicrosoft.WindowsDesktop.App.WindowsForms， compile once， cross-platformwindows、linux、macos run
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms
{
    [DefaultProperty(nameof(Text))]
    [DefaultEvent(nameof(MouseDoubleClick))]
    [Designer($"System.Windows.Forms.Design.NotifyIconDesigner, {AssemblyRef.SystemDesign}")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public partial class NotifyIcon : Component
    {

        internal const int MaxTextSize = 127;

        /// <summary>
        /// The platform half of this component. Gtk 4 removed <c>GtkStatusIcon</c>, so the system tray is
        /// reached through the native protocol of each platform instead - see
        /// <see cref="INotifyIconBackend" />.
        /// </summary>
        private readonly INotifyIconBackend _backend;

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
            _backend = NotifyIconBackendFactory.Create();

            _backend.Activated += OnBackendActivated;
            _backend.PointerDown += (_, e) => OnMouseDown(ToMouseEventArgs(e));
            _backend.PointerUp += (_, e) => OnMouseUp(ToMouseEventArgs(e));
            _backend.PointerMove += (_, e) => OnMouseMove(ToMouseEventArgs(e));
            _backend.BalloonShown += (_, _) =>
                ((EventHandler?)Events[EVENT_BALLOONTIPSHOWN])?.Invoke(this, EventArgs.Empty);
            _backend.BalloonClicked += (_, _) =>
                ((EventHandler?)Events[EVENT_BALLOONTIPCLICKED])?.Invoke(this, EventArgs.Empty);
            _backend.BalloonClosed += (_, _) =>
                ((EventHandler?)Events[EVENT_BALLOONTIPCLOSED])?.Invoke(this, EventArgs.Empty);
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
                _backend.SetMenu(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this platform and desktop session actually provide a system
        /// tray.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when a tray icon can be shown; otherwise <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        /// When this is <see langword="false" /> the component is inert: every property still round-trips,
        /// but no icon appears and no input event is ever raised. The common cause is GNOME Shell without
        /// the AppIndicator extension, which registers no <c>StatusNotifierWatcher</c> for the tray
        /// protocol to talk to; a headless session behaves the same way.
        /// </para>
        /// <para>
        /// This member has no counterpart in the Microsoft implementation. It exists because a tray is
        /// optional on Linux in a way it never was on Windows, and silently doing nothing would otherwise
        /// be indistinguishable from a bug.
        /// </para>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystemTrayAvailable => _backend.IsAvailable;

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
                    _backend.SetIcon(value);
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
                    _backend.SetToolTip(_text);
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
                    // The property round-trips whatever the platform does: where no tray exists the
                    // backend is inert, and IsSystemTrayAvailable - not Visible - is what reports that.
                    _visible = value;
                    _backend.SetVisible(value);
                }
            }
        }

        public void ShowBalloonTip(int timeout)
        {
            ShowBalloonTip(timeout, _balloonTipTitle, _balloonTipText, _balloonTipIcon);
        }

        /// <summary>
        /// Displays a balloon tip with the given title, text and icon for the specified time.
        /// </summary>
        /// <param name="timeout">
        /// The time, in milliseconds, the balloon should remain visible. Honoured on Linux; ignored on
        /// Windows, where notification duration has been a systemwide accessibility setting since Windows
        /// Vista.
        /// </param>
        /// <param name="tipTitle">The title of the balloon tip.</param>
        /// <param name="tipText">The text of the balloon tip.</param>
        /// <param name="tipIcon">The severity, mapped to the platform's standard notification glyph.</param>
        /// <remarks>
        /// This posts a real desktop notification through the platform's notification service, so it works
        /// on Linux even where no system tray host exists. On macOS it is a no-op outside a signed
        /// application bundle, which is the only context in which that platform permits notifications.
        /// </remarks>
        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            _backend.ShowBalloonTip(timeout, tipTitle ?? string.Empty, tipText ?? string.Empty, tipIcon);
        }

        protected override void Dispose(bool disposing)
        {
            _contextMenuStrip = null;
            _backend.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Translates a backend activation into the WinForms click events, in the order the Microsoft
        /// implementation raises them.
        /// </summary>
        /// <param name="sender">The backend.</param>
        /// <param name="e">The normalised pointer information.</param>
        private void OnBackendActivated(object? sender, TrayPointerEventArgs e)
        {
            var args = ToMouseEventArgs(e);

            OnClick(EventArgs.Empty);
            OnMouseClick(args);

            if (e.Clicks > 1)
            {
                OnDoubleClick(EventArgs.Empty);
                OnMouseDoubleClick(args);
            }
        }

        private static MouseEventArgs ToMouseEventArgs(TrayPointerEventArgs e)
        {
            return new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, 0);
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

    }

}