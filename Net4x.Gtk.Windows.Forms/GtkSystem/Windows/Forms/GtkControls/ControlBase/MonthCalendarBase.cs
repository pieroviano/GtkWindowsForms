namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// MonthCalendarBase is a base class for MonthCalendar controls in the GtkSystem.Windows.Forms library. It provides a foundation for creating calendar-based UI elements within the framework. The MonthCalendarBase class inherits from Gtk.Calendar, which is a widget that displays a calendar and allows users to select dates. This class also implements the IControlGtk interface, which provides additional functionality for integrating with the GtkSystem.Windows.Forms framework. The MonthCalendarBase class can be extended to create more specific calendar controls that can be used in applications built with the GtkSystem.Windows.Forms library.
/// </summary>
/// <seealso cref="Gtk.Calendar" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class MonthCalendarBase : Gtk.Calendar, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.MonthCalendarBase" /> class.
    /// </summary>
    public MonthCalendarBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("MonthCalendar");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override { get; set; }

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        this.Override.AddClass(cssClass);
    }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">The Cairo context.
    /// </param>
    /// <returns>
    /// </returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
    }

    /// <summary>
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}