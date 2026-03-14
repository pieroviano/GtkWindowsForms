using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ListBoxBase is a base class for ListBox controls in the GtkSystem.Windows.Forms library. It provides a scrollable container for a Gtk.ListBox, allowing for easy integration of list-based UI elements within the framework. The ListBoxBase class inherits from ScrollableBoxBase, which provides common functionality for scrollable containers, and it initializes a Gtk.ListBox with appropriate properties to ensure it expands and fills the available space within the container. This class serves as a foundation for creating more specific list box controls that can be used in applications built with the GtkSystem.Windows.Forms library.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class ListBoxBase : ScrollableBoxBase
{
    /// <summary>
    /// The ListBox
    /// </summary>
    public Gtk.ListBox ListBox = new Gtk.ListBox();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ListBoxBase" /> class.
    /// </summary>
    public ListBoxBase() : base()
    {
        this.Override.AddClass("ListBox");
        ListBox.BorderWidth = 1;
        ListBox.Margin = 0;
        ListBox.Hexpand = true;
        ListBox.Vexpand = true;
        base.AutoScroll = true;
        base.Add(ListBox);
    }

}