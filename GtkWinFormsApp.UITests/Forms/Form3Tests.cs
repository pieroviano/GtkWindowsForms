using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>
/// Form3 is the app's startup form (<c>Application.Run(new Form3())</c>). These tests stand in for the
/// author's manual "launch the app and click around" pass.
/// </summary>
public class Form3Tests
{
    [Fact]
    public void Launches_WithExpectedControls()
    {
        UiDriver.Run(() => new Form3(), ctx =>
        {
            var form = (Form3)ctx.Form;

            // Startup smoke: the window came up and realized without throwing.
            Assert.True(form.self.IsRealized, "Form3 window was not realized.");

            // Its designer controls exist (reached via internal + InternalsVisibleTo, no reflection).
            Assert.NotNull(form.button1);
            Assert.NotNull(form.button5);
            Assert.NotNull(form.label1);
            Assert.NotNull(form.panel1);
            Assert.NotNull(form.menuStrip1);
            Assert.NotNull(form.statusStrip1);

            ctx.Shot("startup");
        });
    }

    [Fact]
    public void ClickingButton5_OpensTest1Window()
    {
        UiDriver.Run(() => new Form3(), ctx =>
        {
            var form = (Form3)ctx.Form;

            var before = Gtk.Window.ListToplevels().Length;
            ctx.Click(form.button5); // wired to Button5_Click => new Test1().Show()
            var after = Gtk.Window.ListToplevels().Length;

            Assert.True(after > before,
                $"Expected a new top-level window after clicking button5 (before={before}, after={after}).");
        });
    }
}
