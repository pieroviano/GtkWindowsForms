using Gtk;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// UserControlBase class is a base class for GTK user control components, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase, allowing it to support scrolling functionality for the content within the user control. The class includes a constructor that initializes the user control and sets various properties such as margin, border width, shadow type, and expansion. By using UserControlBase, developers can easily create custom user controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.ScrollableBoxBase" />
public sealed class UserControlBase : ScrollableBoxBase
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.UserControlBase" /> class.
    /// </summary>
    public UserControlBase() : base()
    {
        this.Override.AddClass("UserControl");
        this.MarginStart = 0;
        this.MarginTop = 0;
        this.Hexpand = false;
        this.Vexpand = false;
        this.Hexpand = false;
        this.Vexpand = false;
    }
}