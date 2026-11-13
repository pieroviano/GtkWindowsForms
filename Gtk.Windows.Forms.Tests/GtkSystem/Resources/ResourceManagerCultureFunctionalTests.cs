using System.Globalization;
using System.Reflection;

namespace Gtk.Resources;

/// <summary>
/// Behavioural tests for culture resolution in <see cref="ResourceManager" />.
/// <para>
/// The culture-taking overloads used to discard their argument and read the neutral resource, so a
/// localized resource could never be reached however it was requested. These tests run against a real
/// pair of compiled resources - <c>Globalization/CultureProbe.resx</c> and its Italian sibling, which the
/// build compiles into the <c>it/</c> satellite assembly - so the fallback chain being asserted is the
/// real one, not a stand-in.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class ResourceManagerCultureFunctionalTests
{
    private const string NeutralGreeting = "Hello";

    private const string ItalianGreeting = "Ciao";

    /// <summary>
    /// The probe's base name, discovered from the manifest so the test does not depend on how the project
    /// happens to derive resource names.
    /// </summary>
    private static string ProbeBaseName
    {
        get
        {
            var assembly = typeof(ResourceManagerCultureFunctionalTests).Assembly;
            var neutral = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("CultureProbe.resources", StringComparison.Ordinal));

            Assert.True(neutral != null,
                "the CultureProbe neutral resource is missing from the test assembly manifest: " +
                string.Join(", ", assembly.GetManifestResourceNames()));

            return neutral!.Substring(0, neutral.Length - ".resources".Length);
        }
    }

    private static ResourceManager Probe() =>
        new ResourceManager(ProbeBaseName, typeof(ResourceManagerCultureFunctionalTests).Assembly);

    [Fact]
    public void GetString_NeutralCulture_ReadsTheNeutralResource()
    {
        Assert.Equal(NeutralGreeting, Probe().GetString("Greeting", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void GetString_RequestedCulture_ReadsTheLocalizedResource()
    {
        // The whole point: the argument must reach the lookup.
        Assert.Equal(ItalianGreeting, Probe().GetString("Greeting", new CultureInfo("it")));
    }

    [Fact]
    public void GetString_SpecificCulture_FallsBackToItsParent()
    {
        // There is no it-IT resource, so the it resource must answer - the standard chain.
        Assert.Equal(ItalianGreeting, Probe().GetString("Greeting", new CultureInfo("it-IT")));
    }

    [Fact]
    public void GetString_CultureWithNoResources_FallsBackToNeutral()
    {
        Assert.Equal(NeutralGreeting, Probe().GetString("Greeting", new CultureInfo("de-DE")));
    }

    [Fact]
    public void GetString_KeyMissingFromTheLocalizedResource_FallsBackToNeutral()
    {
        // Per-key fallback, not per-file: "NeutralOnly" exists only in the neutral resource.
        Assert.Equal("Only in the neutral resource", Probe().GetString("NeutralOnly", new CultureInfo("it")));
    }

    [Fact]
    public void GetString_UnknownKey_ReturnsNull()
    {
        Assert.Null(Probe().GetString("NoSuchResourceKey", new CultureInfo("it")));
    }

    [Fact]
    public void GetString_WithoutCulture_FollowsCurrentUiCulture()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("it");
            Assert.Equal(ItalianGreeting, Probe().GetString("Greeting"));

            CultureInfo.CurrentUICulture = new CultureInfo("de");
            Assert.Equal(NeutralGreeting, Probe().GetString("Greeting"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void GetObject_RequestedCulture_ReadsTheLocalizedResource()
    {
        // Strings arrive through GetObject too - the designer reads most resources that way.
        Assert.Equal(ItalianGreeting, Probe().GetObject("Greeting", new CultureInfo("it")));
    }

    [Fact]
    public void GetObject_UnknownKeyWithCulture_ReturnsNullRatherThanAPlaceholder()
    {
        // The parameterless overload substitutes a placeholder image for a missing resource; the
        // culture-qualified contract is to return null.
        Assert.Null(Probe().GetObject("NoSuchResourceKey", new CultureInfo("it")));
    }
}
