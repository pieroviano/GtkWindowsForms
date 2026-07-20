using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using GtkSystem.Windows.Forms.Resources;

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
    public ResourceInfo GetResourceInfo = new();

    internal const string ResFileExtension = ".resources";

    private readonly Assembly? _assembly;

    private readonly string? _baseName;

    private readonly Type? _resourceSource;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Resources.ResourceManager" /> class.
    /// </summary>
    /// <param name="usingResourceSet">
    /// The using resource set.
    /// </param>
    public ResourceManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? usingResourceSet) : this(string.Empty, Assembly.GetExecutingAssembly(), usingResourceSet)
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
    public ResourceManager(string? baseName, Assembly? assembly, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? usingResourceSet) : base(baseName, assembly, usingResourceSet)
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
    public override object? GetObject(string name, CultureInfo culture)
    {
        return GetObject(name);
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
            obj = ReadResourceFile(name);

        if (obj == null)
        {
            if (name.EndsWith(".ImageStream"))
            {
                return new ImageListStreamer(new ImageList()) { ResourceInfo = GetResourceInfo };
            }
            else
            {
                byte[]? filebytes = null;
                var fileName = name;
                var _formName = Path.GetExtension(this._baseName).TrimStart('.');
                if (_formName.EndsWith("Resources"))
                    _formName = "";

                var directory = Path.Combine("Resources", _formName);
                var files = Directory.GetFiles(directory, $"{fileName}.*", SearchOption.TopDirectoryOnly);
                if (files != null && files.Length > 0)
                {
                    fileName = files[0];
                    filebytes = File.ReadAllBytes(files[0]);
                }

                if (name.EndsWith(".Icon"))
                {
                    if (filebytes != null)
                    {
                        return new System.Drawing.Icon(filebytes) { FileName = fileName };
                    }
                    else
                    {
                        var stream = typeof(GtkSystem.Resources.ResourceManager).Assembly.GetManifestResourceStream(AssemblyResources.ToSystemUri("Panel.ico"));

                        return new System.Drawing.Icon(stream);
                    }
                }
                else
                {
                    if (filebytes != null)
                    {
                        return new System.Drawing.Bitmap(filebytes) { FileName = fileName };
                    }
                    else
                    {
                        var stream = typeof(GtkSystem.Resources.ResourceManager).Assembly.GetManifestResourceStream(AssemblyResources.ToSystemUri("image-missing16.png"));

                        return new System.Drawing.Bitmap(stream);
                    }
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
    public override string? GetString(string name)
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
    public override string? GetString(string name, CultureInfo? culture)
    {
        return ReadResourceText(name);
    }

    private string? DetectByFileHeader(Stream stream)
    {
        try
        {
            using (var reader = new BinaryReader(stream))
            {
                // Read the first 16 bytes
                var header = reader.ReadBytes(16);
                if (header.Length < 2)
                    return " the file is too small";

                // JPEG: FF D8
                if (header[0] == 0xFF && header[1] == 0xD8)
                    return "JPEG";

                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (header[0] == 0x89 && header[1] == 0x50 &&
                    header[2] == 0x4E && header[3] == 0x47 &&
                    header[4] == 0x0D && header[5] == 0x0A &&
                    header[6] == 0x1A && header[7] == 0x0A)
                    return "PNG";

                // GIF: GIF87a or GIF89a
                if (header[0] == 0x47 && header[1] == 0x49 &&
                    header[2] == 0x46 && header[3] == 0x38 &&
                    (header[4] == 0x37 || header[4] == 0x39) &&
                    header[5] == 0x61)
                    return "GIF";

                // BMP: BM
                if (header[0] == 0x42 && header[1] == 0x4D)
                    return "BMP";

                // TIFF: II or MM
                if ((header[0] == 0x49 && header[1] == 0x49) ||
                    (header[0] == 0x4D && header[1] == 0x4D))
                    return "TIFF";

                // WebP: RIFF + WEBP
                if (header[0] == 0x52 && header[1] == 0x49 &&
                    header[2] == 0x46 && header[3] == 0x46 &&
                    header[8] == 0x57 && header[9] == 0x45 &&
                    header[10] == 0x42 && header[11] == 0x50)
                    return "WebP";

                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private object? ReadResourceData(string name)
    {
        if (_assembly == null)
            throw new FileNotFoundException();
        else
        {
            try
            {
                var stream = _assembly.GetManifestResourceStream(_baseName + ResFileExtension);
                var reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
                var dict = reader.GetEnumerator();
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

    private byte[]? ReadResourceFile(string name)
    {
        byte[]? result = null;
        try
        {
            var filepath = $"./{ReadResourceData("this_FileName")}"; //linux the path must use/
            if (!System.IO.File.Exists(filepath))
                filepath = $"{Path.GetExtension(_baseName).TrimStart('.')}.resx";

            if (System.IO.File.Exists(filepath))
            {
                try
                {
                    var doc = new XmlDocument();
                    var xmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };
                    doc.Load(filepath);
                    var docElem = doc.DocumentElement;
                    var nodes = docElem.SelectNodes("data");

                    foreach (XmlNode xn in nodes)
                    {
                        if (xn.Attributes["name"].Value == name)
                        {
                            var data = xn.SelectSingleNode("value").InnerText;
                            result = System.Convert.FromBase64String(data);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug($"Unexpected exception in ReadResourceFile", ex);
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in ReadResourceFile", ex);
        }

        return result;
    }

    private string? ReadResourceText(string name)
    {
        if (_assembly == null)
            throw new FileNotFoundException();
        else
        {
            var stream = _assembly.GetManifestResourceStream(_baseName + ResFileExtension);
            var reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
            var dict = reader.GetEnumerator();
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
        public Assembly? Assembly
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
        public string? BaseName
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
        public byte[]? ImageBytes
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
        public string? ResourceName
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
        public Type? SourceType
        {
            get;
            set;
        }
    }
}