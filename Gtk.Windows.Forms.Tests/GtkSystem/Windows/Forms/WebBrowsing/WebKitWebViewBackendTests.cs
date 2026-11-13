#pragma warning disable CS0612

#pragma warning disable CS0612

using System.Diagnostics;
using System.Windows.Forms;
using Gtk.Windows.Forms.WebBrowsing;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.Gtk.Windows.Forms.WebBrowsing;

/// <summary>
/// Integration tests for <see cref="WebKitWebViewBackend"/> against a real engine. Skipped, not silently
/// passed, on a machine without WebKitGTK 6.0 - which is every Windows machine and most macOS ones. The
/// unit-level behaviour of everything above the seam is covered by the tests that use the fake backend;
/// what only a real engine can show is that the injected bridge script actually round-trips a document.
/// </summary>
[Collection("NonParallelCollection")]
public class WebKitWebViewBackendTests
{
    /// <summary>How long to pump the GTK main loop before giving a load up for lost.</summary>
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(20);

    /// <summary>GTK has to be up before a WebKit view can be constructed.</summary>
    public WebKitWebViewBackendTests()
    {
        Application.Init();
    }

    /// <summary>
    /// Whether this machine can load WebKitGTK 6.0. Probed once through the backend's own entry point, so
    /// the gate and the production path agree.
    /// </summary>
    private static bool WebKitIsAvailable
    {
        get
        {
            using var probe = WebKitWebViewBackend.TryCreate();
            return probe != null;
        }
    }

    [SkippableFact]
    public void The_Factory_Chooses_WebKit_When_The_Engine_Is_Installed()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        using var backend = WebViewBackendFactory.Create();

        Assert.True(backend.IsFunctional);
        Assert.IsType<WebKitWebViewBackend>(backend);
    }

    [SkippableFact]
    public void Loading_Blank_Raises_LoadFinished_And_Yields_A_Body()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        using var backend = new WebKitWebViewBackend();
        using var host = Show(backend);

        var finished = WaitForLoad(backend, () => backend.Navigate(new Uri("about:blank"), null, null, null));

        Assert.True(finished, "about:blank did not finish loading within the timeout.");
        Assert.Equal(WebBrowserReadyState.Complete, backend.ReadyState);
        Assert.False(backend.Dom.Snapshot.IsEmpty);
        Assert.NotEqual(DomNode.NoHandle, backend.Dom.Snapshot.BodyHandle);
    }

    [SkippableFact]
    public void A_LoadHtml_Round_Trip_Comes_Back_Through_The_Snapshot()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        using var backend = new WebKitWebViewBackend();
        using var host = Show(backend);

        var finished = WaitForLoad(backend,
            () => backend.LoadHtml(
                "<!doctype html><html><head><title>Round trip</title></head>" +
                "<body><h1 id=\"heading\">written text</h1></body></html>",
                new Uri("https://example.invalid/")));

        Assert.True(finished, "The written document did not finish loading within the timeout.");

        var snapshot = backend.Dom.Snapshot;
        var heading = snapshot.GetElementById("heading");

        Assert.NotNull(heading);
        Assert.Equal("H1", heading!.TagName);
        Assert.Equal("written text", heading.InnerText);
        Assert.Equal("Round trip", snapshot.Title);
    }

    [SkippableFact]
    public async Task InvokeScriptAsync_Returns_The_Value_The_Page_Computed()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        using var backend = new WebKitWebViewBackend();
        using var host = Show(backend);

        WaitForLoad(backend, () => backend.LoadHtml("<!doctype html><html><body></body></html>", null));

        Assert.Equal(3d, await backend.InvokeScriptAsync("1 + 2", CancellationToken.None));
        Assert.Equal("abc", await backend.InvokeScriptAsync("'a' + 'b' + 'c'", CancellationToken.None));
        Assert.Equal(true, await backend.InvokeScriptAsync("1 < 2", CancellationToken.None));
    }

    [SkippableFact]
    public void A_Posted_Mutation_Reaches_The_Page()
    {
        Skip.IfNot(WebKitIsAvailable, "WebKitGTK 6.0 is not installed on this machine.");

        using var backend = new WebKitWebViewBackend();
        using var host = Show(backend);

        WaitForLoad(backend,
            () => backend.LoadHtml("<!doctype html><html><body><p id=\"target\">before</p></body></html>", null));

        var target = backend.Dom.Snapshot.GetElementById("target");
        Assert.NotNull(target);

        backend.Dom.Post(new DomMutation(DomMutationKind.SetInnerText, target!.Handle) { Value = "after" });

        // The mutation is posted, not awaited, so the page needs a turn of the main loop to apply it
        // before the next snapshot can see it.
        Pump(TimeSpan.FromSeconds(2));
        backend.Dom.RefreshAsync(CancellationToken.None).GetAwaiter().GetResult();

        Assert.Equal("after", backend.Dom.Snapshot.GetElementById("target")!.InnerText);
    }

    /// <summary>Puts the engine's widget in a mapped window: WebKit does not load in an unrealised view.</summary>
    private static global::Gtk.Window Show(IWebViewBackend backend)
    {
        var window = new global::Gtk.Window();
        window.SetDefaultSize(400, 300);
        window.Child = backend.Widget;
        window.Show();
        Pump(TimeSpan.FromMilliseconds(200));
        return window;
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

    /// <summary>Starts a load and pumps until it finishes or the timeout expires.</summary>
    private static bool WaitForLoad(IWebViewBackend backend, Action start)
    {
        var finished = false;
        void OnFinished(object? sender, BackendLoadEventArgs e) => finished = true;

        backend.LoadFinished += OnFinished;
        try
        {
            start();

            var clock = Stopwatch.StartNew();
            while (!finished && clock.Elapsed < LoadTimeout)
            {
                while (global::Gtk.Application.EventsPending())
                {
                    global::Gtk.Application.RunIteration(false);
                }

                Thread.Sleep(10);
            }

            return finished;
        }
        finally
        {
            backend.LoadFinished -= OnFinished;
        }
    }
}
#pragma warning restore CS0612
