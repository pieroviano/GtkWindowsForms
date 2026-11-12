namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// CheckBoxBase is a base class for GTK check button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of check button controls.
/// </summary>
/// <seealso cref="Gtk.CheckButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class CheckBoxBase : Gtk.CheckButton, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.CheckBoxBase" /> class.
    /// </summary>
    public CheckBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("CheckBox");
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

    /// <summary>
    /// Sets the button's content to an optional icon beside its caption, reproducing the
    /// <c>image</c> / <c>always-show-image</c> / <c>image-position</c> trio that Gtk 3's
    /// <see cref="T:Gtk.Button" /> offered and Gtk 4 removed.
    /// </summary>
    /// <remarks>
    /// A Gtk 4 check button has a single content slot: its caption <em>is</em> its
    /// <see cref="P:Gtk.CheckButton.Child" />, so assigning an image to that slot destroys the label and
    /// the caption disappears. An icon and a caption therefore have to share one child widget, which is
    /// what this method builds.
    /// </remarks>
    /// <param name="image">The icon to show, or <see langword="null" /> for a caption-only button.</param>
    /// <param name="text">The caption.</param>
    /// <param name="imagePosition">Where the icon sits relative to the caption.</param>
    /// <returns>
    /// The label that carries <paramref name="text" />, so callers can style it, or
    /// <see langword="null" /> if the button ended up with no caption widget at all.
    /// </returns>
    public Gtk.Label? SetImageAndLabel(Gtk.Image? image, string? text, Gtk.PositionType imagePosition)
    {
        if (image == null)
        {
            // No icon: the built-in label is the child, exactly as before.
            base.Label = text ?? string.Empty;
            return base.Child as Gtk.Label;
        }

        var vertical = imagePosition == Gtk.PositionType.Top || imagePosition == Gtk.PositionType.Bottom;
        var label = new Gtk.Label(text ?? string.Empty)
        {
            // An icon above or below the caption centres it, the way Gtk 3 laid out a top/bottom image
            // position; beside the caption the text stays left aligned.
            Halign = vertical ? Gtk.Align.Center : Gtk.Align.Start,
            Valign = Gtk.Align.Center,
            Xalign = vertical ? 0.5f : 0.0f,
            Yalign = 0.5f,
            Ellipsize = Pango.EllipsizeMode.End
        };

        var content = new Gtk.Box(vertical ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal, 4)
        {
            Halign = Gtk.Align.Start,
            Valign = Gtk.Align.Center
        };

        if (imagePosition == Gtk.PositionType.Right || imagePosition == Gtk.PositionType.Bottom)
        {
            content.Append(label);
            content.Append(image);
        }
        else
        {
            content.Append(image);
            content.Append(label);
        }

        base.Child = content;
        return label;
    }
}