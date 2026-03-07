namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class TrackBarBase : Gtk.Viewport, IControlGtk
    {
        public TrackBarBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("TrackBar");
            base.Halign = Gtk.Align.Start;
            base.Valign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
        }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnDrawnBackground(cr, rec);
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
