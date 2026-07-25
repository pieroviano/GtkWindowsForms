using System.Drawing;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="ToolStripControlHost"/> — the base type for putting an arbitrary
/// <see cref="Control"/> on a <see cref="ToolStrip"/>. It was absent (a compile break for custom hosted
/// items and any type reference). These exercise the contract ported code depends on: the control is held
/// and reachable, common item properties forward to it, the subscribe hook fires so subclasses can bridge
/// events, and a host can be added to a strip's item collection.
/// </summary>
[Collection("NonParallelCollection")]
public class ToolStripControlHostFunctionalTests
{
    static ToolStripControlHostFunctionalTests()
    {
        using var _ = new Form();
    }

    private sealed class RecordingHost : ToolStripControlHost
    {
        public int SubscribeCount { get; private set; }
        public int UnsubscribeCount { get; private set; }

        public RecordingHost(Control c) : base(c)
        {
        }

        protected override void OnSubscribeControlEvents(Control control) => SubscribeCount++;

        protected override void OnUnsubscribeControlEvents(Control control) => UnsubscribeCount++;
    }

    [Fact]
    public void Constructor_NullControl_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripControlHost(null!));
    }

    [Fact]
    public void Control_ReturnsTheHostedControl()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button);

        Assert.Same(button, host.Control);
    }

    [Fact]
    public void Constructor_WithName_SetsTheItemName()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button, "hostedButton");

        Assert.Equal("hostedButton", host.Name);
    }

    [Fact]
    public void Text_ForwardsToTheHostedControl()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button);

        host.Text = "Click me";

        Assert.Equal("Click me", button.Text);
        Assert.Equal("Click me", host.Text);
    }

    [Fact]
    public void Enabled_ForwardsToTheHostedControl()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button);

        host.Enabled = false;

        Assert.False(button.Enabled);
        Assert.False(host.Enabled);
    }

    [Fact]
    public void Font_GetterReflectsTheHostedControl()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button);

        // The host's font getter must report the hosted control's font, not an independent item font.
        Assert.Equal(button.Font, host.Font);
    }

    [Fact]
    public void SubscribeHook_FiresWhenHosting_AndUnsubscribeOnDispose()
    {
        using var button = new Button();
        var host = new RecordingHost(button);

        Assert.Equal(1, host.SubscribeCount);

        host.Dispose();

        Assert.Equal(1, host.UnsubscribeCount);
    }

    [Fact]
    public void AHostCanBeAddedToAToolStrip()
    {
        using var button = new Button();
        var host = new ToolStripControlHost(button, "hosted");
        using var strip = new ToolStrip();

        strip.Items.Add(host);

        Assert.Contains(host, strip.Items.Cast<ToolStripItem>());
    }
}
