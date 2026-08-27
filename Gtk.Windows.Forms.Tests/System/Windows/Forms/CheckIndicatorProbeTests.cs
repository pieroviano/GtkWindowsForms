namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class CheckIndicatorProbeTests
{
    public CheckIndicatorProbeTests() => Application.Init();

    [Fact]
    public void Probe()
    {
        var plainItem = new ToolStripMenuItem("Item");
        var checkableItem = new ToolStripMenuItem("Item") { CheckOnClick = true };
        var plain = plainItem.checkButton;
        var checkable = checkableItem.checkButton;

        plain.Measure(Gtk.Orientation.Horizontal, -1, out var pmin, out var pnat, out _, out _);
        checkable.Measure(Gtk.Orientation.Horizontal, -1, out var cmin, out var cnat, out _, out _);
        throw new Exception($"plain min={pmin} nat={pnat} | checkable min={cmin} nat={cnat}");
    }
}
