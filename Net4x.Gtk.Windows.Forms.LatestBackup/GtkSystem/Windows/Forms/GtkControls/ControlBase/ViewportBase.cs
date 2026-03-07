namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class ViewportBase : Gtk.Viewport, IControlGtk
    {
        public ViewportBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            base.Halign = Gtk.Align.Start;
            base.Valign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnDrawnBackground(cr, rec);

            return base.OnDrawn(cr);
        }

    }
}
