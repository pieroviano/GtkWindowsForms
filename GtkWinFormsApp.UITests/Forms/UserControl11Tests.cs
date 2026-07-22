using System.Windows.Forms;
using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>UserControl11 is a UserControl, not a window, so it is hosted in a bare Form to be driven and
/// captured. Its constructor adds a PictureBox child.</summary>
public class UserControl11Tests
{
    [Fact]
    public void HostsAndRenders_InsideForm()
    {
        UserControl11? control = null;

        UiDriver.Run(
            () =>
            {
                // A named host so the screenshot baseline folder is UserControl11Host, not the generic "Form".
                var host = new UserControl11Host();
                control = new UserControl11();
                host.Controls.Add(control);
                return host;
            },
            ctx =>
            {
                Assert.True(ctx.Form.self.IsRealized, "Host form was not realized.");
                Assert.NotNull(control);

                // The UserControl adds a PictureBox in its constructor.
                Assert.True(control!.Controls.Count > 0,
                    "UserControl11 should contain its constructed PictureBox child.");

                ctx.Shot("hosted");
            });
    }

    /// <summary>A bare host window sized to the control, named so its screenshot baseline is identifiable.</summary>
    private sealed class UserControl11Host : Form
    {
        public UserControl11Host()
        {
            Width = 200;
            Height = 180;
        }
    }
}
