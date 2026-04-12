using System.Runtime.Serialization;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Windows.FormsDesigner.Tests;

public class DesignerTests
{
    [Fact]
    public void RuntimeOptions_ShouldBeSettable()
    {
        // Arrange
        var options = new runtimeOptions();
        var designer = new designer
        {
            // Act
            runtimeOptions = options
        };

        // Assert
        Assert.Same(options, designer.runtimeOptions);
    }

    [Fact]
    public void RuntimeOptions_DefaultValue_ShouldBeNull()
    {
        // Act
        var designer = new designer();

        // Assert
        Assert.Null(designer.runtimeOptions);
    }

    [Fact]
    public void Serialization_ShouldPreserveRuntimeOptions()
    {
        // Arrange
        var original = new designer
        {
            runtimeOptions = new runtimeOptions()
        };

        var serializer = new DataContractSerializer(typeof(designer));

        using var stream = new MemoryStream();

        // Act - Serialize
        serializer.WriteObject(stream, original);
        stream.Position = 0;

        // Act - Deserialize
        var result = (designer)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.runtimeOptions);
    }

    [Fact]
    public void Serialization_ShouldHandleNullRuntimeOptions()
    {
        // Arrange
        var original = new designer
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            runtimeOptions = null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        };

        var serializer = new DataContractSerializer(typeof(designer));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, original);
        stream.Position = 0;

        var result = (designer)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.runtimeOptions);
    }
}