using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>Form4: controls playground. button3/button6 open native file dialogs (not driven here); the
/// button4/button10 handlers are side-effect-free and safe to click.</summary>
public class Form4Tests
{
    [Fact]
    public void Launches_WithExpectedControls()
    {
        UiDriver.Run(() => new Form4(), ctx =>
        {
            var form = (Form4)ctx.Form;

            Assert.True(form.self.IsRealized, "Form4 window was not realized.");
            Assert.NotNull(form.button4);
            Assert.NotNull(form.button10);

            ctx.Shot("startup");
        });
    }

    [Fact]
    public void ClickingSafeButtons_DoesNotThrow()
    {
        UiDriver.Run(() => new Form4(), ctx =>
        {
            var form = (Form4)ctx.Form;

            // Button4_Click / Button10_Click are empty handlers: the point is that raising them through the
            // real event path runs cleanly (no GTK-CRITICAL, no exception).
            ctx.Click(form.button4);
            ctx.Click(form.button10);
        });
    }
}
