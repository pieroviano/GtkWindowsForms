#pragma warning disable CS0612

#pragma warning disable CS0612

using System.Windows.Forms;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class NumericUpDownTests
{
    [Fact]
    public void Constructor_ShouldInitializeSelfAndGtkControl()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = control.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(control.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(control.self, gtkControl);
    }

    [Fact]
    public void DecimalPlaces_ShouldSetAndGetValue()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        var value = 0;

        form.Load += (s, e) =>
        {
            control.DecimalPlaces = 3;
            value = control.DecimalPlaces;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(3, value);
    }

    [Fact]
    public void Increment_ShouldSetAndGetValue()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        decimal value = 0;

        form.Load += (s, e) =>
        {
            control.Increment = 0.5m;
            value = control.Increment;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0.5m, value);
    }

    [Fact]
    public void MinimumAndMaximum_ShouldSetAndGetValues()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        decimal min = 0;
        decimal max = 0;

        form.Load += (s, e) =>
        {
            control.Minimum = 5m;
            control.Maximum = 100m;

            min = control.Minimum;
            max = control.Maximum;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(5m, min);
        Assert.Equal(100m, max);
    }

    [Fact]
    public void Value_ShouldSetAndGetValue()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        decimal value = 0;

        form.Load += (s, e) =>
        {
            control.Value = 42m;
            value = control.Value;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(42m, value);
    }

    [Fact]
    public void ValueChanged_ShouldRaiseEvent_WhenVisible()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        var raised = false;

        form.Load += (s, e) =>
        {
            control.ValueChanged += (sender, args) => raised = true;

            // Trigger GTK change
            control.self.Value += 1;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(raised);
    }

    [Fact]
    public void ValueChanged_ShouldNotRaiseEvent_WhenNotVisible()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        var raised = false;

        form.Load += (s, e) =>
        {
            control.ValueChanged += (sender, args) => raised = true;

            // Force hidden state
            control.self.Hide();

            control.self.Value += 1;

            form.Close();
        };

        form.ShowDialog();

        Assert.False(raised);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var control = new NumericUpDown();
        form.Controls.Add(control);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = control.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(control.self, gtkControl);
    }
}
#pragma warning restore CS0612