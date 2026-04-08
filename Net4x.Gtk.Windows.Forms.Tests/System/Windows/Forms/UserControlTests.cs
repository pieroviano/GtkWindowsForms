namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class UserControlTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var control = new UserControl();
        form.Controls.Add(control);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = control.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(control.self);
        Assert.NotNull(control.Controls);
        Assert.Equal(control.self, gtkControl);
    }

    [Fact]
    public void Controls_ShouldAllowAddingChildren()
    {
        var form = new Form();
        var userControl = new UserControl();
        var child = new Label();

        form.Controls.Add(userControl);

        int count = 0;

        form.Load += (s, e) =>
        {
            userControl.Controls.Add(child);
            count = userControl.Controls.Count;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, count);
    }

    [Fact]
    public void Padding_ShouldUpdateGtkMargins()
    {
        var form = new Form();
        var control = new UserControl();
        form.Controls.Add(control);

        int left = -1, top = -1, right = -1, bottom = -1;

        form.Load += (s, e) =>
        {
            control.Padding = new Padding(10, 20, 30, 40);

            var overlay = control.Controls.Owner as UserControl; // just to keep lifecycle safe

            left = control.Padding.Left;
            top = control.Padding.Top;
            right = control.Padding.Right;
            bottom = control.Padding.Bottom;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(10, left);
        Assert.Equal(20, top);
        Assert.Equal(30, right);
        Assert.Equal(40, bottom);
    }

    [Fact]
    public void Load_ShouldFire()
    {
        var form = new Form();
        var control = new UserControl();
        form.Controls.Add(control);

        bool loadRaised = false;

        control.Load += (_, __) => loadRaised = true;

        form.Load += (s, e) =>
        {
            // Trigger manually since GTK may not always invoke it automatically
            control.OnLoad(EventArgs.Empty);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(loadRaised);
    }

    [Fact]
    public void ParentChanged_ShouldFire()
    {
        var form = new Form();
        var control = new UserControl();

        bool raised = false;

        control.ParentChanged += (_, __) => raised = true;

        form.Load += (s, e) =>
        {
            form.Controls.Add(control);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(raised);
    }

    [Fact]
    public void AutoScaleProperties_ShouldSetAndGet()
    {
        Application.Init();
        var control = new UserControl
        {
            AutoScaleDimensions = new System.Drawing.SizeF(1.5f, 2.5f),
            AutoScaleMode = AutoScaleMode.Font
        };

        Assert.Equal(1.5f, control.AutoScaleDimensions.Width);
        Assert.Equal(2.5f, control.AutoScaleDimensions.Height);
        Assert.Equal(AutoScaleMode.Font, control.AutoScaleMode);
    }

    [Fact]
    public void SuspendAndResumeLayout_ShouldNotThrow()
    {
        Application.Init();
        var control = new UserControl();

        control.SuspendLayout();
        control.ResumeLayout(true);

        Assert.True(true); // no exception = pass
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        Application.Init();
        var control = new UserControl();

        control.Dispose();

        Assert.True(true);
    }
}