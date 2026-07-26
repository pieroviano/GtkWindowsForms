namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class MenuStripTests
{
    [Fact]
    public void Constructor_ShouldInitializeGtkControl()
    {
        var form = new Form();
        var menu = new MenuStrip();
        form.Controls.Add(menu);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = menu.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(menu.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(menu.self, gtkControl);
    }

    [Fact]
    public void Constructor_ShouldSetDockTop()
    {
        var form = new Form();
        var menu = new MenuStrip();
        form.Controls.Add(menu);

        var dock = DockStyle.None;

        form.Load += (s, e) =>
        {
            dock = menu.Dock;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(DockStyle.Top, dock);
    }

    [Fact]
    public void Constructor_ShouldAddMenuStripCssClass()
    {
        var form = new Form();
        var menu = new MenuStrip();
        form.Controls.Add(menu);

        var hasClass = false;

        form.Load += (s, e) =>
        {
            // GTK StyleContext check
            var context = menu.self.StyleContext;
            hasClass = context.HasClass("MenuStrip");
            form.Close();
        };

        form.ShowDialog();

        Assert.True(hasClass);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var menu = new MenuStrip();
        form.Controls.Add(menu);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = menu.GtkControl;
            self = menu.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void ShouldInheritFromToolStrip()
    {
        var form = new Form();
        var menu = new MenuStrip();
        form.Controls.Add(menu);

        var isToolStrip = false;

        form.Load += (s, e) =>
        {
            isToolStrip = menu is ToolStrip;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(isToolStrip);
    }
}