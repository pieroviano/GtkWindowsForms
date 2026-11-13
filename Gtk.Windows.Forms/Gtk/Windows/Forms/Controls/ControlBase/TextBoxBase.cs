namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// TextBoxBase class is a base class for GTK text box controls, providing common functionality and properties for derived classes. It inherits from Gtk.Entry and implements the IControlGtk interface. The class includes a constructor that initializes the text box and sets various properties such as alignment and CSS class. It also provides an Override property of type GtkControlOverride, which allows for custom styling and behavior of the control. By using TextBoxBase, developers can easily create custom text box controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Entry" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class TextBoxBase : Gtk.Entry, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.TextBoxBase" /> class.
    /// </summary>
    public TextBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("TextBox");
        base.Halign = Gtk.Align.Start;
        base.Valign = Gtk.Align.Start;
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

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        this.StyleContext.AddClass(cssClass);
    }
}