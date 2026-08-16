using System.Text;
using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="WebBrowser"/>'s navigation contract. What applications ported from
/// real WinForms depend on is the event sequence - Navigating, then Navigated, then DocumentCompleted,
/// once each - and that cancelling the first suppresses the rest. They also depend on the URL
/// normalisation real WinForms performs, which is why a bare host name still navigates.
/// </summary>
[Collection("NonParallelCollection")]
public class WebBrowserNavigationTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    /// <summary>Registers the fake as the only backend so the control never reaches a real engine.</summary>
    public WebBrowserNavigationTests()
    {
        Application.Init();
        WebViewBackendFactory.Clear();
        _backend = new FakeWebViewBackend();
        WebViewBackendFactory.Register("fake", 1000, () => _backend);
    }

    /// <summary>Leaves the registry empty for the next test in the collection.</summary>
    public void Dispose()
    {
        WebViewBackendFactory.Clear();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Navigate_Raises_Navigating_Navigated_DocumentCompleted_Once_Each_In_Order()
    {
        using var browser = new WebBrowser();
        var order = new List<string>();

        browser.Navigating += (_, _) => order.Add("navigating");
        browser.Navigated += (_, _) => order.Add("navigated");
        browser.DocumentCompleted += (_, _) => order.Add("completed");

        browser.Navigate("https://example.com/");
        _backend.CompleteNavigation(new Uri("https://example.com/"));

        Assert.Equal(new[] { "navigating", "navigated", "completed" }, order);
    }

    [Fact]
    public void Cancelling_Navigating_Suppresses_The_Navigation()
    {
        using var browser = new WebBrowser();
        var navigated = 0;
        var completed = 0;

        browser.Navigating += (_, e) => e.Cancel = true;
        browser.Navigated += (_, _) => navigated++;
        browser.DocumentCompleted += (_, _) => completed++;

        browser.Navigate("https://example.com/");

        Assert.Empty(_backend.Navigations);
        Assert.Equal(0, navigated);
        Assert.Equal(0, completed);
        Assert.Null(browser.Url);
    }

    [Fact]
    public void Navigating_Reports_The_Target_Frame_And_Url()
    {
        using var browser = new WebBrowser();
        WebBrowserNavigatingEventArgs? seen = null;
        browser.Navigating += (_, e) => seen = e;

        browser.Navigate(new Uri("https://example.com/page"), "content");

        Assert.NotNull(seen);
        Assert.Equal(new Uri("https://example.com/page"), seen!.Url);
        Assert.Equal("content", seen.TargetFrameName);
    }

    [Fact]
    public void AllowNavigation_False_Blocks_Only_The_Second_Navigation()
    {
        using var browser = new WebBrowser { AllowNavigation = false };

        browser.Navigate("https://example.com/first");
        browser.Navigate("https://example.com/second");

        Assert.Equal(new[] { new Uri("https://example.com/first") }, _backend.Navigations);
    }

    [Fact]
    public void AllowNavigation_Defaults_To_True_And_Permits_Repeated_Navigation()
    {
        using var browser = new WebBrowser();

        Assert.True(browser.AllowNavigation);

        browser.Navigate("https://example.com/first");
        browser.Navigate("https://example.com/second");

        Assert.Equal(2, _backend.Navigations.Count);
    }

    [Theory]
    [InlineData("example.com", "http://example.com/")]
    [InlineData("https://example.com/x", "https://example.com/x")]
    [InlineData("  example.com  ", "http://example.com/")]
    [InlineData("", "about:blank")]
    [InlineData("   ", "about:blank")]
    [InlineData(null, "about:blank")]
    public void Navigate_Normalises_The_Url_The_Way_Real_WinForms_Does(string? input, string expected)
    {
        using var browser = new WebBrowser();

        browser.Navigate(input!);

        Assert.Equal(new Uri(expected), Assert.Single(_backend.Navigations));
    }

    [Fact]
    public void Setting_Url_To_Null_Navigates_To_Blank()
    {
        using var browser = new WebBrowser();

        browser.Url = null;

        Assert.Equal(new Uri("about:blank"), Assert.Single(_backend.Navigations));
    }

    [Fact]
    public void Url_Reads_Back_From_The_Engine()
    {
        using var browser = new WebBrowser();

        browser.Navigate("https://example.com/page");

        Assert.Equal(new Uri("https://example.com/page"), browser.Url);
    }

    [Fact]
    public void GoBack_Is_Refused_When_There_Is_No_History()
    {
        using var browser = new WebBrowser { };

        Assert.False(browser.GoBack());
        Assert.Equal(0, _backend.BackCount);

        _backend.CanGoBack = true;

        Assert.True(browser.GoBack());
        Assert.Equal(1, _backend.BackCount);
    }

    [Fact]
    public void GoForward_Is_Refused_When_There_Is_No_History()
    {
        using var browser = new WebBrowser();

        Assert.False(browser.GoForward());
        Assert.Equal(0, _backend.ForwardCount);

        _backend.CanGoForward = true;

        Assert.True(browser.GoForward());
        Assert.Equal(1, _backend.ForwardCount);
    }

    [Fact]
    public void Refresh_Bypasses_The_Cache_Only_For_Completely()
    {
        using var browser = new WebBrowser();

        browser.Refresh();
        browser.Refresh(WebBrowserRefreshOption.Normal);
        browser.Refresh(WebBrowserRefreshOption.Continue);
        browser.Refresh(WebBrowserRefreshOption.Completely);

        Assert.Equal(new[] { false, false, false, true }, _backend.Reloads);
    }

    [Fact]
    public void Stop_Reaches_The_Engine()
    {
        using var browser = new WebBrowser();

        browser.Stop();

        Assert.Equal(1, _backend.StopCount);
    }

    [Fact]
    public void DocumentText_Set_Loads_The_Markup_Without_A_Network_Request()
    {
        using var browser = new WebBrowser();

        browser.DocumentText = "<html><body>hi</body></html>";

        Assert.Equal("<html><body>hi</body></html>", Assert.Single(_backend.LoadedHtml));
        Assert.Empty(_backend.Navigations);
    }

    [Fact]
    public void DocumentStream_Set_Loads_The_Stream_And_Get_Stays_Null()
    {
        using var browser = new WebBrowser();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("<html><body>stream</body></html>"));

        browser.DocumentStream = stream;

        Assert.Equal("<html><body>stream</body></html>", Assert.Single(_backend.LoadedHtml));
        Assert.Null(browser.DocumentStream);
    }

    [Fact]
    public void NewWindow_Cancellation_Is_Reported_Back_To_The_Engine()
    {
        using var browser = new WebBrowser();
        browser.NewWindow += (_, e) => e.Cancel = true;

        Assert.True(_backend.RaiseNewWindow(new Uri("https://example.com/popup")));
    }

    [Fact]
    public void NewWindow_Is_Allowed_When_Nothing_Cancels_It()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.NewWindow += (_, _) => raised++;

        Assert.False(_backend.RaiseNewWindow(new Uri("https://example.com/popup")));
        Assert.Equal(1, raised);
    }

    [Fact]
    public void GoHome_And_GoSearch_Navigate_To_The_Configured_Pages_Not_A_Vendor_Url()
    {
        var home = WebBrowser.HomeUrl;
        var search = WebBrowser.SearchUrl;
        try
        {
            WebBrowser.HomeUrl = new Uri("https://home.example/");
            WebBrowser.SearchUrl = new Uri("https://search.example/");

            using var browser = new WebBrowser();
            browser.GoHome();
            browser.GoSearch();

            Assert.Equal(new[] { new Uri("https://home.example/"), new Uri("https://search.example/") },
                _backend.Navigations);
        }
        finally
        {
            WebBrowser.HomeUrl = home;
            WebBrowser.SearchUrl = search;
        }
    }

    [Fact]
    public void Document_Is_Null_Until_A_Load_Completes_And_Usable_In_DocumentCompleted()
    {
        using var browser = new WebBrowser();
        HtmlDocument? inHandler = null;

        Assert.Null(browser.Document);

        browser.DocumentCompleted += (_, _) => inHandler = browser.Document;
        browser.Navigate("https://example.com/");

        Assert.Null(browser.Document);

        _backend.CompleteNavigation(new Uri("https://example.com/"));

        Assert.NotNull(inHandler);
        Assert.NotNull(browser.Document);
    }

    [Fact]
    public void A_New_Navigation_Drops_The_Previous_Document()
    {
        using var browser = new WebBrowser();
        browser.Navigate("https://example.com/one");
        _backend.CompleteNavigation(new Uri("https://example.com/one"));

        var first = browser.Document;
        Assert.NotNull(first);

        browser.Navigate("https://example.com/two");

        Assert.Null(browser.Document);
    }
}
