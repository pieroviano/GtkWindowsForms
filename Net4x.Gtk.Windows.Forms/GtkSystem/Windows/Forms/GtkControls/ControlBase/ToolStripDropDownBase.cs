namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ToolStripDropDownBase class is a base class for GTK tool strip drop-down controls, providing common functionality and properties for derived classes. It inherits from Gtk.Menu and implements the IControlGtk interface. The class includes a constructor that initializes the tool strip drop-down and sets various properties such as alignment. It also provides an Override property of type GtkControlOverride, which allows for custom styling and behavior of the control. By using ToolStripDropDownBase, developers can easily create custom tool strip drop-down controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Menu" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ToolStripDropDownBase : Gtk.Menu, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ToolStripDropDownBase" /> class.
    /// </summary>
    public ToolStripDropDownBase() : base()
    {
        this.Override = new GtkControlOverride(this);
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
    public GtkControlOverride Override { get; set; }

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        this.Override.AddClass(cssClass);
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