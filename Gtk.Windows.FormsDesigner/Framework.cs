using System.Runtime.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.FormsDesigner;

/// <summary>
/// Represents a software framework with its name and version information.
/// </summary>
[DataContract]
public class framework
{
    /// <summary>
    /// Gets or sets the name associated with the object.
    /// </summary>
    [DataMember(Order = 0)]
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the version associated with the object.
    /// </summary>
    [DataMember(Order = 1)]
    public string version { get; set; }
}
