namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// A <see cref="MaskedTextBox" /> must mask as soon as it is told to, not when it is first shown.
/// </summary>
/// <remarks>
/// Found by comparing snapshots of the sample on both engines: the reference rendered the password box as
/// <c>*******</c> and the Gtk4 port showed the text in clear. Masking was applied from a handler wired to
/// <c>GtkWidget::show</c>, a signal Gtk4 removed, so on Gtk4 it never ran at all.
/// </remarks>
[Collection("NonParallelCollection")]
public class MaskedTextBoxMaskingTests
{
    // Constructing the control builds GTK widgets, which needs the type system initialised.
    public MaskedTextBoxMaskingTests() => Application.Init();

    [Fact]
    public void SettingPasswordChar_HidesTheTextImmediately()
    {
        var box = new MaskedTextBox { Text = "sdfdf43" };

        Assert.True(box.self.Visibility, "text is visible before a password character is set");

        box.PasswordChar = '*';

        Assert.False(box.self.Visibility, "assigning PasswordChar must mask the entry there and then");
        Assert.Equal('*', box.self.InvisibleChar);
        Assert.Equal('*', box.PasswordChar);
    }

    [Fact]
    public void ClearingPasswordChar_RevealsTheTextAgain()
    {
        var box = new MaskedTextBox { Text = "secret", PasswordChar = '*' };
        Assert.False(box.self.Visibility);

        box.PasswordChar = '\0';

        Assert.True(box.self.Visibility, "clearing PasswordChar must unmask the entry");
    }

    [Fact]
    public void SettingAMask_FormatsTheTextImmediately()
    {
        // The mask formatting hung off the same dead signal, so a masked box was never formatted either.
        var box = new MaskedTextBox { Text = "7" };

        box.Mask = "00/00";

        Assert.Equal("00/00", box.Mask);
        Assert.Contains("_", box.Text);
    }
}
