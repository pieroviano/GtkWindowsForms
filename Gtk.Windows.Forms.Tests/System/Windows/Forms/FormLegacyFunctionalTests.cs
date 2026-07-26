namespace System.Windows.Forms;

extern alias gtk;

using System.ComponentModel;

/// <summary>
/// Verifies the pre-2.0 <see cref="Form" /> lifecycle surface: the legacy <c>Closing</c>/<c>Closed</c>
/// events (wired into the modern <c>FormClosing</c>/<c>FormClosed</c> raising, including cancel
/// propagation), owned-form management, <c>ParentForm</c> and validation. Reflection covers the API shape;
/// behaviour runs in a realized form.
/// </summary>
[Collection("NonParallelCollection")]
public class FormLegacyFunctionalTests
{
    private sealed class ClosingProbe : Form
    {
        public void RaiseFormClosing(FormClosingEventArgs e) => OnFormClosing(e);

        public void RaiseFormClosed(FormClosedEventArgs e) => OnFormClosed(e);
    }

    [Fact]
    public void LegacyMembers_Exist()
    {
        var t = typeof(Form);
        Assert.NotNull(t.GetEvent("Closing"));
        Assert.NotNull(t.GetEvent("Closed"));
        Assert.NotNull(t.GetEvent("Activated"));
        Assert.NotNull(t.GetEvent("Deactivate"));
        Assert.NotNull(t.GetProperty("OwnedForms"));
        Assert.NotNull(t.GetProperty("ParentForm"));
        Assert.NotNull(t.GetMethod("Validate", Type.EmptyTypes));
        Assert.NotNull(t.GetMethod("AddOwnedForm"));
    }

    [Fact]
    public void LegacyClosingClosed_FireFromModernRaisers_WithCancelPropagation()
    {
        // No ShowDialog: the Form static constructor initializes GTK, so the raisers can be exercised
        // directly. (Re-entering OnFormClosed inside a live modal loop is unsafe on this GTK host.)
        var form = new ClosingProbe();

        var closingFired = 0;
        var closedFired = 0;
        form.Closing += (s, e) => { closingFired++; e.Cancel = true; };
        form.Closed += (s, e) => closedFired++;

        // The legacy Closing runs when FormClosing is raised, and its cancel flows back to the modern args.
        var closingArgs = new FormClosingEventArgs(CloseReason.UserClosing, false);
        form.RaiseFormClosing(closingArgs);
        Assert.Equal(1, closingFired);
        Assert.True(closingArgs.Cancel);

        // The legacy Closed runs when FormClosed is raised.
        form.RaiseFormClosed(new FormClosedEventArgs(CloseReason.UserClosing));
        Assert.Equal(1, closedFired);

        form.Dispose();
    }

    [Fact]
    public void OwnedForms_And_ParentForm_Validate()
    {
        var owner = new Form();

        owner.Load += (s, e) =>
        {
            try
            {
                var owned = new Form();
                Assert.Empty(owner.OwnedForms);

                owner.AddOwnedForm(owned);
                Assert.Single(owner.OwnedForms);
                Assert.Same(owned, owner.OwnedForms[0]);
                Assert.Same(owner, owned.ParentForm);

                owner.RemoveOwnedForm(owned);
                Assert.Empty(owner.OwnedForms);
                Assert.Null(owned.ParentForm);

                // validation shims succeed
                Assert.True(owner.Validate());
                Assert.True(owner.ValidateChildren());
                owned.Dispose();
            }
            finally
            {
                owner.Close();
            }
        };

        owner.ShowDialog();
        owner.Dispose();
    }
}
