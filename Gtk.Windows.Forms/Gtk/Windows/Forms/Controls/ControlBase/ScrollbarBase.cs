using GLib;
using Gtk;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// ScrollbarBase class is a base class for GTK scrollbar controls, providing common functionality and properties for derived classes. It inherits from Gtk.Scrollbar and implements the IControlGtk interface. The class includes an internal constructor that initializes the scrollbar with a specified orientation and a default adjustment. It also provides a static GType property that returns the appropriate GType based on the generic type parameter T, which can be either Gtk.HScrollbar or Gtk.VScrollbar. By using ScrollbarBase, developers can easily create custom scrollbar controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Scrollbar" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class ScrollbarBase : Gtk.Scrollbar, IControlGtk
{
    internal ScrollbarBase(Orientation orientation) : base(orientation, new Gtk.Adjustment(0, 0, 100, 1, 10, 0))
    {
        this.Override = new GtkControlOverride(this);
    }

    /// <summary>
    /// Initializes static members of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.ScrollbarBase{T}" /> class. Forces the runtime to materialize the closed generic type members so the GType registration is resolved.
    /// </summary>
    static ScrollbarBase()
    {
        typeof(ScrollbarBase).GetMembers();
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
            return Gtk.Scrollbar.GType;
        }
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
}