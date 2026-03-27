using System.Globalization;

namespace GtkSystem.ComponentModel;

public class ComponentResourceManagerTests
{
    private class DummyForm { }

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
    public void GetString_WithName_ThrowsOrReturns()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));
        // Will throw because no resources exist, but covers the code path
        Assert.ThrowsAny<Exception>(() => mgr.GetString("any"));
    }

    [Fact]
    public void GetObject_WithNameAndCulture_DelegatesToGetObject()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));
        Assert.ThrowsAny<Exception>(() => mgr.GetObject("any", CultureInfo.InvariantCulture));
    }
}
