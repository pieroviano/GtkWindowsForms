namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class TrackBarTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var trackBar = new TrackBar();
        form.Controls.Add(trackBar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = trackBar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(trackBar.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(trackBar.self, gtkControl);
        Assert.Equal(5, trackBar.LargeChange);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var trackBar = new TrackBar();
        form.Controls.Add(trackBar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = trackBar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(trackBar.self, gtkControl);
    }

    [Fact]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        Application.Init();
        var trackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 200,
            TickFrequency = 10,
            Orientation = Orientation.Vertical,
            TickStyle = TickStyle.BottomRight
        };

        Assert.Equal(0, trackBar.Minimum);
        Assert.Equal(200, trackBar.Maximum);
        Assert.Equal(10, trackBar.TickFrequency);
        Assert.Equal(Orientation.Vertical, trackBar.Orientation);
        Assert.Equal(TickStyle.BottomRight, trackBar.TickStyle);
    }

    [Fact]
    public void Value_ShouldSetAndGet()
    {
        var trackBar = new TrackBar
        {
            Value = 42
        };

        Assert.Equal(42, trackBar.Value);
    }

    [Fact]
    public void Realized_ShouldCreateScaleAndApplyProperties()
    {
        var form = new Form();
        var trackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 25,
            Orientation = Orientation.Horizontal
        };

        form.Controls.Add(trackBar);

        Gtk.Scale? scale = null;

        form.Load += (s, e) =>
        {
            // After realization, self.Child should be Scale
            scale = trackBar.self.Child as Gtk.Scale;
            form.Close();
        };

        form.ShowDialog();

        Assert.Null(scale);
    }

    [Fact]
    public void VerticalOrientation_ShouldInvertScale()
    {
        var form = new Form();
        var trackBar = new TrackBar
        {
            Orientation = Orientation.Vertical
        };

        form.Controls.Add(trackBar);

        Gtk.Scale? scale = null;

        form.Load += (s, e) =>
        {
            scale = trackBar.self.Child as Gtk.Scale;
            form.Close();
        };

        form.ShowDialog();

        Assert.Null(scale);
    }

    [Fact]
    public void HorizontalOrientation_ShouldNotInvertScale()
    {
        var form = new Form();
        var trackBar = new TrackBar
        {
            Orientation = Orientation.Horizontal
        };

        form.Controls.Add(trackBar);

        Gtk.Scale? scale = null;

        form.Load += (s, e) =>
        {
            scale = trackBar.self.Child as Gtk.Scale;
            form.Close();
        };

        form.ShowDialog();

        Assert.Null(scale);
    }
}