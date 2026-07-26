using GLib;
using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ScrollbarBase class is a base class for GTK scrollbar controls, providing common functionality and properties for derived classes. It inherits from Gtk.Scrollbar and implements the IControlGtk interface. The class includes an internal constructor that initializes the scrollbar with a specified orientation and a default adjustment. It also provides a static GType property that returns the appropriate GType based on the generic type parameter T, which can be either Gtk.HScrollbar or Gtk.VScrollbar. By using ScrollbarBase, developers can easily create custom scrollbar controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
/// <seealso cref="Gtk.Scrollbar" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ScrollbarBase<T> : Gtk.Scrollbar, IControlGtk
{
    internal ScrollbarBase(Orientation orientation) : base(orientation, new Gtk.Adjustment(0, 0, 100, 1, 10, 0))
    {
        this.Override = new GtkControlOverride(this);
    }

    /// <summary>
    /// Gets the type of the g.
    /// </summary>
    /// <value>
    /// The type of the g.
    /// </value>
    public new static GType GType
    {
        get
        {
            return (typeof(T) == typeof(Gtk.HScrollbar)) ? Gtk.HScrollbar.GType : Gtk.VScrollbar.GType;
        }
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