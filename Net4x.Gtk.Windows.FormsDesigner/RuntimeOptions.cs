using System.Runtime.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.FormsDesigner;

/// <summary>
/// Represents runtime configuration options for a .NET application, including target framework, supported frameworks,
/// probing paths, and additional configuration properties.
/// </summary>
/// <remarks>This class is typically used to deserialize runtime configuration data from sources such as JSON
/// files or service responses. It provides information required for runtime selection, assembly probing, and framework
/// compatibility. All properties are optional and may be null depending on the configuration source.</remarks>
[DataContract]
public class runtimeOptions
{
    /// <summary>
    /// Gets or sets the target framework moniker (TFM) for the project or assembly.
    /// </summary>
    /// <remarks>The TFM identifies the target framework that the project is intended to run on, such as
    /// ".NETCoreApp,Version=v6.0". This value should conform to the standard TFM format recognized by .NET tools and
    /// libraries.</remarks>
    [DataMember(Order = 0)]
    public string tfm { get; set; }

    /// <summary>
    /// Gets or sets the collection of frameworks associated with this instance.
    /// </summary>
    [DataMember(Order = 1)]
    public framework[] frameworks { get; set; }

    /// <summary>
    /// Gets or sets the collection of additional file system paths to probe when resolving assemblies.
    /// </summary>
    /// <remarks>Each path in the array is used as a search location for assembly resolution. Paths should be
    /// valid and accessible directories. This property is typically used to extend the default probing locations when
    /// loading assemblies dynamically.</remarks>
    [DataMember(Order = 2)]
    public string[] additionalProbingPaths { get; set; }

    /// <summary>
    /// Gets or sets the configuration properties that control runtime serialization and application path settings.
    /// </summary>
    [DataMember(Order = 3)]
    public configProperties configProperties { get; set; }
}
