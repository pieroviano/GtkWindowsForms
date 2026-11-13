namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// FlowLayoutPanelBase is a base class for GTK flow layout panel controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of flow layout panel controls.
/// </summary>
/// <seealso cref="Gtk.FlowBox" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class FlowLayoutPanelBase : Gtk.FlowBox
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.FlowLayoutPanelBase" /> class.
    /// </summary>
    public FlowLayoutPanelBase() : base()
    {
        this.SelectionMode = Gtk.SelectionMode.None;
    }
}