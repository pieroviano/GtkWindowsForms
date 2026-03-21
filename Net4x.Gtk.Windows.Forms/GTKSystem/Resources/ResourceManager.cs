using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GtkSystem.Resources;

/// <summary>
/// ResourceManager is a class that provides convenient access to culture-specific resources at runtime. It enables you to retrieve resources based on the current culture or a specified culture. This class is typically used in applications that support multiple languages and cultures, allowing you to manage and access localized resources such as strings, images, and other data.
/// </summary>
/// <seealso cref="T:System.Resources.ResourceManager" />
[EditorBrowsable(EditorBrowsableState.Never)]
public class ResourceManager : System.Resources.ResourceManager
{
    /// <summary>
    /// The get resource information
    /// </summary>
    public ResourceInfo GetResourceInfo = new ResourceInfo();

    internal const string ResFileExtension = ".resources";

    private readonly Assembly _assembly;

    private readonly string _baseName;

    private readonly System.Type _resourceSource;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    /// <param name="usingResourceSet">
    /// The using resource set.
    /// </param>
    public ResourceManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet) : this(null, null, usingResourceSet)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    /// <param name="baseName">
    /// The root name of the resource file without its extension but including any fully qualified namespace name. For example, the root name for the resource file named MyApplication.MyResource.en-US.resources is MyApplication.MyResource.
    /// </param>
    /// <param name="assembly">
    /// The main assembly for the resources.
    /// </param>
    public ResourceManager(string baseName, Assembly assembly) : this(baseName, assembly, null)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    /// <param name="baseName">
    /// The root name of the resource file without its extension but including any fully qualified namespace name. For example, the root name for the resource file named MyApplication.MyResource.en-US.resources is MyApplication.MyResource.
    /// </param>
    /// <param name="assembly">
    /// The main assembly for the resources.
    /// </param>
    /// <param name="usingResourceSet">
    /// The type of the custom
    /// <see cref="T:System.Resources.ResourceSet">
    /// </see> to use. If null, the default runtime
    /// <see cref="T:System.Resources.ResourceSet">
    /// </see> object is used.
    /// </param>
    public ResourceManager(string baseName, Assembly assembly, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet) : base(baseName, assembly, usingResourceSet)
    {
        this._baseName = baseName;
        this._assembly = assembly;
        this._resourceSource = usingResourceSet;
        GetResourceInfo.Assembly = assembly;
        GetResourceInfo.BaseName = baseName;
        GetResourceInfo.SourceType = usingResourceSet;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    protected ResourceManager()
    {

    }

    /// <summary>
    /// Gets the value of the specified non-string resource localized for the specified culture.
    /// </summary>
    /// <param name="name">
    /// The name of the resource to get.
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
    /// <paramref name="name">
    /// name
    /// </paramref> cannot be found, the method returns null.
    /// </returns>
    public override object GetObject(string name, CultureInfo culture)
    {
        var result = GetObject(name);
        var stack = new StackTrace(true);
        var method = (System.Reflection.MethodInfo)stack.GetFrame(1).GetMethod();
        if (method.ReturnType.Name == "Object" || method.ReturnType.Name.Equals(result.GetType().Name))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the value of the specified non-string resource.
    /// </summary>
    /// <param name="name">
    /// The name of the resource to get.
    /// </param>
    /// <returns>
    /// The value of the resource localized for the caller's current culture settings. If an appropriate resource set exists but
    /// <paramref name="name">
    /// name
    /// </paramref> cannot be found, the method returns null.
    /// </returns>
    public override object GetObject(string name)
    {
        GetResourceInfo.ResourceName = name;
        var obj = ReadResourceData(name);
        if (obj == null)
        {
            if (name.EndsWith(".ImageStream"))
            {
                return new ImageListStreamer(new ImageList()) { ResourceInfo = GetResourceInfo };
            }
            else
            {
                var fileName = name;
                var filebytes = ReadResourceFile(name);
                if (filebytes == null)
                {
                    var _formName = Path.GetExtension(this.BaseName).TrimStart('.');
                    var files = Directory.GetFiles($"./Resources/{_formName}", $"{fileName}.*", SearchOption.AllDirectories);
                    if (files != null && files.Length > 0)
                    {
                        fileName = files[0];
                        filebytes = File.ReadAllBytes(files[0]);
                    }
                }
                if (name.EndsWith(".BackgroundImage"))
                {
                    return new System.Drawing.Bitmap(filebytes) { FileName = fileName };
                }
                else if (name.EndsWith(".Image"))
                {
                    return new System.Drawing.Bitmap(filebytes) { FileName = fileName };
                }
                else if (name.EndsWith(".Icon"))
                {
                    return new System.Drawing.Icon(filebytes) { FileName = fileName };
                }
                else if (filebytes == null)
                {
                    return new System.Drawing.Bitmap(0, 0);
                }
                else
                {
                    return new System.Drawing.Bitmap(filebytes) { FileName = fileName };
                }
            }
        }
        else
        {
            return obj;
        }
    }

    /// <summary>
    /// Returns the value of the specified string resource.
    /// </summary>
    /// <param name="name">
    /// The name of the resource to retrieve.
    /// </param>
    /// <returns>
    /// The value of the resource localized for the caller's current UI culture, or null if
    /// <paramref name="name">
    /// name
    /// </paramref> cannot be found in a resource set.
    /// </returns>
    public override string GetString(string name)
    {
        return GetString(name, null);
    }

    /// <summary>
    /// Returns the value of the string resource localized for the specified culture.
    /// </summary>
    /// <param name="name">
    /// The name of the resource to retrieve.
    /// </param>
    /// <param name="culture">
    /// An object that represents the culture for which the resource is localized.
    /// </param>
    /// <returns>
    /// The value of the resource localized for the specified culture, or null if
    /// <paramref name="name">
    /// name
    /// </paramref> cannot be found in a resource set.
    /// </returns>
    public override string GetString(string name, CultureInfo culture)
    {
        return ReadResourceText(name);
    }

    private object ReadResourceData(string name)
    {
        if (_assembly == null)
            throw new FileNotFoundException();
        else
        {
            try
            {
                var stream = _assembly.GetManifestResourceStream(_baseName + ResFileExtension);
                var reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
                IDictionaryEnumerator dict = reader.GetEnumerator();
                while (dict.MoveNext())
                {
                    if (dict.Key.ToString() == name)
                    {
                        try
                        {
                            if (dict.Value is ImageListStreamer streamer)
                            {
                                streamer.ResourceInfo = GetResourceInfo;
                                return streamer;
                            }
                            else
                                return dict.Value;
                        }
                        catch
                        {
                            // image format content cannot be extracted
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private byte[] ReadResourceFile(string name)
    {
        byte[] result = null;
        try
        {
            var filepath = $"./{ReadResourceData("this_FileName")}"; //linux the path must use/
            if (System.IO.File.Exists(filepath))
            {
                try
                {
                    var doc = new XmlDocument();
                    var xmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };
                    doc.Load(filepath);
                    var docElem = doc.DocumentElement;
                    XmlNodeList nodes = docElem.SelectNodes("data");
                    //<data name="pictureBox1.Image" type="System.Drawing.Bitmap, System.Drawing.Common" mimetype="application/x-microsoft.net.object.bytearray.base64">
                    //<value> </value>
                    //</data>

                    foreach (XmlNode xn in nodes)
                    {
                        if (xn.Attributes["name"].Value == name)
                        {
                            string data = xn.SelectSingleNode("value").InnerText;
                            result = System.Convert.FromBase64String(data);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.Write($"Unexpected exception in ReadResourceFile: {ex}");
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Trace.Write($"Unexpected exception in ReadResourceFile: {ex}");
        }

        return result;
    }

    private string ReadResourceText(string name)
    {
        if (_assembly == null)
            throw new FileNotFoundException();
        else
        {
            var stream = _assembly.GetManifestResourceStream(_baseName + ".resources");
            var reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
            IDictionaryEnumerator dict = reader.GetEnumerator();
            while (dict.MoveNext())
            {
                if (dict.Key.ToString() == name)
                {
                    try
                    {
                        return dict.Value.ToString();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// ResourceInfo is a class that contains information about a resource, such as the assembly it belongs to, the base name of the resource, the bytes of the image (if applicable), the name of the resource, and the source type of the resource. This class is used to store and access information about resources in a convenient way.
    /// </summary>
    public class ResourceInfo
    {
        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        /// <value>
        /// The assembly.
        /// </value>
        public Assembly Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the base.
        /// </summary>
        /// <value>
        /// The name of the base.
        /// </value>
        public string BaseName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image bytes.
        /// </summary>
        /// <value>
        /// The image bytes.
        /// </value>
        public byte[] ImageBytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value>
        public System.Type SourceType
        {
            get;
            set;
        }
    }
}