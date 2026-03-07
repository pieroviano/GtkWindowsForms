namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class CheckBoxBase : Gtk.CheckButton, IControlGtk
    {
        public CheckBoxBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("CheckBox");
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
