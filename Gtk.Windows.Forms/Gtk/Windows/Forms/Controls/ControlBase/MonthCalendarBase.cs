using System.Windows.Forms;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// MonthCalendarBase is a base class for MonthCalendar controls in the Gtk.Windows.Forms library. It provides a foundation for creating calendar-based UI elements within the framework. The MonthCalendarBase class inherits from Gtk.Calendar, which is a widget that displays a calendar and allows users to select dates. This class also implements the IControlGtk interface, which provides additional functionality for integrating with the Gtk.Windows.Forms framework. The MonthCalendarBase class can be extended to create more specific calendar controls that can be used in applications built with the Gtk.Windows.Forms library.
/// </summary>
/// <seealso cref="Gtk.Calendar" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class MonthCalendarBase : Gtk.Calendar, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.MonthCalendarBase" /> class.
    /// </summary>
    public MonthCalendarBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("MonthCalendar");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
        base.DaySelected += MonthCalendarBase_DaySelected;
    }

    private void MonthCalendarBase_DaySelected(object sender, EventArgs e)
    {
        OnDaySelected(e);
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="MonthCalendarBase"/> is visibility.
    /// </summary>
    /// <value>
    ///   <c>true</c> if visibility; otherwise, <c>false</c>.
    /// </value>
    public bool Visibility
    {
        get => field;
        set
        {
            field = value;
            if (value)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    public new event EventHandler DaySelected;

    internal void OnDaySelected(EventArgs e)
    {
        DaySelected?.Invoke(this, e);
    }
}