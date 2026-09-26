using System.Text.Json;

namespace Gtk.Windows.FormsDesigner.Tests;

public class DesignerConfigTests : IDisposable
{
    private readonly string _objPath =
        Path.Combine(Path.GetTempPath(), "DesignerConfigTests." + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_objPath))
            Directory.Delete(_objPath, true);
    }

    private void Write() => DesignerConfig.Write(_objPath, "Sample", "net10.0", "v10.0");

    private string DepsPath => DesignerConfig.DependenciesPath(_objPath, "Sample");

    private string RuntimeConfigPath => DesignerConfig.RuntimeConfigPath(_objPath, "Sample");

    private JsonElement RuntimeOptions() =>
        JsonDocument.Parse(File.ReadAllText(RuntimeConfigPath)).RootElement.GetProperty("runtimeOptions");

    [Fact]
    public void Write_ShouldCreateTheObjDirectory_WhenItIsMissing()
    {
        Write();

        Assert.True(Directory.Exists(_objPath));
    }

    [Fact]
    public void Write_ShouldListTheWindowsDesktopFramework()
    {
        Write();

        var frameworks = RuntimeOptions().GetProperty("frameworks").EnumerateArray()
            .Select(f => f.GetProperty("name").GetString())
            .ToArray();

        Assert.Equal(new[] { "Microsoft.NETCore.App", "Microsoft.WindowsDesktop.App" }, frameworks);
    }

    [Fact]
    public void Write_ShouldTakeTheFrameworkVersionFromTheTargetFrameworkVersion()
    {
        Write();

        var versions = RuntimeOptions().GetProperty("frameworks").EnumerateArray()
            .Select(f => f.GetProperty("version").GetString());

        Assert.All(versions, version => Assert.Equal("10.0.0", version));
    }

    [Fact]
    public void Write_ShouldSetAppPaths()
    {
        Write();

        Assert.True(RuntimeOptions().GetProperty("configProperties")
            .GetProperty("Microsoft.NETCore.DotNetHostPolicy.SetAppPaths").GetBoolean());
    }

    [Fact]
    public void Write_ShouldWriteAnEmptyDependencyList()
    {
        Write();

        var deps = JsonDocument.Parse(File.ReadAllText(DepsPath)).RootElement;

        Assert.Equal(".NETCoreApp,Version=v10.0", deps.GetProperty("runtimeTarget").GetProperty("name").GetString());
        Assert.Empty(deps.GetProperty("targets").GetProperty(".NETCoreApp,Version=v10.0").EnumerateObject());
        Assert.Empty(deps.GetProperty("libraries").EnumerateObject());
    }

    /// <summary>
    ///     The SDK regenerates the file with the project's whole package closure whenever the project or
    ///     its restore assets change; that list makes the designer resolve the GTK implementation instead
    ///     of the real System.Windows.Forms, so it has to be replaced and not merely left alone.
    /// </summary>
    [Fact]
    public void Write_ShouldReplaceADependencyListTheSdkRegenerated()
    {
        Directory.CreateDirectory(_objPath);
        File.WriteAllText(DepsPath,
            """
            {
              "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0", "signature": "" },
              "targets": { ".NETCoreApp,Version=v10.0": { "Gtk.Windows.Forms.Base/1.0.0": {} } },
              "libraries": { "Gtk.Windows.Forms.Base/1.0.0": {} }
            }
            """);

        Write();

        var deps = JsonDocument.Parse(File.ReadAllText(DepsPath)).RootElement;
        Assert.Empty(deps.GetProperty("libraries").EnumerateObject());
    }

    /// <summary>
    ///     The same applies to the runtimeconfig: what the SDK writes for a project with
    ///     <c>UseWindowsForms</c> off names Microsoft.NETCore.App on its own.
    /// </summary>
    [Fact]
    public void Write_ShouldReplaceARuntimeConfigThatNamesOnlyTheCoreFramework()
    {
        Directory.CreateDirectory(_objPath);
        File.WriteAllText(RuntimeConfigPath,
            """
            {
              "runtimeOptions": {
                "tfm": "net10.0",
                "framework": { "name": "Microsoft.NETCore.App", "version": "10.0.0" },
                "configProperties": { "Microsoft.NETCore.DotNetHostPolicy.SetAppPaths": true }
              }
            }
            """);

        Write();

        Assert.Contains("Microsoft.WindowsDesktop.App", File.ReadAllText(RuntimeConfigPath));
    }

    /// <summary>A file the user pinned by hand wins over what this task would write.</summary>
    [Fact]
    public void Write_ShouldLeaveReadOnlyFilesAlone()
    {
        Directory.CreateDirectory(_objPath);
        const string pinned = "{ \"pinned\": true }";
        File.WriteAllText(DepsPath, pinned);
        File.WriteAllText(RuntimeConfigPath, pinned);
        File.SetAttributes(DepsPath, FileAttributes.ReadOnly);
        File.SetAttributes(RuntimeConfigPath, FileAttributes.ReadOnly);

        try
        {
            Write();

            Assert.Equal(pinned, File.ReadAllText(DepsPath));
            Assert.Equal(pinned, File.ReadAllText(RuntimeConfigPath));
        }
        finally
        {
            File.SetAttributes(DepsPath, FileAttributes.Normal);
            File.SetAttributes(RuntimeConfigPath, FileAttributes.Normal);
        }
    }
}
