using System.Windows.Forms;
using Gdk;
using Gtk;
using ScrollEventHandler = System.Windows.Forms.ScrollEventHandler;
// Gdk and Gtk both define an EventArgs; the WinForms handler pattern here always means System.EventArgs.
using EventArgs = System.EventArgs;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// FormBase is a base class for GTK form controls, providing common functionality and properties for derived classes. It implements the IControlGtk, IScrollableBoxBase, and IWin32Window interfaces, allowing it to integrate with the GTK system and provide consistent behavior across different types of form controls. The FormBase class includes features such as auto-scrolling, event handling for closing the window, and custom drawing capabilities.
/// </summary>
/// <seealso cref="Gtk.Dialog" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" />
/// <seealso cref="T:System.Windows.Forms.IWin32Window" />
public sealed class FormBase : Gtk.Dialog, IControlGtk, IScrollableBoxBase, IWin32Window
{

    /// <summary>
    /// The overlay container hosting the form background and content.
    /// </summary>
    public Gtk.Overlay contaner = new();

    /// <summary>
    /// The scrolled view
    /// </summary>
    public readonly Gtk.ScrolledWindow ScrolledView = new();

    private Size prevSize = new Size(0, 0);

    private bool isCloseWindow;

    private Gtk.Button? maximize;

    private Gtk.Button? minimize;

    /// <summary>
    /// Occurs when the close window event occurs.
    /// </summary>
    public event CloseWindowHandler? CloseWindowEvent;

