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

namespace GtkSystem.Resources
{
    /// <summary>
    /// ResourceManager is a class that provides convenient access to culture-specific resources at runtime. It enables you to retrieve resources based on the current culture or a specified culture. This class is typically used in applications that support multiple languages and cultures, allowing you to manage and access localized resources such as strings, images, and other data.
    /// </summary>
    /// <seealso cref="T:System.Resources.ResourceManager" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ResourceManager : System.Resources.ResourceManager
    {

        public ResourceInfo GetResourceInfo = new ResourceInfo();

        internal const string ResFileExtension = ".resources";

        private Assembly _assembly;

        private string _baseName;

        private System.Type _resourceSource;

        public ResourceManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet) : this(null, null, usingResourceSet)
        {

        }

        public ResourceManager(string baseName, Assembly assembly) : this(baseName, assembly, null)
        {

        }

        public ResourceManager(string baseName, Assembly assembly, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet) : base(baseName, assembly, usingResourceSet)
        {
            this._baseName = baseName;
            this._assembly = assembly;
            this._resourceSource = usingResourceSet;
            GetResourceInfo.Assembly = assembly;
            GetResourceInfo.BaseName = baseName;
            GetResourceInfo.SourceType = usingResourceSet;
        }

        protected ResourceManager()
        {

        }

        public override object GetObject(string name, CultureInfo culture)
        {
            object result = GetObject(name);
            var stack = new StackTrace(true);
            System.Reflection.MethodInfo method = (System.Reflection.MethodInfo)stack.GetFrame(1).GetMethod();
            if (method.ReturnType.Name == "Object" || method.ReturnType.Name.Equals(result.GetType().Name))
            {
                return result;
            }
            else
            {
                return null;
            }

        }

        public override object GetObject(string name)
        {
            GetResourceInfo.ResourceName = name;
            object obj = ReadResourceData(name);
            if (obj == null)
            {
                if (name.EndsWith(".ImageStream"))
                {
                    return new ImageListStreamer(new ImageList()) { ResourceInfo = GetResourceInfo };
                }
                else
                {
                    string fileName = name;
                    byte[] filebytes = ReadResourceFile(name);
                    if (filebytes == null)
                    {
                        string _formName = Path.GetExtension(this.BaseName).TrimStart('.');
                        string[] files = Directory.GetFiles($"./Resources/{_formName}", $"{fileName}.*", SearchOption.AllDirectories);
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

        public override string GetString(string name)
        {
            return GetString(name, null);
        }

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
                    Stream stream = _assembly.GetManifestResourceStream(_baseName + ResFileExtension);
                    GtkSystem.Resources.Extensions.DeserializingResourceReader reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
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
                string filepath = $"./{ReadResourceData("this_FileName")}"; //linux the path must use/
                if (System.IO.File.Exists(filepath))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };
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
                    catch { }
                }
            }
            catch (System.Exception ex)
            {

            }

            return result;
        }

        private string ReadResourceText(string name)
        {
            if (_assembly == null)
                throw new FileNotFoundException();
            else
            {
                Stream stream = _assembly.GetManifestResourceStream(_baseName + ".resources");
                GtkSystem.Resources.Extensions.DeserializingResourceReader reader = new GtkSystem.Resources.Extensions.DeserializingResourceReader(stream);
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

        public class ResourceInfo
        {

            public Assembly Assembly { get; set; }

            public string BaseName { get; set; }

            public byte[] ImageBytes { get; set; }

            public string ResourceName { get; set; }

            public System.Type SourceType { get; set; }

        }
    }
}
