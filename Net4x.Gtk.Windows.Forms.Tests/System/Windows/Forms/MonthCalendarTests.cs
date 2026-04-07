namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class MonthCalendarTests
{
    [Fact]
    public void Constructor_ShouldInitializeGtkControl()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = calendar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(calendar.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(calendar.self, gtkControl);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = calendar.GtkControl;
            self = calendar.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void TodayDate_ShouldMapToGtkDate()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        DateTime setDate = new DateTime(2024, 5, 10);
        DateTime result = default;

        form.Load += (s, e) =>
        {
            calendar.TodayDate = setDate;
            result = calendar.TodayDate;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(setDate, result);
    }

    [Fact]
    public void Properties_ShouldPersistValues()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        Day firstDay = Day.Monday;
        DateTime min = new DateTime(2020, 1, 1);
        DateTime max = new DateTime(2030, 1, 1);
        bool showToday = true;
        bool showCircle = true;
        bool showWeek = true;

        form.Load += (s, e) =>
        {
            calendar.FirstDayOfWeek = firstDay;
            calendar.MinDate = min;
            calendar.MaxDate = max;
            calendar.ShowToday = showToday;
            calendar.ShowTodayCircle = showCircle;
            calendar.ShowWeekNumbers = showWeek;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(firstDay, calendar.FirstDayOfWeek);
        Assert.Equal(min, calendar.MinDate);
        Assert.Equal(max, calendar.MaxDate);
        Assert.True(calendar.ShowToday);
        Assert.True(calendar.ShowTodayCircle);
        Assert.True(calendar.ShowWeekNumbers);
    }

    [Fact]
    public void SelectionRange_ShouldPersist()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        var range = new SelectionRange(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 10));

        SelectionRange? result = null;

        form.Load += (s, e) =>
        {
            calendar.SelectionRange = range;
            result = calendar.SelectionRange;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(result);
        Assert.Equal(range.Start, result.Start);
        Assert.Equal(range.End, result.End);
    }

    [Fact]
    public void DateChanged_Event_ShouldFire_WhenDaySelected()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        bool eventFired = false;

        form.Load += (s, e) =>
        {
            calendar.SelectionRange = new SelectionRange(DateTime.Today, DateTime.Today);

            calendar.DateChanged += (sender, args) =>
            {
                eventFired = true;
            };
        };

        form.Shown += (s, e) =>
        {
            // simulate GTK event
            calendar.self.OnDaySelected(e);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(eventFired);
    }

    [Fact]
    public void DateSelected_Event_ShouldFire_WhenDaySelected()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        bool eventFired = false;

        form.Load += (s, e) =>
        {
            calendar.SelectionRange = new SelectionRange(DateTime.Today, DateTime.Today);

            calendar.DateSelected += (sender, args) =>
            {
                eventFired = true;
            };
        };

        form.Shown += (s, e) =>
        {
            calendar.self.OnDaySelected(e);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(eventFired);
    }

    [Fact]
    public void DateEvents_ShouldNotFire_WhenNotVisible()
    {
        var form = new Form();
        var calendar = new MonthCalendar();
        form.Controls.Add(calendar);

        bool changedFired = false;
        bool selectedFired = false;

        form.Load += (s, e) =>
        {
            calendar.SelectionRange = new SelectionRange(DateTime.Today, DateTime.Today);

            calendar.DateChanged += (s2, e2) => changedFired = true;
            calendar.DateSelected += (s2, e2) => selectedFired = true;

            // Force invisible
            calendar.self.Visibility = false;
        };

        form.Shown += (s, e) =>
        {
            calendar.self.OnDaySelected(e);
            form.Close();
        };

        form.ShowDialog();

        Assert.False(changedFired);
        Assert.False(selectedFired);
    }
}