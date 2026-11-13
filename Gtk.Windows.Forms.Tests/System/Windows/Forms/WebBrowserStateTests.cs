using System.Windows.Forms;
using Gtk.Windows.Forms.WebBrowsing;
using GtkWinForms.Tests.Mocks;

using Application = System.Windows.Forms.Application;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the state <see cref="WebBrowser"/> reports and the change notifications it
/// raises. The distinction that matters is between reading a value and being told it moved: WinForms
/// applications bind to CanGoBackChanged and friends, so those must fire when the backing value actually
/// changes and stay silent when it does not.
/// </summary>
[Collection("NonParallelCollection")]
public class WebBrowserStateTests : IDisposable
{
    private readonly FakeWebViewBackend _backend;

    /// <summary>Registers the fake as the only backend so the control never reaches a real engine.</summary>
    public WebBrowserStateTests()
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
    public void CanGoBack_And_CanGoForward_Delegate_To_The_Engine()
    {
        using var browser = new WebBrowser();

        Assert.False(browser.CanGoBack);
        Assert.False(browser.CanGoForward);

        _backend.CanGoBack = true;
        _backend.CanGoForward = true;

        Assert.True(browser.CanGoBack);
        Assert.True(browser.CanGoForward);
    }

    [Fact]
    public void CanGoBackChanged_Fires_On_Change_And_Not_On_Every_Poll()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.CanGoBackChanged += (_, _) => raised++;

        _backend.CanGoBack = true;
        _backend.CompleteNavigation(new Uri("https://example.com/one"));
        Assert.Equal(1, raised);

        _backend.CompleteNavigation(new Uri("https://example.com/two"));
        Assert.Equal(1, raised);