    /// <summary>
    /// Occurs when a scroll event occurs..
    /// </summary>
    public event ScrollEventHandler? Scroll;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.FormBase" /> class.
    /// </summary>
    public FormBase() : base("title", null, DialogFlags.UseHeaderBar | DialogFlags.DestroyWithParent)
    {
        this.Override = new GtkControlOverride(this);
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        if (Titlebar is Gtk.HeaderBar headerbar)
        {
            headerbar.DecorationLayout = "menu:close";
            headerbar.ShowCloseButton = false;
            var close = new Gtk.Button()
            {
                IconName = "window-close-symbolic",
                Name = "close",
                Visible = true,
                HasFrame = false,
                Valign = Align.Center,
                Halign = Align.Center
            };
            close.StyleContext.AddClass("close");
            close.StyleContext.AddClass("titlebutton");
            close.Clicked += Close_Clicked;
            headerbar.PackEnd(close);

            maximize = new Gtk.Button()
            {
                IconName = "window-maximize-symbolic",
                Name = "maximize",
                Visible = true,
                HasFrame = false,
                Valign = Align.Center,
                Halign = Align.Center
            };
            maximize.StyleContext.AddClass("maximize");
            maximize.StyleContext.AddClass("titlebutton");
            maximize.Clicked += maximize_Clicked;
            headerbar.PackEnd(maximize);
            minimize = new Gtk.Button()
            {
                IconName = "window-minimize-symbolic",
                Name = "minimize",
                Visible = true,
                HasFrame = false,
                Valign = Align.Center,
                Halign = Align.Center
            };
            minimize.StyleContext.AddClass("minimize");
            minimize.StyleContext.AddClass("titlebutton");
            minimize.Clicked += minimize_Clicked;
            headerbar.PackEnd(minimize);
        }

        this.SetDefaultSize(100, 10);
        // Gtk4: Gdk.WindowTypeHint / Window.TypeHint removed (WM decides window type).
        this.Deletable = true;
        this.Decorated = true;
        this.Close += FormBase_Close;
        this.Response += FormBase_Response;
        // Gtk 4: DeleteEvent -> CloseRequest (RetVal=true now VETOES the close, matching !isCloseWindow).
        this.CloseRequest += FormBase_CloseRequest;
        // Gtk 4 removed WindowStateEvent; external maximize/restore no longer re-skins the button here
        // (the maximize button itself still toggles its icon). TODO(gtk4): observe notify::maximized.
        ScrolledView.Valign = Gtk.Align.Fill;
        ScrolledView.Halign = Gtk.Align.Fill;
        ScrolledView.Hexpand = true;
        ScrolledView.Vexpand = true;
        ScrolledView.HscrollbarPolicy = PolicyType.Automatic;
        ScrolledView.VscrollbarPolicy = PolicyType.Automatic;
        ScrolledView.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
        ScrolledView.Vadjustment.ValueChanged += Vadjustment_ValueChanged;
        this.ContentArea.Spacing = 0;
        this.ContentArea.Homogeneous = false;
        this.ContentArea.Append(ScrolledView);
        this.ContentArea.StyleContext.AddClass("Form");
        contaner.Valign = Gtk.Align.Fill;
        contaner.Halign = Gtk.Align.Fill;
        var background = new Gtk.DrawingArea();
        // Gtk 4: DrawingArea renders through a DrawFunc rather than the removed Drawn signal.
        background.DrawFunc = Background_DrawFunc;
        contaner.AddChild(background);
        ScrolledView.Child = contaner;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool AutoScroll
    {
        get => ScrolledView.VscrollbarPolicy == Gtk.PolicyType.Automatic;
        set
        {
            if (value == true)
            {
                if (VScroll)
                    ScrolledView.VscrollbarPolicy = Gtk.PolicyType.Automatic;
                if (HScroll)
                    ScrolledView.HscrollbarPolicy = Gtk.PolicyType.Automatic;
            }
            else
            {
                ScrolledView.VscrollbarPolicy = Gtk.PolicyType.External;
                ScrolledView.HscrollbarPolicy = Gtk.PolicyType.External;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether  horizontal scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if  horizontal scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HScroll
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the window icon shown in the header bar.
    /// </summary>
    public new Gdk.Pixbuf Icon
    {
        get => base.Icon;
        set
        {
            base.Icon = value;
            if (value != null && base.Titlebar != null && base.Titlebar is Gtk.HeaderBar titlebar)
            {
                var flag = new Gtk.Image(value.ScaleSimple(24, 24, Gdk.InterpType.Bilinear));
                flag.Visible = true;
                titlebar.PackStart(flag);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the maximize button is visible.
    /// </summary>
    public bool MaximizeBox
    {
        get => maximize?.Visible??false;
        set
        {
            if (maximize != null)
            {
                maximize.Visible = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the minimize button is visible.
    /// </summary>
    public bool MinimizeBox
    {
        get => minimize?.Visible??false;
        set
        {
            if (minimize != null)
            {
                minimize.Visible = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control.
    /// This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events
    /// so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="T:System.Windows.Forms.PaintEventHandler" />).
    /// The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the scrolled window that hosts the form content.
    /// </summary>
    public Gtk.ScrolledWindow ScrolledWindow { get => ScrolledView; }

    /// <summary>
    /// Gets or sets a value indicating whether vertical scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if vertical scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool VScroll
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Adds the specified child.
    /// </summary>
    /// <param name="child">
    /// The child.
    /// </param>
    public new void Add(Gtk.Widget child)
    {
        ScrolledView.Child = child;
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    /// <param name="isResponse">
    /// <c>
    /// true
    /// </c> when invoked while handling a dialog response.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the window was closed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public bool CloseWindow(bool isResponse = false)
    {
        isCloseWindow = false;
        if (CloseWindowEvent?.Invoke(this, EventArgs.Empty) != true)
        {
            if (this.Data["Control"] is Form from1)
            {
                from1.Dispose();
                this.Destroy();
            }
            isCloseWindow = true;
        }
        else if (isResponse == true) // Gtk4: TypeHint removed; FormBase is a Gtk.Dialog
        {
            Run();
            isCloseWindow = true;
        }

        return isCloseWindow;
    }

    /// <summary>
    /// Scrolls the view to the specified horizontal and vertical values.
    /// </summary>
    /// <param name="hscrollValue">
    /// The horizontal scroll value, or -1 to leave it unchanged.
    /// </param>
    /// <param name="vscrollValue">
    /// The vertical scroll value, or -1 to leave it unchanged.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if any scroll position changed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public bool ScrollView(double hscrollValue, double vscrollValue)
    {
        var res = false;
        if (hscrollValue > -1 && ScrolledView.HscrollbarPolicy != Gtk.PolicyType.Never && ScrolledView.HscrollbarPolicy != Gtk.PolicyType.External)
        {
            res = true;
            ScrolledView.Hadjustment.Value = hscrollValue;
        }
        if (vscrollValue > -1 && ScrolledView.VscrollbarPolicy != Gtk.PolicyType.Never && ScrolledView.VscrollbarPolicy != Gtk.PolicyType.External)
        {
            res = true;
            ScrolledView.Vadjustment.Value = vscrollValue;
        }

        return res;
    }

    private void Background_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        Override.OnPaint(cr);
    }

    private void Close_Clicked(object? sender, EventArgs e)
    {
        this.CloseWindow();
    }

    private void FormBase_Close(object sender, EventArgs e)
    {
        var result = MessageBox.Show(this, System.Properties.Resources.Gtk_FormBase_FormBase_Close, "Esc keyboard operation hints", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            this.Respond(ResponseType.DeleteEvent);
        }
    }

    private void FormBase_CloseRequest(object o, Gtk.CloseRequestArgs args)
    {
        // Gtk 4 CloseRequest: RetVal = true vetoes the close. Veto unless we are the ones closing it.
        args.RetVal = !isCloseWindow;
    }

    private void FormBase_Response(object o, ResponseArgs args)
    {
        if (args.ResponseId == ResponseType.DeleteEvent)
        {
            this.CloseWindow(true);
        }
    }

    private void Hadjustment_ValueChanged(object sender, EventArgs e)
    {
        if (Scroll != null)
        {
            var adj = (Gtk.Adjustment)sender;
            Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.HorizontalScroll));
        }
    }

    private void maximize_Clicked(object? sender, EventArgs e)
    {
        var maximize = sender as Gtk.Button;
        if (maximize == null)
            return;
        if (maximize.Name == "restore")
        {
            if ((this.Parent == null))
            {
                this.Unmaximize();
            }
            else
            {
                this.Halign = Align.Start;
                this.Valign = Align.Start;
                this.SetSizeRequest(prevSize.Width, prevSize.Height);
            }
            maximize.Name = "maximize";
        }
        else
        {
            if ((this.Parent == null))
            {
                this.Maximize();
            }
            else
            {
                prevSize.Width = this.AllocatedWidth;
                prevSize.Height = this.AllocatedHeight;
                this.Halign = Align.Fill;
                this.Valign = Align.Fill;
                this.SetSizeRequest(-1, -1);
            }
            maximize.Name = "restore";
        }
    }

    private void minimize_Clicked(object? sender, EventArgs e)
    {
        if ((this.Parent == null))
        {
            this.Minimize();
        }
        else
        {
            maximize!.IconName = "window-restore-symbolic";
            maximize.Name = "restore";
            prevSize.Width = this.AllocatedWidth;
            prevSize.Height = this.AllocatedHeight;
            this.Halign = Align.Start;
            this.Valign = Align.End;
            this.SetSizeRequest(100, 50);
        }
    }

    /// <summary>
    /// Raises the CloseWindowEvent event.
    /// </summary>
    private void OnCloseWindowEvent(EventArgs e)
    {
        CloseWindowEvent?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Scroll event.
    /// </summary>
    private void OnScroll(System.Windows.Forms.ScrollEventArgs e)
    {
        Scroll?.Invoke(this, e);
    }

    private void Vadjustment_ValueChanged(object sender, EventArgs e)
    {
        if (Scroll != null)
        {
            var adj = (Gtk.Adjustment)sender;
            Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.VerticalScroll));
        }
    }

    /// <summary>
    /// CloseWindowHandler delegate.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    /// <returns>
    /// </returns>
    public delegate bool CloseWindowHandler(object sender, EventArgs e);

}