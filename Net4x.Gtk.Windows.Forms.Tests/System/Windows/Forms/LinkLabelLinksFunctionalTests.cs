using System.Collections;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary><see cref="LinkLabel"/> stood at 24 throw sites. They were
/// all in one place: <c>Links</c> was a never-assigned auto-property (so it was null), the collection had no
/// backing store — <c>Add</c> overwrote the label's <c>Text</c>, <c>Contains</c> did a substring search on
/// it — and <see cref="LinkLabel.Link"/> discarded its constructor arguments. A multi-link LinkLabel, the
/// control's whole reason for existing over a Label, was impossible.</summary>
[Collection("NonParallelCollection")]
public class LinkLabelLinksFunctionalTests
{
    private static LinkLabel Labelled() => new() { Text = "visit our site or email us today" };

    // ---- Link ------------------------------------------------------------

    [Fact]
    public void Link_KeepsTheSpanItWasConstructedWith()
    {
        var link = new LinkLabel.Link(6, 3);

        Assert.Equal(6, link.Start);
        Assert.Equal(3, link.Length);
    }

    [Fact]
    public void Link_KeepsTheDataItWasConstructedWith()
    {
        var link = new LinkLabel.Link(6, 3, "https://example.com");

        Assert.Equal(6, link.Start);
        Assert.Equal(3, link.Length);
        Assert.Equal("https://example.com", link.LinkData);
    }

    [Fact]
    public void Link_IsEnabledAndUnvisitedByDefault()
    {
        var link = new LinkLabel.Link();

        Assert.True(link.Enabled, "a new link is enabled in WinForms");
        Assert.False(link.Visited, "a new link has not been visited");
    }

    [Fact]
    public void Link_StatePropertiesRoundTrip()
    {
        var link = new LinkLabel.Link(0, 5)
        {
            Enabled = false,
            Visited = true,
            Name = "home",
            Tag = 42,
            Description = "the home page",
            LinkData = "https://example.com"
        };

        Assert.False(link.Enabled);
        Assert.True(link.Visited);
        Assert.Equal("home", link.Name);
        Assert.Equal(42, link.Tag);
        Assert.Equal("the home page", link.Description);
        Assert.Equal("https://example.com", link.LinkData);
    }

    [Fact]
    public void Link_SpanCanBeChangedAfterConstruction()
    {
        var link = new LinkLabel.Link(0, 1) { Start = 10, Length = 4 };

        Assert.Equal(10, link.Start);
        Assert.Equal(4, link.Length);
    }

    // ---- Links is reachable ----------------------------------------------

    [Fact]
    public void Links_IsNotNullOnAFreshLinkLabel()
    {
        using var label = Labelled();

        Assert.NotNull(label.Links);
        Assert.Empty(label.Links);
    }

    [Fact]
    public void Links_IsTheSameCollectionEachTime()
    {
        using var label = Labelled();

        Assert.Same(label.Links, label.Links);
    }

    // ---- Collection semantics --------------------------------------------

    [Fact]
    public void Add_BySpanCreatesAndReturnsTheLink()
    {
        using var label = Labelled();

        var link = label.Links.Add(6, 3);

        Assert.Single(label.Links);
        Assert.Equal(6, link.Start);
        Assert.Equal(3, link.Length);
        Assert.Same(link, label.Links[0]);
    }

    [Fact]
    public void Add_BySpanAndDataCarriesTheData()
    {
        using var label = Labelled();

        var link = label.Links.Add(6, 3, "https://example.com");

        Assert.Equal("https://example.com", link.LinkData);
    }

    [Fact]
    public void AddingSeveralLinksKeepsThemInOrder()
    {
        using var label = Labelled();

        var first = label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);
        var third = label.Links.Add(10, 4);

