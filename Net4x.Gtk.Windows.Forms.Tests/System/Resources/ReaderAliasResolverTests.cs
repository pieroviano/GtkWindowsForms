using System.Reflection;
#pragma warning disable xUnit1012

namespace System.Resources;

public class ReaderAliasResolverTests
{
    private readonly System.Resources.ResXResourceReader.ReaderAliasResolver _sut
        = new System.Resources.ResXResourceReader.ReaderAliasResolver();

    [Fact]
    public void PushAlias_ThenResolveAlias_ShouldReturnAssemblyName()
    {
        var assemblyName = new AssemblyName("TestAssembly");

        _sut.PushAlias("alias1", assemblyName);

        var result = _sut.ResolveAlias("alias1");

        Assert.Equal(assemblyName, result);
    }

    [Fact]
    public void ResolveAlias_UnknownAlias_ShouldReturnNull()
    {
        var result = _sut.ResolveAlias("unknown");

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void PushAlias_NullOrEmptyAlias_ShouldNotStore(string alias)
    {
        var assemblyName = new AssemblyName("TestAssembly");

        _sut.PushAlias(alias, assemblyName);

        var result = _sut.ResolveAlias(alias);

        Assert.Null(result);
    }

    [Fact]
    public void PushAlias_ShouldOverwriteExistingAlias()
    {
        var first = new AssemblyName("First");
        var second = new AssemblyName("Second");

        _sut.PushAlias("alias", first);
        _sut.PushAlias("alias", second);

        var result = _sut.ResolveAlias("alias");

        Assert.Equal(second, result);
    }

    [Fact]
    public void PushAlias_WithNullAssemblyName_ShouldStoreNull()
    {
        _sut.PushAlias("alias", null);

        var result = _sut.ResolveAlias("alias");

        Assert.Null(result);
    }
}