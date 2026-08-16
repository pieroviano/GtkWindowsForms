using System.Windows.Forms;
using Gtk.Windows.Forms.WebView2;
using GtkSystem.Windows.Forms.WebBrowsing;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.WebView2;

/// <summary>
/// Behavioural tests for the WebView2 backend's host-side logic: what it does before, and without, a
/// browser. The parts that need a real runtime are gated; the parts that do not - the registration, the
/// probe's refusal, and the command queue that keeps an early navigation from vanishing - run everywhere.
/// </summary>
[Collection("NonParallelCollection")]
public class WebView2BackendTests
{
    /// <summary>Widgets are constructed here, so GTK has to be up.</summary>
    public WebView2BackendTests()
    {
        Application.Init();
    }

    [Fact]
    public void Referencing_The_Package_Registers_The_Backend()
    {
        // The module initializer has already run by the time any test in this assembly executes.
        Assert.Contains(WebView2BackendRegistration.Name, WebViewBackendFactory.RegisteredNames());
    }

    [Fact]
    public void The_Backend_Registers_Above_WebKitGTK()
    {
        var names = WebViewBackendFactory.RegisteredNames();
        var webView2 = names.ToList().IndexOf(WebView2BackendRegistration.Name);
        var webKit = names.ToList().IndexOf("WebKitGTK");

        Assert.True(webView2 >= 0, "The WebView2 backend is not registered.");
        Assert.True(webKit < 0 || webView2 < webKit,
            "The WebView2 backend must be tried before the WebKitGTK one on Windows.");
    }

    [Fact]
    public void The_Probe_Declines_Rather_Than_Throwing_When_No_Runtime_Is_Installed()
    {
        var bootstrap = WebView2Runtime.AllowRuntimeBootstrap;
        try
        {
            WebView2Runtime.AllowRuntimeBootstrap = false;

            var backend = WebView2Backend.TryCreate();

            if (WebView2Runtime.IsRuntimeAvailable)
            {
                Assert.NotNull(backend);
                backend!.Dispose();
            }
            else
            {
                Assert.Null(backend);
            }
        }
        finally
        {
            WebView2Runtime.AllowRuntimeBootstrap = bootstrap;
        }
    }

    [Fact]
    public void The_Bootstrapper_Is_Opt_In_And_Off_By_Default()
    {
        Assert.False(WebView2Runtime.AllowRuntimeBootstrap);
    }

    [Fact]
    public void IsRuntimeAvailable_And_InstalledVersion_Agree()
    {
        Assert.Equal(WebView2Runtime.IsRuntimeAvailable, WebView2Runtime.InstalledVersion != null);
    }

    [SkippableFact]
    public void Commands_Issued_Before_The_Browser_Is_Ready_Are_Queued_Not_Dropped()
    {
        Skip.IfNot(WebView2Runtime.IsRuntimeAvailable, "The WebView2 runtime is not installed on this machine.");

        using var backend = new WebView2Backend();

        // Nothing has realised the widget, so the browser has not been created yet: this is exactly the
        // window in which an application navigates from a form's constructor.
        backend.Navigate(new Uri("https://example.com/"), null, null, null);
        backend.GoBack();
        backend.Stop();

        Assert.Equal(3, backend.PendingCommandCount);
    }

    [SkippableFact]
    public void An_Unstarted_Backend_Reports_Honest_Empty_State()
    {
        Skip.IfNot(WebView2Runtime.IsRuntimeAvailable, "The WebView2 runtime is not installed on this machine.");

        using var backend = new WebView2Backend();

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
}
