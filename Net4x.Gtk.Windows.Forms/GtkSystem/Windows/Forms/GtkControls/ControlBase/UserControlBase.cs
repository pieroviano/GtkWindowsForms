using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// UserControlBase class is a base class for GTK user control components, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase, allowing it to support scrolling functionality for the content within the user control. The class includes a constructor that initializes the user control and sets various properties such as margin, border width, shadow type, and expansion. By using UserControlBase, developers can easily create custom user controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class UserControlBase : ScrollableBoxBase
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.UserControlBase" /> class.
    /// </summary>
    public UserControlBase() : base()
    {
        this.Override.AddClass("UserControl");
        this.MarginStart = 0;
        this.MarginTop = 0;
        this.BorderWidth = 0;
        this.ShadowType = ShadowType.None;
        this.Events = Gdk.EventMask.AllEventsMask;
        this.Expand = false;
        this.Hexpand = false;
        this.Vexpand = false;
    }

}