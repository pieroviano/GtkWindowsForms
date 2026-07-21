using System.Windows.Forms;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class PanelTests
{
    [Fact]
    public void Constructor_ShouldInitializeSelfContainerAndGtkControl()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        object? gtkControl = null;
        object? container = null;

        form.Load += (s, e) =>
        {
            gtkControl = panel.GtkControl;
            container = panel.Container;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(panel.self);
        Assert.NotNull(container);
        Assert.NotNull(gtkControl);
        Assert.Equal(panel.self, gtkControl);
    }

    [Fact]
    public void Controls_ShouldReturnCustomCollection()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        Control.ControlCollection? controls = null;

        form.Load += (s, e) =>
        {
            controls = panel.Controls;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(controls);
    }

    [Fact]
    public void Controls_Add_ShouldAddChildControl()
    {
        var form = new Form();
        var panel = new Panel();
        var button = new Button();

        form.Controls.Add(panel);

        var count = 0;

        form.Load += (s, e) =>
        {
            panel.Controls.Add(button);
            count = panel.Controls.Count;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, count);
    }

    [Fact]
    public void Controls_Remove_ShouldRemoveChildControl()
    {
        var form = new Form();
        var panel = new Panel();
        var button = new Button();

        form.Controls.Add(panel);

        var count = 0;

        form.Load += (s, e) =>
        {
            panel.Controls.Add(button);
            panel.Controls.Remove(button);
            count = panel.Controls.Count;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(0, count);
    }

    [Fact]
    public void Padding_ShouldBeStoredInBaseProperty()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        var padding = new Padding(5, 6, 7, 8);
        Padding result = default;

        form.Load += (s, e) =>
        {
            panel.Padding = padding;
            result = panel.Padding;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(padding, result);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = panel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(panel.self, gtkControl);
    }
}