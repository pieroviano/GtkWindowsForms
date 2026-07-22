using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>Form5: minimal form (a button + tooltip). A pure smoke + baseline capture.</summary>
public class Form5Tests
{
    [Fact]
    public void Launches_AndRenders()
    {
        UiDriver.Run(() => new Form5(), ctx =>
        {
            var form = (Form5)ctx.Form;

            Assert.True(form.self.IsRealized, "Form5 window was not realized.");
            Assert.NotNull(form.button1);

            ctx.Shot("startup");
        });
    }
}
