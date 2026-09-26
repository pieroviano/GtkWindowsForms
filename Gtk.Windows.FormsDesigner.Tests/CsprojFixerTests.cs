using System.Xml.Linq;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Gtk.Windows.FormsDesigner.Tests;

public class CsprojFixerTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [Fact]
    public void FixProject_FileDoesNotExist_ShouldThrow()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csproj");

        // Act & Assert
        var ex = Assert.Throws<FileNotFoundException>(() => CsprojFixer.FixProject(path));
        Assert.Contains("Project file not found", ex.Message);
    }

    [Fact]
    public void FixProject_ShouldRemoveWindowsFromTargetFramework()
    {
        // Arrange
        File.WriteAllText(_tempFile, @"
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
  </PropertyGroup>
</Project>");

        // Act
        CsprojFixer.FixProject(_tempFile);

        // Assert
        var doc = XDocument.Load(_tempFile);
        var tf = doc.Descendants("TargetFramework").First().Value;

        Assert.Equal("net8.0", tf);
    }

    [Fact]
    public void FixProject_ShouldHandleMultipleTargetFrameworks()
    {
        // Arrange
        File.WriteAllText(_tempFile, @"
<Project>
  <PropertyGroup>
    <TargetFrameworks>net8.0-windows;net7.0-windows;;</TargetFrameworks>
  </PropertyGroup>
</Project>");

        // Act
        CsprojFixer.FixProject(_tempFile);

        // Assert
        var doc = XDocument.Load(_tempFile);
        var tfs = doc.Descendants("TargetFrameworks").First().Value;

        Assert.Equal("net8.0;net7.0", tfs);
    }

    [Theory]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("trur")] // typo case handled by implementation
    public void FixProject_ShouldSetUseWindowsFormsToFalse_WhenTrueLike(string input)
    {
        // Arrange
        File.WriteAllText(_tempFile, $@"
<Project>
  <PropertyGroup>
    <UseWindowsForms>{input}</UseWindowsForms>
  </PropertyGroup>
</Project>");

        // Act
        CsprojFixer.FixProject(_tempFile);

        // Assert
        var doc = XDocument.Load(_tempFile);
        var value = doc.Descendants("UseWindowsForms").First().Value;

        Assert.Equal("false", value);
    }

    [Fact]
    public void FixProject_ShouldNotChangeUseWindowsForms_WhenAlreadyFalse()
    {
        // Arrange
        File.WriteAllText(_tempFile, @"
<Project>
  <PropertyGroup>
    <UseWindowsForms>false</UseWindowsForms>
  </PropertyGroup>
</Project>");

        // Act
        CsprojFixer.FixProject(_tempFile);

        // Assert
        var doc = XDocument.Load(_tempFile);
        var value = doc.Descendants("UseWindowsForms").First().Value;

        Assert.Equal("false", value);
    }

    [Fact]
    public void FixProject_ShouldHandleXmlNamespace()
    {
        // Arrange
        File.WriteAllText(_tempFile, @"
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
  </PropertyGroup>
</Project>");

        // Act
        CsprojFixer.FixProject(_tempFile);

        // Assert
        var doc = XDocument.Load(_tempFile);
        var ns = doc.Root.Name.Namespace;

        var tf = doc.Descendants(ns + "TargetFramework").First().Value;
        Assert.Equal("net8.0", tf);
    }

    /// <summary>
    ///     The task runs on every build, and now at design time too, so a project that already is
    ///     GTK-shaped must come out byte for byte the same: rewriting it would make the IDE keep asking
    ///     to reload the project.
    /// </summary>
    [Fact]
    public void FixProject_ShouldNotRewriteTheFile_WhenNothingNeedsFixing()
    {
        // Arrange
        const string text = @"
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <UseWindowsForms>false</UseWindowsForms>
  </PropertyGroup>
</Project>";
        File.WriteAllText(_tempFile, text);

        // Act
        var rewritten = CsprojFixer.FixProject(_tempFile);

        // Assert
        Assert.False(rewritten);
        Assert.Equal(text, File.ReadAllText(_tempFile));
    }

    [Fact]
    public void FixProject_ShouldReportThatItFixedTheFile()
    {
        // Arrange
        File.WriteAllText(_tempFile, @"
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
  </PropertyGroup>
</Project>");

        // Act & Assert
        Assert.True(CsprojFixer.FixProject(_tempFile));
    }

    [Fact]
    public void FixProject_ShouldNotFail_WhenElementsMissing()
    {
        // Arrange
        File.WriteAllText(_tempFile, "<Project></Project>");

        // Act
        var ex = Record.Exception(() => CsprojFixer.FixProject(_tempFile));

        // Assert
        Assert.Null(ex);
    }
}