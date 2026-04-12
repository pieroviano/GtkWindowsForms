using System.Runtime.Serialization;
using System.Text;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Windows.FormsDesigner.Tests;

public class FrameworkTests
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var framework = new framework
        {
            // Act
            name = "net",
            version = "8.0"
        };

        // Assert
        Assert.Equal("net", framework.name);
        Assert.Equal("8.0", framework.version);
    }

    [Fact]
    public void DefaultValues_ShouldBeNull()
    {
        // Act
        var framework = new framework();

        // Assert
        Assert.Null(framework.name);
        Assert.Null(framework.version);
    }

    [Fact]
    public void Serialization_ShouldPreserveValues()
    {
        // Arrange
        var original = new framework
        {
            name = "net",
            version = "8.0"
        };

        var serializer = new DataContractSerializer(typeof(framework));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, original);
        stream.Position = 0;

        var result = (framework)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.name, result.name);
        Assert.Equal(original.version, result.version);
    }

    [Fact]
    public void Serialization_Output_ShouldContainExpectedElements_InOrder()
    {
        // Arrange
        var framework = new framework
        {
            name = "net",
            version = "8.0"
        };

        var serializer = new DataContractSerializer(typeof(framework));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, framework);
        var xml = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        var nameIndex = xml.IndexOf("<name>", StringComparison.Ordinal);
        var versionIndex = xml.IndexOf("<version>", StringComparison.Ordinal);

        Assert.True(nameIndex >= 0, "name element not found");
        Assert.True(versionIndex >= 0, "version element not found");
        Assert.True(nameIndex < versionIndex, "Order of elements is incorrect");
    }

    [Fact]
    public void Serialization_ShouldHandleNullValues()
    {
        // Arrange
        var framework = new framework
        {
            name = null,
            version = null
        };

        var serializer = new DataContractSerializer(typeof(framework));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, framework);
        stream.Position = 0;

        var result = (framework)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.name);
        Assert.Null(result.version);
    }
}