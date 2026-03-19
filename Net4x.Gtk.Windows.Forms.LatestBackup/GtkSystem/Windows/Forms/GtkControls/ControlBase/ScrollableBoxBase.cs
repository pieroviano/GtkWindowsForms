using System;
using System.Windows.Forms;
using Gdk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ScrollableBoxBase class is a base class for GTK controls that require scrolling functionality. It inherits from Gtk.ScrolledWindow and implements the IControlGtk and IScrollableBoxBase interfaces. The class provides properties for enabling horizontal and vertical scrolling, as well as an AutoScroll property that automatically adjusts the scrollbar policies based on the content size. It also includes an event handler for scroll events, allowing developers to respond to user interactions with the scrollbars. By using ScrollableBoxBase, developers can easily create custom GTK controls that support scrolling while maintaining a consistent look and feel with other GTK controls.
/// </summary>
/// <seealso cref="Gtk.ScrolledWindow" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" />
public abstract class ScrollableBoxBase : Gtk.ScrolledWindow, IControlGtk, IScrollableBoxBase
{
    /// <summary>
    /// Occurs when scroll.
    /// </summary>
    public virtual event ScrollEventHandler Scroll;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" /> class.
    /// </summary>
    public ScrollableBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.ShadowType = Gtk.ShadowType.None;
        this.BorderWidth = 1;
        this.Events = Gdk.EventMask.AllEventsMask;
        base.Halign = Gtk.Align.Start;
        base.Valign = Gtk.Align.Start;
        base.Hexpand = false;
        base.Vexpand = false;
        base.VscrollbarPolicy = Gtk.PolicyType.External;
        base.HscrollbarPolicy = Gtk.PolicyType.External;
        base.OverlayScrolling = false;
        base.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
        base.Vadjustment.ValueChanged += Vadjustment_ValueChanged;
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
    public virtual bool AutoScroll
    {
        get => base.VscrollbarPolicy == Gtk.PolicyType.Automatic;
        set
        {
            if (value == true)
            {
                if (VScroll)
                    base.VscrollbarPolicy = Gtk.PolicyType.Automatic;
                if (HScroll)
                    base.HscrollbarPolicy = Gtk.PolicyType.Automatic;
            }
            else
            {
                base.VscrollbarPolicy = Gtk.PolicyType.External;
                base.HscrollbarPolicy = Gtk.PolicyType.External;
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
    public bool HScroll { get; set; } = true;

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="T:System.Windows.Forms.PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override { get; set; }

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
    public bool VScroll { get; set; } = true;

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
    /// Gets the draw rectangle.
    /// </summary>
    /// <returns>
    /// </returns>
    protected virtual Gdk.Rectangle GetDrawRectangle()
    {
        return new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
    }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <returns>
    /// </returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        var rec = GetDrawRectangle();
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
    }

    /// <summary>
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

    private void Hadjustment_ValueChanged(object sender, EventArgs e)
    {
        if (Scroll != null)
        {
            var adj = (Gtk.Adjustment)sender;
            Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.HorizontalScroll));
        }
    }

    private void Vadjustment_ValueChanged(object sender, EventArgs e)
    {
        if (Scroll != null)
        {
            var adj = (Gtk.Adjustment)sender;
            Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.VerticalScroll));
        }
    }

}