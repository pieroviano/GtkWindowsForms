namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies <c>Form.CenterToScreen</c> and <c>Form.CenterToParent</c>, which the port previously defined as
/// empty no-ops. They now set <see cref="Control.Location" /> to the WinForms-centered top-left (centered in
/// the screen working area, or in the owner form's bounds).
/// </summary>
[Collection("NonParallelCollection")]
public class FormCenterFunctionalTests
{
    // CenterToScreen/CenterToParent are protected; expose them for the test.
    private sealed class CenterableForm : Form
    {
        public void CenterScreenNow() => CenterToScreen();

        public void CenterParentNow() => CenterToParent();
    }

    [Fact]
    public void CenterToScreen_SetsLocationCenteredInWorkingArea()
    {
        var form = new CenterableForm { Width = 300, Height = 200 };

        form.CenterScreenNow();

        var area = Screen.FromControl(form).WorkingArea;
        Assert.Equal(Math.Max(area.X, area.X + (area.Width - form.Width) / 2), form.Left);
        Assert.Equal(Math.Max(area.Y, area.Y + (area.Height - form.Height) / 2), form.Top);

        form.Dispose();
    }

    [Fact]
    public void CenterToParent_WithOwner_CentersOnOwnerBounds()
    {
        var owner = new CenterableForm { Width = 400, Height = 300 };
        owner.Location = new System.Drawing.Point(120, 80);
        var child = new CenterableForm { Width = 200, Height = 100 };
        owner.AddOwnedForm(child);

        child.CenterParentNow();

        Assert.Equal(owner.Left + (owner.Width - child.Width) / 2, child.Left);
        Assert.Equal(owner.Top + (owner.Height - child.Height) / 2, child.Top);

        child.Dispose();
        owner.Dispose();
    }

    [Fact]
    public void CenterToParent_WithoutOwner_FallsBackToScreen()
    {
        var form = new CenterableForm { Width = 250, Height = 150 };

        form.CenterParentNow();

        var area = Screen.FromControl(form).WorkingArea;
        Assert.Equal(Math.Max(area.X, area.X + (area.Width - form.Width) / 2), form.Left);
        Assert.Equal(Math.Max(area.Y, area.Y + (area.Height - form.Height) / 2), form.Top);

        form.Dispose();
    }
}
