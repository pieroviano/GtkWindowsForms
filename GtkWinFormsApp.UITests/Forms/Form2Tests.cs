using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>Form2: list/log form (ListView + ListBox + RichTextBox), shown by Form3 as a dialog.</summary>
public class Form2Tests
{
    [Fact]
    public void Launches_WithExpectedControls()
    {
        UiDriver.Run(() => new Form2(), ctx =>
        {
            var form = (Form2)ctx.Form;

            Assert.True(form.self.IsRealized, "Form2 window was not realized.");
            Assert.NotNull(form.listView1);
            Assert.NotNull(form.listBox1);
            Assert.NotNull(form.richTextBox1);

            // NOTE: no screenshot baseline for Form2 - timer1 is enabled and appends DateTime.Now text to
            // listBox1/richTextBox1, so its pixels change every run (SampleUITests-Plan §11). State only.
        });
    }
}
