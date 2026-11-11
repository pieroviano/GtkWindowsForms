using System.Drawing;
using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="HtmlDocument"/> over a fixed snapshot. The snapshot is checked in as
/// JSON rather than built in code, so these tests also cover the parser that turns what the injected page
/// script emits into the cached model every Html* getter reads.
/// </summary>
[Collection("NonParallelCollection")]
public class HtmlDocumentSnapshotTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    private readonly WebBrowser _browser;

    /// <summary>Loads the fixture snapshot and completes a navigation, so <c>Document</c> is available.</summary>
    public HtmlDocumentSnapshotTests()
    {
        Application.Init();
        WebViewBackendFactory.Clear();
        _backend = new FakeWebViewBackend();
        WebViewBackendFactory.Register("fake", 1000, () => _backend);

        _backend.FakeDom.Snapshot = LoadFixture();
        _browser = new WebBrowser();
        _browser.Navigate("https://example.com/fixture");
        _backend.CompleteNavigation(new Uri("https://example.com/fixture"));
    }

    /// <summary>Loads the checked-in fixture that the parser and every getter here read.</summary>
    internal static DomSnapshot LoadFixture()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "WebBrowserDomSnapshot.json");
        return DomSnapshot.FromJson(File.ReadAllText(path));
    }

    /// <summary>Disposes the control and leaves the registry empty.</summary>
    public void Dispose()
    {
        _browser.Dispose();
        WebViewBackendFactory.Clear();
        GC.SuppressFinalize(this);
    }

    private HtmlDocument Document => _browser.Document!;

    [Fact]
    public void The_Fixture_Parses_Into_A_Non_Empty_Snapshot()
    {
        var snapshot = LoadFixture();

        Assert.False(snapshot.IsEmpty);
        Assert.Equal(7, snapshot.Nodes.Count);
        Assert.Equal(1, snapshot.DocumentElementHandle);
        Assert.Equal(2, snapshot.BodyHandle);
    }

    [Fact]
    public void Malformed_Json_Yields_The_Empty_Snapshot_Rather_Than_Throwing()
    {
        Assert.True(DomSnapshot.FromJson("{ not json").IsEmpty);
        Assert.True(DomSnapshot.FromJson("[1, 2, 3]").IsEmpty);
        Assert.True(DomSnapshot.FromJson(string.Empty).IsEmpty);
        Assert.True(DomSnapshot.FromJson(null).IsEmpty);
    }

    [Fact]
    public void Document_Level_Values_Come_From_The_Snapshot()
    {
        Assert.Equal("Snapshot fixture", Document.Title);
        Assert.Equal(new Uri("https://example.com/fixture"), Document.Url);
        Assert.Equal("utf-8", Document.Encoding);
        Assert.Equal("utf-8", Document.DefaultEncoding);
        Assert.Equal("a=1", Document.Cookie);
        Assert.Equal("example.com", Document.Domain);
    }

    [Fact]
    public void Colours_Are_Parsed_From_Both_Hex_And_Rgb_Notation()
    {
        Assert.Equal(Color.FromArgb(255, 255, 255, 255), Document.BackColor);
        Assert.Equal(Color.FromArgb(255, 0x10, 0x20, 0x30), Document.ForeColor);
        Assert.Equal(Color.FromArgb(255, 0, 0, 238), Document.LinkColor);
        Assert.Equal(Color.FromArgb(255, 255, 0, 0), Document.ActiveLinkColor);
        Assert.Equal(Color.FromArgb(255, 0x55, 0x1A, 0x8B), Document.VisitedLinkColor);
    }

    [Fact]
    public void Body_And_ActiveElement_Resolve()
    {
        Assert.Equal("BODY", Document.Body!.TagName);
        Assert.Equal("INPUT", Document.ActiveElement!.TagName);
    }

    [Fact]
    public void GetElementById_Finds_The_Element_And_Returns_Null_For_A_Miss()
    {
        Assert.Equal("H1", Document.GetElementById("heading")!.TagName);
        Assert.Null(Document.GetElementById("no-such-id"));
        Assert.Null(Document.GetElementById(string.Empty));
    }

    [Fact]
    public void GetElementsByTagName_Walks_The_Whole_Document()
    {
        Assert.Single(Document.GetElementsByTagName("h1"));
        Assert.Single(Document.GetElementsByTagName("H1"));
        Assert.Empty(Document.GetElementsByTagName("table"));
    }

    [Fact]
    public void Forms_Images_And_Links_Select_The_Right_Elements()
    {
        Assert.Equal("FORM", Assert.IsType<HtmlElement>(Document.Forms[0]).TagName);
        Assert.Single(Document.Forms);

        Assert.Equal("IMG", Assert.IsType<HtmlElement>(Document.Images[0]).TagName);
        Assert.Single(Document.Images);

        Assert.Single(Document.Links);
        Assert.Equal("https://example.org/", Document.Links[0]!.GetAttribute("href"));
    }

    [Fact]
    public void All_Includes_The_Document_Element_And_Every_Descendant()
    {
        Assert.Equal(7, Document.All.Count);
    }

    [Fact]
    public void RightToLeft_Reads_And_Writes_The_Dir_Attribute()
    {
        Assert.False(Document.RightToLeft);

        Document.RightToLeft = true;

        Assert.True(Document.RightToLeft);
        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.SetAttribute, mutation.Kind);
        Assert.Equal("dir", mutation.Name);
        Assert.Equal("rtl", mutation.Value);
    }

    [Fact]
    public void Title_Set_Updates_The_Snapshot_And_Posts_One_Mutation()
    {
        Document.Title = "Renamed";

        Assert.Equal("Renamed", Document.Title);
        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.SetDocumentProperty, mutation.Kind);
        Assert.Equal("title", mutation.Name);
    }

    [Fact]
    public void Write_Posts_The_Markup()
    {
        Document.Write("<p>appended</p>");

        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.Write, mutation.Kind);
        Assert.Equal("<p>appended</p>", mutation.Value);
    }

    [Fact]
    public void CreateElement_Registers_A_Detached_Node_That_Cannot_Collide_With_The_Page()
    {
        var created = Document.CreateElement("div");

        Assert.Equal("DIV", created.TagName);
        Assert.Null(created.Parent);
        Assert.True(created.Node.Handle < DomNode.NoHandle);

        var mutation = Assert.Single(_backend.FakeDom.Mutations);
        Assert.Equal(DomMutationKind.CreateElement, mutation.Kind);
        Assert.Equal("div", mutation.Name);
    }

    [Fact]
    public void InvokeScript_Posts_The_Call_And_Returns_Null()
    {
        Assert.Null(Document.InvokeScript("doThing", new object[] { 1, "two" }));

        var invocation = Assert.Single(_backend.FakeDom.Invocations);
        Assert.Equal("doThing", invocation.Member);
        Assert.Equal(new object[] { 1, "two" }, invocation.Args);
    }

    [Fact]
    public void GetElementFromPoint_Picks_The_Innermost_Element_Containing_It()
    {
        Assert.Equal("H1", Document.GetElementFromPoint(new Point(20, 20))!.TagName);
        Assert.Equal("A", Document.GetElementFromPoint(new Point(20, 65))!.TagName);
        Assert.Equal("BODY", Document.GetElementFromPoint(new Point(500, 500))!.TagName);
        Assert.Null(Document.GetElementFromPoint(new Point(5000, 5000)));
    }

    [Fact]
    public void An_Attached_Handler_Sees_The_Page_Event()
    {
        var seen = 0;
        Document.AttachEventHandler("onclick", (_, _) => seen++);

        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(2, "click"));

        Assert.Equal(1, seen);
    }

    [Fact]
    public void The_Click_Event_Carries_The_Page_Event_Detail()
    {
        HtmlElementEventArgs? seen = null;
        Document.Click += (_, e) => seen = e;

        _backend.FakeDom.RaiseDomEvent(new DomEventArgs(2, "click")
        {
            CtrlKeyPressed = true,
            KeyPressedCode = 13,
            Buttons = MouseButtons.Left,
            MousePosition = new Point(4, 5),
            FromHandle = 3,
            ToHandle = 4
        });

        Assert.NotNull(seen);
        Assert.Equal("click", seen!.EventType);
        Assert.True(seen.CtrlKeyPressed);
        Assert.Equal(13, seen.KeyPressedCode);
        Assert.Equal(MouseButtons.Left, seen.MouseButtonsPressed);
        Assert.Equal(new Point(4, 5), seen.MousePosition);
        Assert.Equal("H1", seen.FromElement!.TagName);
        Assert.Equal("A", seen.ToElement!.TagName);
    }

    [Fact]
    public void Documents_Over_The_Same_Page_Are_Equal()
    {
        var first = _browser.Document;
        var second = _browser.Document;

        Assert.True(first == second);
        Assert.False(first != second);
        Assert.True(first!.Equals(second));
        Assert.Equal(first.GetHashCode(), second!.GetHashCode());
    }
}
