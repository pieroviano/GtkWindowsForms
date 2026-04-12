extern alias gtk;
using System.Reflection;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Diagnostics.CodeAnalysis;

public class NotNullWhenAttributeTests
{
    [Fact]
    public void Constructor_ShouldSetReturnValue_True()
    {
        // Act
        var attr = new gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true);

        // Assert
        Assert.True(attr.ReturnValue);
    }

    [Fact]
    public void Constructor_ShouldSetReturnValue_False()
    {
        // Act
        var attr = new gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(false);

        // Assert
        Assert.False(attr.ReturnValue);
    }

    [Fact]
    public void AttributeUsage_ShouldBeParameterOnly()
    {
        // Arrange
        var usage = typeof(gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute)
            .GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Parameter, usage.ValidOn);
    }

    [Fact]
    public void AttributeUsage_ShouldNotBeInherited()
    {
        // Arrange
        var usage = typeof(gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute)
            .GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        Assert.False(usage.Inherited);
    }

    [Fact]
    public void Attribute_ShouldBeApplicableToParameter()
    {
        // Arrange
        var method = typeof(TestClass)
            .GetMethod(nameof(TestClass.Method));

        var parameter = method.GetParameters().First();

        // Act
        var attr = parameter.GetCustomAttribute<gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute>();

        // Assert
        Assert.NotNull(attr);
        Assert.True(attr.ReturnValue);
    }

    private class TestClass
    {
        public bool Method([gtk::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] string? input)
        {
            return input != null;
        }
    }
}