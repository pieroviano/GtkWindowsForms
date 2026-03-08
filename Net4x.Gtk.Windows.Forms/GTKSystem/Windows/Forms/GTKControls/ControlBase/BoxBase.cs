using Gtk;
using System;
using System.Drawing;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// BoxBase is a base class for GTK box controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of box controls.
    /// </summary>
    /// <seealso cref="Gtk.Box" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    public sealed class BoxBase : Gtk.Box, IControlGtk
    {
        public BoxBase(Orientation orientation, int spacing) : base(orientation, spacing)
        {
            this.Override = new GtkControlOverride(this);
            base.Valign = Gtk.Align.Start;
            base.Halign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.DrawnBackColor(cr, rec);

            return base.OnDrawn(cr);
        }

    }
}
