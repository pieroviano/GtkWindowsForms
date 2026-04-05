extern alias gtk;
using System;
using System.Reflection;
using System.Globalization;
using Xunit;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class DateTimePickerTests
{
    [Fact]
    public void DateTimePicker_GtkControl_IsNotNull()
    {
        var dtp = new DateTimePicker();
        Assert.NotNull(dtp.GtkControl);
    }

    [Fact]
    public void Value_Set_Get_Formats_Text_BasedOn_Format()
    {
        var dtp = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short
        };

        var dt = new DateTime(2020, 1, 2, 3, 4, 5);
        dtp.Value = dt;

        // Short format uses "yyyy/MM/dd" in the implementation
        Assert.Equal("2020/01/02", dtp.Text);
        Assert.Equal(dt.Date, dtp.Value.Date);
    }

    [Fact]
    public void Value_Clamped_To_MaxDate_And_MinDate_Behavior_Matches_Implementation()
    {
        var form = new Form();
        var dtp = new DateTimePicker();
        form.Controls.Add(dtp);

        DateTime? maxResult = null;
        DateTime? minResult = null;

        // Ensure GTK widgets/models are realized by performing operations inside Form.Load
        form.Load += (s, e) =>
        {
            // MaxDate branch: setter uses MaxDate.AddDays(-1) when provided value > MaxDate
            dtp.MaxDate = new DateTime(2020, 1, 10);
            dtp.Value = new DateTime(2020, 2, 1);
            maxResult = dtp.Value;

            // MinDate branch: setter uses MinDate.AddDays(1) when provided value < MinDate
            dtp.MinDate = new DateTime(2019, 1, 10);
            dtp.Value = new DateTime(2010, 1, 1);
            minResult = dtp.Value;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(new DateTime(2020, 1, 10), maxResult);
        Assert.Equal(new DateTime(2019, 1, 10), minResult);
    }

    [Fact]
    public void CustomFormat_Updates_Mask()
    {
        var dtp = new DateTimePicker();

        var custom = "yyyy/MM/dd HH:mm:ss";
        dtp.CustomFormat = custom;

        // Mask is set on the base MaskedTextBox; read via reflection in case it's non-public
        var maskProp = typeof(DateTimePicker).BaseType!.GetProperty("Mask", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(maskProp);

        var mask = maskProp!.GetValue(dtp) as string;

        // Regex in implementation replaces letters [ymdhs] with underscores
        var expectedMask = System.Text.RegularExpressions.Regex.Replace(custom, "[ymdhs]", "_", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        Assert.Equal(expectedMask, mask);
    }

    [Fact]
    public void OnValueChanged_Raises_ValueChanged_Event()
    {
        var dtp = new DateTimePicker();
        var invoked = false;
        dtp.ValueChanged += (s, e) => invoked = true;

        var onValueChanged = typeof(DateTimePicker).GetMethod("OnValueChanged", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onValueChanged);

        onValueChanged!.Invoke(dtp, new object[] { EventArgs.Empty });

        Assert.True(invoked);
    }

    [Fact]
    public void Todaybtn_Clicked_Sets_Value_To_Now_And_Hides_Popover()
    {
        var form = new Form();
        var dtp = new DateTimePicker();
        form.Controls.Add(dtp);

        DateTime? valueAfter = null;
        bool popVisible = true;

        // Run the GTK-dependent interaction inside Form.Load and ShowDialog to ensure widgets are realized,
        // mirroring the pattern used by other tests that require deterministic GTK behavior.
        form.Load += (s, e) =>
        {
            dtp.Format = DateTimePickerFormat.Time;
            // set a deterministic previous value
            dtp.Value = new DateTime(2000, 1, 1);

            // get private Todaybtn_Clicked method
            var method = typeof(DateTimePicker).GetMethod("Todaybtn_Clicked", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);

            // call the method (sender can be null)
            method!.Invoke(dtp, new object?[] { null, EventArgs.Empty });
            valueAfter = dtp.Value;

            // Verify popver was hidden: access private field 'popver' and its Visible property
            var popField = typeof(DateTimePicker).GetField("popver", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(popField);
            var pop = popField!.GetValue(dtp);
            Assert.NotNull(pop);
            var visibleProp = pop!.GetType().GetProperty("Visible", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (visibleProp != null)
            {
                popVisible = (bool)visibleProp.GetValue(pop)!;
            }

            form.Close();
        };

        form.ShowDialog();

        Assert.True(valueAfter.HasValue);
        var diff = Math.Abs((valueAfter.Value - DateTime.Now).TotalSeconds);
        Assert.True(diff < 5, $"Value was not updated to now (delta seconds = {diff})");
        Assert.False(popVisible);
    }
}