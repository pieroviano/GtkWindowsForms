/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 */

using System.ComponentModel;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkSystem.Windows.Forms.Resources;
using Icon = System.Drawing.Icon;

namespace System.Windows.Forms;

/// <summary>
/// Represents a window or dialog box that makes up an application's user interface.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
[DesignerCategory("Form")]
[DefaultEvent(nameof(Load)),
InitializationEvent(nameof(Load))]
public partial class Form : ContainerControl, IWin32Window
{
    /// <summary>
    /// Underlying GTK window wrapper used by this form.
    /// </summary>
    public FormBase self = new();

    /// <summary>
    /// The GTK overlay hosting the MDI children of this form.
    /// </summary>
    internal Gtk.Overlay MdiContainer = null!;

    /// <summary>
    /// A hidden GTK overlay reserved for the MDI client area.
    /// </summary>
    internal Gtk.Overlay MdiContainer2 = null!;

    /// <summary>
    /// The scrolled window hosting the MDI container.
    /// </summary>
    internal Gtk.ScrolledWindow scrolledWindow = null!;

    private Form? _ActiveMdiChild;

    private bool _ControlBox = true;

    private Point _location = new();

    private Form? _MdiParent;

    private ControlCollection _ObjectCollection = null!;

    private FormWindowState _WindowState = FormWindowState.Normal;

    private Gtk.Application app = Application.Init();

    private FormBorderStyle formBorderStyle = FormBorderStyle.Sizable;

    private bool Is_Control_Shown = false;

    private double prevheight = 0;

    private double prevmarginstart = 0;

    private double prevmargintop = 0;

    private double prevwidth = 0;

    /// <summary>
    /// Occurs after the form has been closed.
    /// </summary>
    public virtual event FormClosedEventHandler? FormClosed;

    /// <summary>
    /// Occurs as the form is being closed. Handlers can cancel the close.
    /// </summary>
    public virtual event FormClosingEventHandler? FormClosing;

    /// <summary>
    /// Occurs when the form is loaded.
    /// </summary>
    public override event EventHandler? Load;

    /// <summary>
    /// Occurs the first time the form is shown.
    /// </summary>
    public virtual event EventHandler? Shown;

    /// <summary>
    /// Occurs when the form's scroll position changes.
    /// </summary>
    public override event ScrollEventHandler Scroll
    {
        add { self.Scroll += value; }
        remove { self.Scroll += value; }
    }

