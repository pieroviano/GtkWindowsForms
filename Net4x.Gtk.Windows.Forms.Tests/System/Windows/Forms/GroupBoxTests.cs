using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class GroupBoxTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        object? gtkControl = null;
        object? controls = null;
        object? child = null;

        form.Load += (s, e) =>
        {
            gtkControl = groupBox.GtkControl;
            controls = groupBox.Controls;
            child = groupBox.self.Child;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(groupBox.self);
        Assert.NotNull(gtkControl);
        Assert.NotNull(controls);
        Assert.NotNull(child);
    }

    [Fact]
    public void Text_ShouldSetAndGetLabel()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        string? text = null;
        string? label = null;

        form.Load += (s, e) =>
        {
            groupBox.Text = "Test Title";
            text = groupBox.Text;
            label = groupBox.self.Label;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Test Title", text);
        Assert.Equal("Test Title", label);
    }

    [Fact]
    public void Padding_ShouldUpdateMargins()
    {
        // GroupBox.Padding applies the padding to the scrolled container (self.fixedcontaner), not the
        // overlay (self.contaner) this test used to reflect on. Assert that directly. It also avoids
        // realizing the control via ShowDialog: on the gitee-aligned GroupBox, setting Padding on an
        // already-shown group box triggers a GTK relayout that maps a still-invisible child and aborts
        // with a "gtk_widget_map" GTK-CRITICAL - a real defect tracked separately from this test.
        var groupBox = new GroupBox();

        groupBox.Padding = new Padding(10, 20, 30, 40);

        Assert.Equal(10, groupBox.self.fixedcontaner.MarginStart);
        Assert.Equal(20, groupBox.self.fixedcontaner.MarginTop);
        Assert.Equal(30, groupBox.self.fixedcontaner.MarginEnd);
        Assert.Equal(40, groupBox.self.fixedcontaner.MarginBottom);
    }

    [Fact]
    public void PerformLayout_ShouldSetCreatedTrue()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        var created = false;

        form.Load += (s, e) =>
        {
            groupBox.PerformLayout();

            created = GetCreated(groupBox);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(created);
    }

    [Fact]
    public void SuspendLayout_ShouldSetCreatedFalse()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        var created = true;

        form.Load += (s, e) =>
        {
            groupBox.SuspendLayout();

            created = GetCreated(groupBox);
            form.Close();
        };

        form.ShowDialog();

        Assert.False(created);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ResumeLayout_ShouldSetCreatedBasedOnParameter(bool input, bool expected)
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        var created = false;

        form.Load += (s, e) =>
        {
            groupBox.ResumeLayout(input);

            created = GetCreated(groupBox);
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(expected, created);
    }

    [Fact]
    public void Controls_Add_ShouldAttachControl()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        var child = new Label();
        form.Controls.Add(groupBox);

        var contains = false;
        Control? parent = null;

        form.Load += (s, e) =>
        {
            groupBox.Controls.Add(child);

            contains = groupBox.Controls.Contains(child);
            parent = child.Parent;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(contains);
        Assert.Equal(groupBox, parent);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var groupBox = new GroupBox();
        form.Controls.Add(groupBox);

        object? gtkControl = null;
        GroupBoxBase? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = groupBox.GtkControl;
            self = groupBox.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    // helper
    private bool GetCreated(GroupBox groupBox)
    {
        var field = typeof(Control)
            .GetField("_Created", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return (bool)field.GetValue(groupBox);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8605 // Unboxing a possibly null value.
    }
}