using System.Reflection;

namespace System.Resources;

public class AssemblyNamesTypeResolutionServiceTests
{
    private readonly AssemblyName[] _names =
    [
        typeof(string).Assembly.GetName()
    ];

    [Fact]
    public void Constructor_ShouldInitialize()
    {
        var sut = CreateSut();

        Assert.NotNull(sut);
    }

    [Fact]
    public void GetAssembly_ShouldLoadAssembly()
    {
        var sut = CreateSut();
        var name = typeof(string).Assembly.GetName();

        var result = sut.GetAssembly(name);

        Assert.NotNull(result);
        Assert.Equal(name.Name, result.GetName().Name);
    }

    [Fact]
    public void GetAssembly_ShouldUseCache()
    {
        var sut = CreateSut();
        var name = typeof(string).Assembly.GetName();

        var first = sut.GetAssembly(name);
        var second = sut.GetAssembly(name);

        Assert.Same(first, second); // cache hit
    }

    [Fact]
    public void GetAssembly_ThrowOnErrorFalse_InvalidAssembly_ShouldNotThrow()
    {
        var sut = new AssemblyNamesTypeResolutionService([
            new AssemblyName("Definitely.Does.Not.Exist")
        ]);

        var result = sut.GetAssembly(new AssemblyName("Definitely.Does.Not.Exist"), false);

        Assert.Null(result);
    }

    [Fact]
    public void GetPathOfAssembly_ShouldReturnCodeBase()
    {
        var sut = CreateSut();
        var name = typeof(string).Assembly.GetName();

        var path = sut.GetPathOfAssembly(name);

#pragma warning disable SYSLIB0044
        Assert.Equal(name.CodeBase, path);
#pragma warning restore SYSLIB0044
    }

    [Fact]
    public void GetType_AssemblyQualifiedName_ShouldResolve()
    {
        var sut = CreateSut();

        var result = sut.GetType(typeof(string).AssemblyQualifiedName);

        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void GetType_IgnoreCase_ShouldWork()
    {
        var sut = CreateSut();

        var result = sut.GetType("system.string", true, true);

        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void GetType_ShouldUseCache()
    {
        var sut = CreateSut();
        var name = typeof(string).FullName;

        var first = sut.GetType(name);
        var second = sut.GetType(name);

        Assert.Same(first, second);
    }

    [Fact]
    public void GetType_SimpleTypeName_ShouldResolve()
    {
        var sut = CreateSut();

        var result = sut.GetType(typeof(string).FullName);

        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void GetType_TypeNotFound_ThrowOnErrorFalse_ReturnsNull()
    {
        var sut = CreateSut();

        var result = sut.GetType("Non.Existing.Type", false);

        Assert.Null(result);
    }

    [Fact]
    public void GetType_TypeNotFound_ThrowOnErrorTrue_Throws()
    {
        var sut = CreateSut();

        Assert.Throws<ArgumentException>(() =>
            sut.GetType("Non.Existing.Type", true));
    }

    [Fact]
    public void ReferenceAssembly_ShouldThrow()
    {
        var sut = CreateSut();

        Assert.Throws<NotSupportedException>(() =>
            sut.ReferenceAssembly(new AssemblyName("Test")));
    }

    private AssemblyNamesTypeResolutionService CreateSut() => new(_names);
}