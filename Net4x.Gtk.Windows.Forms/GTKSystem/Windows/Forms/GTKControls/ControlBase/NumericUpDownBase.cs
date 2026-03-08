namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// NumericUpDownBase is a base class for NumericUpDown controls in the GtkSystem.Windows.Forms library. It provides a foundation for creating numeric input controls that allow users to select a value from a range by clicking up and down buttons. The NumericUpDownBase class inherits from Gtk.SpinButton, which is a widget that provides this functionality, and it implements the IControlGtk interface for integration with the GtkSystem.Windows.Forms framework. This class can be extended to create more specific numeric up-down controls that can be used in applications built with the GtkSystem.Windows.Forms library.
    /// </summary>
    /// <seealso cref="Gtk.SpinButton" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    public sealed class NumericUpDownBase : Gtk.SpinButton, IControlGtk
    {
        public NumericUpDownBase() : base(0, 100, 1)
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("NumericUpDown");
            this.Value = 0;
            this.Orientation = Gtk.Orientation.Horizontal;
            base.Valign = Gtk.Align.Start;
            base.Halign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
        }

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
