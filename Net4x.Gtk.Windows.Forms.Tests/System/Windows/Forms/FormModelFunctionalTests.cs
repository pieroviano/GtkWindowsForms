using System.Windows.Forms;
using Point = global::System.Drawing.Point;
using Size = global::System.Drawing.Size;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="Form"/>: the window-chrome properties must round-trip, the
/// control collection must parent children to the form, and the MDI relationships must be symmetric.</summary>
[Collection("NonParallelCollection")]
public class FormModelFunctionalTests
{
    [Fact]
    public void ConstructedWithATitle_CarriesItAsText()
    {
        using var form = new Form("My window");

        Assert.Equal("My window", form.Text);
    }

    [Fact]
    public void TextRoundTrips()
    {
        using var form = new Form();

        form.Text = "Renamed";

        Assert.Equal("Renamed", form.Text);
    }

    [Fact]
    public void ChromeFlagsRoundTrip()
    {
        using var form = new Form
        {
            ControlBox = false,
            MaximizeBox = false,
            MinimizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            TopMost = true,
            KeyPreview = true
        };

        Assert.False(form.ControlBox);
        Assert.False(form.MaximizeBox);
        Assert.False(form.MinimizeBox);
        Assert.False(form.ShowIcon);
        Assert.False(form.ShowInTaskbar);
        Assert.True(form.TopMost);
        Assert.True(form.KeyPreview);
    }

    [Fact]
    public void BorderStyleAndStartPositionRoundTrip()
    {
        using var form = new Form
        {
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };

        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal(FormStartPosition.CenterScreen, form.StartPosition);
    }

    [Fact]
    public void WindowStateRoundTrips()
    {
        using var form = new Form { WindowState = FormWindowState.Maximized };

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
    }

    [Fact]
    public void OpacityRoundTrips()
    {
        using var form = new Form { Opacity = 0.5 };

        Assert.Equal(0.5, form.Opacity, 3);
    }

    [Fact]
    public void DialogResultRoundTrips()
    {
        using var form = new Form { DialogResult = DialogResult.Cancel };

        Assert.Equal(DialogResult.Cancel, form.DialogResult);
    }

    [Fact]
    public void SizeAndLocationRoundTrip()
    {
        using var form = new Form { Size = new Size(400, 300), Location = new Point(20, 30) };

        Assert.Equal(new Size(400, 300), form.Size);
        Assert.Equal(new Point(20, 30), form.Location);
    }

    [Fact]
    public void ClientSizeIsWithinTheFormSize()
    {
        using var form = new Form { Size = new Size(400, 300) };

        Assert.True(form.ClientSize.Width <= 400,
            $"client width {form.ClientSize.Width} should not exceed the form width");
        Assert.True(form.ClientSize.Height <= 300,
            $"client height {form.ClientSize.Height} should not exceed the form height");
    }

    [Fact]
    public void AutoScalePropertiesRoundTrip()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.Dpi,
            AutoScaleDimensions = new global::System.Drawing.SizeF(96f, 96f)
        };

        Assert.Equal(AutoScaleMode.Dpi, form.AutoScaleMode);
        Assert.Equal(96f, form.AutoScaleDimensions.Width, 2);
    }

    [Fact]
    public void ControlsAddedToTheForm_AreParentedToIt()
    {
        using var form = new Form();
        var button = new Button { Name = "ok" };

        form.Controls.Add(button);

        Assert.Single(form.Controls);
        Assert.Same(form, button.Parent);
        Assert.True(form.Controls.Contains(button), "the form should contain the child it was given");
    }

    [Fact]
    public void ControlsRemovedFromTheForm_AreDropped()
    {
        using var form = new Form();
        var button = new Button();
        form.Controls.Add(button);

        form.Controls.Remove(button);

        Assert.Empty(form.Controls);
    }

    [Fact]
    public void MainMenuStripRoundTrips()
    {
        using var form = new Form();
        var menu = new MenuStrip();

        form.MainMenuStrip = menu;

        Assert.Same(menu, form.MainMenuStrip);
    }

    [Fact]
    public void MdiParent_MakesTheChildReportItsParentAndAppearInMdiChildren()
    {
        using var parent = new Form { IsMdiContainer = true };
        var child = new Form();

        child.MdiParent = parent;

        Assert.True(child.IsMdiChild, "a form given an MDI parent is an MDI child");
        Assert.Same(parent, child.MdiParent);
        Assert.Contains(child, parent.MdiChildren);
    }

    [Fact]
    public void IsMdiChild_IsFalseForAStandaloneForm()
    {
        using var form = new Form();

        Assert.False(form.IsMdiChild, "a form with no MDI parent is not an MDI child");
        Assert.Empty(form.MdiChildren);
    }

    [Fact]
    public void IsMdiContainerRoundTrips()
    {
        using var form = new Form { IsMdiContainer = true };

        Assert.True(form.IsMdiContainer);
    }

    [Fact]
    public void OwnerRoundTrips()
    {
        using var owner = new Form();
        using var owned = new Form();

        owned.Owner = owner;

        Assert.Same(owner, owned.Owner);
    }

    [Fact]
    public void PaddingRoundTrips()
    {
        using var form = new Form { Padding = new Padding(3, 4, 5, 6) };

        Assert.Equal(new Padding(3, 4, 5, 6), form.Padding);
    }

    [Fact]
    public void HideMakesTheFormInvisible()
    {
        using var form = new Form();

        form.Hide();

        Assert.False(form.Visible, "a hidden form should not report itself visible");
    }

    [Fact]
    public void TopLevelRoundTrips()
    {
        using var form = new Form();

        Assert.True(form.TopLevel, "a form is top-level by default");
    }

    [Fact]
    public void FormsAreTheirOwnTopLevelControl()
    {
        using var form = new Form();
        var panel = new Panel();
        form.Controls.Add(panel);

        Assert.Same(form, form.TopLevelControl);
        Assert.Same(form, panel.TopLevelControl);
    }
}
