using System.Globalization;

namespace GtkSystem.ComponentModel;

/// <summary>
/// This is System.ComponentModel.ComponentResourceManager image retrieval implementation
/// Usage method： at form create new under the projectSystem.ComponentModel.ComponentResourceManager and inheritGtkSystem.ComponentModel.ComponentResourceManager that is enough
/// </summary>
public class ComponentResourceManager : System.ComponentModel.ComponentResourceManager
{

    private readonly string formName;

    private readonly Type formtype;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.ComponentModel.ComponentResourceManager" /> class.
    /// </summary>
    /// <param name="form">The form.
    /// </param>
    public ComponentResourceManager(Type form) : base(form)
    {
        formtype = form;
        formName = form.Name;
    }

    /// <summary>
    /// Gets the value of the specified non-string resource localized for the specified culture.
    /// </summary>
    /// <param name="name">The name of the resource to get.
    /// </param>
    /// <param name="culture">
    /// The culture for which the resource is localized. If the resource is not localized for this culture, the resource manager uses fallback rules to locate an appropriate resource.   If this value is null, the
    /// <see cref="T:System.Globalization.CultureInfo">
    /// </see> object is obtained by using the
    /// <see cref="T:System.Globalization.CultureInfo.CurrentUICulture">
    /// </see> property.
    /// </param>
    /// <returns>
    /// The value of the resource, localized for the specified culture. If an appropriate resource set exists but
    /// <paramref name="name">name
    /// </paramref> cannot be found, the method returns null.
    /// </returns>
    public override object GetObject(string name, CultureInfo culture)
    {
        return GetObject(name);
    }

    /// <summary>
    /// Returns the value of the specified non-string resource.
    /// </summary>
    /// <param name="name">The name of the resource to get.
    /// </param>
    /// <returns>
    /// The value of the resource localized for the caller's current culture settings. If an appropriate resource set exists but
    /// <paramref name="name">name
    /// </paramref> cannot be found, the method returns null.
    /// </returns>
    public override object GetObject(string name)
    {
        GtkSystem.Resources.ResourceManager temp = new GtkSystem.Resources.ResourceManager(formtype.FullName, formtype.Assembly);

        return temp.GetObject(name);
    }

    /// <summary>
    /// Returns the value of the specified string resource.
    /// </summary>
    /// <param name="name">The name of the resource to retrieve.
    /// </param>
    /// <returns>
    /// The value of the resource localized for the caller's current UI culture, or null if
    /// <paramref name="name">name
    /// </paramref> cannot be found in a resource set.
    /// </returns>
    public override string GetString(string name)
    {
        GtkSystem.Resources.ResourceManager temp = new GtkSystem.Resources.ResourceManager(formtype.FullName, formtype.Assembly);

        return temp.GetString(name);
    }

}