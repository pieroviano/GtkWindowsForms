// This file is mainly to overwrite the native System.ComponentModel.ComponentResourceManager class.
// The native class does not support reading project resource image files.
// GtkSystem.ComponentModel.ComponentResourceManager implements reading of project resource files and image files.
// If the resource image file is not used in the project, there is no need to create this file.

using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides resource management for components, enabling retrieval of localized resources such as strings, images,
    /// and objects for a specified form type.
    /// </summary>
    /// <remarks>Use this class to access resources associated with a component or form, typically for
    /// localization purposes. Resources can be retrieved by name and, optionally, by culture. This class is commonly
    /// used in applications that require support for multiple languages or regional settings.
    /// </remarks>
    public class ComponentResourceManager : GtkSystem.ComponentModel.ComponentResourceManager
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GtkSystem.ComponentModel.ComponentResourceManager" /> class.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        public ComponentResourceManager(Type form) : base(form)
        {

        }

        /// <summary>
        /// Retrieves an object from the resource set with the specified name, using the specified culture information.
        /// </summary>
        /// <param name="name">The name of the resource object to retrieve. Cannot be null.
        /// </param>
        /// <param name="culture">The culture information to use when searching for the resource. If null, the default culture is used.
        /// </param>
        /// <returns>An object representing the requested resource, or null if the resource is not found.
        /// </returns>
        public new object GetObject(string name, CultureInfo culture)
        {

            return GetObject(name);
        }

        /// <summary>
        /// Retrieves an object associated with the specified name from the resource set.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve. Cannot be null.
        /// </param>
        /// <returns>The object corresponding to the specified resource name, or null if the resource is not found.
        /// </returns>
        public new object GetObject(string name)
        {
            return base.GetObject(name);
        }

    }

}