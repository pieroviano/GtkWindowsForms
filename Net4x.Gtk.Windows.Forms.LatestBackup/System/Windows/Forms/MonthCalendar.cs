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

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MonthCalendar" /> class.
        /// </summary>
        public MonthCalendar() : base()
        {
            self.DaySelected += MonthCalendar_DaySelected;
        }

        /// <summary>
        /// Gets or sets the first day of the week as displayed in the month calendar.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Day" /> values. The default is
        /// <see cref="System.Windows.Forms.Day.Default" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.Day" /> enumeration members.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Day FirstDayOfWeek
        {
            get; set;
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the maximum allowable date.
        /// </summary>
        /// <returns>A
        /// <see cref="System.DateTime" /> representing the maximum allowable date. The default is 12/31/9998.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value is less than the
        /// <see cref="System.Windows.Forms.MonthCalendar.MinDate" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DateTime MaxDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the minimum allowable date.
        /// </summary>
        /// <returns>A
        /// <see cref="System.DateTime" /> representing the minimum allowable date. The default is 01/01/1753.
        /// </returns>
        /// <exception cref="System.ArgumentException">The date set is greater than the
        /// <see cref="System.Windows.Forms.MonthCalendar.MaxDate" />-or-The date set is earlier than 01/01/1753.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DateTime MinDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the selected range of dates for a month calendar control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.SelectionRange" /> with the start and end dates of the selected range.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The
        /// <see cref="System.Windows.Forms.SelectionRange.Start" /> value of the assigned
        /// <see cref="System.Windows.Forms.SelectionRange" /> is less than the minimum date allowable for a month calendar control.-or- The
        /// <see cref="System.Windows.Forms.SelectionRange.Start" /> value of the assigned
        /// <see cref="System.Windows.Forms.SelectionRange" /> is greater than the maximum allowable date for a month calendar control.-or- The
        /// <see cref="System.Windows.Forms.SelectionRange.End" /> value of the assigned
        /// <see cref="System.Windows.Forms.SelectionRange" /> is less than the minimum date allowable for a month calendar control.-or- The
        /// <see cref="System.Windows.Forms.SelectionRange.End" /> value of the assigned
        /// <see cref="System.Windows.Forms.SelectionRange" /> is greater than the maximum allowable date for a month calendar control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public SelectionRange SelectionRange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the date represented by the
        /// <see cref="System.Windows.Forms.MonthCalendar.TodayDate" /> property is displayed at the bottom of the control.
        /// </summary>
        /// <returns>true if today's date is displayed; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowToday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether today's date is identified with a circle or a square.
        /// </summary>
        /// <returns>true if today's date is identified with a circle or a square; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowTodayCircle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the month calendar control displays week numbers (1-52) to the left of each row of days.
        /// </summary>
        /// <returns>true if the week numbers are displayed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowWeekNumbers { get; set; }

        /// <summary>
        /// Gets or sets the value that is used by
        /// <see cref="System.Windows.Forms.MonthCalendar" /> as today's date.
        /// </summary>
        /// <returns>A
        /// <see cref="System.DateTime" /> representing today's date. The default value is the current system date.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than the minimum allowable date.-or- The value is greater than the maximum allowable date.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
