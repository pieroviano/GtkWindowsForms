using System.Windows.Forms;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkSystem.Windows.Forms.WebBrowsing;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// Behavioural tests for <see cref="NullWebViewBackend"/>, the placeholder chosen when no web engine can
/// be loaded. Its job is to keep an application running rather than to do nothing: a navigation must
/// still raise the events an application's state machine is written against, reporting honest failure,
/// so that code ported from real WinForms does not simply hang waiting for a
/// <c>DocumentCompleted</c> that never arrives.
/// </summary>
[Collection("NonParallelCollection")]
public class NullWebViewBackendTests
{
    /// <summary>Widgets are constructed here, so GTK has to be up.</summary>
    public NullWebViewBackendTests()
    {
        Application.Init();
    }

    [Fact]
    public void Reports_Itself_As_Non_Functional_And_Uninitialized()
    {
        using var backend = new NullWebViewBackend();

        Assert.False(backend.IsFunctional);
        Assert.Equal(WebBrowserReadyState.Uninitialized, backend.ReadyState);
        Assert.False(backend.IsBusy);
        Assert.False(backend.CanGoBack);
        Assert.False(backend.CanGoForward);
        Assert.Equal(0, backend.Progress);
        Assert.Equal(string.Empty, backend.DocumentTitle);
        Assert.Equal(string.Empty, backend.StatusText);
        Assert.Null(backend.Url);
    }

    [Fact]
    public void Shows_A_Non_Empty_Placeholder_Message()
    {
        using var backend = new NullWebViewBackend();

        var label = Assert.IsType<Gtk.Label>(backend.Widget);
        Assert.False(string.IsNullOrWhiteSpace(label.Text));
    }

    [Fact]
    public void Navigate_Raises_Navigating_Then_A_Failed_LoadFinished()
    {
        using var backend = new NullWebViewBackend();
        var order = new List<string>();
        BackendLoadEventArgs? load = null;

        backend.Navigating += (_, _) => order.Add("navigating");
        backend.Navigated += (_, _) => order.Add("navigated");
        backend.LoadFinished += (_, e) =>
        {
            order.Add("finished");
            load = e;
        };

        backend.Navigate(new Uri("https://example.com/"), null, null, null);

        Assert.Equal(new[] { "navigating", "finished" }, order);
        Assert.NotNull(load);
        Assert.False(load!.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(load.ErrorMessage));
        Assert.Equal(new Uri("https://example.com/"), backend.Url);
    }

    [Fact]
    public void Cancelling_Navigating_Suppresses_The_Load_And_Leaves_Url_Unchanged()
    {
        using var backend = new NullWebViewBackend();
        var finished = 0;

        backend.Navigating += (_, e) => e.Cancel = true;
        backend.LoadFinished += (_, _) => finished++;

        backend.Navigate(new Uri("https://example.com/"), null, null, null);

        Assert.Equal(0, finished);
        Assert.Null(backend.Url);
    }

    [Fact]
    public void Reload_Before_Any_Navigation_Raises_Nothing()
    {
        using var backend = new NullWebViewBackend();
        var finished = 0;
        backend.LoadFinished += (_, _) => finished++;

        backend.Reload(true);

        Assert.Equal(0, finished);
    }

    [Fact]
    public void History_And_Stop_Are_Inert()
    {
        using var backend = new NullWebViewBackend();

        backend.GoBack();
        backend.GoForward();
        backend.Stop();

        Assert.Null(backend.Url);
    }

    [Fact]
    public async Task InvokeScriptAsync_Returns_Null_Without_Throwing()
    {
        using var backend = new NullWebViewBackend();

        Assert.Null(await backend.InvokeScriptAsync("1 + 1", CancellationToken.None));
    }

    [Fact]
    public async Task The_Dom_Bridge_Is_Empty_And_Inert()
    {
        using var backend = new NullWebViewBackend();

        Assert.True(backend.Dom.Snapshot.IsEmpty);

        backend.Dom.Post(new DomMutation(DomMutationKind.SetInnerText, 1) { Value = "x" });
        await backend.Dom.RefreshAsync(CancellationToken.None);

        Assert.True(backend.Dom.Snapshot.IsEmpty);
        Assert.Null(await backend.Dom.InvokeMemberAsync(1, "click", Array.Empty<object>(), CancellationToken.None));
    }

    [Fact]
    public void WebViewBase_Constructs_Over_The_Placeholder_And_Hosts_Its_Widget()
    {
        WebViewBackendFactory.Clear();
        try
        {
            using var host = new WebViewBase();

            Assert.IsType<NullWebViewBackend>(host.Backend);
            Assert.NotNull(host.Override);
            Assert.Same(host, host.Backend.Widget.Parent);
        }
        finally
        {
            WebViewBackendFactory.Clear();
        }
    }
}
