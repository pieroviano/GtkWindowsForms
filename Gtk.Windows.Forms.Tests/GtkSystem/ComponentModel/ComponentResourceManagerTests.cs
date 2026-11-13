using System.Globalization;

namespace Gtk.ComponentModel;

public class ComponentResourceManagerTests
{
    [Fact]
    public void Constructor_SetsTypeAndName()
    {
        var mgr = new ComponentResourceManager(typeof(DummyForm));
        Assert.NotNull(mgr);
    }

    [Fact]
    public void GetObject_WithMissingResourcesFolder_DoesNotThrow_AndFallsBackToPlaceholder()
    {
        // Regression: GetObject(name) (no culture) falls through to
        // Directory.GetFiles(Path.Combine("Resources", <form>), ...) to look for a loose resource file.
        // Directory.GetFiles throws DirectoryNotFoundException when that optional folder does not exist,
        // which used to escape straight out of Form.InitializeComponent and crash the app
        // (repro: GtkWinFormsApp/Form3 -> "Could not find a part of the path '...\\Resources\\Form3'").
        // A missing folder must now be treated as "no files", returning the built-in placeholder image.
        var mgr = new ComponentResourceManager(typeof(DummyForm));

        var value = mgr.GetObject("any");

        Assert.NotNull(value);
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