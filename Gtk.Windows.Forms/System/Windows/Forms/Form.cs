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
    /// Gets or sets the height and width of the client area of the form.
    /// </summary>
    public override Size ClientSize
    {
        get
        {
            // Gtk4 allocates asynchronously: at Form.Load the window is realized but not yet mapped, so the
            // content area's allocation is still 0. Fall back to the requested (default) size until a real
            // allocation exists, so ClientSize round-trips the value set before Show (as WinForms does)
            // instead of momentarily reporting 0.
            if (self.IsRealized && self.ContentArea.AllocatedWidth > 0)
                return new Size(self.ContentArea.AllocatedWidth, self.ContentArea.AllocatedHeight);

            return new Size(self.DefaultWidth, self.DefaultHeight);
        }
        set
        {
            _designedClientSize = value;
            self.SetDefaultSize(value.Width, value.Height);
            // Gtk4's default size covers the whole window, client-side-decoration titlebar included, so on
            // its own it leaves the client area short by the height of that titlebar. Requesting the height
            // on the content box is what actually makes the *client* open at the designed size; the request
            // is released again after the first allocation (see ApplyDesignedClientShortfall) so the window
            // stays freely resizable, as a WinForms form is. Height only - constraining the width here also
            // perturbs the scroller's width negotiation.
            self.ContentArea.SetSizeRequest(-1, value.Height);
        }
    }

    /// <summary>
    /// The client size the form was given before being shown, which the content-fit pass treats as a floor.
    /// </summary>
    /// <remarks>
    /// <see cref="Size.Empty" /> for a form that never sets <see cref="ClientSize" />, which leaves the fit
    /// pass free to size purely from content, as before.
    /// </remarks>
    private Size _designedClientSize;

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

    private IButtonControl? _acceptButton;
    private IButtonControl? _cancelButton;

    /// <summary>
    /// Gets or sets the button on the form that is clicked when the user presses the ENTER key.
    /// </summary>
    /// <remarks>
    /// The button's GTK widget is made the window's default widget, so ENTER activates it natively; the
    /// form additionally applies the button's dialog result and closes, as WinForms does.
    /// </remarks>
    public IButtonControl? AcceptButton
    {
        get => _acceptButton;
        set
        {
            _acceptButton = value;
            if (value is Control control && control.Widget is Gtk.Widget widget)
            {
                // Gtk4: Widget.CanDefault removed; the default widget is set on the window.
                self.DefaultWidget = widget;
            }
        }
    }

    /// <summary>
    /// Gets or sets the button control that is clicked when the user presses the ESC key.
    /// </summary>
    public IButtonControl? CancelButton
    {
        get => _cancelButton;
        set => _cancelButton = value;
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
        set { _location.X = value; base.Left = value; /* Gtk4: Window.Move removed (WM-managed) */ }
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
    /// <summary>
    /// Gets or sets the legacy <see cref="T:System.Windows.Forms.MainMenu" /> displayed on this form.
    /// </summary>
    /// <remarks>
    /// This is how the legacy menu API attaches to a form. Assigning it adds the underlying MenuStrip to
    /// the form and makes it the main menu strip, so old and new menu code end up in the same place.
    /// </remarks>
    public MainMenu? Menu
    {
        get => _menu;
        set
        {
            if (ReferenceEquals(_menu, value))
            {
                return;
            }

            if (_menu != null)
            {
                Controls.Remove(_menu.Strip);
            }

            _menu = value;
            if (value != null)
            {
                Controls.Add(value.Strip);
                MainMenuStrip = value.Strip;
            }
        }
    }

    private MainMenu? _menu;

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
                foreach (var ofm in MdiContainer.GetChildrenArray())
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
                    _MdiParent.MdiContainer2 = new Gtk.Overlay() { WidthRequest = 0, HeightRequest = 0 };
                    _MdiParent.Controls.Add(_MdiParent.MdiContainer2);
                    _MdiParent.MdiContainer = new Gtk.Overlay();
                    _MdiParent.scrolledWindow = new Gtk.ScrolledWindow();
                    _MdiParent.scrolledWindow.StyleContext.AddClass("MDIPanel");
                    _MdiParent.scrolledWindow.AddChild(_MdiParent.MdiContainer);
                    _MdiParent.Controls.Add(_MdiParent.scrolledWindow);

                    if (_MdiParent.scrolledWindow.Parent is Gtk.Overlay olay)
                    {
                        var left = 0; var top = 0; var right = 0; var bottom = 0;
                        foreach (var w in olay.GetChildrenArray())
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
                this.self.StyleContext.AddClass("MDIForm");
                this.Size = new Size(self.DefaultWidth, self.DefaultHeight);
                this.self.Halign = Align.Start;
                this.self.Valign = Align.Start;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
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
        // GTK quantises the window opacity to 8 bits, so reading it back returned 0.502 for 0.5.
        // Report the value that was asked for, as WinForms does.
        get { return _opacity; }
        set
        {
            _opacity = value;
            self.Opacity = value;
        }
    }

    private double _opacity = 1d;

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
    // Gtk4 removed Window.SkipTaskbarHint (taskbar presence is WM/shell policy). Backed by a field.
    private bool _showInTaskbar = true;

    public bool ShowInTaskbar
    {
        get { return _showInTaskbar; }
        set { _showInTaskbar = value; }
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
        set { _location.Y = value; base.Top = value; /* Gtk4: Window.Move removed (WM-managed) */ }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed as a top-level window.
    /// </summary>
    public bool TopLevel
    {
        // Gtk4 removed Widget.IsToplevel; a Form's backing window is always a top-level window.
        get { return self.Parent == null; }
        set { }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed on top of all other windows.
    /// </summary>
    public bool TopMost
    {
        // This reported whether the window was *active*, which has nothing to do with being kept
        // above other windows, so TopMost never read back what was assigned.
        get { return _topMost; }
        set
        {
            _topMost = value;

            // Assigning TopMost must never display a form: WinForms lets you set it on a form that
            // has not been shown, and ShowDialog then refuses an already-visible form. Upstream only
            // presented a realized window (it guarded on Gdk.Window being non-null); that guard was
            // lost when Gdk.Window went away in Gtk4, so assigning TopMost mapped an unshown form and
            // Visible turned true. The recorded value is applied when the form is shown instead.
            if (!self.IsRealized)
                return;

            // Gtk4 removed Gdk.Window/WindowState; the iconified check is no longer queryable.
            if (self.IsVisible == false || self.IsActive == false)
                self.Present();
            self.Activate();
        }
    }

    private bool _topMost;

    /// <summary>
    /// Gets or sets the state (normal, minimized, maximized) of the form.
    /// Setting this property will attempt to change the native window state when the window is mapped.
    /// </summary>
    public FormWindowState WindowState
    {
        get
        {
            // Gtk4 removed Gdk.Window/WindowState. IsMaximized/IsFullscreen are queryable;
            // the iconified (minimized) state is not, so an external minimize is not reflected
            // here - _WindowState keeps whatever was last set programmatically in that case.
            if (self.IsMaximized || self.IsFullscreen)
                _WindowState = FormWindowState.Maximized;
            else if (_WindowState != FormWindowState.Minimized)
                _WindowState = FormWindowState.Normal;

            return _WindowState;
        }
        set
        {
            // Remember the bounds the form should restore to. Capture on the first move away from Normal
            // and hold it across a Minimized<->Maximized switch; clear it once the form is Normal again.
            if (value == FormWindowState.Normal)
            {
                _restoreBounds = null;
            }
            else
            {
                _restoreBounds ??= Bounds;
            }

            _WindowState = value;
            if (value == FormWindowState.Maximized)
            {
                self.Maximize();
            }
            else if (value == FormWindowState.Minimized)
            {
                self.Minimize();
            }
            else if (self.IsMapped)
            {
                self.Present();
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
    /// Closes the form. The close is synchronous, as in WinForms: when this returns the form has raised
    /// <see cref="FormClosing" />/<see cref="FormClosed" /> and given up its handle.
    /// </summary>
    /// <remarks>
    /// This used to hand the close to a 100 ms timer whenever <c>MessageBox.Instance</c> was not the default
    /// <c>NonStaticMessageBox</c>, which made <c>Close()</c> asynchronous - and made whether it was
    /// asynchronous depend on an unrelated global: installing any custom <see cref="IMessageBox" /> silently
    /// turned every form's close into a deferred one, so <see cref="Control.HandleDestroyed" /> (and
    /// everything built on it, e.g. <see cref="ApplicationContext" /> ending its loop) no longer fired by the
    /// time the caller looked. Closing a form has nothing to do with message boxes. The one case that really
    /// needs deferral - closing a form while it is running its own nested modal loop, which Gtk4 cannot
    /// survive re-entrantly - is handled where it belongs, in <c>FormBase.CloseWindow</c>, which quits the
    /// loop and lets <c>RunDialog</c> perform the destroy once the loop has unwound.
    /// </remarks>
    public void Close()
    {
        CloseImmediately();
    }

    /// <summary>
    /// Immediately closes the native window backing this form.
    /// </summary>
    /// <remarks>
    /// The one exception is a close issued from a <see cref="Control.Load" /> handler: GTK is then part-way
    /// through showing/realizing this very window, and tearing it down underneath that call leaves GTK
    /// asserting on a half-shown toplevel (<c>gdk_toplevel_present</c> / <c>gtk_widget_map</c>). The close is
    /// then handed to the next main-loop iteration, which is the first moment the show has unwound - the same
    /// shape of deferral <c>FormBase.CloseWindow</c> already uses for a close issued from inside the form's
    /// own modal loop.
    /// </remarks>
    public void CloseImmediately()
    {
        if (self == null)
        {
            return;
        }

        if (Control.InRealizeCallback)
        {
            GLib.Idle.Add(() =>
            {
                CloseImmediately();

                return false;
            });

            return;
        }

        if (self.CloseWindow())
        {
            this.Dispose(true);
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
                double cellsize = width * height / MdiContainer.GetChildrenArray().Length;
                var cellheight = (int)(Math.Sqrt(cellsize) / width * height);
                var cellwidth = (int)(cellsize / cellheight);
                cellwidth = (int)(width / Math.Ceiling(width * 1f / cellwidth));
                Gtk.Widget? last = null;
                foreach (var fm in MdiContainer.GetChildrenArray())
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
                double cellsize = width * height / MdiContainer.GetChildrenArray().Length;
                var cellheight = (int)(Math.Sqrt(cellsize) / width * height);
                var cellwidth = (int)(cellsize / cellheight);
                cellwidth = (int)(width / Math.Ceiling(width * 1f / cellwidth));
                Gtk.Widget? last = null;
                foreach (var fm in MdiContainer.GetChildrenArray())
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
                    foreach (var fm in MdiContainer.GetChildrenArray())
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
        // This was an empty override, so it swallowed the base implementation entirely - including
        // the auto-scaling a designer-generated form relies on being applied when layout resumes.
        base.ResumeLayout(resume);
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
            // Gtk 4 removed programmatic window positioning (Gtk.WindowPosition); placement is WM-managed.
            // CenterParent is approximated by TransientFor + modality; other StartPositions no-op here.
            if (this.MaximizeBox == false)
            {
                self.Resizable = false;
            }
            self.SetDefaultSize(self.DefaultWidth, self.DefaultHeight); // Gtk4: Window.Resize removed
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

        // Re-showing an already-created form must only show THIS window. A previous version iterated
        // self.Group.ListWindows() and Present()/Show()'d every window in the group - but that is the shared
        // default group, which accumulates every window opened in the process, and presenting a
        // torn-down/leaked one faults (gtk_window_present / gtk_widget_get_realized: GTK_IS_WIDGET failed).
        // self.Show()/Present() below already handle this window; the group sweep was both wrong and unsafe.
        self.Show();
        // Gtk4: Show() maps the window but does not raise it, so a newly opened form (e.g. Form4 via
        // "Split Container Layout") could appear behind its owner. Present() raises and focuses it - it must
        // follow Show() because presenting an unrealized window aborts (gdk_toplevel_present).
        // Show() also realizes the window, which is what raises Load - and Load is user code that may close
        // the form again right away. Raising a window that has just been destroyed trips
        // gdk_toplevel_present/gtk_window_present assertions, so only present one that is still there.
        if (!IsDisposed && self.Handle != IntPtr.Zero)
        {
            self.Present();
        }
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
        if (owner == this)
        {
            throw new ArgumentException("OwnsSelfOrOwner", "showDialog");
        }

        if (base.Visible)
        {
            throw new InvalidOperationException("ShowDialogOnVisible");
        }

        if (!base.Enabled)
        {
            throw new InvalidOperationException("ShowDialogOnDisabled");
        }
        Show(owner);
        // A handler of the Load event (raised during Show) may Close() the form, which disposes it. Only
        // enter the modal loop while the native dialog is still alive; otherwise self.Run() dereferences a
        // disposed/null dialog.
        if (self != null)
        {
            var irun = self.RunDialog();
        }

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
        // NOTE: upstream (gitee) sets `self = null!;` here. That turns any later access to the form's
        // window into a NullReferenceException - e.g. ShowDialog closed from its own Load handler, where
        // Run() then dereferences the nulled field. The fork keeps the (disposed) reference instead.
    }

    /// <summary>
    /// Raises the FormClosed event.
    /// </summary>
    protected virtual void OnFormClosed(System.Windows.Forms.FormClosedEventArgs e)
    {
        FormClosed?.Invoke(this, e);
        // Also raise the legacy Closed event so pre-2.0 source keeps working.
        OnClosed(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the FormClosing event.
    /// </summary>
    protected virtual void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e)
    {
        FormClosing?.Invoke(this, e);
        // Also raise the legacy Closing event; propagate its cancellation back to the modern args.
        var closing = new System.ComponentModel.CancelEventArgs(e.Cancel);
        OnClosing(closing);
        if (closing.Cancel)
        {
            e.Cancel = true;
        }
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
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
        // Gtk4: Widget.FocusInEvent/FocusOutEvent removed. Form Activated/Deactivate should be wired via
        // a Gtk.EventControllerFocus (Enter/Leave) - deferred to the input-controller pass (migration #11).
    }

    /// <summary>
    /// Raises the button's click and, when it carries a dialog result, applies it to the form and closes,
    /// matching how WinForms' accept/cancel buttons dismiss a dialog.
    /// </summary>
    /// <param name="button">The accept or cancel button being activated by the keyboard.</param>
    private void ActivateDialogButton(IButtonControl button)
    {
        (button as Control)?.PerformClick();
        if (button is Button concrete && concrete.DialogResult != DialogResult.None)
        {
            DialogResult = concrete.DialogResult;
            Close();
        }
    }

    private void OnShownHandler()
    {
        OnShown(EventArgs.Empty);
        // Gtk4: the content overlay's natural size follows its (0-sized) background child, so the ScrolledView
        // never requests the room the absolutely-positioned controls actually occupy and the window opens too
        // short - the lowest controls (e.g. a bottom progress bar) scroll off. Gtk3 grew the window to fit its
        // content; reproduce that by sizing the overlay's background child to the controls' real extent once
        // they have been allocated, which raises the ScrolledView's requested height and grows the window.
        _contentFitMax = 0;
        _contentFitStableTicks = 0;
        _contentFitTotalTicks = 0;
        GLib.Timeout.Add(16, EnsureContentFits);
    }

    /// <summary>The largest content extent seen so far; the fit request only ever grows to this.</summary>
    private int _contentFitMax;

    /// <summary>Consecutive ticks the extent has not grown; the watch stops once it has settled.</summary>
    private int _contentFitStableTicks;

    /// <summary>Hard cap on total ticks so a never-mapped window (display-less test host) cannot spin forever.</summary>
    private int _contentFitTotalTicks;

    /// <summary>
    /// Grows the form window so all of its controls are visible. The content lives in an
    /// <see cref="Gtk.Overlay" /> whose size follows its background main child; that child requests nothing, so
    /// GTK gives the ScrolledView only the leftover space after the header and status bar and clips the lower
    /// controls. This watches the children's bottom extent as the async layout fills in and raises the
    /// ScrolledView's requested height to match, so the (resizable) window grows to contain them - matching
    /// the Gtk3 render. It is monotonic (only grows) and keeps watching until the extent settles, so it cannot
    /// latch onto a half-laid-out snapshot (a one-shot measured too early left the bottom controls clipped).
    /// </summary>
    private bool EnsureContentFits()
    {
        // The watch is a GLib timeout, so it outlives the form if the window is closed before the extent
        // settles - a short-lived form, or an application closing one straight after showing it. Touching a
        // torn-down window from here throws (or crashes natively), so stop the watch instead. GtkSharp
        // zeroes a wrapper's Handle when the native object is freed, which is the reliable liveness check.
        if (self == null || self.Handle == IntPtr.Zero)
            return false;

        var overlay = self.contaner;
        if (overlay == null || overlay.Child is not Gtk.Widget background)
            return false;

        int maxBottom = 0;
        foreach (var w in overlay.GetChildrenArray())
        {
            if (ReferenceEquals(w, background) || w.AllocatedHeight <= 1)
                continue;
            maxBottom = Math.Max(maxBottom, w.MarginTop + w.AllocatedHeight);
        }

        ApplyDesignedClientShortfall();

        if (maxBottom > _contentFitMax)
        {
            // More of the layout has resolved - raise the request and reset the settle counter.
            _contentFitMax = maxBottom;
            _contentFitStableTicks = 0;
            if (maxBottom > background.HeightRequest)
                background.HeightRequest = maxBottom;
            // The ScrolledView can scroll, so a bigger child alone would only add a scrollbar; raising its own
            // minimum content height is what forces the (resizable) window to grow to contain the controls.
            if (maxBottom > self.ScrolledView.MinContentHeight)
                self.ScrolledView.MinContentHeight = maxBottom;
            // The grow compensates for the squeezed viewport, not a user resize: forget the anchor baseline so
            // the re-layout keeps controls at their designed positions instead of sliding bottom/right-anchored
            // ones by the delta (which otherwise pushed e.g. a bottom-anchored label down onto the control below).
            ResetLayoutBaseline();
        }
        else if (_contentFitMax > 0)
        {
            _contentFitStableTicks++;
        }

        // Keep watching until the extent has been stable for a short spell, then stop (hard-capped at ~2s).
        return _contentFitStableTicks < 10 && ++_contentFitTotalTicks < 120;
    }

    /// <summary>
    /// Grows the window once so its client area is at least the form's designed
    /// <see cref="ClientSize" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// WinForms opens a form at exactly its <see cref="ClientSize" />. Gtk4's
    /// <c>gtk_window_set_default_size</c>, which the setter maps onto, sizes the whole window <em>including
    /// the client-side-decoration titlebar</em> - so the client area came out short by the height of that
    /// titlebar on every form. The content-fit pass could not make it up either: its measurement is
    /// self-referential wherever a control is docked to an edge, because a bottom-docked StatusStrip sits at
    /// the bottom of whatever the client currently is and therefore always measures exactly as tall as the
    /// client already is. The visible symptom was the designed gap between the last control and the strip
    /// vanishing - the strip sat flush against the content.
    /// </para>
    /// <para>
    /// The correction is applied to the window rather than to the scrolled view's minimum content height.
    /// Raising that minimum also perturbs the scroller's width negotiation, which was enough to push a
    /// TreeView in another form below its own minimum width and abort in GTK's measure pass.
    /// </para>
    /// </remarks>
    private void ApplyDesignedClientShortfall()
    {
        if (_clientShortfallApplied || _designedClientSize.Height <= 0)
        {
            return;
        }

        // Wait for the client box to reach the requested height, then drop the request so the window can be
        // resized freely - including smaller than its designed size, as a WinForms form can be. The content
        // area can already be gone if the form is closing while this pass is still running.
        var content = self.ContentArea;
        if (content == null || content.AllocatedHeight < _designedClientSize.Height)
        {
            return;
        }

        _clientShortfallApplied = true;
        content.SetSizeRequest(-1, -1);
    }

    /// <summary>Whether the designed-client-size correction has already been applied to this window.</summary>
    private bool _clientShortfallApplied;

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

    private void UpdateMDILayout(Gtk.Overlay? lay)
    {
        if (lay != null)
        {
            lay.SetSizeRequest(-1, -1);
            foreach (var widget in lay.GetChildrenArray())
            {
                if (widget.AllocatedWidth > 1 && widget.AllocatedWidth + widget.MarginStart > lay.WidthRequest)
                    lay.WidthRequest = widget.AllocatedWidth + widget.MarginStart;

                if (widget.AllocatedHeight > 1 && widget.AllocatedHeight + widget.MarginTop > lay.HeightRequest)
                    lay.HeightRequest = widget.AllocatedHeight + widget.MarginTop;
            }
        }
    }
}
