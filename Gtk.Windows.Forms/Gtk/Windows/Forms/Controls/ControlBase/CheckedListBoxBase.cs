namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// CheckedListBoxBase is a base class for GTK checked list box controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of checked list box controls.
/// </summary>
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.ScrollableBoxBase" />
public sealed class CheckedListBoxBase : ScrollableBoxBase
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.CheckedListBoxBase" /> class.
    /// </summary>
    public CheckedListBoxBase() : base()
    {
        this.StyleContext.AddClass("CheckedListBox");
    }
}