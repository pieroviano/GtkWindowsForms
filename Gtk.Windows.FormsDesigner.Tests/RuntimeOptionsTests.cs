using System.Runtime.Serialization;
using System.Text;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Gtk.Windows.FormsDesigner.Tests;

public class RuntimeOptionsTests
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var options = new runtimeOptions
        {
            // Act
            tfm = "net8.0",
            frameworks = new[]
            {
                new framework { name = "net", version = "8.0" }
            },
            additionalProbingPaths = new[] { "/usr/lib", "/app/bin" },
            configProperties = new configProperties()
        };

        // Assert
        Assert.Equal("net8.0", options.tfm);
        Assert.Single(options.frameworks);
        Assert.Equal(2, options.additionalProbingPaths.Length);
        Assert.NotNull(options.configProperties);
    }

    [Fact]
    public void DefaultValues_ShouldBeNull()
    {
        // Act
        var options = new runtimeOptions();

        // Assert
        Assert.Null(options.tfm);
        Assert.Null(options.frameworks);
        Assert.Null(options.additionalProbingPaths);
        Assert.Null(options.configProperties);
    }

    [Fact]
    public void Serialization_ShouldPreserveAllValues()
    {
        // Arrange
        var original = new runtimeOptions
        {
            tfm = "net8.0",
            frameworks = new[]
            {
                new framework { name = "net", version = "8.0" },
                new framework { name = "netcoreapp", version = "7.0" }
            },
            additionalProbingPaths = new[] { "/path1", "/path2" },
            configProperties = new configProperties
            {
                EnableUnsafeBinaryFormatterSerialization = false,
                SetAppPaths = true
            }
        };

        var serializer = new DataContractSerializer(typeof(runtimeOptions));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, original);
        stream.Position = 0;

        var result = (runtimeOptions)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(original.tfm, result.tfm);

        Assert.Equal(original.frameworks.Length, result.frameworks.Length);
        Assert.Equal(original.frameworks[0].name, result.frameworks[0].name);
        Assert.Equal(original.frameworks[1].version, result.frameworks[1].version);

        Assert.Equal(original.additionalProbingPaths, result.additionalProbingPaths);

        Assert.NotNull(result.configProperties);
        Assert.Equal(original.configProperties.EnableUnsafeBinaryFormatterSerialization,
            result.configProperties.EnableUnsafeBinaryFormatterSerialization);
        Assert.Equal(original.configProperties.SetAppPaths,
            result.configProperties.SetAppPaths);
    }

    [Fact]
    public void Serialization_ShouldHandleNullValues()
    {
        // Arrange
        var original = new runtimeOptions
        {
            tfm = null,
            frameworks = null,
            additionalProbingPaths = null,
            configProperties = null
        };

        var serializer = new DataContractSerializer(typeof(runtimeOptions));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, original);
        stream.Position = 0;

        var result = (runtimeOptions)serializer.ReadObject(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.tfm);
        Assert.Null(result.frameworks);
        Assert.Null(result.additionalProbingPaths);
        Assert.Null(result.configProperties);
    }

    [Fact]
    public void Serialization_Output_ShouldContainExpectedElements_InOrder()
    {
        // Arrange
        var options = new runtimeOptions
        {
            tfm = "net8.0",
            frameworks = new[]
            {
                new framework { name = "net", version = "8.0" }
            },
            additionalProbingPaths = new[] { "/path" },
            configProperties = new configProperties()
        };

        var serializer = new DataContractSerializer(typeof(runtimeOptions));

        using var stream = new MemoryStream();

        // Act
        serializer.WriteObject(stream, options);
        var xml = Encoding.UTF8.GetString(stream.ToArray());

        // Assert (order matters due to DataMember(Order = ...))
        var tfmIndex = xml.IndexOf("<tfm>", StringComparison.Ordinal);
        var frameworksIndex = xml.IndexOf("<frameworks>", StringComparison.Ordinal);
        var probingIndex = xml.IndexOf("<additionalProbingPaths", StringComparison.Ordinal);
        var configIndex = xml.IndexOf("<configProperties>", StringComparison.Ordinal);

        Assert.True(tfmIndex >= 0);
        Assert.True(frameworksIndex >= 0);
        Assert.True(probingIndex >= 0);
        Assert.True(configIndex >= 0);

        Assert.True(tfmIndex < frameworksIndex);
        Assert.True(frameworksIndex < probingIndex);
        Assert.True(probingIndex < configIndex);
    }

    [Fact]
    public void Frameworks_Array_ShouldAllowEmpty()
    {
        // Arrange
        var options = new runtimeOptions
        {
            frameworks = new framework[0]
        };

        // Assert
        Assert.NotNull(options.frameworks);
        Assert.Empty(options.frameworks);
    }

    [Fact]
    public void AdditionalProbingPaths_ShouldAllowEmpty()
    {
        // Arrange
        var options = new runtimeOptions
        {
            additionalProbingPaths = new string[0]
        };

        // Assert
        Assert.NotNull(options.additionalProbingPaths);
        Assert.Empty(options.additionalProbingPaths);
    }
}