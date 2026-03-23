/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows control that allows the user to select a date and a time and to display the date and time with a specified format.
/// </summary>
[DesignerCategory("Component")]
public partial class DateTimePicker : MaskedTextBox
{
    /// <summary>
    /// The format
    /// </summary>
    public DateTimePickerFormat _Format;

    readonly Gtk.Calendar calendar = new Gtk.Calendar();

    readonly Gtk.Popover popver;

    private string _CustomFormat= System.Properties.Resources.Gtk_DateTimePicker_Value_yyyy_MM_dd;

    /// <summary>
    /// ValueChanged event is raised when the Value property is changed, either by user action or programmatically.
    /// </summary>
    public virtual event EventHandler ValueChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DateTimePicker" /> class.
    /// </summary>
    public DateTimePicker() : base("DateTimePicker")
    {
        base.Mask = System.Properties.Resources.Gtk_DateTimePicker_DateTimePicker_Mask;

        self.SecondaryIconActivatable = true;
        self.SecondaryIconStock = "open-menu";
        self.SecondaryIconPixbuf = new Gdk.Pixbuf(this.GetType().Assembly, "Resources.System.MonthCalendar.ico");
        self.IconRelease += DateTimePicker_IconRelease;
        self.Shown += Self_Shown;

        popver = new Gtk.Popover(self)
        {
            BorderWidth = 5,
            Modal = true,
            Position = PositionType.Bottom
        };

        calendar.Halign = Gtk.Align.Fill;
        calendar.Valign = Gtk.Align.Fill;
        calendar.DaySelected += Calendar_DaySelected;
        calendar.PrevYear += Calendar_PrevYear;
        calendar.NextYear += Calendar_NextYear;
        calendar.PrevMonth += Calendar_PrevMonth;
        calendar.NextMonth += Calendar_NextMonth;

        var popbody= new Gtk.Box(Gtk.Orientation.Vertical, 6)
        {
            calendar
        };
        var todaybtn = new Gtk.Button() { Label = System.Properties.Resources.Gtk_DateTimePicker_DateTimePicker_Choose_Today+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
        todaybtn.Clicked += Todaybtn_Clicked;
        popbody.Add(todaybtn);
        popver.Add(popbody);
        self.Changed += Self_Changed;
    }

    /// <summary>
    /// Gets or sets the calendar font.
    /// </summary>
    /// <value>
    /// The calendar font.
    /// </value>
    public Font CalendarFont
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the calendar fore.
    /// </summary>
    /// <value>
    /// The color of the calendar fore.
    /// </value>
    public Color CalendarForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the calendar month background.
    /// </summary>
    /// <value>
    /// The calendar month background.
    /// </value>
    public Color CalendarMonthBackground
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the calendar title back.
    /// </summary>
    /// <value>
    /// The color of the calendar title back.
    /// </value>
    public Color CalendarTitleBackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the calendar title fore.
    /// </summary>
    /// <value>
    /// The color of the calendar title fore.
    /// </value>
    public Color CalendarTitleForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the calendar trailing fore.
    /// </summary>
    /// <value>
    /// The color of the calendar trailing fore.
    /// </value>
    public Color CalendarTrailingForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the custom format.
    /// </summary>
    /// <value>
    /// The custom format.
    /// </value>
    public string CustomFormat
    {
        get => _CustomFormat;
        set
        {
            _CustomFormat = value;
            base.Mask = Regex.Replace(value, "[ymdhs]", "_", RegexOptions.IgnoreCase);
        }
    }

    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    /// <value>
    /// The format.
    /// </value>
    public DateTimePickerFormat Format
    {
        get => _Format;
        set
        {
            _Format = value;
            if (Format == DateTimePickerFormat.Long)
                base.Mask = System.Properties.Resources.Gtk_DateTimePicker_DateTimePicker_Mask;
            else if (Format == DateTimePickerFormat.Short)
                base.Mask = "____/__/__";
            else if (Format == DateTimePickerFormat.Time)
                base.Mask = "__:__:__";
            else if (Format == DateTimePickerFormat.Custom)
                base.Mask = Regex.Replace(CustomFormat, "[ymdhs]", "_", RegexOptions.IgnoreCase);
            else
                base.Mask = System.Properties.Resources.Gtk_DateTimePicker_DateTimePicker_Mask;
        }
    }

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
    } = DateTime.MaxValue;

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
    } = DateTime.MinValue;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public DateTime Value
    {
        get
        {
            var result = DateTime.Now;
            var provider = CultureInfo.InvariantCulture;
            if (string.IsNullOrWhiteSpace(Text))
            { }
            else if (Format == DateTimePickerFormat.Custom && DateTime.TryParseExact(Text, CustomFormat, provider, Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
            { }
            else if (DateTime.TryParse(Text, provider, Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
            { }
            else
            {
                result = DateTime.Now;
            }
            if (result > MaxDate)
            {
                return MaxDate;
            }
            if (result < MinDate)
            {
                return MinDate;
            }

            return result;
        }
        set
        {
            if (value > MaxDate)
            {
                value = MaxDate.AddDays(-1);
            }
            if (value < MinDate)
            {
                value = MinDate.AddDays(1);
            }
            if (Format == DateTimePickerFormat.Long)
                base.Text = value.ToString(System.Properties.Resources.Gtk_DateTimePicker_Value_yyyy_MM_dd);
            else if (Format == DateTimePickerFormat.Short)
                base.Text = value.ToString("yyyy/MM/dd");
            else if (Format == DateTimePickerFormat.Time)
                base.Text = value.ToString("HH:mm:ss");
            else if (Format == DateTimePickerFormat.Custom)
                base.Text = value.ToString(CustomFormat);
            else
                base.Text = value.ToString(System.Properties.Resources.Gtk_DateTimePicker_Value_yyyy_MM_dd);
        }
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    protected virtual void OnValueChanged(System.EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void Calendar_DaySelected(object sender, EventArgs e)
    {
        var calendar = sender as Gtk.Calendar;
        DateTime dt = calendar.GetDate();
        if (dt > MaxDate || dt < MinDate)
        {
            calendar.Date = Value;
            MessageBox.Show($" the selected date is out of the allowed range \n maximum time：{MaxDate.ToString("yyyy/MM/dd HH:mm:ss")}\n minimum time：{MinDate.ToString("yyyy/MM/dd HH:mm:ss")}", System.Properties.Resources.Gtk_DateTimePicker_Calendar_DaySelected_Date_restrictions);
        }
        else
        {
            var current = Value;
            this.Value = dt.AddHours(current.Hour).AddMinutes(current.Minute).AddSeconds(current.Second);
        }
    }

    private void Calendar_NextMonth(object sender, EventArgs e)
    {
        if (calendar.GetDate() > MaxDate)
        {
            calendar.Date = MaxDate.Date.AddDays(-1);
        }
    }

    private void Calendar_NextYear(object sender, EventArgs e)
    {
        if (calendar.GetDate() > MaxDate)
        {
            calendar.Date = MaxDate.Date.AddDays(-1);
        }
    }

    private void Calendar_PrevMonth(object sender, EventArgs e)
    {
        if (calendar.GetDate() < MinDate)
        {
            calendar.Date = MinDate.Date.AddDays(1);
        }
    }

    private void Calendar_PrevYear(object sender, EventArgs e)
    {
        if (calendar.GetDate() < MinDate)
        {
            calendar.Date = MinDate.Date.AddDays(1);
        }
    }

    private void DateTimePicker_IconRelease(object o, Gtk.IconReleaseArgs args)
    {
        var current = Value;
        if (current >= MaxDate)
        {
            calendar.Date = MaxDate.Date.AddDays(-1);
        }
        else if (current <= MinDate)
        {
            calendar.Date = MinDate.AddDays(1);
        }
        else
        {
            calendar.Date = Value;
        }
        popver.ShowAll();
    }

    private void Self_Changed(object sender, EventArgs e)
    {
        if (ValueChanged != null && self.IsMapped)
            OnValueChanged(e);
    }

    private void Self_Shown(object sender, EventArgs e)
    {
        this.Value = DateTime.Now;
    }

    private void Todaybtn_Clicked(object sender, EventArgs e)
    {
        this.Value = DateTime.Now;
        popver.Hide();
    }
}