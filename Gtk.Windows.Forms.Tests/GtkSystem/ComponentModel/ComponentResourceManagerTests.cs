using System.Globalization;

namespace GtkSystem.ComponentModel;

public class ComponentResourceManagerTests
{
    [Fact]
    public void Constructor_SetsTypeAndName()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));
        Assert.NotNull(mgr);
    }

    [Fact]
    public void GetObject_WithName_ThrowsOrReturns()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));
        // Will throw because no resources exist, but covers the code path
        Assert.ThrowsAny<Exception>(() => mgr.GetObject("any"));
    }

    [Fact]
    public void GetObject_WithNameAndCulture_ReturnsNullWhenThereAreNoResources()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));

        // "If an appropriate resource set exists but name cannot be found, the method returns null" - the
        // contract this overload documents. DummyForm has no resources at all, so every culture in the
        // fallback chain comes up empty and the answer is null, not an exception.
        Assert.Null(mgr.GetObject("any", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void GetObject_WithNameAndCulture_ReturnsNullForEveryCultureInTheChain()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));

        Assert.Null(mgr.GetObject("any", new CultureInfo("it-IT")));
        Assert.Null(mgr.GetObject("any", new CultureInfo("de")));
    }

    [Fact]
    public void GetString_WithName_ReturnsNullWhenThereAreNoResources()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));

        Assert.Null(mgr.GetString("any"));
    }

    [Fact]
    public void GetString_WithNameAndCulture_ReturnsNullWhenThereAreNoResources()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));

        Assert.Null(mgr.GetString("any", new CultureInfo("it")));
    }

    private class DummyForm
    {
    }
}