    static Form()
    {
        Application.Init();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Form" /> class.
    /// </summary>
    public Form() : base()
    {
        self.Override.sender = this;
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Form" /> class with the specified title.
    /// </summary>
    /// <param name="title">
    /// The initial text displayed in the form's title bar.
    /// </param>
    public Form(string title) : this()
    {
        self.Title = title;
    }

    /// <summary>
    /// Gets the currently active multiple-document interface (MDI) child window.
    /// </summary>
    public Form? ActiveMdiChild
    {
        get => _ActiveMdiChild?.self == null ? null : _ActiveMdiChild;
        internal set => _ActiveMdiChild = value;
    }

    /// <summary>
    /// Gets or sets the design-time autoscale dimensions for the form.
    /// </summary>
    public SizeF AutoScaleDimensions
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the automatic scaling mode of the form.
    /// </summary>
    public AutoScaleMode AutoScaleMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the height and width of the client area of the form.
    /// </summary>
    public override Size ClientSize
    {
        get
        {
            if (self.IsRealized)
                return new Size(self.ContentArea.AllocatedWidth, self.ContentArea.AllocatedHeight);
            else
                return new Size(self.DefaultWidth, self.DefaultHeight);
        }
        set
        {
            self.SetDefaultSize(value.Width, value.Height);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a control box is displayed in the caption bar of the form.
    /// </summary>
    public bool ControlBox
    {
        get => _ControlBox;
        set { _ControlBox = value; self.MinimizeBox = value; self.MaximizeBox = value; self.Deletable = value; }
    }

    /// <summary>
    /// Gets the collection of controls contained within the form.
    /// </summary>
    public override ControlCollection Controls { get { return _ObjectCollection; } }

    /// <summary>
    /// Gets or sets the dialog result produced by the form.
    /// </summary>
    public DialogResult DialogResult
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the border style of the form.
    /// </summary>
    public FormBorderStyle FormBorderStyle
    {
        get { return formBorderStyle; }
        set
        {
            formBorderStyle = value;
            self.Resizable = value == FormBorderStyle.Sizable || value == FormBorderStyle.SizableToolWindow || value == FormBorderStyle.None;
            if (value == FormBorderStyle.None)
            {
                self.Decorated = false;
            }
            else
            {
                self.Decorated = true;
            }
        }
    }

    /// <summary>
    /// Gets the underlying GTK control object used by the form.
    /// </summary>
    public override object GtkControl { get => self; }

    /// <summary>
    /// Gets the native window handle for this form.
    /// </summary>
    public override IntPtr Handle => self.Handle;

    /// <summary>
    /// Gets or sets the icon associated with this form.
    /// </summary>
    public Icon? Icon
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the form is a multiple-document interface (MDI) child form.
    /// </summary>
    public bool IsMdiChild { get => _MdiParent != null; }

    /// <summary>
    /// Gets or sets a value indicating whether the form is a container for multiple-document interface (MDI) child forms.
    /// </summary>
    public bool IsMdiContainer
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form will receive key events before they are passed to the control that has focus.
    /// </summary>
    public bool KeyPreview
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the distance, in pixels, between the left edge of the form and the left edge of its container.
    /// </summary>
    public override int Left
    {
        get => _location.X;
        set { _location.X = value; base.Left = value; self.Move(_location.X, _location.Y); }
    }

    /// <summary>
    /// Gets or sets the coordinates of the upper-left corner of the form relative to its container.
    /// </summary>
    public override Point Location
    {
        get => _location;
        set { Left = value.X; Top = value.Y; }
    }

    /// <summary>
    /// Gets or sets the primary menu strip for the form.
    /// </summary>
    public MenuStrip? MainMenuStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Maximize button is displayed.
    /// </summary>
    public bool MaximizeBox
    {
        get => self.MaximizeBox;
        set => self.MaximizeBox = value && _ControlBox;
    }

    /// <summary>
    /// Gets an array of forms that represent the multiple-document interface (MDI) child forms parented to this form.
    /// </summary>
    public Form[] MdiChildren
    {
        get
        {
            var forms = new List<Form>();
            if (this.IsMdiContainer && MdiContainer != null)
            {
                foreach (var ofm in MdiContainer.Children)
                {
                    if (ofm.Data.ContainsKey("Control") && ofm.Data["Control"] is Form fm)
                        forms.Add(fm);
                }
            }

            return forms.ToArray();
        }
    }

    /// <summary>
    /// Gets or sets the current multiple-document interface (MDI) parent form of this form.
    /// </summary>
    public Form? MdiParent
    {
        get => _MdiParent;
        set
        {
            if (value == null)
            {
                if (_MdiParent != null)
                {
                    self.CloseWindow();
                }
                _MdiParent = value;
            }
            else
            {
                if (value.IsMdiContainer == false)
                    throw new Exception("MdiParent is not MdiContainer");

                _MdiParent = value;

                if (_MdiParent.MdiContainer == null)
                {
                    _MdiParent.MdiContainer2 = new Gtk.Overlay() { NoShowAll = true, WidthRequest = 0, HeightRequest = 0 };
                    _MdiParent.Controls.Add(_MdiParent.MdiContainer2);
                    _MdiParent.MdiContainer = new Gtk.Overlay();
                    _MdiParent.scrolledWindow = new Gtk.ScrolledWindow();
                    _MdiParent.scrolledWindow.StyleContext.AddClass("MDIPanel");
                    _MdiParent.scrolledWindow.Add(_MdiParent.MdiContainer);
                    _MdiParent.Controls.Add(_MdiParent.scrolledWindow);

                    if (_MdiParent.scrolledWindow.Parent is Gtk.Overlay olay)
                    {
                        var left = 0; var top = 0; var right = 0; var bottom = 0;
                        foreach (var w in olay.Children)
                        {
                            if (w.Halign == Align.Fill && w.Valign == Align.Start)
                            {
                                top = Math.Max(top, w.HeightRequest + w.MarginTop);
                            }
                            else if (w.Halign == Align.Fill && w.Valign == Align.End)
                            {
                                bottom += w.HeightRequest;
                            }

                            if (w.Valign == Align.Fill && w.Halign == Align.Start)
                            {
                                left = Math.Max(left, w.WidthRequest + w.MarginStart);
                            }
                            else if (w.Valign == Align.Fill && w.Halign == Align.End)
                            {
                                right += w.WidthRequest;
                            }
                        }
                        _MdiParent.scrolledWindow.MarginTop = top;
                        _MdiParent.scrolledWindow.MarginStart = left;
                        _MdiParent.scrolledWindow.MarginEnd = right;
                        _MdiParent.scrolledWindow.MarginBottom = bottom;
                    }
                }
                this.self.CanFocus = true;
                this.self.BorderWidth = 2;
                this.self.StyleContext.AddClass("MDIForm");
                this.Size = new Size(self.DefaultWidth, self.DefaultHeight);
                this.self.Halign = Align.Start;
                this.self.Valign = Align.Start;
                this.self.WidgetEventAfter += Self_WidgetEventAfter;
                _MdiParent.MdiContainer.WidgetEventAfter += Contaner_WidgetEventAfter;
                _MdiParent.MdiContainer.AddOverlay(self);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Minimize button is displayed.
    /// </summary>
    public bool MinimizeBox
    {
        get => self.MinimizeBox;
        set => self.MinimizeBox = value && _ControlBox;
    }

    /// <summary>
    /// Gets or sets the form's opacity (0.0 - fully transparent, 1.0 - fully opaque).
    /// </summary>
    public double Opacity
    {
        get { return self.Opacity; }
        set { self.Opacity = value; }
    }

    /// <summary>
    /// Gets or sets the form that owns this form.
    /// </summary>
    public Form? Owner
    {
        get => this.Parent as Form;
        set
        {
            if (this.Parent != null && this.Parent is Form parent)
                parent.self.Group.RemoveWindow(self);
            this.Parent = value;
            if (value != null)
            {
                self.TransientFor = value.self;
                self.DestroyWithParent = true;
                value.self.Group.AddWindow(self);
            }
        }
    }

    /// <summary>
    /// Gets or sets the padding within the form. Padding is applied to the internal GTK overlay margins.
    /// </summary>
    public override Padding Padding
    {
        get => base.Padding;
        set
        {
            base.Padding = value;
            self.contaner.MarginStart = value.Left;
            self.contaner.MarginTop = value.Top;
            self.contaner.MarginEnd = value.Right;
            self.contaner.MarginBottom = value.Bottom;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form's icon is shown.
    /// </summary>
    public bool ShowIcon
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed in the taskbar.
    /// </summary>
    public bool ShowInTaskbar
    {
        get { return self.SkipTaskbarHint == false; }
        set { self.SkipTaskbarHint = value == false; }
    }

    /// <summary>
    /// Gets or sets the starting position of the form when first shown.
    /// </summary>
    public FormStartPosition StartPosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text associated with the form (displayed in the title bar).
    /// </summary>
    public override string Text
    {
        get { return self.Title; }
        set { self.Title = value; }
    }

    /// <summary>
    /// Gets or sets the distance, in pixels, between the top edge of the form and the top edge of its container.
    /// </summary>
    public override int Top
    {
        get => _location.Y;
        set { _location.Y = value; base.Top = value; self.Move(_location.X, _location.Y); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed as a top-level window.
    /// </summary>
    public bool TopLevel
    {
        get { return self.IsToplevel; }
        set { }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed on top of all other windows.
    /// </summary>
    public bool TopMost
    {
        get { return self.IsActive; }
        set
        {
            self.KeepAbove = value;
            if (self.Window != null)
                if (self.IsVisible == false || self.IsActive == false || self.Window.State.HasFlag(Gdk.WindowState.Iconified))
                    self.Present();
            self.Activate();
        }
    }

    /// <summary>
    /// Gets or sets the state (normal, minimized, maximized) of the form.
    /// Setting this property will attempt to change the native window state when the window is mapped.
    /// </summary>
    public FormWindowState WindowState
    {
        get
        {
            if (self.Window != null)
            {
                if (self.Window.State.HasFlag(Gdk.WindowState.Iconified))
                    _WindowState = FormWindowState.Minimized;
                else if (self.Window.State.HasFlag(Gdk.WindowState.Maximized) || self.Window.State.HasFlag(Gdk.WindowState.Fullscreen))
                    _WindowState = FormWindowState.Maximized;
                else
                    _WindowState = FormWindowState.Normal;
            }

            return _WindowState;
        }
        set
        {
            _WindowState = value;
            if (value == FormWindowState.Maximized)
            {
                self.Maximize();
            }
            else if (value == FormWindowState.Minimized)
            {
                self.Iconify();
            }
            else if (self.IsMapped)
            {
                self.Deiconify();
                self.Present();
            }
        }
    }

    /// <summary>
    /// Gets or sets the scrollable base used by the form.
    /// </summary>
    protected override IScrollableBoxBase? scrollbase
    {
        get => self;
        set => base.scrollbase = value;
    }

    /// <summary>
    /// Activates the form and gives it focus.
    /// </summary>
    /// <returns>
    /// True if the activation succeeded; otherwise false.
    /// </returns>
    public bool Activate()
    {
        return self.Activate();
    }

    /// <summary>
    /// Closes the form. Uses a short timer to defer actual close so UI handlers can complete.
    /// </summary>
    public void Close()
    {
        if (MessageBox.Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            CloseImmediately();
            return;
        }
        var timer = new System.Windows.Forms.Timer
        {
            Interval = 100
        };

        timer.Tick += (s, e) =>
        {
            timer.Stop();
            CloseImmediately();
        };

        timer.Start();
    }

    /// <summary>
    /// Immediately closes the native window backing this form.
    /// </summary>
    public void CloseImmediately()
    {
        if (self != null)
        {
            if (self.CloseWindow())
            {
                this.Dispose(true);
            }
        }
    }

    /// <summary>
    /// Hides the form.
    /// </summary>
    public override void Hide()
    {
        if (self != null)
        {
            self.Hide();
        }
    }

    /// <summary>
    /// Arranges the multiple-document interface (MDI) child forms within this form.
    /// </summary>
    /// <param name="value">
    /// The layout to apply to the MDI child forms.
    /// </param>
    public void LayoutMdi(MdiLayout value)
    {
        if (MdiContainer != null)
        {
            if (value == MdiLayout.TileVertical)
            {
                int x = 0, y = 0;
                var width = scrolledWindow.Parent.AllocatedWidth - scrolledWindow.MarginStart - scrolledWindow.MarginEnd;
                var height = scrolledWindow.Parent.AllocatedHeight - scrolledWindow.MarginTop - scrolledWindow.MarginBottom;
                double cellsize = width * height / MdiContainer.Children.Length;
                var cellheight = (int)(Math.Sqrt(cellsize) / width * height);
                var cellwidth = (int)(cellsize / cellheight);
                cellwidth = (int)(width / Math.Ceiling(width * 1f / cellwidth));
                Gtk.Widget? last = null;
                foreach (var fm in MdiContainer.Children)
                {
                    last = fm;
                    fm.MarginStart = x;
                    fm.MarginTop = y;
                    fm.HeightRequest = cellheight;
                    fm.WidthRequest = cellwidth;
                    y += cellheight;
                    if (y + 50 >= height)
                    {
                        x += cellwidth;
                        y = 0;
                    }
                }
                if (height > y && last != null)
                    last.HeightRequest = last.HeightRequest + height - y;

                MdiContainer.QueueResize();
                MdiContainer.QueueDraw();
            }
            else if (value == MdiLayout.TileHorizontal)
            {
                int x = 0, y = 0;
                var width = scrolledWindow.Parent.AllocatedWidth - scrolledWindow.MarginStart - scrolledWindow.MarginEnd;
                var height = scrolledWindow.Parent.AllocatedHeight - scrolledWindow.MarginTop - scrolledWindow.MarginBottom;
                double cellsize = width * height / MdiContainer.Children.Length;
                var cellheight = (int)(Math.Sqrt(cellsize) / width * height);
                var cellwidth = (int)(cellsize / cellheight);
                cellwidth = (int)(width / Math.Ceiling(width * 1f / cellwidth));
                Gtk.Widget? last = null;
                foreach (var fm in MdiContainer.Children)
                {
                    last = fm;
                    fm.MarginStart = x;
                    fm.MarginTop = y;
                    fm.HeightRequest = cellheight;
                    fm.WidthRequest = cellwidth;
                    x += cellwidth;
                    if (x + 50 >= width)
                    {
                        x = 0;
                        y += cellheight;
                    }
                }
                if (width > x && last != null)
                    last.WidthRequest = last.WidthRequest + width - x;

                MdiContainer.QueueResize();
                MdiContainer.QueueDraw();
            }
            else if (value == MdiLayout.Cascade || value == MdiLayout.ArrangeIcons)
            {
                if (this.IsMdiContainer && MdiContainer != null)
                {
                    int x = 0, y = 0;
                    foreach (var fm in MdiContainer.Children)
                    {
                        fm.MarginStart = x;
                        fm.MarginTop = y;
                        x += 30;
                        y += 30;
                    }
                    MdiContainer.QueueResize();
                    MdiContainer.QueueDraw();
                }
            }
            GLib.Timeout.Add(500, () =>
            {
                UpdateMDILayout(MdiContainer);
                return false;
            });
        }
    }

    /// <summary>
    /// Forces the control to perform layout; marks the form as created.
    /// </summary>
    public override void PerformLayout()
    {
    }

    /// <summary>
    /// Resumes layout logic for the form.
    /// </summary>
    /// <param name="resume">
    /// If true, resume layout processing; otherwise adjust created flag.
    /// </param>
    public override void ResumeLayout(bool resume)
    {
    }

    /// <summary>
    /// Shows the form.
    /// </summary>
    public override void Show()
    {
        this.Show(null);
    }

    /// <summary>
    /// Shows the form with an optional owner window.
    /// </summary>
    /// <param name="owner">
    /// The owner window. May be null.
    /// </param>
    public void Show(IWin32Window? owner)
    {
        if (owner == this)
        {
            throw new InvalidOperationException("Owns Self Or Owner");
        }
        if (_Created == false && self.IsRealized == false)
        {
            _Created = true;
            if (owner != null && owner is Form parent)
            {
                Owner = parent;
            }
            if (StartPosition == FormStartPosition.CenterScreen)
                self.WindowPosition = WindowPosition.Center;
            else if (StartPosition == FormStartPosition.Manual)
                self.WindowPosition = WindowPosition.Mouse;
            else if (this.Parent != null && StartPosition == FormStartPosition.CenterParent)
                self.WindowPosition = WindowPosition.CenterOnParent;
            else
                self.WindowPosition = WindowPosition.Center;

            if (this.MaximizeBox == false)
            {
                self.Resizable = false;
            }
            self.Resize(self.DefaultWidth, self.DefaultHeight);
            try
            {
                if (this.ShowIcon)
                {
                    if (this.Icon != null)
                    {
                        if (this.Icon.Pixbuf != null)
                            self.Icon = this.Icon.Pixbuf;
                        else if (this.Icon.PixbufData != null)
                            self.Icon = new Gdk.Pixbuf(this.Icon.PixbufData);
                    }
                    else
                    {
                        if (System.IO.File.Exists("Resources/icon.png"))
                            self.Icon = new Gdk.Pixbuf("Resources/icon.png");
                        else if (System.IO.File.Exists("Resources/icon.ico"))
                            self.Icon = new Gdk.Pixbuf("Resources/icon.ico");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Show", ex);
                self.Icon = new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("image-missing16.png"));
            }
        }
        else
        {
            foreach (var item in this.self.Group.ListWindows())
            {
                item.Deiconify();
                item.ShowAll();
            }
        }
        self.ShowAll();
    }

    /// <summary>
    /// Shows the form as a modal dialog.
    /// </summary>
    /// <returns>
    /// The dialog result produced by the form.
    /// </returns>
    public DialogResult ShowDialog()
    {
        return ShowDialog(null);
    }

    /// <summary>
    /// Shows the form as a modal dialog with the specified owner.
    /// </summary>
    /// <param name="owner">
    /// The owner window. May be null.
    /// </param>
    /// <returns>
    /// The dialog result produced by the form.
    /// </returns>
    public DialogResult ShowDialog(IWin32Window? owner)
    {
        Show(owner);
        self.TypeHint = Gdk.WindowTypeHint.Dialog;
        var irun = self.Run();

        return this.DialogResult;
    }

    /// <summary>
    /// Suspends layout logic for the form.
    /// </summary>
    public override void SuspendLayout()
    {
        _Created = false;
    }

    /// <summary>
    /// Activates the specified multiple-document interface (MDI) child form.
    /// </summary>
    /// <param name="form">
    /// The MDI child form to activate.
    /// </param>
    protected void ActivateMdiChild(Form? form)
    {
        form?.Activate();
        ActiveMdiChild = form;
    }

    /// <summary>
    /// Releases the resources used by the form.
    /// </summary>
    /// <param name="disposing">
    /// if set to
    /// <c>
    /// true
    /// </c> managed resources are released.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        self = null!;
    }

    /// <summary>
    /// Raises the FormClosed event.
    /// </summary>
    protected virtual void OnFormClosed(System.Windows.Forms.FormClosedEventArgs e)
    {
        FormClosed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the FormClosing event.
    /// </summary>
    protected virtual void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
    {
        FormClosing?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Load event.
    /// </summary>
    protected override void OnLoad(System.EventArgs e)
    {
        Load?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Shown event.
    /// </summary>
    protected virtual void OnShown(System.EventArgs e)
    {
        Shown?.Invoke(this, e);
    }

    private void Contaner_WidgetEventAfter(object o, WidgetEventAfterArgs args)
    {
        if (args.Event.Type == Gdk.EventType.ButtonRelease)
        {
            var lay = o as Gtk.Overlay;
            UpdateMDILayout(lay);
        }
    }

    private void Control_Shown(object sender, EventArgs e)
    {
        if (Is_Control_Shown == false)
        {
            Is_Control_Shown = true;
        }
        OnShownHandler();
    }

    private void Init()
    {
        _ObjectCollection = new ControlCollection(this, self.contaner);
        self.Shown += Control_Shown;
        self.CloseWindowEvent += Self_CloseWindowEvent;
    }

    private void OnShownHandler()
    {
        OnShown(EventArgs.Empty);
    }

    private bool Self_CloseWindowEvent(object sender, EventArgs e)
    {
        var closing = new FormClosingEventArgs(CloseReason.UserClosing, false);
        OnFormClosing(closing);

        if (closing.Cancel == false)
        {
            OnFormClosed(new FormClosedEventArgs(CloseReason.UserClosing));

            this.Dispose();
        }

        return closing.Cancel;
    }

    private void Self_WidgetEventAfter(object o, WidgetEventAfterArgs args)
    {
        var owidget = o as Gtk.Dialog;
        if (owidget == null)
            return;
        if (args.Event is Gdk.EventMotion motion && motion.State.HasFlag(Gdk.ModifierType.Button1Mask))
        {
            owidget.Window.GetOrigin(out var x, out var y);
            if (motion.XRoot > x && motion.YRoot < y + 50)
            {
                owidget.MarginStart = Math.Max(0, (int)(prevmarginstart + motion.XRoot));
                owidget.MarginTop = Math.Max(0, (int)(prevmargintop + motion.YRoot));
            }
            if (motion.XRoot > x + owidget.AllocatedWidth - 10 || motion.YRoot > y + owidget.AllocatedHeight - 10)
            {
                owidget.WidthRequest = Math.Max(0, (int)(prevwidth + motion.XRoot));
                owidget.HeightRequest = Math.Max(0, (int)(prevheight + motion.YRoot));
            }
        }
        else if (args.Event.Type == Gdk.EventType.ButtonPress && args.Event is Gdk.EventButton btn)
        {
            prevmarginstart = owidget.MarginStart - btn.XRoot;
            prevmargintop = owidget.MarginTop - btn.YRoot;

            prevwidth = owidget.AllocatedWidth - btn.XRoot;
            prevheight = owidget.AllocatedHeight - btn.YRoot;
        }
        else if (args.Event.Type == Gdk.EventType.ButtonRelease)
        {
            if (owidget.Parent is Gtk.Overlay lay)
            {
                lay.ReorderOverlay(owidget, lay.Children.Length);
                if (_MdiParent != null)
                    _MdiParent.ActiveMdiChild = this;
            }
        }
    }

    private void UpdateMDILayout(Gtk.Overlay? lay)
    {
        if (lay != null)
        {
            lay.SetSizeRequest(-1, -1);
            foreach (var widget in lay.Children)
            {
                if (widget.AllocatedWidth > 1 && widget.AllocatedWidth + widget.MarginStart > lay.WidthRequest)
                    lay.WidthRequest = widget.AllocatedWidth + widget.MarginStart;

                if (widget.AllocatedHeight > 1 && widget.AllocatedHeight + widget.MarginTop > lay.HeightRequest)
                    lay.HeightRequest = widget.AllocatedHeight + widget.MarginTop;
            }
        }
    }
}
