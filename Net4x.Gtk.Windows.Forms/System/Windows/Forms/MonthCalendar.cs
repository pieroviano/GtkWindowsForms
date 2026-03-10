/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows control that enables the user to select a date using a visual monthly calendar display.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class MonthCalendar : Control
    {

        public readonly MonthCalendarBase self = new MonthCalendarBase();

        public event DateRangeEventHandler DateChanged;

        public event DateRangeEventHandler DateSelected;

        public MonthCalendar() : base()
        {
            self.DaySelected += MonthCalendar_DaySelected;
        }


        public Day FirstDayOfWeek
        {
            get; set;
        }

        public override object GtkControl => self;

        public DateTime MaxDate
        {
            get; set;
        }

        public DateTime MinDate
        {
            get; set;
        }

        public SelectionRange SelectionRange { get; set; }

        public bool ShowToday { get; set; }

        public bool ShowTodayCircle { get; set; }

        public bool ShowWeekNumbers { get; set; }

        public DateTime TodayDate
        {
            get { return self.Date; }
            set { self.Date = value; }
        }

        private void MonthCalendar_DaySelected(object sender, EventArgs e)
        {
            if (DateChanged != null && self.IsVisible)
                DateChanged(this, new DateRangeEventArgs(SelectionRange.Start, SelectionRange.End));

            if (DateSelected != null && self.IsVisible)
                DateSelected(this, new DateRangeEventArgs(SelectionRange.Start, SelectionRange.End));
        }

    }
}
