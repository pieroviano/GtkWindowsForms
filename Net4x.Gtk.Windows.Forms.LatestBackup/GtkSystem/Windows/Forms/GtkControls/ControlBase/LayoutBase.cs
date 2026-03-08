using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// LayoutBase is a base class for GTK layout controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of layout controls.
    /// </summary>
    /// <seealso cref="Gtk.Layout" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    public sealed class LayoutBase : Gtk.Layout, IControlGtk
    {
        public LayoutBase(Adjustment hadjustment, Adjustment vadjustment) : base(hadjustment, vadjustment)
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
