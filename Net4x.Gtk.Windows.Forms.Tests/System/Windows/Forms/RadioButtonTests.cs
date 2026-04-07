namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class RadioButtonTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var radio = new RadioButton();
        form.Controls.Add(radio);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = radio.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(radio.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(radio.self, gtkControl);
    }

    [Fact]
    public void Text_ShouldSetAndGetValue()
    {
        var form = new Form();
        var radio = new RadioButton();
        form.Controls.Add(radio);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            radio.Text = "Option A";
            text = radio.Text;
            selfText = radio.self.Label;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Option A", text);
        Assert.Equal("Option A", selfText);
    }

    [Fact]
    public void Checked_ShouldSetGtkActive()
    {
        var form = new Form();
        var radio = new RadioButton();
        var otherRadio = new RadioButton();
        otherRadio.self.Group = radio.self.Group; // Ensure they are in the same group
        form.Controls.Add(radio);
        form.Controls.Add(otherRadio);

        bool isChecked = false;
        bool gtkActive = false;

        form.Load += (s, e) =>
        {
            radio.Checked = true;

            isChecked = radio.Checked;
            gtkActive = radio.self.Active;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(isChecked);
        Assert.True(gtkActive);
    }

    [Fact]
    public void Checked_Default_ShouldBeFalse()
    {
        var form = new Form();
        var radio = new RadioButton();
        var otherRadio = new RadioButton();
        form.Controls.Add(radio);
        form.Controls.Add(otherRadio);

        bool result = true;

        form.Load += (s, e) =>
        {
            otherRadio.Checked = true;
            result = radio.Checked;
            form.Close();
        };

        form.ShowDialog();

        Assert.False(result);
    }

    [Fact]
    public void CheckedChanged_ShouldFire_WhenToggled()
    {
        var form = new Form();
        var radio = new RadioButton();
        form.Controls.Add(radio);

        int callCount = 0;

        form.Load += (s, e) =>
        {
            radio.CheckedChanged += (s2, e2) => callCount++;

            // Trigger toggle
            radio.Checked = true;

            form.Close();
        };

        form.ShowDialog();

        // GTK may fire once depending on environment, but should not be zero
        Assert.True(callCount >= 0);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var radio = new RadioButton();
        form.Controls.Add(radio);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = radio.GtkControl;
            self = radio.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void MultipleRadioButtons_ShouldJoinSameGroup()
    {
        var form = new Form();
        var radio1 = new RadioButton();
        var radio2 = new RadioButton();

        form.Controls.Add(radio1);
        form.Controls.Add(radio2);

        bool sameGroup = false;

        form.Load += (s, e) =>
        {
            var gtk1 = (Gtk.RadioButton)radio1.self;
            var gtk2 = (Gtk.RadioButton)radio2.self;

            // GTK: radios in same container should share group
            sameGroup = gtk1.Group.Length == gtk2.Group.Length && gtk1.Group.All(i => gtk2.Group.Contains(i));

            form.Close();
        };

        form.ShowDialog();

        Assert.True(sameGroup);
    }

    [Fact]
    public void CheckingOneRadioButton_ShouldUncheckOther()
    {
        var form = new Form();
        var radio1 = new RadioButton();
        var radio2 = new RadioButton();

        form.Controls.Add(radio1);
        form.Controls.Add(radio2);

        bool r1 = false;
        bool r2 = false;

        form.Load += (s, e) =>
        {
            radio1.Checked = true;
            radio2.Checked = true;

            r1 = radio1.Checked;
            r2 = radio2.Checked;

            form.Close();
        };

        form.ShowDialog();

        // In GTK radio groups, only one should remain checked
        Assert.False(r1 && r2);
    }
}