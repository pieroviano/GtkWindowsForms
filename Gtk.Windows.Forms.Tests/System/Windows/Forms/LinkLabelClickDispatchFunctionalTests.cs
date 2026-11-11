using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Pins the classic-WinForms contract that a link activation raises <see cref="LinkLabel.LinkClicked"/>
/// <em>through</em> <see cref="LinkLabel.OnLinkClicked"/>. The GTK activate-link handler used to invoke the
/// event directly, so an <c>OnLinkClicked</c> override never ran and could not suppress the event.</summary>
[Collection("NonParallelCollection")]
public class LinkLabelClickDispatchFunctionalTests
{
    /// <summary>A LinkLabel that records, and can suppress, the OnLinkClicked funnel.</summary>
    private sealed class SpyLinkLabel : LinkLabel
    {
        public int Calls { get; private set; }
        public LinkLabelLinkClickedEventArgs? LastArgs { get; private set; }
        public bool CallBase { get; set; } = true;

        protected internal override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            Calls++;
            LastArgs = e;
            if (CallBase)
            {
                base.OnLinkClicked(e);
            }
        }
    }

    [Fact]
    public void ActivatingALink_InvokesTheOnLinkClickedOverride()
    {
        using var label = new SpyLinkLabel { Text = "https://example.com" };

        label.LinkLabel_ActivateLink(null!, null!); // the GTK activate-link handler

        Assert.Equal(1, label.Calls);
        Assert.Equal("https://example.com", label.LastArgs!.Link.LinkData);
    }

    [Fact]
    public void ActivatingALink_RaisesLinkClickedThroughTheFunnel()
    {
        using var label = new SpyLinkLabel { Text = "https://example.com" };
        LinkLabel.Link? received = null;
        label.LinkClicked += (_, e) => received = e.Link;

        label.LinkLabel_ActivateLink(null!, null!);

        Assert.NotNull(received);
        Assert.Equal("https://example.com", received!.LinkData);
    }

    [Fact]
    public void AnOverrideThatDoesNotCallBase_SuppressesLinkClicked()
    {
        using var label = new SpyLinkLabel { Text = "https://example.com", CallBase = false };
        var raised = false;
        label.LinkClicked += (_, _) => raised = true;

        label.LinkLabel_ActivateLink(null!, null!);

        Assert.Equal(1, label.Calls);
        Assert.False(raised, "an override that does not call base suppresses LinkClicked, as in WinForms");
    }
}
