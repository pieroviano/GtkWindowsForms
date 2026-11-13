using System.Windows.Forms;
using Gtk.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// The asynchronous siblings live in Gtk.Windows.Extensions, which this assembly does not reference, so
/// these tests exercise the seam they are built on: that the internals they reach through
/// (<see cref="WebBrowserBase.Backend"/>, <see cref="HtmlElement.Bridge"/>,
/// <see cref="HtmlElement.Handle"/>) address the right engine and the right node, and that the defaults
/// <c>WebBrowserDefaults</c> forwards to actually drive <see cref="WebBrowser.GoHome"/>.
/// </summary>
[Collection("NonParallelCollection")]
public class WebBrowserAsyncExtensionsTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    /// <summary>Registers the fake as the only backend so the control never reaches a real engine.</summary>
    public WebBrowserAsyncExtensionsTests()
    {
        Application.Init();
        WebViewBackendFactory.Clear();
        _backend = new FakeWebViewBackend();
        WebViewBackendFactory.Register("fake", 1000, () => _backend);
    }

    /// <summary>Leaves the registry empty and the defaults as they were.</summary>
    public void Dispose()
    {
        WebViewBackendFactory.Clear();
        WebBrowser.HomeUrl = new Uri("about:blank");
        WebBrowser.SearchUrl = new Uri("about:blank");
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void The_Control_Exposes_Its_Engine_To_The_Extensions_Package()
    {
        using var browser = new WebBrowser();

        Assert.Same(_backend, browser.Backend);
        Assert.Same(_backend.FakeDom, browser.Backend.Dom);
    }

    [Fact]
    public void An_Element_Exposes_Its_Bridge_And_Handle()
    {
        _backend.FakeDom.Snapshot = HtmlDocumentSnapshotTests.LoadFixture();

        using var browser = new WebBrowser();
        browser.Navigate("https://example.com/fixture");
        _backend.CompleteNavigation(new Uri("https://example.com/fixture"));

        var heading = browser.Document!.GetElementById("heading")!;

        Assert.Same(_backend.FakeDom, heading.Bridge);
        Assert.Equal(3, heading.Handle);
    }

    [Fact]
    public void The_Home_And_Search_Defaults_Are_Blank_Until_An_Application_Sets_Them()
    {
        Assert.Equal(new Uri("about:blank"), WebBrowser.HomeUrl);
        Assert.Equal(new Uri("about:blank"), WebBrowser.SearchUrl);
    }

    [Fact]
    public void GoHome_Follows_The_Configured_Default()
    {
        WebBrowser.HomeUrl = new Uri("https://intranet.example/start");

        using var browser = new WebBrowser();
        browser.GoHome();

        Assert.Equal(new Uri("https://intranet.example/start"), Assert.Single(_backend.Navigations));
    }
}
