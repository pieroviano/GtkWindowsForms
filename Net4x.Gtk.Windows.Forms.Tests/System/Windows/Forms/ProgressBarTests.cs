using System.Windows.Forms;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class ProgressBarTests
{
    [Fact]
    public void Constructor_ShouldInitializeSelfAndGtkControl()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = progressBar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(progressBar.GtkControl, gtkControl);
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        int min = 0, max = 0, speed = 0;

        form.Load += (s, e) =>
        {
            min = progressBar.Minimum;
            max = progressBar.Maximum;
            speed = progressBar.MarqueeAnimationSpeed;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0, min);
        Assert.Equal(100, max);
        Assert.Equal(100, speed);
    }

    [Fact]
    public void MinimumMaximum_ShouldSetAndGetValues()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        int min = 0, max = 0;

        form.Load += (s, e) =>
        {
            progressBar.Minimum = 10;
            progressBar.Maximum = 200;

            min = progressBar.Minimum;
            max = progressBar.Maximum;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(10, min);
        Assert.Equal(200, max);
    }

    [Fact]
    public void Value_ShouldSetAndGetValue()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        var value = 0;

        form.Load += (s, e) =>
        {
            progressBar.Value = 50;
            value = progressBar.Value;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(50, value);
    }

    [Fact]
    public void Step_ShouldSetAndGetValue()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        var step = 0;

        form.Load += (s, e) =>
        {
            progressBar.Step = 5;
            step = progressBar.Step;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(5, step);
    }

    [Fact]
    public void Style_ShouldSetAndGetValue()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        ProgressBarStyle style = default;

        form.Load += (s, e) =>
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            style = progressBar.Style;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(ProgressBarStyle.Marquee, style);
    }

    [Fact]
    public void Padding_ShouldSetAndGetValue()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        var padding = new Padding(1, 2, 3, 4);
        Padding result = default;

        form.Load += (s, e) =>
        {
            progressBar.Padding = padding;
            result = progressBar.Padding;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(padding, result);
    }

    [Fact]
    public void Control_Realized_ShouldApplyMinimumMaximumAndValue()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        int min = 0, max = 0, value = 0;

        form.Load += (s, e) =>
        {
            progressBar.Minimum = 5;
            progressBar.Maximum = 150;
            progressBar.Value = 50;

            // Force realization logic via reflection
            progressBar.GetType()
                .GetMethod("Control_Realized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(progressBar, new object[] { progressBar, EventArgs.Empty });

            var self = (GtkSystem.Windows.Forms.GtkControls.ControlBase.ProgressBarBase)progressBar.GtkControl;

            min = (int)self.MinValue;
            max = (int)self.MaxValue;
            value = (int)self.Value;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(5, min);
        Assert.Equal(150, max);
        Assert.Equal(50, value);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var progressBar = new ProgressBar();
        form.Controls.Add(progressBar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = progressBar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(progressBar.GtkControl, gtkControl);
    }
}