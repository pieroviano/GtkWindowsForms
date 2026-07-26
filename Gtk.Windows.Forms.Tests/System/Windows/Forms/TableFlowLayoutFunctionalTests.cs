using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// The first measurement of <see cref="TableLayoutPanel"/> and <see cref="FlowLayoutPanel"/> against real
/// WinForms — the open item EMULATION-GAPS.md §3 flagged. It surfaced a divergence, so these are
/// **characterization tests**: they pin the shim's *current* behaviour and document how it differs from the
/// WinForms oracle, so a future fix trips them and prompts an update.
/// <para>
/// **The divergence.** Unlike dock/anchor (which the managed layout arithmetic drives by setting GTK
/// margins), <c>TableLayoutPanel</c> and <c>FlowLayoutPanel</c> lay their children out with the native
/// <c>Gtk.Grid</c>/<c>Gtk.FlowBox</c>. Those position children by allocation, not by the
/// <c>MarginStart</c>/<c>MarginTop</c> that <see cref="Control.Left"/>/<see cref="Control.Top"/> read — so a
/// child reports position <c>(0,0)</c> at the WinForms API even though GTK arranges it correctly on screen.
/// WinForms would report the laid-out rectangles (table cells in a grid; flow items wrapping):
/// </para>
/// <code>
/// TABLE  cols [Absolute 60, Percent 100], rows [Absolute 30, Percent 100]:
///        c00=(3,3,54,24) c10=(63,3,134,24) c01=(3,33,54,64) c11=(63,33,134,64)
/// FLOW   100 wide, LeftToRight+WrapContents, three 40x20 panels: a=(3,3) b=(49,3) c=(3,29)
/// </code>
/// Closing it means reproducing the table/flow measure-arrange in managed code and pushing margins, as the
/// dock/anchor work did — a dedicated effort, not a patch.
/// </summary>
[Collection("NonParallelCollection")]
public class TableFlowLayoutFunctionalTests
{
    private static void WhenLaidOut(Form form, Action body)
    {
        ExceptionDispatchInfo? failure = null;
        form.Load += (_, _) =>
        {
            try
            {
                Application.DoEvents();
                body();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                form.Close();
            }
        };
        form.ShowDialog();
        failure?.Throw();
    }

    [Fact]
    public void TableLayoutPanel_ChildrenReportOrigin_KnownDivergenceFromWinForms()
    {
        var form = new Form { Width = 240, Height = 160 };
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Panel c00 = new(), c10 = new(), c01 = new(), c11 = new();
        table.Controls.Add(c00, 0, 0);
        table.Controls.Add(c10, 1, 0);
        table.Controls.Add(c01, 0, 1);
        table.Controls.Add(c11, 1, 1);
        form.Controls.Add(table);

        WhenLaidOut(form, () =>
        {
            // The cell/row/column arithmetic is done by the native Gtk.Grid, which does not write the
            // margins Control.Left/Top read — so every cell reports the origin. WinForms would place them at
            // c00=(3,3) c10=(63,3) c01=(3,33) c11=(63,33). Documented gap; see the class summary.
            // The native Gtk.Grid arranges the cells on screen, but at the WinForms API every cell reports
            // the same origin (the container's border offset) — the column/row arithmetic is not surfaced
            // through Control.Left/Top. WinForms would separate them: c10.Left > c00.Left, c01.Top > c00.Top.
            Assert.Equal(c00.Left, c10.Left);
            Assert.Equal(c00.Left, c01.Left);
            Assert.Equal(c00.Top, c01.Top);
            Assert.Equal(c00.Top, c10.Top);
        });
    }

    [Fact]
    public void FlowLayoutPanel_ChildrenReportOrigin_ButKeepTheirSize_KnownDivergence()
    {
        var form = new Form { Width = 120, Height = 160 };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
        var a = new Panel { Width = 40, Height = 20 };
        var b = new Panel { Width = 40, Height = 20 };
        var c = new Panel { Width = 40, Height = 20 };
        flow.Controls.Add(a);
        flow.Controls.Add(b);
        flow.Controls.Add(c);
        form.Controls.Add(flow);

        WhenLaidOut(form, () =>
        {
            // The Gtk.FlowBox flows and wraps the children on screen, but their WinForms position is the
            // origin because the flow does not set MarginStart/MarginTop. Their explicit size is preserved.
            // WinForms would place them at a=(3,3) b=(49,3) c=(3,29). Documented gap; see the class summary.
            Assert.Equal(0, a.Left);
            Assert.Equal(0, b.Left);
            Assert.Equal(0, c.Top);
            Assert.Equal(40, a.Width);
            Assert.Equal(20, a.Height);
        });
    }
}
