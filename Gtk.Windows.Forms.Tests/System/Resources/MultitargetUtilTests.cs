namespace System.Resources;

public class MultitargetUtilTests
{
    [Fact]
    public void GetAssemblyQualifiedName_ConverterIsNull_ReturnsAssemblyQualifiedName()
    {
        var type = typeof(string);

        var result = MultitargetUtil
            .GetAssemblyQualifiedName(type, null);

        Assert.Equal(type.AssemblyQualifiedName, result);
    }

    [Theory]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    [InlineData("")]
    public void GetAssemblyQualifiedName_ConverterReturnsNullOrEmpty_FallsBack(string converterResult)
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            return converterResult;
        }

        var result = MultitargetUtil
            .GetAssemblyQualifiedName(type, Converter);

        Assert.Equal(type.AssemblyQualifiedName, result);
    }

    [Fact]
    public void GetAssemblyQualifiedName_ConverterReturnsValue_UsesConverter()
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            return "custom-name";
        }

        var result = MultitargetUtil
            .GetAssemblyQualifiedName(type, Converter);

        Assert.Equal("custom-name", result);
    }

    [Fact]
    public void GetAssemblyQualifiedName_ConverterThrowsIndexOutOfRangeException_Rethrows()
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            throw new IndexOutOfRangeException();
        }

        Assert.Throws<IndexOutOfRangeException>(() =>
            MultitargetUtil
                .GetAssemblyQualifiedName(type, Converter));
    }

    [Fact]
    public void GetAssemblyQualifiedName_ConverterThrowsNonCriticalException_FallsBack()
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            throw new InvalidOperationException();
        }

        var result = MultitargetUtil
            .GetAssemblyQualifiedName(type, Converter);

        Assert.Equal(type.AssemblyQualifiedName, result);
    }

    [Fact]
    public void GetAssemblyQualifiedName_ConverterThrowsNullReferenceException_Rethrows()
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            throw new NullReferenceException();
        }

        Assert.Throws<NullReferenceException>(() =>
            MultitargetUtil
                .GetAssemblyQualifiedName(type, Converter));
    }

    [Fact]
    public void GetAssemblyQualifiedName_ConverterThrowsOutOfMemoryException_Rethrows()
    {
        var type = typeof(string);

        string Converter(Type t)
        {
            throw new OutOfMemoryException();
        }

        Assert.Throws<OutOfMemoryException>(() =>
            MultitargetUtil
                .GetAssemblyQualifiedName(type, Converter));
    }

    [Fact]
    public void GetAssemblyQualifiedName_TypeIsNull_ReturnsNull()
    {
        var result = MultitargetUtil
            .GetAssemblyQualifiedName(null, null);

        Assert.Null(result);
    }
}