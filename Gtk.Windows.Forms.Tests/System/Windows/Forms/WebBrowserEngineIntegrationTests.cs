using System.Diagnostics;
using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Drives the <see cref="WebBrowser"/> control itself against a real engine, over a page on disk.
/// </summary>
/// <remarks>
/// Every other suite here exercises either the control over a fake backend or the backend without the
/// control, so a break in the wiring between the two - the control subscribing to the wrong backend
/// event, the Html* layer reading a bridge the control never attached - would pass all of them. These
/// are the scenarios mono's own <c>WebBrowserTest.cs</c> covered (MIT, (c) 2008 Novell; see
/// THIRD-PARTY-NOTICES.txt), re-expressed for this substrate.
///
/// Two deliberate departures from mono's version: it waited with <c>while (!stop) {}</c>, a hot spin with
/// no timeout that hangs the whole run forever if the event never arrives, so these pump the GTK main
/// loop with a deadline instead; and where mono only read a written value back out of the same wrapper,
/// these also re-pull the snapshot from the page, which is what actually proves the write crossed into
/// the engine rather than only updating the local cache.
///
/// Skipped, not silently passed, where WebKitGTK 6.0 is absent - every Windows machine and most macOS
/// ones. CI installs libwebkitgtk-6.0-4 so they run there.
/// </remarks>
[Collection("NonParallelCollection")]
public class WebBrowserEngineIntegrationTests : IDisposable
{
    /// <summary>How long to pump the GTK main loop before giving a load up for lost.</summary>
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(30);

    private readonly List<string> _pages = new();

    /// <summary>Puts a real WebKitGTK backend behind the control, in place of whatever a sibling suite left
    /// registered.</summary>
    public WebBrowserEngineIntegrationTests()
    {
        Application.Init();
        WebViewBackendFactory.Clear();
        WebViewBackendFactory.Register("WebKitGTK", 50, WebKitWebViewBackend.TryCreate);
    }

    /// <summary>Removes the temp pages and leaves the registry empty for the next suite.</summary>
    public void Dispose()
    {
        WebViewBackendFactory.Clear();

        foreach (var page in _pages)
        {
            try
            {
                File.Delete(page);
            }
            catch (IOException)
            {
                // A page the engine still holds open. A stray temp file is not worth failing a test over.
            }
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>Whether this machine can load WebKitGTK 6.0, probed through the production entry point.</summary>
    private static bool WebKitIsAvailable
    {
        get
        {
            using var probe = WebKitWebViewBackend.TryCreate();
            return probe != null;
        }
    }

    [SkippableFact]
    public void Navigating_A_File_Url_Raises_The_Events_In_Order_And_Yields_A_Document()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        var url = CreateTestPage("<html><head><title>Loaded</title></head><body><p>hello</p></body></html>");

        using var browser = new WebBrowser();
        using var host = Show(browser);

        var order = new List<string>();
        HtmlDocument? documentInHandler = null;

        browser.Navigating += (_, _) => order.Add("navigating");
        browser.Navigated += (_, _) => order.Add("navigated");
        browser.DocumentCompleted += (_, _) =>
        {
            order.Add("completed");
            documentInHandler = browser.Document;
        };

        var completed = Navigate(browser, url);

        Assert.True(completed, "The page on disk did not finish loading within the timeout.");
        Assert.Equal(new[] { "navigating", "navigated", "completed" }, order);

        // mono asserted only that navigating and navigated had both happened by this point. The stronger
        // claim - and the one applications depend on - is that Document is readable inside the handler,
        // because the snapshot is pulled before the event is raised.
        Assert.NotNull(documentInHandler);
        Assert.NotNull(browser.Document);
        Assert.NotNull(browser.Document!.Body);
        Assert.Equal(WebBrowserReadyState.Complete, browser.ReadyState);
        Assert.False(browser.IsBusy);
    }

    [SkippableFact]
    public void The_Document_Reflects_The_Page_That_Was_Loaded()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        var url = CreateTestPage(
            "<html><head><title>Fixture</title></head><body>" +
            "<div id=\"testid\">original</div>" +
            "<a href=\"https://example.org/\">link</a>" +
            "<img id=\"pic\" src=\"none.png\">" +
            "</body></html>");

        using var browser = new WebBrowser();
        using var host = Show(browser);

        Assert.True(Navigate(browser, url), "The page on disk did not finish loading within the timeout.");

        var document = browser.Document!;

        Assert.Equal("Fixture", document.Title);
        Assert.Equal("Fixture", browser.DocumentTitle);

        var element = document.GetElementById("testid");
        Assert.NotNull(element);
        Assert.Equal("DIV", element!.TagName);
        Assert.Equal("original", element.InnerHtml);

        Assert.Equal(1, document.Links.Count);
        Assert.Equal(1, document.Images.Count);
        Assert.Null(document.GetElementById("no-such-id"));
    }

