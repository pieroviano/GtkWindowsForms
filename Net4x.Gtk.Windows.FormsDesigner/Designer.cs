using System.Runtime.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Gtk.Windows.FormsDesigner;

/// <summary>
/// Represents a designer configuration that includes runtime options for customization.
/// </summary>
[DataContract]
public class designer
{
    /// <summary>
    /// Gets or sets the runtime configuration options for the current instance.
    /// </summary>
    /// <remarks>Use this property to specify or retrieve settings that control runtime behavior. The
    /// available options depend on the definition of the <c>runtimeOptions</c> type. Changing these options may affect
    /// how the instance operates during execution.</remarks>
    [DataMember]
    public runtimeOptions runtimeOptions { get; set; }
}
