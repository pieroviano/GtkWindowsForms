using System;
using System.Windows.Forms;
using Gtk;

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
    /// The scroll view
    /// </summary>
    public readonly Gtk.ScrolledWindow ScrollView = new Gtk.ScrolledWindow();

    /// <summary>
    /// Occurs when the close window event occurs.
    /// </summary>
    public event CloseWindowHandler CloseWindowEvent;

    /// <summary>
    /// Occurs when a scroll event occurs..
    /// </summary>
    public event System.Windows.Forms.ScrollEventHandler Scroll;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.FormBase" /> class.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    public FormBase(Gtk.Window parent = null) : base("title", Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive), DialogFlags.UseHeaderBar)
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("Form");
        this.WindowPosition = Gtk.WindowPosition.Center;
        this.BorderWidth = 0;
        this.ContentArea.BorderWidth = 0;
        this.ContentArea.Spacing = 0;
        this.ContentArea.Homogeneous = false;

        this.SetDefaultSize(100, 100);
        this.TypeHint = Gdk.WindowTypeHint.Normal;
        this.AppPaintable = false;
        this.Deletable = true;
        this.Response += FormBase_Response;
        ScrollView.BorderWidth = 0;
        ScrollView.Valign = Gtk.Align.Fill;
        ScrollView.Halign = Gtk.Align.Fill;
        ScrollView.Hexpand = true;
        ScrollView.Vexpand = true;
        ScrollView.OverlayScrolling = true;
        ScrollView.KineticScrolling = true;
        ScrollView.HscrollbarPolicy = PolicyType.Automatic;
        ScrollView.VscrollbarPolicy = PolicyType.Automatic;
        ScrollView.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
        ScrollView.Vadjustment.ValueChanged += Vadjustment_ValueChanged;
        this.ContentArea.PackStart(ScrollView, true, true, 0);
        this.Drawn += FormBase_Drawn;
        this.Close += FormBase_Close;
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
        get => ScrollView.VscrollbarPolicy == Gtk.PolicyType.Automatic;
        set
        {
            if (value == true)
            {
                if (VScroll)
                    ScrollView.VscrollbarPolicy = Gtk.PolicyType.Automatic;
                if (HScroll)
                    ScrollView.HscrollbarPolicy = Gtk.PolicyType.Automatic;
            }
            else
            {
                ScrollView.VscrollbarPolicy = Gtk.PolicyType.External;
                ScrollView.HscrollbarPolicy = Gtk.PolicyType.External;
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
        ScrollView.Child = child;
    }

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        this.Override.AddClass(cssClass);
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void CloseWindow()
    {
        this.Respond(ResponseType.DeleteEvent);
    }

    private void FormBase_Close(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show(this, System.Properties.Resources.Gtk_FormBase_FormBase_Close, "Esc keyboard operation hints", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            this.Respond(ResponseType.DeleteEvent);
        }
    }

    private void FormBase_Drawn(object o, DrawnArgs args)
    {
        var rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnPaint(args.Cr, rec);
    }

    private void FormBase_Response(object o, ResponseArgs args)
    {
        if (args.ResponseId == ResponseType.DeleteEvent)
        {
            if (CloseWindowEvent(this, EventArgs.Empty))
            {
                this.OnClose();
                this.Group.CurrentGrab?.Destroy();
                this.Destroy();
            }
            else
                this.Run();
        }
    }

    private void Hadjustment_ValueChanged(object sender, EventArgs e)
    {
        if (Scroll != null)
        {
            var adj = (Gtk.Adjustment)sender;
            OnScroll(new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.HorizontalScroll));
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
            OnScroll(new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.VerticalScroll));
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