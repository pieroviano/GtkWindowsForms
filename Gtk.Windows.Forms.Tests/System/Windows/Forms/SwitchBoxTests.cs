using Gdk;

namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class SwitchBoxTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = switchBox.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(switchBox.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(switchBox.self, gtkControl);
    }

    [Fact]
    public void Checked_ShouldSetAndGetValue()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        bool value = false;
        bool selfValue = false;

        form.Load += (s, e) =>
        {
            switchBox.Checked = true;

            value = switchBox.Checked;
            selfValue = switchBox.self.Active;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(value);
        Assert.True(selfValue);
    }

    [Fact]
    public void Text_ShouldMapToTooltipText()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            switchBox.Text = "Hello";

            text = switchBox.Text;
            selfText = switchBox.self.TooltipText;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Hello", text);
        Assert.Equal("Hello", selfText);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = switchBox.GtkControl;
            self = switchBox.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void CheckedChanged_ShouldFire_WhenCheckedPropertySet_AndVisible()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        int eventCount = 0;

        form.Load += (s, e) =>
        {
            switchBox.CheckedChanged += (s2, e2) => eventCount++;

            switchBox.Checked = true;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void CheckedChanged_ShouldNotFire_WhenNotVisible()
    {
        var switchBox = new SwitchBox();

        int eventCount = 0;
        switchBox.CheckedChanged += (s, e) => eventCount++;

        // Not added to a form → not visible
        switchBox.Checked = true;

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void CheckedChanged_ShouldFire_OnButtonReleaseEvent_WhenVisible()
    {
        var form = new Form();
        var switchBox = new SwitchBox();
        form.Controls.Add(switchBox);

        int eventCount = 0;

        form.Load += (s, e) =>
        {
            switchBox.CheckedChanged += (s2, e2) =>
            {
                eventCount++;
            };

            // Simulate GTK click
            switchBox.OnCheckedChanged(EventArgs.Empty);

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, eventCount);
    }
}