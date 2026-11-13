/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows control that enables the user to select a date using a visual monthly calendar display.
/// </summary>
[DesignerCategory("Component")]
public partial class MonthCalendar : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly MonthCalendarBase self = new();

    /// <summary>
    /// Occurs when date changed.
    /// </summary>
    public virtual event DateRangeEventHandler? DateChanged;

    /// <summary>
    /// Occurs when date selected.
    /// </summary>
    public virtual event DateRangeEventHandler? DateSelected;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MonthCalendar" /> class.
    /// </summary>
    public MonthCalendar() : base()
    {
        self.Override.sender = this;
        self.DaySelected += MonthCalendar_DaySelected;
    }

    /// <summary>
    /// Gets or sets the first day of week.
    /// </summary>
    /// <value>
    /// The first day of week.
    /// </value>
    public Day FirstDayOfWeek
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the maximum date.
    /// </summary>
    /// <value>
    /// The maximum date.
    /// </value>
    public DateTime MaxDate
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum date.
    /// </summary>
    /// <value>
    /// The minimum date.
    /// </value>
    public DateTime MinDate
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>
    /// The selection range.
    /// </value>
    public SelectionRange? SelectionRange
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show today.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show today; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowToday
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show today circle.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show today circle; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowTodayCircle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show week numbers.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show week numbers; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowWeekNumbers
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the today date.
    /// </summary>
    /// <value>
    /// The today date.
    /// </value>
    public DateTime TodayDate
    {
        get { return self.Date.ToDateTime(); }
        set { self.Date = value.ToGLibDateTime(); }
    }

    /// <summary>
    /// Raises the DateChanged event.
    /// </summary>
    protected virtual void OnDateChanged(System.Windows.Forms.DateRangeEventArgs e)
    {
        DateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DateSelected event.
    /// </summary>
    protected virtual void OnDateSelected(System.Windows.Forms.DateRangeEventArgs e)
    {
        DateSelected?.Invoke(this, e);
    }

    private void MonthCalendar_DaySelected(object sender, EventArgs e)
    {
        if (DateChanged != null && self.IsVisible)
            OnDateChanged(new DateRangeEventArgs(SelectionRange?.Start ?? default, SelectionRange?.End ?? default));

        if (DateSelected != null && self.IsVisible)
            OnDateSelected(new DateRangeEventArgs(SelectionRange?.Start ?? default, SelectionRange?.End ?? default));
    }
}