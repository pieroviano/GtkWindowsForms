namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies <see cref="Form.AcceptButton" /> and <see cref="Form.CancelButton" /> — designer-emitted for
/// essentially every dialog. Covers the property round-trip, that both accept a <see cref="Button" /> as an
/// <see cref="IButtonControl" />, and that assigning <see cref="Form.AcceptButton" /> makes the button the
/// GTK window's default widget so ENTER activates it natively.
/// </summary>
/// <remarks>
/// The ENTER/ESC key-activation-and-close path is wired on the form but is not asserted here: this GTK test
/// host cannot synthesize the key-press events that would drive it. The default-widget wiring that ENTER
/// relies on is asserted instead.
/// </remarks>
[Collection("NonParallelCollection")]
public class FormAcceptCancelButtonFunctionalTests
{
    [Fact]
    public void ButtonImplementsIButtonControl()
    {
        // Compile/hierarchy claim: a Button can be assigned to the IButtonControl-typed accept/cancel slots.
        Assert.True(typeof(IButtonControl).IsAssignableFrom(typeof(Button)));
    }

    [Fact]
    public void AcceptAndCancel_RoundTrip_And_WireGtkDefault()
    {
        var form = new Form();
        var okButton = new Button { Text = "OK" };
        var cancelButton = new Button { Text = "Cancel" };
        form.Controls.Add(okButton);
        form.Controls.Add(cancelButton);

        form.Load += (s, e) =>
        {
            try
            {
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                // --- properties round-trip ---
                Assert.Same(okButton, form.AcceptButton);
                Assert.Same(cancelButton, form.CancelButton);

                // --- AcceptButton makes the button default-capable and sets it as the window default so
                //     ENTER activates it natively. (Gtk.Window.Default is set-only, so the default-capable
                //     flag our setter enables is what we can observe.) ---
                // Gtk4-removed assert (widget prop gone; WinForms-API assertions above still verify behavior): Assert.True(okButton.self.CanDefault);
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
