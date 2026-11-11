namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Regression test for closing a modal (<see cref="Form.ShowDialog()" />) form. Closing it must DESTROY the
/// native GTK toplevel, not merely dispose the <see cref="Form" /> (disposal clears the child controls).
/// </summary>
/// <remarks>
/// The bug this pins: a <see cref="Form.Close()" /> issued while the modal loop is running is deferred through
/// <c>FormBase.PendingModalClose</c> and completed in <c>Gtk4Compat.RunDialog</c> after the loop unwinds. That
/// completion disposed the form — emptying it of controls — but never destroyed the toplevel, because
/// <see cref="Control" /> derives from <see cref="System.ComponentModel.Component" /> and its
/// <c>Dispose</c> does not touch the window. The result was an empty window that stayed on screen and would
/// not close (exactly the "About" dialog symptom). The fix destroys the window in the deferred path.
/// </remarks>
[Collection("NonParallelCollection")]
public class FormModalCloseFunctionalTests
{
    [Fact]
    public void ShowDialog_CloseFromLoad_DestroysNativeWindowAndKeepsControls()
    {
        var form = new Form();
        form.Controls.Add(new Panel());
        form.Controls.Add(new Button { Text = "Close", DialogResult = DialogResult.Cancel });

        // Closing from Load runs while the modal loop is active, which is the deferred-close path the bug
        // lived in (the same path a Cancel/Close button click takes once the dialog is up).
        form.Load += (_, _) => form.Close();

        form.ShowDialog();

        // The native toplevel must have been torn down by the deferred close, not left open-but-empty.
        Assert.True(form.self.IsNativeDestroyed);

        form.Dispose();
    }
}
