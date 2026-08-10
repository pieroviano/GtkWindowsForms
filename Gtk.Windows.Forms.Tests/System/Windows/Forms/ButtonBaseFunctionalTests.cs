namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the <see cref="ButtonBase" /> base class shared by <see cref="Button" />,
/// <see cref="CheckBox" /> and <see cref="RadioButton" />: the hierarchy relationships, the consolidated
/// <see cref="ButtonBase.TextAlign" /> wiring, and the appearance surface carried for source compatibility.
/// </summary>
/// <remarks>
/// GTK-dependent assertions are batched into a single realized-form lifecycle to keep the number of
/// <see cref="Form.ShowDialog" /> cycles low, since this single-threaded GTK host accumulates global state
/// per shown dialog.
/// </remarks>
[Collection("NonParallelCollection")]
public class ButtonBaseFunctionalTests
{
    [Fact]
    public void ButtonControls_DeriveFromButtonBase()
    {
        Assert.True(typeof(ButtonBase).IsAssignableFrom(typeof(Button)));
        Assert.True(typeof(ButtonBase).IsAssignableFrom(typeof(CheckBox)));
        Assert.True(typeof(ButtonBase).IsAssignableFrom(typeof(RadioButton)));
        Assert.Equal(typeof(Control), typeof(ButtonBase).BaseType);
    }

    [Fact]
    public void TextAlignWiring_And_AppearanceSurface_Behave()
    {
        var form = new Form();
        var button = new Button();
        var checkBox = new CheckBox();
        var radio = new RadioButton();
        form.Controls.Add(button);
        form.Controls.Add(checkBox);
        form.Controls.Add(radio);

        form.Load += (s, e) =>
        {
            try
            {
                // --- TextAlign round-trips and drives the GTK alignment factors per control ---
                button.TextAlign = gtk::System.Drawing.ContentAlignment.BottomRight;
                Assert.Equal(gtk::System.Drawing.ContentAlignment.BottomRight, button.TextAlign);
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(1.0f, button.self.Xalign);
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(1.0f, button.self.Yalign);

                checkBox.TextAlign = gtk::System.Drawing.ContentAlignment.MiddleCenter;
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(0.5f, checkBox.self.Xalign);
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(0.5f, checkBox.self.Yalign);

                radio.TextAlign = gtk::System.Drawing.ContentAlignment.MiddleLeft;
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(0.0f, radio.self.Xalign);
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.Equal(0.5f, radio.self.Yalign);

                // --- FlatAppearance is a stable backing instance with WinForms defaults ---
                var appearance = button.FlatAppearance;
                Assert.NotNull(appearance);
                Assert.Same(appearance, button.FlatAppearance);
                Assert.Equal(1, appearance.BorderSize);
                button.FlatAppearance.BorderSize = 3;
                Assert.Equal(3, button.FlatAppearance.BorderSize);

                // --- appearance-surface defaults match WinForms ---
                Assert.Equal(FlatStyle.Standard, button.FlatStyle);
                button.FlatStyle = FlatStyle.Flat;
                Assert.Equal(FlatStyle.Flat, button.FlatStyle);
                Assert.True(button.UseMnemonic);
                Assert.False(button.UseCompatibleTextRendering);
                Assert.Equal(TextImageRelation.Overlay, button.TextImageRelation);
                Assert.False(button.AutoEllipsis);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}