    [SkippableFact]
    public void Writing_InnerHtml_Reads_Back_And_Reaches_The_Page()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        const string written = "testing inner html";
        var url = CreateTestPage("<html><head></head><body><div id=\"testid\"></div></body></html>");

        using var browser = new WebBrowser();
        using var host = Show(browser);

        Assert.True(Navigate(browser, url), "The page on disk did not finish loading within the timeout.");

        var element = browser.Document!.GetElementById("testid")!;
        element.InnerHtml = written;

        // Immediately, from the optimistically updated snapshot - this is mono's assertion.
        Assert.Equal(written, element.InnerHtml);

        // And again after re-reading the page, which is what proves the write actually crossed into the
        // engine rather than only updating the local cache.
        Pump(TimeSpan.FromSeconds(2));
        browser.Backend.Dom.RefreshAsync(CancellationToken.None).GetAwaiter().GetResult();

        Assert.Equal(written, browser.Document!.GetElementById("testid")!.InnerHtml);
    }

    [SkippableFact]
    public void Cancelling_Navigating_Stops_The_Engine_Reaching_The_Page()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        var url = CreateTestPage("<html><body><div id=\"testid\">should not load</div></body></html>");

        using var browser = new WebBrowser();
        using var host = Show(browser);

        var completed = 0;
        browser.Navigating += (_, e) => e.Cancel = true;
        browser.DocumentCompleted += (_, _) => completed++;

        browser.Navigate(url);
        Pump(TimeSpan.FromSeconds(3));

        Assert.Equal(0, completed);
        Assert.Null(browser.Document);
    }

    [SkippableFact]
    public void History_Follows_Two_Real_Navigations()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        var first = CreateTestPage("<html><head><title>First</title></head><body>one</body></html>");
        var second = CreateTestPage("<html><head><title>Second</title></head><body>two</body></html>");

        using var browser = new WebBrowser();
        using var host = Show(browser);

        Assert.True(Navigate(browser, first), "The first page did not finish loading within the timeout.");
        Assert.False(browser.CanGoBack);

        Assert.True(Navigate(browser, second), "The second page did not finish loading within the timeout.");
        Assert.True(browser.CanGoBack);
        Assert.Equal("Second", browser.DocumentTitle);

        Assert.True(browser.GoBack());
        Assert.True(WaitFor(() => string.Equals(browser.DocumentTitle, "First", StringComparison.Ordinal)),
            "Going back did not return to the first page within the timeout.");
        Assert.True(browser.CanGoForward);
    }

    /// <summary>Writes a page to a temp file and returns its <c>file://</c> URL, as mono's test did.</summary>
    private string CreateTestPage(string html)
    {
        var path = Path.Combine(Path.GetTempPath(), "gtkwf-webbrowser-" + Guid.NewGuid().ToString("N") + ".html");
        File.WriteAllText(path, html);
        _pages.Add(path);

        return new Uri(path).AbsoluteUri;
    }

    /// <summary>Starts a navigation and pumps until it completes or the deadline passes.</summary>
    private static bool Navigate(WebBrowser browser, string url)
    {
        var completed = false;
        void OnCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) => completed = true;

        browser.DocumentCompleted += OnCompleted;
        try
        {
            browser.Navigate(url);
            return WaitFor(() => completed);
        }
        finally
        {
            browser.DocumentCompleted -= OnCompleted;
        }
    }

    /// <summary>Runs the GTK main loop for a while, so the engine can make progress.</summary>
    private static void Pump(TimeSpan duration)
    {
        var clock = Stopwatch.StartNew();
        while (clock.Elapsed < duration)
        {
            while (global::Gtk.Application.EventsPending())
            {
                global::Gtk.Application.RunIteration(false);
            }

            Thread.Sleep(10);
        }
    }

    /// <summary>Puts the control's widget in a mapped window: WebKit does not load in an unrealised view.</summary>
    private static global::Gtk.Window Show(WebBrowser browser)
    {
        var window = new global::Gtk.Window();
        window.SetDefaultSize(640, 480);
        window.Child = (global::Gtk.Widget)browser.GtkControl;
        window.Show();
        Pump(TimeSpan.FromMilliseconds(200));

        return window;
    }

    /// <summary>Pumps the GTK main loop until a condition holds or the deadline passes.</summary>
    private static bool WaitFor(Func<bool> condition)
    {
        var clock = Stopwatch.StartNew();
        while (!condition() && clock.Elapsed < LoadTimeout)
        {
            while (global::Gtk.Application.EventsPending())
            {
                global::Gtk.Application.RunIteration(false);
            }

            Thread.Sleep(10);
        }

        return condition();
    }
}
