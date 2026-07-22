using GtkWinFormsApp.UITests.Infrastructure;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>Test1: the dashboard form Form3 opens. Its DataGridView is bound to a 50-row DataTable in the
/// constructor, and the background banner is culture-aware (English under a non-zh UI culture).</summary>
public class Test1Tests
{
    [Fact]
    public void Launches_WithBoundGrid()
    {
        UiDriver.Run(() => new Test1(), ctx =>
        {
            var form = (Test1)ctx.Form;

            Assert.True(form.self.IsRealized, "Test1 window was not realized.");
            Assert.NotNull(form.dataGridView1);

            // The constructor binds a DataTable of 50 rows to the grid.
            Assert.True(form.dataGridView1.RowCount > 0,
                $"Test1 grid should have bound rows but RowCount={form.dataGridView1.RowCount}.");

            // NOTE: no screenshot baseline for Test1 - the grid cells are seeded with DateTime.Now.Ticks,
            // so its pixels change every run and can't be gated against a fixed baseline (SampleUITests-Plan
            // §11, time-varying content). State assertions carry this form.
        });
    }
}
