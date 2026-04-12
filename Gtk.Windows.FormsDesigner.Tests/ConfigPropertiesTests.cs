using System.Runtime.Serialization;
using System.Text;

namespace Gtk.Windows.FormsDesigner.Tests;

public class ConfigPropertiesTests
{
    [Fact]
    public void DefaultValues_ShouldBeTrue()
    {
        // Act
        var config = new configProperties();

        // Assert
        Assert.True(config.EnableUnsafeBinaryFormatterSerialization);
        Assert.True(config.SetAppPaths);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var config = new configProperties
        {
            // Act
            EnableUnsafeBinaryFormatterSerialization = false,
            SetAppPaths = false
        };

        // Assert
        Assert.False(config.EnableUnsafeBinaryFormatterSerialization);
        Assert.False(config.SetAppPaths);
    }

    [Fact]
    public void Serialization_ShouldPreserveValues()
    {
        // Arrange
        var original = new configProperties
        {
            EnableUnsafeBinaryFormatterSerialization = false,
            SetAppPaths = true
        };

        var serializer = new DataContractSerializer(typeof(configProperties));

        using var stream = new MemoryStream();

        // Act - Serialize
        serializer.WriteObject(stream, original);

        stream.Position = 0;

        // Act - Deserialize
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        var deserialized = (configProperties)serializer.ReadObject(stream);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.EnableUnsafeBinaryFormatterSerialization,
            deserialized.EnableUnsafeBinaryFormatterSerialization);
        Assert.Equal(original.SetAppPaths,
            deserialized.SetAppPaths);
    }

    [Fact]
    public void Serialization_Output_ShouldContainExpectedNames()
    {
        // Arrange
        var config = new configProperties();
        var serializer = new DataContractSerializer(typeof(configProperties));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, config);
        var xml = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Contains("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", xml);
        Assert.Contains("Microsoft.NETCore.DotNetHostPolicy.SetAppPaths", xml);
    }
}