#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace System.Windows.Forms.Controls;

public class ControlTests
{
    [Fact]
    public void ActivateControl_SetsActiveControl_OnContainer()
    {
        var container = new ContainerControl();
        var label = new Label();

        container.Controls.Add(label);

        // ActivateControl stores the control even if the GTK activation outcome may vary.
        var ok = container.ActivateControl(label);

        Assert.Equal(label, container.ActiveControl);
    }

    [Fact]
    public void AddWidget_SetsParent_OnProvidedControl()
    {
        var container = new ContainerControl();
        var label = new Label();

        // AddWidget registers the GTK widget and sets the Control.Parent to the owner.
        container.Controls.AddWidget(label.Widget, label);

        Assert.Equal(container, label.Parent);
    }

    [Fact]
    public void Controls_Add_Works_And_Item_IsAccessibleByIndex()
    {
        var container = new ContainerControl();
        var label = new Label();

        container.Controls.Add(label);

        // Controls is a non-generic collection; check Contains and indexer
        Assert.True(container.Controls.Contains(label));
        Assert.Equal(label, container.Controls[0]);
    }

    [Fact]
    public void Controls_Clear_RemovesAll()
    {
        var container = new ContainerControl();
        var a = new Label();
        var b = new Label();

        container.Controls.Add(a);
        container.Controls.Add(b);

        Assert.Equal(2, container.Controls.Count);

        container.Controls.Clear();

#pragma warning disable xUnit2013
        Assert.Equal(0, container.Controls.Count);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Controls_Find_ByName_ReturnsControl()
    {
        var container = new ContainerControl();
        var label = new Label();
        label.Name = "findme";

        container.Controls.Add(label);

        var results = container.Controls.Find("findme", false);

        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(label, results[0]);
    }

    // --- Additional coverage added below ---

    [Fact]
    public void Controls_Remove_RemovesItem()
    {
        var container = new ContainerControl();
        var label = new Label();

        container.Controls?.Add(label);
        Assert.True(container.Controls.Contains(label));

        container.Controls.Remove(label);

        Assert.False(container.Controls.Contains(label));
    }

    [Fact]
    public void Focus_Sets_WidgetFocus_And_FocusedPropertyReflectsIt()
    {
        Gtk.Application.Init();
        var textBox = new TextBox();
        var form = new Form();
        form.Controls?.Add(textBox);
        var result = false;
        var focused = false;
        form.Load += (s, e) =>
        {
            result = textBox.Focus();
            focused = textBox.Focused;
            form.Close();
        };
        form.ShowDialog();

        // Focus() should return true for a Control that exposes a widget,
        // and the Focused property reads widget.IsFocus.
        Assert.True(result);
        Assert.True(focused);
    }

    [Fact]
    public void Label_GtkControl_And_Widget_AreNotNull()
    {
        var label = new Label();

        Assert.NotNull(label.GtkControl);
        Assert.NotNull(label.Widget);
    }

    [Fact]
    public void Label_Text_And_Name_SetGet_Work()
    {
        var label = new Label();

        label.Text = "hello";
        label.Name = "lbl1";

        Assert.Equal("hello", label.Text);
        Assert.Equal("lbl1", label.Name);
    }

    [Fact]
    public void Padding_SetGet_Works()
    {
        var label = new Label();

        var padding = new Padding(1, 2, 3, 4);
        label.Padding = padding;

        Assert.Equal(padding, label.Padding);
    }

    [Fact]
    public void Visible_SetGet_Works()
    {
        var label = new Label();

        // default visible may be false; ensure we can set and read back
        label.Visible = true;
        Assert.True(label.Visible);

        label.Visible = false;
        Assert.False(label.Visible);
    }
}