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
            container = panel.contaner;
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

        int count = 0;

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

        int count = 0;

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
    public void Padding_ShouldSetContainerMargins()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        Padding padding = new Padding(10, 20, 30, 40);

        int left = 0, top = 0, right = 0, bottom = 0;

        form.Load += (s, e) =>
        {
            panel.Padding = padding;

            left = panel.contaner.MarginStart;
            top = panel.contaner.MarginTop;
            right = panel.contaner.MarginEnd;
            bottom = panel.contaner.MarginBottom;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(10, left);
        Assert.Equal(20, top);
        Assert.Equal(30, right);
        Assert.Equal(40, bottom);
    }

    [Fact]
    public void Padding_ShouldBeStoredInBaseProperty()
    {
        var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        Padding padding = new Padding(5, 6, 7, 8);
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