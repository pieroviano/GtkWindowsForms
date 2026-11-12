namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// SplitContainerBase class is a base class for GTK split container controls, providing common functionality and properties for derived classes. It inherits from Gtk.Paned and implements the IControlGtk interface. The class includes a constructor that initializes the split container with a vertical orientation and sets various properties such as border width, handle visibility, and alignment. It also provides an Override property of type GtkControlOverride, which allows for custom styling and behavior of the control. By using SplitContainerBase, developers can easily create custom split container controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Paned" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class SplitContainerBase : Gtk.Paned, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.SplitContainerBase" /> class.
    /// </summary>
    public SplitContainerBase() : base(Gtk.Orientation.Vertical)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("SplitContainer");
        this.WideHandle = true;
        this.Orientation = Gtk.Orientation.Horizontal;
        base.Halign = Gtk.Align.Start;
        base.Valign = Gtk.Align.Start;
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
}