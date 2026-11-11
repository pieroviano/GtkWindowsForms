namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Regression test for <see cref="Form.Activated" /> / <see cref="Form.OnActivated" />. WinForms raises
/// Activated when a form becomes the active window; a very common pattern (e.g. a modal "please wait" form)
/// does its work from <c>OnActivated</c> and then closes. The Gtk4 port defined <c>OnActivated</c> but never
/// raised it, so such overrides silently never ran. This pins that showing a form raises Activated.
/// </summary>
[Collection("NonParallelCollection")]
public class FormActivatedFunctionalTests
{
    [Fact]
    public void ShowDialog_RaisesActivated_DrivingOnActivatedWork()
    {
        var form = new Form();
        form.Controls.Add(new Label { Text = "Working..." });

        var activatedCount = 0;
        form.Activated += (_, _) =>
        {
            activatedCount++;
            form.Close();
        };

        // Safety net: if Activated is never raised (the regression), close the form after a short delay so the
        // modal loop ends and the assertion fails — instead of hanging the test run forever.
        GLib.Timeout.Add(3000, () =>
        {
            if (!form.IsDisposed)
            {
                form.Close();
            }

            return false;
        });

        form.ShowDialog();

        Assert.True(activatedCount > 0, "Form.Activated was not raised while the form was shown.");

        form.Dispose();
    }
}
