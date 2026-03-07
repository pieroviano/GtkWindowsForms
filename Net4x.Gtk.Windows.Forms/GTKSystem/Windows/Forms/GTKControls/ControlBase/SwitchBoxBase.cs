using GLib;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class SwitchBoxBase : Gtk.Switch, IControlGtk
    {
        public SwitchBoxBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("SwitchBox");
            base.Valign = Gtk.Align.Start;
            base.Halign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }
        protected override void OnShown()
        {
            Override.OnAddClass();
            base.OnShown();
        }
    }
}
