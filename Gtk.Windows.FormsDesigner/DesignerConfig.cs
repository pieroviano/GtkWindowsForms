using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Gtk.Windows.FormsDesigner.Properties;

namespace Gtk.Windows.FormsDesigner;

/// <summary>
///     Writes the two files Visual Studio launches the out-of-process Windows Forms designer server with.
/// </summary>
/// <remarks>
///     <para>
///         A GTK project targets a plain <c>netX.0</c> moniker with <c>UseWindowsForms</c> off, so the SDK
///         writes it a designer runtimeconfig naming <c>Microsoft.NETCore.App</c> alone. The designer server
///         is a real Windows Forms process: started that way it cannot load
///         <c>System.Windows.Forms</c> and dies, and the IDE reports a <c>ServerProcess</c> failure instead
///         of opening the form. <c>Microsoft.WindowsDesktop.App</c> has to be there as well.
///     </para>
///     <para>
///         The dependency list stays empty for the opposite reason: with the project's package closure in it
///         the server resolves <c>System.Windows.Forms</c> from the GTK implementation the project compiles
///         against rather than from the shared framework. What the designer needs beyond the framework comes
///         from the shadow-copied output directory, through <c>SetAppPaths</c>.
///     </para>
///     <para>
///         Kept free of Microsoft.Build so the rules can be tested without an MSBuild host;
///         <see cref="GtkWindowsForms" /> is only the task wrapper.
///     </para>
/// </remarks>
internal static class DesignerConfig
{
    private const string CoreFramework = "Microsoft.NETCore.App";

    private const string DesktopFramework = "Microsoft.WindowsDesktop.App";

    /// <summary>Writes both files for one project and target framework.</summary>
    /// <param name="objPath">The intermediate output directory, created when it does not exist yet.</param>
    /// <param name="fileName">The assembly name the designer derives the two file names from.</param>
    /// <param name="targetFramework">The target framework moniker, e.g. <c>net10.0</c>.</param>
    /// <param name="targetFrameworkVersion">The MSBuild target framework version, e.g. <c>v10.0</c>.</param>
    internal static void Write(string objPath, string fileName, string targetFramework,
                              string targetFrameworkVersion)
    {
        if (!Directory.Exists(objPath))
        {
            Directory.CreateDirectory(objPath);
        }

        WriteDependencies(DependenciesPath(objPath, fileName), targetFrameworkVersion);
        WriteRuntimeConfig(RuntimeConfigPath(objPath, fileName), targetFramework, targetFrameworkVersion);
    }

    /// <summary>The path of the dependency list the designer server is launched with.</summary>
    internal static string DependenciesPath(string objPath, string fileName) =>
        Path.Combine(objPath, fileName + ".designer.deps.json");

    /// <summary>The path of the runtime configuration the designer server is launched with.</summary>
    internal static string RuntimeConfigPath(string objPath, string fileName) =>
        Path.Combine(objPath, fileName + ".designer.runtimeconfig.json");

    /// <summary>
    ///     Replaces the dependency list with the empty one. The SDK regenerates it from the project's whole
    ///     package closure whenever the project or its restore assets change, so this overwrites rather than
    ///     creates.
    /// </summary>
    private static void WriteDependencies(string path, string targetFrameworkVersion) =>
        WriteIfDifferent(path, Resources.GtkWindowsForms_Content.Replace("[0]", targetFrameworkVersion));

    /// <summary>Adds <see cref="DesktopFramework" /> to the runtime configuration, keeping any it already names.</summary>
    private static void WriteRuntimeConfig(string path, string targetFramework, string targetFrameworkVersion)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists && fileInfo.IsReadOnly)
        {
            return;
        }

        var version = FrameworkVersion(targetFrameworkVersion);

        using (var fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            try
            {
                var graph = new designer();
                var contractJsonSerializer = new DataContractJsonSerializer(typeof(designer));
                if (fileStream.Length > 100L)
                {
                    try
                    {
                        graph = contractJsonSerializer.ReadObject(fileStream) as designer ?? graph;
                        if (graph.runtimeOptions?.frameworks != null &&
                            graph.runtimeOptions.frameworks.Length >= 2 &&
                            graph.runtimeOptions.frameworks.Any(m => m.name == DesktopFramework))
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(ex);
                    }
                }

                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                graph.runtimeOptions = new runtimeOptions
                {
                    tfm = targetFramework,
                    frameworks = new[]
                    {
                        new framework { name = CoreFramework, version = version },
                        new framework { name = DesktopFramework, version = version }
                    },
                    additionalProbingPaths = new[]
                    {
                        folderPath + "\\.dotnet\\store\\|arch|\\|tfm|",
                        folderPath + "\\.nuget\\packages"
                    },
                    configProperties = new configProperties()
                };
                fileStream.SetLength(0L);
                fileStream.Flush();
                contractJsonSerializer.WriteObject(fileStream, graph);
                fileStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>The framework version a runtimeconfig names, from an MSBuild target framework version.</summary>
    private static string FrameworkVersion(string targetFrameworkVersion) =>
        targetFrameworkVersion.Substring(1) + ".0";

    /// <summary>
    ///     Writes <paramref name="contents" /> to <paramref name="path" /> unless the file already holds
    ///     them, or is read-only because the user pinned it by hand.
    /// </summary>
    private static void WriteIfDifferent(string path, string contents)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            if (fileInfo.IsReadOnly)
            {
                return;
            }

            try
            {
                if (File.ReadAllText(path) == contents)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }
        }

        File.WriteAllText(path, contents);
    }
}
