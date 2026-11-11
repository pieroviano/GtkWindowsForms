using System.Windows.Forms;
using Gtk.Windows.Forms.InternetExplorer;
using GtkSystem.Windows.Forms.WebBrowsing;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.InternetExplorer;

/// <summary>
/// Behavioural tests for the Internet Explorer backend's host-side logic. The registration order, the
/// probe, the command queue and the honest empty state run everywhere; actually hosting the control is
/// gated on Trident being present.
/// </summary>
[Collection("NonParallelCollection")]
public class InternetExplorerBackendTests
{
    /// <summary>Widgets are constructed here, so GTK has to be up.</summary>
    public InternetExplorerBackendTests()
    {
        Application.Init();
    }

    [Fact]
    public void Referencing_The_Package_Registers_The_Backend()
    {
        Assert.Contains(InternetExplorerBackendRegistration.Name, WebViewBackendFactory.RegisteredNames());
    }

    [Fact]
    public void The_Backend_Registers_Below_WebView2_And_Above_WebKitGTK()
    {
        WebViewBackendFactory.Register("WebView2", 100, () => null);
        try
        {
            var names = WebViewBackendFactory.RegisteredNames().ToList();
            var webView2 = names.IndexOf("WebView2");
            var ie = names.IndexOf(InternetExplorerBackendRegistration.Name);
            var webKit = names.IndexOf("WebKitGTK");

            Assert.True(webView2 >= 0 && ie >= 0, "Both backends must be registered.");
            Assert.True(webView2 < ie, "WebView2 must be preferred over Internet Explorer.");
            Assert.True(webKit < 0 || ie < webKit, "Internet Explorer must be tried before WebKitGTK.");
        }
        finally
        {
            WebViewBackendFactory.Unregister("WebView2");
        }
    }

    [Fact]
    public void PreferOverWebView2_Moves_The_Backend_Above_WebView2()
    {
        WebViewBackendFactory.Register("WebView2", 100, () => null);
        try
        {
            InternetExplorerBackendRegistration.PreferOverWebView2();
            var names = WebViewBackendFactory.RegisteredNames().ToList();

            Assert.True(names.IndexOf(InternetExplorerBackendRegistration.Name) < names.IndexOf("WebView2"),
                "After PreferOverWebView2, Internet Explorer must be tried first.");
        }
        finally
        {
            WebViewBackendFactory.Unregister("WebView2");
            // Restore the default fallback priority so later tests see the shipped ordering.
            WebViewBackendFactory.Register(InternetExplorerBackendRegistration.Name,
                InternetExplorerBackendRegistration.FallbackPriority, InternetExplorerBackend.TryCreate);
        }
    }

    [Fact]
    public void Commands_Issued_Before_The_Control_Is_Ready_Are_Queued_Not_Dropped()
    {
        using var backend = new InternetExplorerBackend();

        // Nothing has realised the widget, so the control has not been created: exactly the window in which
        // an application navigates from a form's constructor.
        backend.Navigate(new Uri("https://example.com/"), null, null, null);
        backend.GoBack();
        backend.Stop();

        Assert.Equal(3, backend.PendingCommandCount);
    }

    [Fact]
    public void An_Unstarted_Backend_Reports_Honest_Empty_State()
    {
        using var backend = new InternetExplorerBackend();

        Assert.True(backend.IsFunctional);
        Assert.Equal(WebBrowserReadyState.Uninitialized, backend.ReadyState);
        Assert.Null(backend.Url);
        Assert.Equal(string.Empty, backend.DocumentTitle);
        Assert.Equal(string.Empty, backend.StatusText);
        Assert.False(backend.CanGoBack);
        Assert.False(backend.CanGoForward);
        Assert.True(backend.Dom.Snapshot.IsEmpty);
        Assert.NotNull(backend.Widget);
    }

    [SkippableFact]
    public void The_Probe_Creates_A_Backend_When_Trident_Is_Present()
    {
        Skip.IfNot(TridentProbe.IsControlAvailable, "Internet Explorer / Trident is not installed on this machine.");

        var backend = InternetExplorerBackend.TryCreate();

        Assert.NotNull(backend);
        backend!.Dispose();
    }
}
