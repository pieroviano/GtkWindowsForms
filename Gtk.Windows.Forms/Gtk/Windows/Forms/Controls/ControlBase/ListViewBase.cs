namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// ListViewBase is a base class for ListView controls in the Gtk.Windows.Forms library. It provides a scrollable container for list-based UI elements, allowing for easy integration of list views within the framework. The ListViewBase class inherits from Gtk.Viewport, which provides a scrollable area, and it initializes a Gtk.Box with vertical orientation to hold the list view items. This class serves as a foundation for creating more specific list view controls that can be used in applications built with the Gtk.Windows.Forms library.
/// </summary>
/// <seealso cref="Gtk.Viewport" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class ListViewBase : Gtk.Viewport, IControlGtk
{
    /// <summary>
    /// The box
    /// </summary>
    public Gtk.Box box = new(Gtk.Orientation.Vertical, 0);

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.ListViewBase" /> class.
    /// </summary>
    public ListViewBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("view");
        this.StyleContext.AddClass("ListView");
        this.AddChild(box);
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }
}