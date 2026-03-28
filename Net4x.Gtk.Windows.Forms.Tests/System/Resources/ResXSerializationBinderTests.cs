using System.ComponentModel.Design;
using NSubstitute;

namespace System.Resources;

public class ResXSerializationBinderTests
{
    // ---------------------------
    // BindToName tests
    // ---------------------------

    [Fact]
    public void BindToName_NoConverter_ShouldFallbackToBase()
    {
        var resolver = Substitute.For<ITypeResolutionService>();
        var sut = new System.Resources.ResXSerializationBinder(resolver);

        sut.BindToName(typeof(string), out var assemblyName, out var typeName);

        Assert.NotNull(assemblyName);
        // base behavior: typeName usually null
    }

    [Fact]
    public void BindToName_ConverterReturnsValidDifferentType_ShouldSplit()
    {
        string Converter(Type t) => "Custom.Type, Custom.Assembly";

        var sut = new System.Resources.ResXSerializationBinder(Converter);

        sut.BindToName(typeof(string), out var assemblyName, out var typeName);

        Assert.Equal("Custom.Assembly", assemblyName);
        Assert.Equal("Custom.Type", typeName);
    }

    [Fact]
    public void BindToName_ConverterReturnsSameType_ShouldNotSetTypeName()
    {
        var type = typeof(string);
        string Converter(Type t) => $"{type.FullName}, Custom.Assembly";

        var sut = new System.Resources.ResXSerializationBinder(Converter);

        sut.BindToName(type, out var assemblyName, out var typeName);

        Assert.Equal("Custom.Assembly", assemblyName);
        Assert.Null(typeName);
    }

    [Fact]
    public void BindToName_ConverterReturnsInvalidString_ShouldFallback()
    {
        string Converter(Type t) => "InvalidFormatWithoutComma";

        var sut = new System.Resources.ResXSerializationBinder(Converter);

        sut.BindToName(typeof(string), out var assemblyName, out var typeName);

        Assert.NotNull(assemblyName); // fallback happened
    }

    // ---------------------------
    // BindToType tests
    // ---------------------------

    [Fact]
    public void BindToType_NoResolver_ReturnsNull()
    {
        var sut = new System.Resources.ResXSerializationBinder((ITypeResolutionService)null);

        var result = sut.BindToType("mscorlib", "System.String");

        Assert.Null(result);
    }

    [Fact]
    public void BindToType_ResolverReturnsType_ShouldReturnDirectly()
    {
        var resolver = Substitute.For<ITypeResolutionService>();
        resolver.GetType(Arg.Any<string>()).Returns(typeof(string));

        var sut = new System.Resources.ResXSerializationBinder(resolver);

        var result = sut.BindToType("mscorlib", "System.String");

        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void BindToType_ResolverReturnsNull_ShouldTryFallback()
    {
        var resolver = Substitute.For<ITypeResolutionService>();

        resolver.GetType(Arg.Is<string>(s => s.Contains("Version=")))
            .Returns((Type)null);

        resolver.GetType("System.String")
            .Returns(typeof(string));

        var sut = new System.Resources.ResXSerializationBinder(resolver);

        var result = sut.BindToType(
            "mscorlib, Version=1.0.0.0, Culture=neutral",
            "System.String");

        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void BindToType_ResolverFailsAll_ReturnsNull()
    {
        var resolver = Substitute.For<ITypeResolutionService>();
        resolver.GetType(Arg.Any<string>()).Returns((Type)null);

        var sut = new System.Resources.ResXSerializationBinder(resolver);

        var result = sut.BindToType("mscorlib", "Unknown.Type");

        Assert.Null(result);
    }
}