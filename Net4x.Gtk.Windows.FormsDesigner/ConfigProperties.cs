using System.Runtime.Serialization;

namespace Gtk.Windows.FormsDesigner;

/// <summary>
/// Represents configuration properties that control runtime serialization and application path settings.
/// </summary>
/// <remarks>This class provides options for enabling unsafe binary formatter serialization and for setting
/// application paths via host policy. These properties correspond to runtime configuration switches and may affect
/// application security and startup behavior. Use caution when enabling unsafe serialization features, as they may
/// introduce security risks.</remarks>
[DataContract]
public class configProperties
{
    /// <summary>
    /// Gets or sets a value that indicates whether support for unsafe BinaryFormatter serialization is enabled.
    /// </summary>
    /// <remarks>Enabling unsafe BinaryFormatter serialization allows deserialization of types using
    /// BinaryFormatter, which can introduce security vulnerabilities. It is recommended to disable this setting unless
    /// compatibility with legacy data is required. Use with caution in production environments.</remarks>
    [DataMember(Name = "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization",
                Order = 0)]
    public bool EnableUnsafeBinaryFormatterSerialization { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether application paths are configured for .NET Core host policy resolution.
    /// </summary>
    [DataMember(Name = "Microsoft.NETCore.DotNetHostPolicy.SetAppPaths", Order = 1)]
    public bool SetAppPaths { get; set; } = true;
}
