namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// FlowLayoutPanelBase is a base class for GTK flow layout panel controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of flow layout panel controls.
/// </summary>
/// <seealso cref="Gtk.FlowBox" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class FlowLayoutPanelBase : Gtk.FlowBox, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlowLayoutPanelBase"/> class.
    /// </summary>
    public FlowLayoutPanelBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("FlowLayoutPanel");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    /// <summary>
    /// Gets or sets the <see cref="GtkControlOverride"/> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via <see cref="PaintGraphicsEventHandler"/> and System.Drawing via <see cref="PaintEventHandler"/>). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active <see cref="GtkControlOverride"/> used by the control.
    /// </value>
    public GtkControlOverride Override { get; set; }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">The Cairo context.</param>
    /// <returns></returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
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

}
