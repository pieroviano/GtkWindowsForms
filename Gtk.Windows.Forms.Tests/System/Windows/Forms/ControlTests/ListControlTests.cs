#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Windows.Forms.ControlTests;

[Collection("NonParallelCollection")]
public class ListControlTests
{
    [Fact]
    public void DataSourceChanged_Event_IsStoredAndInvoked()
    {
        var control = new TestListControl();
        var raised = false;

        EventHandler handler = (_, _) => raised = true;
        control.DataSourceChanged += handler;

        // Access internal EventHandlerList via reflection
        var eventsField = typeof(ListControl)
            .GetField("Events", BindingFlags.NonPublic | BindingFlags.Instance);

        var eventList = eventsField?.GetValue(control) as EventHandlerList;

        var key = "DataSourceChanged";
        var del = (EventHandler)eventList?[key];

        del?.Invoke(control, EventArgs.Empty);

        Assert.True(raised);
    }

    [Fact]
    public void DisplayMemberChanged_Event_IsRaised()
    {
        var control = new TestListControl();
        var raised = false;

        control.DisplayMemberChanged += (_, _) => raised = true;

        control.RaiseDisplayMemberChanged();

        Assert.True(raised);
    }

    [Fact]
    public void Format_Event_IsRaised()
    {
        var control = new TestListControl();
        var raised = false;

        control.Format += (_, _) => raised = true;

        var args = new ListControlConvertEventArgs("a", typeof(string), null);
        control.RaiseFormat(args);

        Assert.True(raised);
    }

    [Fact]
    public void GetItemText_ReturnsNull_ForNull()
    {
        var control = new TestListControl();

        var result = control.GetItemText(null);

        Assert.Null(result);
    }

    [Fact]
    public void GetItemText_ReturnsToString()
    {
        var control = new TestListControl();

        var result = control.GetItemText(123);

        Assert.Equal("123", result);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var control = new TestListControl();

        var dataSource = new object();

        control.DataSource = dataSource;
        control.DisplayMember = "Name";
        control.FormatInfo = CultureInfo.InvariantCulture;
        control.FormatString = "C";
        control.FormattingEnabled = true;
        control.SelectedIndex = 5;
        control.SelectedValue = 42;
        control.ValueMember = "Id";

        Assert.Equal(dataSource, control.DataSource);
        Assert.Equal("Name", control.DisplayMember);
        Assert.NotNull(control.FormatInfo);
        Assert.Equal("C", control.FormatString);
        Assert.True(control.FormattingEnabled);
        Assert.Equal(5, control.SelectedIndex);
        Assert.Equal(42, control.SelectedValue);
        Assert.Equal("Id", control.ValueMember);
    }

    [Fact]
    public void SelectedIndexChanged_Event_AddRemove_Works()
    {
        var control = new TestListControl();

        EventHandler handler = (_, _) => _ = true;

        control.SelectedIndexChanged += handler;
        control.SelectedIndexChanged -= handler;

        var eventsField = typeof(ListControl)
            .GetField("Events", BindingFlags.NonPublic | BindingFlags.Instance);

        var eventList = (EventHandlerList)eventsField.GetValue(control);

        var del = (EventHandler)eventList["SelectedIndexChanged"];

        Assert.Null(del); // should be removed
    }

    [Fact]
    public void ValueMemberChanged_Event_IsRaised()
    {
        var control = new TestListControl();
        var raised = false;

        control.ValueMemberChanged += (_, _) => raised = true;

        control.RaiseValueMemberChanged();

        Assert.True(raised);
    }

    public class TestListControl : ListControl
    {
        public override int SelectedIndex { get; set; }

        // Expose protected methods for testing
        public void RaiseDisplayMemberChanged() => OnDisplayMemberChanged(EventArgs.Empty);
        public void RaiseFormat(ListControlConvertEventArgs e) => OnFormat(e);
        public void RaiseFormatInfoChanged() => OnFormatInfoChanged(EventArgs.Empty);
        public void RaiseFormatStringChanged() => OnFormatStringChanged(EventArgs.Empty);
        public void RaiseFormattingEnabledChanged() => OnFormattingEnabledChanged(EventArgs.Empty);
        public void RaiseValueMemberChanged() => OnValueMemberChanged(EventArgs.Empty);
    }
}