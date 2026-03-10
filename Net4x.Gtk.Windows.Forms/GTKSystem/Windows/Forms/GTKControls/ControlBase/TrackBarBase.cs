namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// TrackBarBase class is a base class for GTK track bar controls, providing common functionality and properties for derived classes. It inherits from Gtk.Viewport and implements the IControlGtk interface. The class includes a constructor that initializes the track bar and sets various properties such as alignment and CSS class. It also provides an Override property of type GtkControlOverride, which allows for custom styling and behavior of the control. By using TrackBarBase, developers can easily create custom track bar controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Viewport" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class TrackBarBase : Gtk.Viewport, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrackBarBase"/> class.
    /// </summary>
    public TrackBarBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("TrackBar");
        base.Halign = Gtk.Align.Start;
        base.Valign = Gtk.Align.Start;
    }

    /// <summary>
    /// Gets or sets the <see cref="GtkControlOverride"/> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via <see cref="PaintGraphicsEventHandler"/> and System.Drawing via <see cref="PaintEventHandler"/>). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active <see cref="GtkControlOverride"/> used by the control.
    /// </value>
    public GtkControlOverride Override { get; set; }

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">The CSS class.</param>
    public void AddClass(string cssClass)
    {
        this.Override.AddClass(cssClass);
    }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">The Cairo context.</param>
    /// <returns></returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnDrawnBackground(cr, rec);
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
