namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ComboBoxBase is a base class for GTK combo box controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of combo box controls.
/// </summary>
/// <seealso cref="Gtk.ComboBoxText" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ComboBoxBase : Gtk.ComboBoxText, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ComboBoxBase" /> class.
    /// </summary>
    public ComboBoxBase() : base(true)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("ComboBox");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ComboBoxBase" /> class.
    /// </summary>
    /// <param name="hasEntry">
    /// if set to
    /// <c>
    /// true
    /// </c>has entry.
    /// </param>
    public ComboBoxBase(bool hasEntry) : base(hasEntry)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("ComboBox");
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
}