namespace System.Windows.Forms;

extern alias gtk;

using GtkSystem.Windows.Forms;

// System.Drawing.ContentAlignment lives in the Gtk.Drawing shim here, not in the real System.Drawing.
using ContentAlignment = gtk::System.Drawing.ContentAlignment;

/// <summary>
/// <see cref="ButtonBase.TextAlign" /> must move the caption.
/// </summary>
/// <remarks>
/// The Gtk4 port stubbed every <c>SetContentAlignmentCore</c> override out with the comment
/// "Gtk4-removed: self.Xalign", so <see cref="ButtonBase.TextAlign" /> stored a value and changed nothing.
/// The sample's Test1 form is the visible consequence: its side buttons are
/// <see cref="ContentAlignment.BottomCenter" /> with the icon supplied as a CSS background at
/// <c>center 30%</c>, so a centred caption lands on top of the icon instead of below it.
/// </remarks>
[Collection("NonParallelCollection")]
public class ButtonTextAlignTests
{
    // Constructing these controls builds GTK widgets, which needs the type system initialised.
    public ButtonTextAlignTests() => Application.Init();

    [Theory]
    [InlineData(ContentAlignment.TopLeft, 0.0f, 0.0f)]
    [InlineData(ContentAlignment.TopCenter, 0.5f, 0.0f)]
    [InlineData(ContentAlignment.TopRight, 1.0f, 0.0f)]
    [InlineData(ContentAlignment.MiddleLeft, 0.0f, 0.5f)]
    [InlineData(ContentAlignment.MiddleCenter, 0.5f, 0.5f)]
    [InlineData(ContentAlignment.MiddleRight, 1.0f, 0.5f)]
    [InlineData(ContentAlignment.BottomLeft, 0.0f, 1.0f)]
    [InlineData(ContentAlignment.BottomCenter, 0.5f, 1.0f)]
    [InlineData(ContentAlignment.BottomRight, 1.0f, 1.0f)]
    public void EveryTextAlign_ReachesTheCaption(ContentAlignment alignment, float x, float y)
    {
        var button = new Button { Text = "Stop" };

        button.TextAlign = alignment;

        var label = button.self.GetLabelWidget();
        Assert.NotNull(label);
        Assert.Equal(x, label!.Xalign);
        Assert.Equal(y, label.Yalign);
    }

    [Fact]
    public void TheCaption_FillsTheButton_SoTheAlignmentHasSomewhereToMove()
    {
        // Xalign/Yalign only move text within the space the label itself holds. A label sized to its own
        // text has no slack, so without Fill every alignment renders identically and the fix looks applied
        // while changing nothing on screen.
        var button = new Button { Text = "Stop", TextAlign = ContentAlignment.BottomCenter };

        var label = button.self.GetLabelWidget();

        Assert.NotNull(label);
        Assert.Equal(Gtk.Align.Fill, label!.Halign);
        Assert.Equal(Gtk.Align.Fill, label.Valign);
    }

    [Fact]
    public void ACheckBoxCaption_IsAlignedToo()
    {
        // A Gtk4 check button keeps its caption outside the child slot, so this only works through the
        // label lookup - the Gtk3 idiom "Child is Gtk.Label" is always false here.
        var checkBox = new CheckBox { Text = "Enabled", TextAlign = ContentAlignment.TopLeft };

        var label = checkBox.self.GetLabelWidget();

        Assert.NotNull(label);
        Assert.Equal(0.0f, label!.Xalign);
        Assert.Equal(0.0f, label.Yalign);
    }

    [Fact]
    public void ARadioButtonCaption_IsAlignedToo()
    {
        var radio = new RadioButton { Text = "Choice", TextAlign = ContentAlignment.BottomRight };

        var label = radio.self.GetLabelWidget();

        Assert.NotNull(label);
        Assert.Equal(1.0f, label!.Xalign);
        Assert.Equal(1.0f, label.Yalign);
    }

    [Fact]
    public void AligningAWidgetWithNoCaption_IsHarmless()
    {
        // GetLabelWidget returns null for a caption-less widget; the helper must not throw on it.
        var image = new Gtk.Image();

        var exception = Record.Exception(() => image.AlignCaption(0.5f, 1.0f));

        Assert.Null(exception);
    }
}