        Assert.Equal(3, label.Links.Count);
        Assert.Same(first, label.Links[0]);
        Assert.Same(second, label.Links[1]);
        Assert.Same(third, label.Links[2]);
    }

    [Fact]
    public void AddingALinkDoesNotDisturbTheLabelText()
    {
        using var label = Labelled();
        var before = label.Text;

        label.Links.Add(6, 3, "https://example.com");

        Assert.Equal(before, label.Text);
    }

    [Fact]
    public void Add_RejectsANullLink()
    {
        using var label = Labelled();

        Assert.Throws<ArgumentNullException>(() => label.Links.Add(null!));
    }

    [Fact]
    public void IndexOf_ReportsThePosition()
    {
        using var label = Labelled();
        label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);

        Assert.Equal(1, label.Links.IndexOf(second));
        Assert.Equal(-1, label.Links.IndexOf(new LinkLabel.Link(99, 1)));
    }

    [Fact]
    public void Insert_PlacesTheLinkAtThatPosition()
    {
        using var label = Labelled();
        var first = label.Links.Add(0, 5);
        var third = label.Links.Add(10, 4);

        var second = new LinkLabel.Link(6, 3);
        label.Links.Insert(1, second);

        Assert.Equal(3, label.Links.Count);
        Assert.Same(first, label.Links[0]);
        Assert.Same(second, label.Links[1]);
        Assert.Same(third, label.Links[2]);
    }

    [Fact]
    public void Remove_DropsTheLinkAndReindexesTheRest()
    {
        using var label = Labelled();
        var first = label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);
        var third = label.Links.Add(10, 4);

        label.Links.Remove(second);

        Assert.Equal(2, label.Links.Count);
        Assert.Same(first, label.Links[0]);
        Assert.Same(third, label.Links[1]);
        Assert.False(label.Links.Contains(second), "the removed link should be gone");
    }

    [Fact]
    public void RemoveAt_DropsThatPosition()
    {
        using var label = Labelled();
        label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);

        label.Links.RemoveAt(0);

        Assert.Single(label.Links);
        Assert.Same(second, label.Links[0]);
    }

    [Fact]
    public void Clear_EmptiesTheCollection()
    {
        using var label = Labelled();
        label.Links.Add(0, 5);
        label.Links.Add(6, 3);

        label.Links.Clear();

        Assert.Empty(label.Links);
    }

    [Fact]
    public void TheIndexerCanReplaceALink()
    {
        using var label = Labelled();
        label.Links.Add(0, 5);
        var replacement = new LinkLabel.Link(6, 3);

        label.Links[0] = replacement;

        Assert.Same(replacement, label.Links[0]);
    }

    [Fact]
    public void Enumeration_YieldsEveryLinkInOrder()
    {
        using var label = Labelled();
        var first = label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);

        var seen = new List<LinkLabel.Link>();
        foreach (LinkLabel.Link link in label.Links)
        {
            seen.Add(link);
        }

        Assert.Equal(new[] { first, second }, seen);
    }

    [Fact]
    public void CopyTo_CopiesEveryLink()
    {
        using var label = Labelled();
        var first = label.Links.Add(0, 5);
        var second = label.Links.Add(6, 3);

        var target = new LinkLabel.Link[2];
        label.Links.CopyTo(target, 0);

        Assert.Same(first, target[0]);
        Assert.Same(second, target[1]);
    }

    [Fact]
    public void TheKeyedLookupFindsALinkByName()
    {
        using var label = Labelled();
        label.Links.Add(0, 5);
        var named = label.Links.Add(6, 3);
        named.Name = "site";

        Assert.True(label.Links.ContainsKey("site"), "the named link should be found");
        Assert.Equal(1, label.Links.IndexOfKey("site"));
        Assert.Same(named, label.Links["site"]);
    }

    [Fact]
    public void TheKeyedLookupIsCaseInsensitiveAndMissesAreNull()
    {
        using var label = Labelled();
        var named = label.Links.Add(0, 5);
        named.Name = "Home";

        Assert.Same(named, label.Links["home"]);
        Assert.Null(label.Links["nothing"]);
        Assert.Equal(-1, label.Links.IndexOfKey("nothing"));
    }

    [Fact]
    public void RemoveByKey_DropsTheNamedLink()
    {
        using var label = Labelled();
        var keep = label.Links.Add(0, 5);
        keep.Name = "keep";
        var drop = label.Links.Add(6, 3);
        drop.Name = "drop";

        label.Links.RemoveByKey("drop");

        Assert.Single(label.Links);
        Assert.Same(keep, label.Links[0]);
    }

    [Fact]
    public void TheCollectionReportsItsCapabilities()
    {
        using var label = Labelled();

        Assert.False(label.Links.IsReadOnly, "a link collection is writable");
        Assert.False(label.Links.IsFixedSize, "a link collection can grow");
        Assert.NotNull(label.Links.SyncRoot);
    }

    [Fact]
    public void LinksAdded_TracksWhetherThereAreAnyLinks()
    {
        using var label = Labelled();

        Assert.False(label.Links.LinksAdded, "a fresh collection has no links");

        label.Links.Add(0, 5);

        Assert.True(label.Links.LinksAdded, "after adding, the collection reports links");
    }

    // ---- The IList face --------------------------------------------------

    [Fact]
    public void TheIListFaceAddsAndLocatesLinks()
    {
        using var label = Labelled();
        IList list = label.Links;
        var link = new LinkLabel.Link(6, 3);

        var index = list.Add(link);

        Assert.Equal(0, index);
        Assert.True(list.Contains(link), "the IList face should see the link");
        Assert.Equal(0, list.IndexOf(link));
        Assert.Same(link, list[0]);
    }

    [Fact]
    public void TheIListFaceRejectsSomethingThatIsNotALink()
    {
        using var label = Labelled();
        IList list = label.Links;

        Assert.Throws<ArgumentException>(() => list.Add("https://example.com"));
    }

    [Fact]
    public void TheIListFaceIgnoresARemoveOfSomethingThatIsNotALink()
    {
        using var label = Labelled();
        IList list = label.Links;
        list.Add(new LinkLabel.Link(0, 5));

        list.Remove("not a link");

        Assert.Single(label.Links);
    }

    // ---- The event carries the link --------------------------------------

    [Fact]
    public void LinkClicked_CarriesTheLinkThatWasClicked()
    {
        using var label = Labelled();
        var link = label.Links.Add(6, 3, "https://example.com");
        LinkLabel.Link? received = null;
        label.LinkClicked += (_, e) => received = e.Link;

        label.OnLinkClicked(new LinkLabelLinkClickedEventArgs(link));

        Assert.Same(link, received);
        Assert.Equal("https://example.com", received!.LinkData);
    }

    [Fact]
    public void AMultiLinkLabelIsNowExpressible()
    {
        // The whole point of the control over a plain Label, and it was impossible before.
        using var label = new LinkLabel { Text = "visit our site or email us today" };

        var site = label.Links.Add(10, 4, "https://example.com");
        var email = label.Links.Add(24, 5, "mailto:us@example.com");
        site.Name = "site";
        email.Name = "email";

        Assert.Equal(2, label.Links.Count);
        Assert.Equal("site", label.Links["site"]!.Name);
        Assert.Equal("mailto:us@example.com", label.Links["email"]!.LinkData);
        Assert.Equal("visit our site or email us today", label.Text);
    }
}
