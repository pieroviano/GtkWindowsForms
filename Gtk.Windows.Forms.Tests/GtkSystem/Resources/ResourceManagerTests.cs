using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Resources;

public class ResourceManagerTests
{
    [Fact]
    public void Constructor_WithBaseNameAndAssembly_SetsResourceInfo()
    {
        var mgr = new ResourceManager("TestBase", Assembly.GetExecutingAssembly());
        Assert.NotNull(mgr.GetResourceInfo);
        Assert.Equal("TestBase", mgr.GetResourceInfo.BaseName);
        Assert.Equal(Assembly.GetExecutingAssembly(), mgr.GetResourceInfo.Assembly);
    }

    [Fact]
    public void Constructor_WithBaseNameAssemblyType_SetsResourceInfo()
    {
        var mgr = new ResourceManager("TestBase", Assembly.GetExecutingAssembly(), typeof(ResourceSet));
        Assert.NotNull(mgr.GetResourceInfo);
        Assert.Equal("TestBase", mgr.GetResourceInfo.BaseName);
        Assert.Equal(Assembly.GetExecutingAssembly(), mgr.GetResourceInfo.Assembly);
        Assert.Equal(typeof(ResourceSet), mgr.GetResourceInfo.SourceType);
    }

    [Fact]
    public void Constructor_WithoutBaseNameAssemblyType_SetsResourceInfo()
    {
        var mgr = new ResourceManager(typeof(ResourceSet));
        Assert.NotNull(mgr.GetResourceInfo);
        Assert.Equal("", mgr.GetResourceInfo.BaseName);
        Assert.Equal(typeof(Form).Assembly, mgr.GetResourceInfo.Assembly);
        Assert.Equal(typeof(ResourceSet), mgr.GetResourceInfo.SourceType);
    }

    [Fact]
    public void GetObject_Throws_WhenAssemblyNull()
    {
        var mgr = (ResourceManager)Activator.CreateInstance(typeof(ResourceManager), true);
        Assert.ThrowsAny<Exception>(() => mgr.GetObject("any"));
    }

    [Fact]
    public void GetString_Throws_WhenAssemblyNull()
    {
        var mgr = (ResourceManager)Activator.CreateInstance(typeof(ResourceManager), true);
        Assert.ThrowsAny<Exception>(() => mgr.GetString("any"));
    }

    [Fact]
    public void GetString_WithCulture_CallsReadResourceText()
    {
        // This will throw because _assembly is null, but we want to cover the method
        var mgr = (ResourceManager)Activator.CreateInstance(typeof(ResourceManager), true);
        Assert.ThrowsAny<Exception>(() => mgr.GetString("any", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ResourceInfo_Properties_CanBeSetAndGet()
    {
        var info = new ResourceManager.ResourceInfo
        {
            Assembly = Assembly.GetExecutingAssembly(),
            BaseName = "Base",
            ImageBytes = [1, 2, 3],
            ResourceName = "ResName",
            SourceType = typeof(int)
        };
        Assert.Equal(Assembly.GetExecutingAssembly(), info.Assembly);
        Assert.Equal("Base", info.BaseName);
        Assert.Equal(new byte[] { 1, 2, 3 }, info.ImageBytes);
        Assert.Equal("ResName", info.ResourceName);
        Assert.Equal(typeof(int), info.SourceType);
    }
}