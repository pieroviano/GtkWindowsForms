namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// LinkLabelBase is a base class for GTK link label controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of link label controls.
/// </summary>
/// <seealso cref="Gtk.LinkButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class LinkLabelBase : Gtk.LinkButton, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LinkLabelBase" /> class.
    /// </summary>
    public LinkLabelBase() : base("")
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("LinkLabel");
        this.BorderWidth = 0;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Drawn += LinkLabelBase_Drawn;
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
    /// Handles the GTK Drawn signal and lets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> paint the control.
    /// </summary>
    /// <param name="o">
    /// The source of the event.
    /// </param>
    /// <param name="args">
    /// The <see cref="Gtk.DrawnArgs" /> instance containing the Cairo context.
    /// </param>
    private void LinkLabelBase_Drawn(object o, Gtk.DrawnArgs args)
    {
        Override.OnPaint(args.Cr);
    }
}