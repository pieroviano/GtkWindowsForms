namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// MonthCalendarBase is a base class for MonthCalendar controls in the GtkSystem.Windows.Forms library. It provides a foundation for creating calendar-based UI elements within the framework. The MonthCalendarBase class inherits from Gtk.Calendar, which is a widget that displays a calendar and allows users to select dates. This class also implements the IControlGtk interface, which provides additional functionality for integrating with the GtkSystem.Windows.Forms framework. The MonthCalendarBase class can be extended to create more specific calendar controls that can be used in applications built with the GtkSystem.Windows.Forms library.
    /// </summary>
    /// <seealso cref="Gtk.Calendar" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    public sealed class MonthCalendarBase : Gtk.Calendar, IControlGtk
    {
        public MonthCalendarBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("MonthCalendar");
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
