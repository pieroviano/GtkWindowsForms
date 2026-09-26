using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.FormsDesigner;

/// <summary>
///     The MSBuild task that writes the configuration Visual Studio launches the out-of-process Windows Forms
///     designer server with, for a project built on the GTK implementation of Windows Forms.
/// </summary>
/// <remarks>
///     A wrapper over <see cref="DesignerConfig" />, which holds the rules and the reasons for them. The
///     package's targets run it before a build and again after the designer output group, because the SDK
///     regenerates both files whenever the project or its restore assets change.
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
    /// Gets or sets the project full path.
    /// </summary>
    /// <value>
    /// The project full path, or an empty value to leave the project file alone.
    /// </value>
    /// <remarks>
    /// Optional on purpose: the design-time pass regenerates only the configuration files, and must not
    /// rewrite the project file the IDE has open.
    /// </remarks>
    public string ProjectFullPath { get; set; }

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
    ///     Writes the designer configuration files for the project, and brings the project file itself into
    ///     the shape the designer needs when <see cref="ProjectFullPath" /> names one.
    /// </summary>
    /// <remarks>See <see cref="DesignerConfig" /> for what the two files have to say and why.</remarks>
    /// <returns>Always true: nothing here is worth failing a build over.</returns>
    public override bool Execute()
    {
        if (!string.IsNullOrEmpty(ProjectFullPath))
        {
            CsprojFixer.FixProject(ProjectFullPath);
        }

        DesignerConfig.Write(ObjPath, FileName, TargetFramework, TargetFrameworkVersion);

        return true;
    }
}
