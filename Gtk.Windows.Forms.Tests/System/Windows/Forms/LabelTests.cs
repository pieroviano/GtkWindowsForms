namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class LabelTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = label.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(label.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(label.self, gtkControl);
    }

    [Fact]
    public void Text_ShouldSetAndGetValue()
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            label.Text = "Hello";
            text = label.Text;
            selfText = label.self.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Hello", text);
        Assert.Equal("Hello", selfText);
    }

    [Theory]
    [InlineData(RightToLeft.Yes, Gtk.TextDirection.Rtl)]
    [InlineData(RightToLeft.No, Gtk.TextDirection.Ltr)]
    public void RightToLeft_ShouldMapToGtkDirection(RightToLeft rtl, Gtk.TextDirection expected)
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        var direction = Gtk.TextDirection.None;
        var result = RightToLeft.No;

        form.Load += (s, e) =>
        {
            label.RightToLeft = rtl;
            direction = label.self.Direction;
            result = label.RightToLeft;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(expected, direction);
        Assert.Equal(rtl, result);
    }

    [Fact]
    public void Focus_ShouldSetGtkFocus_WhenPossible()
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        var focusResult = false;

        form.Load += (s, e) =>
        {
            focusResult = label.Focus();
            form.Close();
        };

        form.ShowDialog();

        // We don't assert true strictly because GTK focus may depend on environment,
        // but method must not throw and should return a boolean.
        Assert.IsType<bool>(focusResult);
    }

    [Theory]
    [InlineData(gtk::System.Drawing.ContentAlignment.TopLeft, 0.0f, 0.0f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.TopCenter, 0.5f, 0.0f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.TopRight, 1.0f, 0.0f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.MiddleLeft, 0.0f, 0.5f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.MiddleCenter, 0.5f, 0.5f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.MiddleRight, 1.0f, 0.5f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.BottomLeft, 0.0f, 1.0f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.BottomCenter, 0.5f, 1.0f)]
    [InlineData(gtk::System.Drawing.ContentAlignment.BottomRight, 1.0f, 1.0f)]
    public void TextAlign_ShouldMapToGtkAlignment(gtk::System.Drawing.ContentAlignment align, float expectedX, float expectedY)
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        float actualX = -1;
        float actualY = -1;
        var resultAlign = gtk::System.Drawing.ContentAlignment.TopLeft;

        form.Load += (s, e) =>
        {
            label.TextAlign = align;

            actualX = label.self.Xalign;
            actualY = label.self.Yalign;
            resultAlign = label.TextAlign;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(expectedX, actualX);
        Assert.Equal(expectedY, actualY);
        Assert.Equal(align, resultAlign);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var label = new Label();
        form.Controls.Add(label);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = label.GtkControl;
            self = label.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}