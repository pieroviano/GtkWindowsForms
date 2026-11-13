using Gtk.Windows.Forms.WebBrowsing;
using NSubstitute;

namespace GtkWinForms.Tests.Gtk.Windows.Forms.WebBrowsing;

/// <summary>
/// Behavioural tests for <see cref="WebViewBackendFactory"/>, the probe-and-fall-back chain that decides
/// which web engine a <see cref="global::System.Windows.Forms.WebBrowser"/> gets. The contract that
/// matters is that a backend which cannot load - a missing native library, a runtime that is not
/// installed - makes the factory try the next candidate rather than fault the control's constructor.
/// </summary>
[Collection("NonParallelCollection")]
public class WebViewBackendFactoryTests : IDisposable
{
    /// <summary>Starts each test from an empty registry, so the result never depends on what the machine
    /// running the suite happens to have installed.</summary>
    public WebViewBackendFactoryTests()
    {
        WebViewBackendFactory.Clear();
    }

    /// <summary>Leaves the registry empty for the next test in the collection.</summary>
    public void Dispose()
    {
        WebViewBackendFactory.Clear();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Create_Falls_Back_To_The_Placeholder_When_Nothing_Is_Registered()
    {
        using var backend = WebViewBackendFactory.Create();

        Assert.IsType<NullWebViewBackend>(backend);
        Assert.False(backend.IsFunctional);
    }

    [Fact]
    public void Create_Prefers_The_Higher_Priority_Backend()
    {
        var low = Substitute.For<IWebViewBackend>();
        var high = Substitute.For<IWebViewBackend>();

        WebViewBackendFactory.Register("low", 10, () => low);
        WebViewBackendFactory.Register("high", 100, () => high);

        Assert.Same(high, WebViewBackendFactory.Create());
    }

    [Fact]
    public void Create_Falls_Through_When_The_Preferred_Backend_Declines()
    {
        var fallback = Substitute.For<IWebViewBackend>();

        WebViewBackendFactory.Register("unavailable", 100, () => null);
        WebViewBackendFactory.Register("available", 10, () => fallback);

        Assert.Same(fallback, WebViewBackendFactory.Create());
    }

    [Fact]
    public void Create_Falls_Through_When_A_Probe_Throws()
    {
        var fallback = Substitute.For<IWebViewBackend>();

        WebViewBackendFactory.Register("broken", 100,
            () => throw new DllNotFoundException("libwebkitgtk-6.0.so.4"));
        WebViewBackendFactory.Register("available", 10, () => fallback);

        Assert.Same(fallback, WebViewBackendFactory.Create());
    }

    [Fact]
    public void Create_Reaches_The_Placeholder_When_Every_Probe_Fails()
    {
        WebViewBackendFactory.Register("broken", 100, () => throw new DllNotFoundException());
        WebViewBackendFactory.Register("declining", 10, () => null);

        using var backend = WebViewBackendFactory.Create();

        Assert.IsType<NullWebViewBackend>(backend);
    }

    [Fact]
    public void Register_Replaces_A_Registration_Of_The_Same_Name()
    {
        var first = Substitute.For<IWebViewBackend>();
        var second = Substitute.For<IWebViewBackend>();

        WebViewBackendFactory.Register("engine", 10, () => first);
        WebViewBackendFactory.Register("engine", 10, () => second);

        Assert.Equal(new[] { "engine" }, WebViewBackendFactory.RegisteredNames());
        Assert.Same(second, WebViewBackendFactory.Create());
    }

    [Fact]
    public void Unregister_Removes_The_Backend()
    {
        WebViewBackendFactory.Register("engine", 10, () => Substitute.For<IWebViewBackend>());

        Assert.True(WebViewBackendFactory.Unregister("engine"));
        Assert.False(WebViewBackendFactory.Unregister("engine"));
        Assert.Empty(WebViewBackendFactory.RegisteredNames());
    }

    [Fact]
    public void RegisteredNames_Lists_Highest_Priority_First()
    {
        WebViewBackendFactory.Register("low", 1, () => null);
        WebViewBackendFactory.Register("high", 100, () => null);
        WebViewBackendFactory.Register("middle", 50, () => null);

        Assert.Equal(new[] { "high", "middle", "low" }, WebViewBackendFactory.RegisteredNames());
    }

    [Fact]
    public void IsAnyEngineAvailable_Is_False_With_Only_The_Placeholder()
    {
        Assert.False(WebViewBackendFactory.IsAnyEngineAvailable());
    }

    [Fact]
    public void IsAnyEngineAvailable_Is_True_When_A_Real_Engine_Registers()
    {
        var engine = Substitute.For<IWebViewBackend>();
        engine.IsFunctional.Returns(true);
        WebViewBackendFactory.Register("engine", 10, () => engine);

        Assert.True(WebViewBackendFactory.IsAnyEngineAvailable());
    }
}
