namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// LabelBase is a base class for GTK label controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of label controls. The LabelBase class includes features such as text alignment, word wrapping, and custom drawing capabilities.
/// </summary>
/// <seealso cref="Gtk.Label" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class LabelBase : Gtk.Label, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase" /> class.
    /// </summary>
    public LabelBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.Wrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
        this.Selectable = true;
        // Gtk 4: Widget.Drawn is gone and there is no overridable OnSnapshot; custom paint moves to CSS (deferred).
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public LabelBase(string? text) : base(text)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.Wrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
        this.Selectable = true;
        this.Drawn += LabelBase_Drawn;
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
