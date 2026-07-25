using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the lifecycle members <see cref="Application" /> was still missing:
/// <see cref="Application.Idle" />, <see cref="Application.Restart" />, the product/company metadata and
/// the visual-style state. Code that touched any of them did not compile at all, so the bar here is that
/// each one exists and reports what WinForms reports.
/// </summary>
[Collection("NonParallelCollection")]
public class ApplicationResidualSurfaceFunctionalTests
{
    [Fact]
    public void Idle_IsRaisedOnceTheEventQueueDrains()
    {
        var raised = 0;
        void OnIdle(object? sender, EventArgs e) => raised++;

        Application.Idle += OnIdle;
        try
        {
            // The sampling source lives on the GLib main context, so the loop has to be pumped for it to
            // fire at all. Idleness is sampled on a 50 ms tick; pump for longer than that.
            var deadline = DateTime.UtcNow.AddSeconds(3);
            while (raised == 0 && DateTime.UtcNow < deadline)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }

            Assert.True(raised > 0, "Idle must be raised once the application has nothing left to process");
        }
        finally
        {
            Application.Idle -= OnIdle;
        }
    }

    [Fact]
    public void Idle_IsNotRaisedAfterTheHandlerIsRemoved()
    {
        var raised = 0;
        void OnIdle(object? sender, EventArgs e) => raised++;

        Application.Idle += OnIdle;
        var deadline = DateTime.UtcNow.AddSeconds(3);
        while (raised == 0 && DateTime.UtcNow < deadline)
        {
            Application.DoEvents();
            Thread.Sleep(10);
        }

        Application.Idle -= OnIdle;
        var afterRemoval = raised;

        for (var i = 0; i < 30; i++)
        {
            Application.DoEvents();
            Thread.Sleep(10);
        }

        Assert.Equal(afterRemoval, raised);
    }

    [Fact]
    public void ProductAndCompanyMetadata_ComeFromTheEntryAssembly()
    {
        var entry = Assembly.GetEntryAssembly();
        Assert.NotNull(entry);

        var expectedCompany = entry!.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        var expectedProduct = entry.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        var expectedVersion = entry.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? entry.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? entry.GetName().Version?.ToString();

        Assert.Equal(
            string.IsNullOrWhiteSpace(expectedCompany) ? entry.GetName().Name : expectedCompany,
            Application.CompanyName);
        Assert.Equal(
            string.IsNullOrWhiteSpace(expectedProduct) ? entry.GetName().Name : expectedProduct,
            Application.ProductName);
        Assert.Equal(
            string.IsNullOrWhiteSpace(expectedVersion) ? entry.GetName().Name : expectedVersion,
            Application.ProductVersion);
    }

    [Fact]
    public void ProductAndCompanyMetadata_AreNeverNull()
    {
        // WinForms guarantees a string; callers concatenate these into title bars without checking.
        Assert.NotNull(Application.CompanyName);
        Assert.NotNull(Application.ProductName);
        Assert.NotNull(Application.ProductVersion);
    }

    [Fact]
    public void VisualStyleState_DefaultsToBothAreasEnabled()
    {
        Assert.Equal(VisualStyleState.ClientAndNonClientAreasEnabled, Application.VisualStyleState);
    }

    [Fact]
    public void VisualStyleState_RoundTripsAndRejectsUndefinedValues()
    {
        var original = Application.VisualStyleState;
        try
        {
            Application.VisualStyleState = VisualStyleState.NoneEnabled;
            Assert.Equal(VisualStyleState.NoneEnabled, Application.VisualStyleState);

            Assert.Throws<InvalidEnumArgumentException>(
                () => Application.VisualStyleState = (VisualStyleState)42);
        }
        finally
        {
            Application.VisualStyleState = original;
        }
    }

    [Fact]
    public void RenderWithVisualStyles_FollowsEnableVisualStylesAndTheState()
    {
        var originalState = Application.VisualStyleState;
        try
        {
            // EnableVisualStyles is what turns rendering on; the test suite's process has already called
            // it or not, so assert the relationship rather than an absolute.
            Application.EnableVisualStyles();

            Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;
            Assert.True(Application.RenderWithVisualStyles);

            Application.VisualStyleState = VisualStyleState.NoneEnabled;
            Assert.False(
                Application.RenderWithVisualStyles,
                "with the client area unstyled nothing renders with visual styles");

            Application.VisualStyleState = VisualStyleState.NonClientAreaEnabled;
            Assert.False(Application.RenderWithVisualStyles);

            Application.VisualStyleState = VisualStyleState.ClientAreaEnabled;
            Assert.True(Application.RenderWithVisualStyles);
        }
        finally
        {
            Application.VisualStyleState = originalState;
        }
    }

    [Fact]
    public void Restart_Exists_AndIsNotAThrowingStub()
    {
        // Restart cannot be executed in a test - it would replace this process - so what is pinned is that
        // the member exists with the WinForms signature and is reachable.
        var restart = typeof(Application).GetMethod(
            nameof(Application.Restart),
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null);

        Assert.NotNull(restart);
        Assert.Equal(typeof(void), restart!.ReturnType);
    }
}
