namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ListViewBase is a base class for ListView controls in the GtkSystem.Windows.Forms library. It provides a scrollable container for list-based UI elements, allowing for easy integration of list views within the framework. The ListViewBase class inherits from Gtk.Viewport, which provides a scrollable area, and it initializes a Gtk.Box with vertical orientation to hold the list view items. This class serves as a foundation for creating more specific list view controls that can be used in applications built with the GtkSystem.Windows.Forms library.
/// </summary>
/// <seealso cref="Gtk.Viewport" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ListViewBase : Gtk.Viewport, IControlGtk
{

    /// <summary>
    /// The box
    /// </summary>
    public Gtk.Box box = new Gtk.Box(Gtk.Orientation.Vertical, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewBase"/> class.
    /// </summary>
    public ListViewBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("view");
        this.Override.AddClass("ListView");
        this.BorderWidth = 1;
        this.Add(box);
    }

    /// <summary>
    /// Gets or sets the <see cref="GtkControlOverride"/> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via <see cref="PaintGraphicsEventHandler"/> and System.Drawing via <see cref="PaintEventHandler"/>). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active <see cref="GtkControlOverride"/> used by the control.
    /// </value>
    public GtkControlOverride Override { get; set; }

}
