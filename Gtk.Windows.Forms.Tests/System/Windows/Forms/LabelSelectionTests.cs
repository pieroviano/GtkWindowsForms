namespace System.Windows.Forms;

/// <summary>
/// A WinForms <see cref="Label" /> never takes focus and its text is never selectable.
/// </summary>
/// <remarks>
/// The port inherited <c>Selectable = true</c> from the Gtk3 build, where it was only a mouse affordance.
/// Gtk4 makes a selectable label focusable, the window's initial focus can land on one, and
/// <c>gtk_label_grab_focus</c> honours <c>gtk-label-select-on-focus</c> by selecting the whole caption - so
/// the sample's status label rendered with an opaque selection box behind its text. The Gtk3 reference shows
/// the same artefact on another label, in the focused selection colour, which is what identified it as a
/// text selection rather than a lost <see cref="Control.BackColor" />.
/// </remarks>
[Collection("NonParallelCollection")]
public class LabelSelectionTests
{
    // Constructing a Label builds a GTK widget, which needs the type system initialised.
    public LabelSelectionTests() => Application.Init();

    [Fact]
    public void ALabel_IsNeitherSelectableNorFocusable()
    {
        var label = new Label { Text = "abc" };

        Assert.False(label.self.Selectable,
            "a WinForms Label has no text selection; a selectable Gtk4 label shows one on focus");
        Assert.False(label.self.Focusable,
            "a WinForms Label has ControlStyles.Selectable clear and TabStop false, so it never takes focus");
    }

    [Fact]
    public void ALabelsText_CannotBeSelected()
    {
        var label = new Label { Text = "abc" };

        label.self.SelectRegion(0, 3);

        Assert.False(label.self.GetSelectionBounds(out _, out _),
            "nothing may select a Label's text - the selection is what the theme paints as a solid box");
    }

    [Fact]
    public void ALabelConstructedWithItsText_IsAlsoNotSelectable()
    {
        // The two LabelBase constructors configure the widget independently, so both have to be covered.
        var label = new Gtk.Windows.Forms.Controls.ControlBase.LabelBase("abc");

        Assert.False(label.Selectable);
        Assert.False(label.Focusable);
    }
}