        _backend.CanGoBack = false;
        _backend.CompleteNavigation(new Uri("https://example.com/three"));
        Assert.Equal(2, raised);
    }

    [Fact]
    public void CanGoForwardChanged_Fires_On_Change_Only()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.CanGoForwardChanged += (_, _) => raised++;

        _backend.CompleteNavigation(new Uri("https://example.com/one"));
        Assert.Equal(0, raised);

        _backend.CanGoForward = true;
        _backend.CompleteNavigation(new Uri("https://example.com/two"));
        Assert.Equal(1, raised);
    }

    [Fact]
    public void DocumentTitleChanged_Fires_When_The_Title_Moves()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.DocumentTitleChanged += (_, _) => raised++;

        _backend.RaiseTitleChanged("First");
        Assert.Equal(1, raised);
        Assert.Equal("First", browser.DocumentTitle);

        _backend.RaiseTitleChanged("First");
        Assert.Equal(1, raised);

        _backend.RaiseTitleChanged("Second");
        Assert.Equal(2, raised);
        Assert.Equal("Second", browser.DocumentTitle);
    }

    [Fact]
    public void StatusTextChanged_Fires_And_StatusText_Follows()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.StatusTextChanged += (_, _) => raised++;

        Assert.Equal(string.Empty, browser.StatusText);

        _backend.RaiseStatusChanged("https://example.org/");

        Assert.Equal(1, raised);
        Assert.Equal("https://example.org/", browser.StatusText);
    }

    [Fact]
    public void EncryptionLevelChanged_Fires_When_The_Level_Moves()
    {
        using var browser = new WebBrowser();
        var raised = 0;
        browser.EncryptionLevelChanged += (_, _) => raised++;

        Assert.Equal(WebBrowserEncryptionLevel.Insecure, browser.EncryptionLevel);

        _backend.EncryptionLevel = WebBrowserEncryptionLevel.Bit128;
        _backend.CompleteNavigation(new Uri("https://example.com/"));

        Assert.Equal(1, raised);
        Assert.Equal(WebBrowserEncryptionLevel.Bit128, browser.EncryptionLevel);
    }

    [Fact]
    public void ReadyState_Walks_Uninitialized_Loading_Interactive_Complete()
    {
        using var browser = new WebBrowser();

        Assert.Equal(WebBrowserReadyState.Uninitialized, browser.ReadyState);

        browser.Navigate("https://example.com/");
        Assert.Equal(WebBrowserReadyState.Loading, browser.ReadyState);

        _backend.RaiseProgress(50, 100);
        Assert.Equal(WebBrowserReadyState.Interactive, browser.ReadyState);

        _backend.CompleteNavigation(new Uri("https://example.com/"));
        Assert.Equal(WebBrowserReadyState.Complete, browser.ReadyState);
    }

    [Fact]
    public void ReadyState_Returns_To_Uninitialized_When_A_Load_Fails()
    {
        using var browser = new WebBrowser();

        browser.Navigate("https://example.com/");
        _backend.CompleteNavigation(new Uri("https://example.com/"), succeeded: false);

        Assert.Equal(WebBrowserReadyState.Uninitialized, browser.ReadyState);
    }

    [Fact]
    public void IsBusy_Is_True_Between_Navigating_And_DocumentCompleted()
    {
        using var browser = new WebBrowser();

        browser.Navigate("https://example.com/");
        Assert.True(browser.IsBusy);

        _backend.CompleteNavigation(new Uri("https://example.com/"));
        Assert.False(browser.IsBusy);
    }

    [Fact]
    public void ProgressChanged_Reports_The_Engine_Numbers()
    {
        using var browser = new WebBrowser();
        WebBrowserProgressChangedEventArgs? seen = null;
        browser.ProgressChanged += (_, e) => seen = e;

        _backend.RaiseProgress(37, 100);

        Assert.NotNull(seen);
        Assert.Equal(37, seen!.CurrentProgress);
        Assert.Equal(100, seen.MaximumProgress);
    }

    [Fact]
    public void Settings_Round_Trip_Through_The_Engine()
    {
        using var browser = new WebBrowser();

        browser.IsWebBrowserContextMenuEnabled = true;
        browser.ScrollBarsEnabled = false;
        browser.WebBrowserShortcutsEnabled = false;
        browser.ScriptErrorsSuppressed = true;
        browser.AllowWebBrowserDrop = true;

        Assert.True(_backend.ContextMenuEnabled);
        Assert.False(_backend.ScrollBarsEnabled);
        Assert.False(_backend.ShortcutsEnabled);
        Assert.True(_backend.ScriptErrorsSuppressed);
        Assert.True(_backend.AllowDrop);

        Assert.True(browser.IsWebBrowserContextMenuEnabled);
        Assert.False(browser.ScrollBarsEnabled);
        Assert.False(browser.WebBrowserShortcutsEnabled);
        Assert.True(browser.ScriptErrorsSuppressed);
        Assert.True(browser.AllowWebBrowserDrop);
    }

    [Fact]
    public void The_Control_Hosts_The_Engine_Widget()
    {
        using var browser = new WebBrowser();

        Assert.Same(browser.self, browser.GtkControl);
        Assert.Same(_backend, browser.self.Backend);
        Assert.Same(browser, browser.self.Override.sender);
    }

    [Fact]
    public void The_Members_That_Depend_On_A_Missing_Dialog_Say_So_Rather_Than_Failing_Silently()
    {
        using var browser = new WebBrowser();

        Assert.Throws<NotSupportedException>(() => browser.ShowPageSetupDialog());
        Assert.Throws<NotSupportedException>(() => browser.ShowPrintPreviewDialog());
        Assert.Throws<NotSupportedException>(() => browser.ShowPropertiesDialog());
        Assert.Throws<NotSupportedException>(() => browser.ShowSaveAsDialog());
    }

    [Fact]
    public void The_ActiveX_Shaped_Members_Throw_Rather_Than_Pretend()
    {
        using var browser = new WebBrowser();

        Assert.Throws<NotSupportedException>(() => _ = ((WebBrowserBase)browser).ActiveXInstance);
        Assert.Throws<NotSupportedException>(() => browser.Text = "x");
        Assert.Throws<NotSupportedException>(() => browser.Cursor = null);
    }

    [Fact]
    public void Print_Asks_The_Page_To_Print()
    {
        using var browser = new WebBrowser();

        browser.Print();

        Assert.Contains("window.print()", _backend.Scripts);
    }
}
