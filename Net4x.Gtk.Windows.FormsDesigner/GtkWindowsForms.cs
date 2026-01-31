using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Gtk.Windows.FormsDesigner.Properties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.FormsDesigner;

/// <summary>
///     Represents an MSBuild task that configures runtime dependencies for Windows Forms applications targeting .NET Core
///     or .NET 5+ on GTK platforms.
/// </summary>
/// <remarks>
///     This task generates or updates designer dependency and runtime configuration files required for
///     Windows Forms projects on GTK environments. It ensures that the necessary framework references are present for
///     proper runtime execution. All required properties must be set before execution. This task is typically used in
///     build
///     processes to support cross-platform Windows Forms development.
/// </remarks>
public class GtkWindowsForms : Task
{
    /// <summary>
    ///     Gets or sets the object path used to identify the target resource.
    /// </summary>
    [Required]
    public string ObjPath { get; set; }

    /// <summary>
    ///     Gets or sets the name of the file associated with this instance.
    /// </summary>
    [Required]
    public string FileName { get; set; }

    /// <summary>
    ///     Gets or sets the target .NET framework for the project.
    /// </summary>
    /// <remarks>
    ///     Specify the framework using a valid framework moniker, such as "net6.0" or "netstandard2.0".
    ///     This property is required for correct project configuration.
    /// </remarks>
    [Required]
    public string TargetFramework { get; set; }

    /// <summary>
    ///     Gets or sets the version of the target .NET Framework for the project.
    /// </summary>
    /// <remarks>
    ///     Specify the target framework version using the format recognized by MSBuild, such as "v4.8"
    ///     or "v3.5". This property determines which .NET APIs are available to the project and may affect compatibility
    ///     with libraries and tools.
    /// </remarks>
    [Required]
    public string TargetFrameworkVersion { get; set; }

    /// <summary>
    ///     Executes the task to ensure required designer configuration files are present and updated for the specified
    ///     target framework.
    /// </summary>
    /// <remarks>
    ///     This method creates or updates designer-related JSON configuration files in the specified
    ///     object path if they do not exist or require modification. It ensures that the configuration reflects the current
    ///     target framework and includes necessary runtime options. The method always returns true, indicating successful
    ///     execution, even if some operations are skipped due to existing files or read-only attributes.
    /// </remarks>
    /// <returns>true if the designer configuration files are successfully created or updated; otherwise, false.</returns>
    public override bool Execute()
    {
        if (!Directory.Exists(ObjPath))
        {
            Directory.CreateDirectory(ObjPath);
        }

        if (Directory.Exists(ObjPath))
        {
            var objPath = ObjPath;
            var str = TargetFrameworkVersion.Substring(1) + ".0";
            var path = $"{objPath}\\{FileName}.designer.deps.json";
            var fileName = $"{objPath}\\{FileName}.designer.runtimeconfig.json";
            if (!File.Exists(path))
            {
                var contents = Resources.GtkWindowsForms_Content.Replace("[0]",
                    TargetFrameworkVersion);
                File.WriteAllText(path, contents);
            }

            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists && fileInfo.IsReadOnly)
            {
                return true;
            }

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
                            graph = (designer)contractJsonSerializer.ReadObject(fileStream);
                            if (graph?.runtimeOptions?.frameworks != null &&
                                graph.runtimeOptions.frameworks.Length >= 2)
                            {
                                if (graph.runtimeOptions.frameworks.Any<framework>(
                                        (Func<framework, bool>)(m => m.name == "Microsoft.WindowsDesktop.App")))
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerFactoryContainer.Instance.LoggerFactory.Debug(ex);
                        }
                    }

                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    graph.runtimeOptions = new runtimeOptions
                    {
                        tfm = TargetFramework,
                        frameworks = new framework[2]
                    };
                    graph.runtimeOptions.frameworks[0] = new framework
                    {
                        name = "Microsoft.NETCore.App",
                        version = str
                    };
                    graph.runtimeOptions.frameworks[1] = new framework
                    {
                        name = "Microsoft.WindowsDesktop.App",
                        version = str
                    };
                    graph.runtimeOptions.additionalProbingPaths = new[]
                    {
                        folderPath + "\\.dotnet\\store\\|arch|\\|tfm|",
                        folderPath + "\\.nuget\\packages"
                    };
                    graph.runtimeOptions.configProperties = new configProperties();
                    fileStream.SetLength(0L);
                    fileStream.Flush();
                    contractJsonSerializer.WriteObject(fileStream, graph);
                    fileStream.Flush();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return true;
    }
}