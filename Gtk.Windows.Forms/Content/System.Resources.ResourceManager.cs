// This file is mainly to overwrite the native System.Resources.ResourceManager class.
// The native class does not support reading project resource image files.
// GtkSystem.Resources.ResourceManager implements reading of project resource files and image files.
// If the resource image file is not used in the project, there is no need to create this file.

using System.ComponentModel;
using System.Reflection;

namespace System.Resources.Extensions;

/// <summary>
/// Provides access to resources for a specified assembly, base name, or resource source type. Used to retrieve
/// localized resources such as strings, images, and other data embedded in assemblies.
/// </summary>
/// <remarks>This class is intended for internal use and is not visible in standard IntelliSense due to
/// the
/// <see cref="T:EditorBrowsableAttribute" />. It inherits functionality from
/// <see cref="T:GtkSystem.Resources.ResourceManager" /> to manage resource retrieval. Use the appropriate constructor to
/// specify the resource source, base name, or assembly when initializing the resource manager.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public class ResourceManager : GtkSystem.Resources.ResourceManager
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    /// <param name="usingResourceSet">
    /// The using resource set.
    /// </param>
    public ResourceManager(System.Type resourceSource) : base(null, null, resourceSource)
    {

    }

    /// <summary>
    /// Initializes a new instance of the ResourceManager class for a specific resource set using the provided base
    /// name and assembly.
    /// </summary>
    /// <remarks>Use this constructor to access resources embedded in a particular assembly. The base
    /// name should match the resource file name and is used to locate the resource set for different
    /// cultures.
    /// </remarks>
    /// <param name="baseName">The root name of the resource files without culture or file extension. Cannot be null or empty.
    /// </param>
    /// <param name="assembly">The assembly that contains the resources. Cannot be null.
    /// </param>
    public ResourceManager(string baseName, Assembly assembly) : base(baseName, assembly, null)
    {

    }

    /// <summary>
    /// Initializes a new instance of the ResourceManager class for a specific resource set, using the provided base
    /// name, assembly, and resource source type.
    /// </summary>
    /// <remarks>Use this constructor when you need to specify a resource source type in addition to
    /// the base name and assembly. This can be useful for advanced resource lookup scenarios, such as when
    /// resources are associated with a specific type rather than just an assembly.
    /// </remarks>
    /// <param name="baseName">The root name of the resource files to search for. Typically corresponds to the fully qualified name of the
    /// resource set without the culture or file extension.
    /// </param>
    /// <param name="assembly">The assembly that contains the resources. Must not be null.
    /// </param>
    /// <param name="resourceSource">The type that serves as the source for the resources. Used to help locate and load resource files.
    /// </param>
    public ResourceManager(string baseName, Assembly assembly, System.Type resourceSource) : base(baseName, assembly, resourceSource)
    {

    }

    /// <summary>
    /// Initializes a new instance of the ResourceManager class for use by derived types.
    /// </summary>
    /// <remarks>This protected constructor is intended to be called by subclasses when implementing
    /// custom resource management functionality. It does not perform any initialization itself; derived classes
    /// should provide any required setup.
    /// </remarks>
    protected ResourceManager()
    {

    }
}