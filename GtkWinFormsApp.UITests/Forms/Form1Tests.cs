using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>Form1: the data-binding form (tree + DataGridView + a flow panel of 80 buttons).</summary>
public class Form1Tests
{
    [Fact]
    public void Launches_WithBoundControls()
    {
        UiDriver.Run(() => new Form1(), ctx =>
        {
            var form = (Form1)ctx.Form;

            Assert.True(form.self.IsRealized, "Form1 window was not realized.");
            Assert.NotNull(form.dataGridView1);
            Assert.NotNull(form.textBox1);

            // The constructor adds 80 buttons to the flow panel (on top of any designer-placed children).
            Assert.True(form.flowLayoutPanel1.Controls.Count >= 80,
                $"Expected at least the 80 constructed buttons but got {form.flowLayoutPanel1.Controls.Count}.");

            ctx.Shot("loaded");
        });
    }

    [Fact]
    public void ClickingButton6_AppendsToCheckedListBox()
    {
        UiDriver.Run(() => new Form1(), ctx =>
        {
            var form = (Form1)ctx.Form;

            var before = form.checkedListBox1.Items.Count;
            ctx.Click(form.button6); // button6_Click adds one item to checkedListBox1
            Assert.Equal(before + 1, form.checkedListBox1.Items.Count);
        });
    }
}
