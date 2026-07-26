
namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// RichTextBoxBase class is a base class for GTK rich text box controls, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase, allowing it to support scrolling functionality for the rich text content. The class includes a Gtk.TextView control, which is used to display and edit rich text content. By using RichTextBoxBase, developers can easily create rich text box controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class RichTextBoxBase : ScrollableBoxBase
{

    internal Gtk.TextView TextView = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.RichTextBoxBase" /> class.
    /// </summary>
    public RichTextBoxBase() : base()
    {
        this.StyleContext.AddClass("RichTextBox");
        this.TextView.BorderWidth = 1;
        this.TextView.Margin = 0;
        this.TextView.WrapMode = Gtk.WrapMode.Char;
        this.TextView.Halign = Gtk.Align.Fill;
        this.TextView.Valign = Gtk.Align.Fill;
        this.TextView.Hexpand = true;
        this.TextView.Vexpand = true;
        this.AutoScroll = true;
        this.Add(TextView);
    }
}
