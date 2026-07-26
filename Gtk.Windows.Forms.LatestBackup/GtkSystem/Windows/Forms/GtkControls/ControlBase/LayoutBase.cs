using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// LayoutBase is a base class for GTK layout controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of layout controls.
/// </summary>
/// <seealso cref="Gtk.Layout" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class LayoutBase : Gtk.Layout, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LayoutBase" /> class.
    /// </summary>
    /// <param name="hadjustment">
    /// The hadjustment.
    /// </param>
    /// <param name="vadjustment">
    /// The vadjustment.
    /// </param>
    public LayoutBase(Adjustment hadjustment, Adjustment vadjustment) : base(hadjustment, vadjustment)
    {
        this.Override = new GtkControlOverride(this);
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
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
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <returns>
    /// </returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        var rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.DrawnBackColor(cr, rec);

        return base.OnDrawn(cr);
    }
}