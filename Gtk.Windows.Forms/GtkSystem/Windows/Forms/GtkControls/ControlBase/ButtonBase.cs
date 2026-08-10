using Gdk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ButtonBase is a base class for GTK button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of button controls.
/// </summary>
/// <seealso cref="Gtk.Button" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ButtonBase : Gtk.Button, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ButtonBase" /> class.
    /// </summary>
    public ButtonBase() : base(new Gtk.Label() { MaxWidthChars = 0, Wrap = false, Ellipsize = Pango.EllipsizeMode.End })
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Button");
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        // Gtk4-removed: this.Events |= EventMask.PointerMotionMask;
        // Gtk4-removed: this.Events |= EventMask.ButtonMotionMask;
        // Gtk 4: Widget.Drawn is gone and there is no overridable OnSnapshot; custom paint moves to CSS (deferred).
    }

    /// <summary>
    /// Gets or sets the dialog result that is assigned to the owning form when this button is released.
    /// </summary>
    /// <value>
    /// The <see cref="!:DialogResult" /> produced by this button.
    /// </value>
    public System.Windows.Forms.DialogResult DialogResult
    {
        get;
        set;
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
    /// Gets or sets the text displayed by the button's child label.
    /// </summary>
    /// <value>
    /// The button caption.
    /// </value>
    public string Text
    {
        get => ((Gtk.Label)Child).Text;
        set => ((Gtk.Label)Child).Text = value;
    }
}