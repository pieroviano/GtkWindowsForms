using System.Collections;
using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="HtmlElementCollection"/>. The indexers matter most: real WinForms
/// returns <c>null</c> for an out-of-range index or an unknown name rather than throwing, and code
/// ported from it relies on that.
/// </summary>
[Collection("NonParallelCollection")]
public class HtmlElementCollectionTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    private readonly WebBrowser _browser;

    /// <summary>Loads the fixture snapshot and completes a navigation, so <c>Document</c> is available.</summary>
    public HtmlElementCollectionTests()
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

    private HtmlElementCollection BodyChildren => _browser.Document!.Body!.Children;

    [Fact]
    public void Count_Matches_The_Snapshot()
    {
        Assert.Equal(4, BodyChildren.Count);
    }

    [Fact]
    public void The_Integer_Indexer_Returns_Null_Out_Of_Range_Rather_Than_Throwing()
    {
        Assert.Equal("H1", BodyChildren[0]!.TagName);
        Assert.Equal("IMG", BodyChildren[3]!.TagName);
        Assert.Null(BodyChildren[4]);
        Assert.Null(BodyChildren[-1]);
    }

    [Fact]
    public void The_String_Indexer_Matches_Id_Then_Name_And_Returns_Null_For_A_Miss()
    {
        Assert.Equal("H1", BodyChildren["heading"]!.TagName);
        Assert.Equal("A", BodyChildren["away"]!.TagName);
        Assert.Null(BodyChildren["no-such-thing"]);
        Assert.Null(BodyChildren[null!]);
    }

    [Fact]
    public void GetElementsByName_Selects_By_The_Name_Attribute_Only()
    {
        var byName = BodyChildren.GetElementsByName("away");

        Assert.Equal(1, byName.Count);
        Assert.Equal("A", byName[0]!.TagName);

        Assert.Equal(0, BodyChildren.GetElementsByName("heading").Count);
        Assert.Equal(0, BodyChildren.GetElementsByName("nothing").Count);
    }

    [Fact]
    public void The_Collection_Enumerates_In_Document_Order()
    {
        var tags = new List<string>();
        foreach (HtmlElement element in BodyChildren)
        {
            tags.Add(element.TagName);
        }

        Assert.Equal(new[] { "H1", "A", "FORM", "IMG" }, tags);
    }

    [Fact]
    public void CopyTo_Fills_An_Array()
    {
        var target = new HtmlElement[4];

        ((ICollection)BodyChildren).CopyTo(target, 0);

        Assert.Equal("H1", target[0].TagName);
        Assert.Equal("IMG", target[3].TagName);
    }

    [Fact]
    public void The_Collection_Reports_Itself_As_Unsynchronised()
    {
        var children = BodyChildren;
        var collection = (ICollection)children;

        Assert.False(collection.IsSynchronized);
        Assert.Same(children, collection.SyncRoot);
    }

    [Fact]
    public void An_Empty_Collection_Is_Well_Behaved()
    {
        var empty = _browser.Document!.GetElementsByTagName("table");

        Assert.Equal(0, empty.Count);
        Assert.Null(empty[0]);
        Assert.Null(empty["anything"]);
        Assert.False(empty.GetEnumerator().MoveNext());
    }
}
