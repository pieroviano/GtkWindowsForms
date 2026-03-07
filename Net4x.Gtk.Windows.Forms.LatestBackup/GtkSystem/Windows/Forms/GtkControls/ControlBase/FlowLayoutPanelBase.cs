namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class FlowLayoutPanelBase : Gtk.FlowBox, IControlGtk
    {
        public FlowLayoutPanelBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("FlowLayoutPanel");
            base.Valign = Gtk.Align.Start;
            base.Halign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }
        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnPaint(cr, rec);

            return base.OnDrawn(cr);
        }

        protected override void OnShown()
        {
            Override.OnAddClass();
            base.OnShown();
        }
    }
}
