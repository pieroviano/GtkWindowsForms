using System.Drawing;
using System.Windows.Forms;
using Gtk.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="HtmlElement"/>. Two things are being pinned here: that a getter reads
/// the cached snapshot rather than nothing, and that a setter both updates that cache - so the value
/// reads back at once - and posts exactly one change to the page. Posting twice would double-apply an
/// edit; posting none would silently drop it.
/// </summary>
[Collection("NonParallelCollection")]
public class HtmlElementTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    private readonly WebBrowser _browser;

    /// <summary>Loads the fixture snapshot and completes a navigation, so <c>Document</c> is available.</summary>
    public HtmlElementTests()
    {
        Application.Init();
        WebViewBackendFactory.Clear();
        _backend = new FakeWebViewBackend();
        WebViewBackendFactory.Register("fake", 1000, () => _backend);

        _backend.FakeDom.Snapshot = HtmlDocumentSnapshotTests.LoadFixture();
        _browser = new WebBrowser();
        _browser.Navigate("https://example.com/fixture");
        _backend.CompleteNavigation(new Uri("https://example.com/fixture"));
    }

    /// <summary>Disposes the control and leaves the registry empty.</summary>
    public void Dispose()
    {
        _browser.Dispose();
        WebViewBackendFactory.Clear();
        GC.SuppressFinalize(this);
    }

    private HtmlDocument Document => _browser.Document!;

    private HtmlElement Heading => Document.GetElementById("heading")!;

    [Fact]
    public void Getters_Read_The_Snapshot()
    {
        Assert.Equal("H1", Heading.TagName);
        Assert.Equal("heading", Heading.Id);
        Assert.Equal("Fixture heading", Heading.InnerText);
        Assert.Equal("Fixture heading", Heading.InnerHtml);
        Assert.Equal("<h1 id=\"heading\" class=\"title\">Fixture heading</h1>", Heading.OuterHtml);
        Assert.Equal("color: red", Heading.Style);
        Assert.Equal((short)3, Heading.TabIndex);
        Assert.Equal("title", Heading.GetAttribute("class"));
        Assert.Equal(new Rectangle(10, 10, 200, 40), Heading.OffsetRectangle);
    }

    [Fact]
    public void A_Missing_Attribute_Reads_As_The_Empty_String()
    {
        Assert.Equal(string.Empty, Heading.GetAttribute("data-nothing"));
        Assert.False(Heading.HasAttribute("data-nothing"));
    }

    [Fact]
    public void Enabled_Is_The_Inverse_Of_The_Disabled_Flag()
    {
        Assert.False(Document.GetElementById("field")!.Enabled);
        Assert.True(Heading.Enabled);
    }

    [Fact]
    public void SetAttribute_Updates_The_Snapshot_And_Posts_Exactly_One_Mutation()
    {
        Heading.SetAttribute("class", "changed");

        Assert.Equal("changed", Heading.GetAttribute("class"));

        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.SetAttribute, mutation.Kind);
        Assert.Equal("class", mutation.Name);
        Assert.Equal("changed", mutation.Value);
        Assert.Equal(Heading.Node.Handle, mutation.TargetHandle);
    }

    [Fact]
    public void Setting_The_Style_Attribute_Also_Moves_The_Style_Property()
    {
        Heading.SetAttribute("style", "color: blue");

        Assert.Equal("color: blue", Heading.Style);
    }

    [Theory]
    [InlineData("InnerText", DomMutationKind.SetInnerText)]
    [InlineData("InnerHtml", DomMutationKind.SetInnerHtml)]
    [InlineData("OuterHtml", DomMutationKind.SetOuterHtml)]
    [InlineData("OuterText", DomMutationKind.SetOuterText)]
    [InlineData("Style", DomMutationKind.SetStyle)]
    public void Each_String_Setter_Caches_The_Value_And_Posts_One_Mutation(string property, DomMutationKind kind)
    {
        var element = Heading;
        var info = typeof(HtmlElement).GetProperty(property)!;

        info.SetValue(element, "written");

        Assert.Equal("written", info.GetValue(element));
        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(kind, mutation.Kind);
        Assert.Equal("written", mutation.Value);
    }

    [Fact]
    public void TabIndex_And_Enabled_Setters_Cache_And_Post()
    {
        Heading.TabIndex = 9;
        Heading.Enabled = false;

        Assert.Equal((short)9, Heading.TabIndex);
        Assert.False(Heading.Enabled);

        Assert.Equal(2, _backend.FakeDom.Mutations.Count);
        Assert.Equal(DomMutationKind.SetTabIndex, _backend.FakeDom.Mutations[0].Kind);
        Assert.Equal(9, _backend.FakeDom.Mutations[0].IntValue);
        Assert.Equal(DomMutationKind.SetDisabled, _backend.FakeDom.Mutations[1].Kind);
        Assert.True(_backend.FakeDom.Mutations[1].Flag);
    }

    [Fact]
    public void Scroll_Setters_Move_Both_The_Offset_And_The_Scroll_Rectangle()
    {
        var body = Document.Body!;

        body.ScrollLeft = 12;
        body.ScrollTop = 34;

        Assert.Equal(12, body.ScrollLeft);
        Assert.Equal(34, body.ScrollTop);
        Assert.Equal(12, body.ScrollRectangle.X);
        Assert.Equal(34, body.ScrollRectangle.Y);
        Assert.Equal(800, body.ScrollRectangle.Width);
        Assert.Equal(1200, body.ScrollRectangle.Height);
    }

    [Fact]
    public void Tree_Navigation_Is_Consistent()
    {
        var body = Document.Body!;

        Assert.Equal("HTML", body.Parent!.TagName);
        Assert.Null(body.Parent.Parent);

        Assert.Equal("H1", body.FirstChild!.TagName);
        Assert.Equal("A", body.FirstChild.NextSibling!.TagName);
        Assert.Equal("FORM", body.FirstChild.NextSibling.NextSibling!.TagName);
        Assert.Equal("IMG", body.FirstChild.NextSibling.NextSibling.NextSibling!.TagName);
        Assert.Null(body.FirstChild.NextSibling.NextSibling.NextSibling.NextSibling);

        Assert.Equal(4, body.Children.Count);
        Assert.Equal(5, body.All.Count);
        Assert.Null(Heading.FirstChild);
    }

    [Fact]
    public void Two_Elements_Over_The_Same_Node_Are_Equal()
    {
        var first = Document.GetElementById("heading")!;
        var second = Document.All["heading"]!;

        Assert.True(first == second);
        Assert.False(first != second);
        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Elements_Over_Different_Nodes_Are_Not_Equal()
    {
        var heading = Document.GetElementById("heading")!;
        var body = Document.Body!;

        Assert.False(heading == body);
        Assert.True(heading != body);
        Assert.False(heading.Equals(body));
    }

    [Fact]
    public void Comparing_With_Null_Does_Not_Throw()
    {
        HtmlElement? nothing = null;

        Assert.False(Heading == nothing);
        Assert.True(Heading != nothing);
        Assert.True(nothing == null);
        Assert.False(Heading.Equals(null));
    }

    [Fact]
    public void CanHaveChildren_Follows_The_Html_Void_Element_List()
    {
        Assert.True(Heading.CanHaveChildren);
        Assert.False(Document.GetElementById("picture")!.CanHaveChildren);
        Assert.False(Document.GetElementById("field")!.CanHaveChildren);
    }

    [Fact]
    public void AppendChild_Reparents_In_The_Snapshot_And_Posts_One_Mutation()
    {
        var created = Document.CreateElement("span");
        _backend.FakeDom.Mutations.Clear();

        var returned = Heading.AppendChild(created);

        Assert.Same(created, returned);
        Assert.Equal("H1", created.Parent!.TagName);
        Assert.Single(Heading.Children);

        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.AppendChild, mutation.Kind);
        Assert.Equal(created.Node.Handle, mutation.RelatedHandle);
    }

    [Fact]
    public void InsertAdjacentElement_Places_The_Element_Where_Asked()
    {
        var before = Document.CreateElement("span");
        Heading.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, before);
        Assert.Equal("SPAN", Document.Body!.FirstChild!.TagName);

        var after = Document.CreateElement("em");
        Heading.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, after);
        Assert.Equal("EM", Heading.NextSibling!.TagName);

        var firstChild = Document.CreateElement("b");
        Heading.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, firstChild);
        Assert.Equal("B", Heading.FirstChild!.TagName);

        var lastChild = Document.CreateElement("i");
        Heading.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, lastChild);
        Assert.Equal("I", Heading.Children[Heading.Children.Count - 1]!.TagName);
    }

    [Fact]
    public void A_Sibling_Insertion_Against_A_Rootless_Element_Returns_Null()
    {
        var detached = Document.CreateElement("div");
        var other = Document.CreateElement("span");

        Assert.Null(detached.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, other));
    }

    [Fact]
    public void InvokeMember_Posts_The_Call_And_Returns_Null()
    {
        Assert.Null(Heading.InvokeMember("scrollIntoView"));
        Assert.Null(Heading.InvokeMember("setAttribute", "a", "b"));

        Assert.Equal(2, _backend.FakeDom.Invocations.Count);
        Assert.Equal("scrollIntoView", _backend.FakeDom.Invocations[0].Member);
        Assert.Equal(new object[] { "a", "b" }, _backend.FakeDom.Invocations[1].Args);
    }

    [Fact]
    public void Focus_Blur_ScrollIntoView_And_RaiseEvent_Post_The_Matching_Mutation()
    {
        Heading.Focus();
        Heading.RemoveFocus();
        Heading.ScrollIntoView(true);
        Heading.RaiseEvent("onclick");

        var kinds = _backend.FakeDom.Mutations.Select(m => m.Kind).ToArray();
        Assert.Equal(
            new[]
            {
                DomMutationKind.Focus, DomMutationKind.Blur, DomMutationKind.ScrollIntoView,
                DomMutationKind.RaiseEvent
            },
            kinds);
        Assert.True(_backend.FakeDom.Mutations[2].Flag);
        Assert.Equal("click", _backend.FakeDom.Mutations[3].Name);
    }

    [Fact]
    public void Element_Events_Fire_Only_For_Their_Own_Node()
    {
        var seen = 0;
        Heading.Click += (_, _) => seen++;

        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(Document.Body!.Node.Handle, "click"));
        Assert.Equal(0, seen);

        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(Heading.Node.Handle, "click"));
        Assert.Equal(1, seen);
    }

    [Fact]
    public void A_Detached_Handler_Stops_Receiving_Events()
    {
        var seen = 0;
        void Handler(object? sender, EventArgs e) => seen++;

        Heading.AttachEventHandler("click", Handler);
        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(Heading.Node.Handle, "click"));
        Assert.Equal(1, seen);

        Heading.DetachEventHandler("click", Handler);
        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(Heading.Node.Handle, "click"));
        Assert.Equal(1, seen);
    }

    [Fact]
    public void An_Element_Whose_Node_Left_The_Snapshot_Reads_Empty_Rather_Than_Throwing()
    {
        var heading = Heading;

        _backend.FakeDom.Snapshot = new DomSnapshot();

        Assert.Equal(string.Empty, heading.TagName);
        Assert.Equal(string.Empty, heading.InnerText);
        Assert.Null(heading.Parent);
        Assert.Empty(heading.Children);
    }
}
