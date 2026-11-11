using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// The members of the Html* surface that cannot work here must throw, and must say why. Returning
/// <c>null</c> instead would let a port compile and then fail somewhere far from the cause; a later
/// refactor quietly turning one of these into a null return is exactly what these tests exist to catch.
/// The reason is always the same: both engines run the page in a separate process, so there is no
/// in-process DOM object to hand out, and one control hosts exactly one engine view.
/// </summary>
[Collection("NonParallelCollection")]
public class HtmlUnsupportedMembersTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    private readonly WebBrowser _browser;

    /// <summary>Loads the fixture snapshot and completes a navigation, so <c>Document</c> is available.</summary>
    public HtmlUnsupportedMembersTests()
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

    [Fact]
    public void DomDocument_Throws_And_Names_The_Process_Split()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _ = Document.DomDocument);

        Assert.Contains("separate process", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DomElement_Throws_And_Names_The_Process_Split()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _ = Document.Body!.DomElement);

        Assert.Contains("separate process", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DomWindow_Throws_And_Names_The_Process_Split()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _ = Document.Window.DomWindow);

        Assert.Contains("separate process", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DomHistory_Throws_And_Names_The_Process_Split()
    {
        var exception = Assert.Throws<NotSupportedException>(() => _ = Document.Window.History.DomHistory);

        Assert.Contains("separate process", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HtmlWindow_Open_Throws_And_Points_At_The_NewWindow_Event()
    {
        var window = Document.Window;

        var exception = Assert.Throws<NotSupportedException>(
            () => window.Open("https://example.com/", "_blank", string.Empty, false));
        Assert.Contains("NewWindow", exception.Message, StringComparison.Ordinal);

        Assert.Throws<NotSupportedException>(
            () => window.Open(new Uri("https://example.com/"), "_blank", string.Empty, false));
        Assert.Throws<NotSupportedException>(() => window.OpenNew("https://example.com/", string.Empty));
        Assert.Throws<NotSupportedException>(() => window.OpenNew(new Uri("https://example.com/"), string.Empty));
    }

    [Fact]
    public void HtmlWindow_Close_Move_And_Resize_Throw_Rather_Than_Doing_Nothing()
    {
        var window = Document.Window;

        Assert.Throws<NotSupportedException>(() => window.Close());
        Assert.Throws<NotSupportedException>(() => window.MoveTo(1, 2));
        Assert.Throws<NotSupportedException>(() => window.ResizeTo(3, 4));
    }

    [Fact]
    public void Setting_The_Document_Domain_Throws_As_The_Dom_Specification_Requires()
    {
        Assert.Throws<NotSupportedException>(() => Document.Domain = "example.org");
    }

    [Fact]
    public void The_Window_That_Cannot_Have_Frames_Reports_An_Empty_Collection_Not_Null()
    {
        Assert.Empty(Document.Window.Frames);
        Assert.Null(Document.Window.Opener);
        Assert.False(Document.Window.IsClosed);
        Assert.Same(Document.Window.Parent, Document.Window.Parent.Parent);
    }

    [Fact]
    public void History_Steps_One_Entry_At_A_Time_Through_The_Engine()
    {
        _backend.CanGoBack = true;
        _backend.CanGoForward = true;

        using var history = Document.Window.History;

        history.Back(2);
        history.Forward(3);
        history.Go(-1);
        history.Go(1);

        Assert.Equal(3, _backend.BackCount);
        Assert.Equal(4, _backend.ForwardCount);
    }

    [Fact]
    public void History_Rejects_A_Negative_Step()
    {
        using var history = Document.Window.History;

        Assert.Throws<ArgumentOutOfRangeException>(() => history.Back(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => history.Forward(-1));
    }

    [Fact]
    public void History_Length_Counts_What_This_Control_Navigated()
    {
        using var history = Document.Window.History;

        Assert.Equal(1, history.Length);

        _browser.Navigate("https://example.com/second");
        _backend.CompleteNavigation(new Uri("https://example.com/second"));

        Assert.Equal(2, Document.Window.History.Length);
    }
}
