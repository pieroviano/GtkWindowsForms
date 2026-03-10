namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// CheckBoxBase is a base class for GTK check button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of check button controls.
/// </summary>
/// <seealso cref="Gtk.CheckButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class CheckBoxBase : Gtk.CheckButton, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckBoxBase"/> class.
    /// </summary>
    public CheckBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("CheckBox");
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
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}
