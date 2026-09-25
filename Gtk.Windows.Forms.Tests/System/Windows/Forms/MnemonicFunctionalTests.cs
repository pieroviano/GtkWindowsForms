using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// A WinForms caption marks its mnemonic with <c>&amp;</c>; GTK marks it with <c>_</c>. Nothing converted between
/// them, so a button whose Text was "&amp;Run" showed the ampersand instead of underlining the R. These tests pin
/// the conversion and, just as importantly, that the caption reads back as the application wrote it.
/// </summary>
[Collection("NonParallelCollection")]
public class MnemonicFunctionalTests
{
    [Fact]
    public void ButtonText_ReadsBackAsItWasWritten()
    {
        using var button = new Button { Text = "&Run" };

        Assert.Equal("&Run", button.Text);
    }

    [Fact]
    public void ButtonCaption_UnderlinesTheMnemonicInsteadOfShowingTheAmpersand()
    {
        using var button = new Button { Text = "&Run" };

        var label = (global::Gtk.Label)button.self.Child;
        // With the mnemonic on, GTK reports the displayed caption: the marker consumed, no ampersand in sight.
        Assert.Equal("Run", label.Text);
        Assert.True(label.UseUnderline);
        Assert.Equal('R', (char)label.MnemonicKeyval); // Alt+R activates it, as it did in WinForms
    }

    [Fact]
    public void ButtonCaption_KeepsADoubledAmpersandAsOne()
    {
        using var button = new Button { Text = "Save && Close" };

        Assert.Equal("Save & Close", ((global::Gtk.Label)button.self.Child).Text);
        Assert.Equal("Save && Close", button.Text);
    }

    [Fact]
    public void ButtonCaption_EscapesAnUnderscoreTheCaptionMeantLiterally()
    {
        using var button = new Button { Text = "A_B" };

        // The underscore was the caption's own, so it survives as one instead of marking B
        Assert.Equal("A_B", ((global::Gtk.Label)button.self.Child).Text);
        Assert.Equal("A_B", button.Text);
    }

    [Fact]
    public void ButtonCaption_ShowsTheAmpersandWhenMnemonicsAreOff()
    {
        using var button = new Button { UseMnemonic = false, Text = "&Run" };

        var label = (global::Gtk.Label)button.self.Child;
        Assert.Equal("&Run", label.Text);
        Assert.False(label.UseUnderline);
    }

    [Fact]
    public void RadioButtonCaption_UnderlinesItsMnemonic()
    {
        using var radio = new RadioButton { Text = "&Simple" };

        Assert.Equal("_Simple", radio.self.Label);
        Assert.True(radio.self.UseUnderline);
        Assert.Equal("&Simple", radio.Text);
    }

    [Fact]
    public void CheckBoxCaption_UnderlinesItsMnemonic()
    {
        using var check = new CheckBox { Text = "&Detailed" };

        Assert.Equal("&Detailed", check.Text);
        Assert.DoesNotContain("&", check.self.Child is global::Gtk.Label label ? label.Text : check.self.Label);
    }

    [Fact]
    public void LabelCaption_UnderlinesItsMnemonic()
    {
        using var label = new Label { Text = "&Name:" };

        Assert.Equal("Name:", label.self.Text);
        Assert.True(label.self.UseUnderline);
        Assert.Equal("&Name:", label.Text);
    }

    [Fact]
    public void GroupBoxCaption_UnderlinesItsMnemonic()
    {
        using var group = new GroupBox { Text = "&Mode" };

        // The frame reads its caption back through the label widget, which with the mnemonic on reports the
        // displayed text - the marker consumed, the ampersand gone.
        Assert.Equal("Mode", group.self.Label);
        Assert.True(((global::Gtk.Label)group.self.LabelWidget).UseUnderline);
        Assert.Equal("&Mode", group.Text);
    }

    [Fact]
    public void ACaptionWithNoMnemonicIsUntouched()
    {
        using var button = new Button { Text = "Close" };

        Assert.Equal("Close", ((global::Gtk.Label)button.self.Child).Text);
        Assert.Equal("Close", button.Text);
    }
}
