// This file is mainly to overwrite the native System.ComponentModel.ComponentResourceManager class.
// The native class does not support reading project resource image files.
// GtkSystem.ComponentModel.ComponentResourceManager implements reading of project resource files and image files.
// If the resource image file is not used in the project, there is no need to create this file.

using System.Globalization;

namespace System.ComponentModel;

/// <summary>
/// Provides resource management for components, enabling retrieval of localized resources such as strings, images,
/// and objects for a specified form type.
/// </summary>
/// <remarks>Use this class to access resources associated with a component or form, typically for
/// localization purposes. Resources can be retrieved by name and, optionally, by culture. This class is commonly
/// used in applications that require support for multiple languages or regional settings.
/// </remarks>
public class ComponentResourceManager : Gtk.ComponentModel.ComponentResourceManager
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.ComponentModel.ComponentResourceManager" /> class.
    /// </summary>
    /// <param name="form">
    /// The form.
    /// </param>
    public ComponentResourceManager(Type form) : base(form)
    {

    }

    // Two `new` GetObject overloads used to sit here. They shadowed rather than overrode, so they only
    // applied when the static type was exactly this class - and the culture-taking one discarded its
    // culture and returned the neutral resource. The base class overrides both correctly, including the
    // culture fallback chain, so forwarding shims are not needed.
}