using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing;

/// <summary>
/// GtkImageConverter is a class that provides a type converter to convert Gtk images to and from various other representations,
/// such as byte arrays and strings. It inherits from
/// <see cref="T:System.ComponentModel.TypeConverter" /> and overrides methods
/// to specify the types it can convert from and to, as well as the actual conversion logic.
/// </summary>
/// <seealso cref="T:System.ComponentModel.TypeConverter" />
public class GtkImageConverter : TypeConverter
{
    /// <summary>
    /// Returns whether this converter can convert an object of the given source type to the converter's native type.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return true;
    }

    /// <summary>
    /// Returns whether this converter can convert the converter's native type to the specified destination type.
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
    {
        if (!(destinationType == typeof(byte[])))
        {
            return destinationType == typeof(string);
        }

        return true;
    }

    /// <summary>
    /// Attempts to convert the specified value object to the converter's native type.
    /// For now this forwards to the base implementation.
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {

        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Attempts to convert the specified value object to the requested destination type.
    /// For now this returns the incoming value unchanged.
    /// </summary>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        return value;
    }

    /// <summary>
    /// Returns a collection of properties for the type of object specified by the value parameter.
    /// This implementation returns the properties of
    /// <see cref="T:System.Drawing.Image" />.
    /// </summary>
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
        return TypeDescriptor.GetProperties(typeof(Image), attributes);
    }

    /// <summary>
    /// Returns whether this object supports properties. Always returns true.
    /// </summary>
    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
        return true;
    }

}

/// <summary>
/// An abstract base class that provides functionality for the
/// <see cref="T:System.Drawing.Bitmap" /> and
/// <see cref="T:System.Drawing.Imaging.Metafile" /> descended classes.
/// </summary>
/// <filterpriority>1
/// </filterpriority>
[Serializable]
public abstract class Image : Gtk.Widget, IDisposable, ICloneable, ISerializable//,MarshalByRefObject
{

    internal IntPtr nativeImage;

    private string _fileName;

    private int _height;

    private Gdk.Pixbuf _Pixbuf;

    private byte[] _PixbufData;

    private readonly byte[] _rawData;

    private ImageFormat _rawFormat;

    private object _userData;

    private int _width;

    /// <summary>
    /// Initializes a new instance of
    /// <see cref="T:System.Drawing.Image" /> from a pixbuf byte buffer.
    /// </summary>
    /// <param name="pixbuf">The raw pixbuf bytes.
    /// </param>
    internal Image(byte[] pixbuf)
    {
        PixbufData = pixbuf;
    }

    /// <summary>
    /// Initializes a new instance of
    /// <see cref="T:System.Drawing.Image" /> that wraps an existing native image handle.
    /// </summary>
    /// <param name="nativeImage">Native image handle.
    /// </param>
    internal Image(IntPtr nativeImage)
    {
        SetNativeImage(nativeImage);
    }

    /// <summary>
    /// Protected parameterless ctor for derived classes.
    /// </summary>
    protected Image()
    {
    }

    /// <summary>
    /// Deserialization constructor. Restores image data from serialized info.
    /// </summary>
    protected Image(SerializationInfo info, StreamingContext context)
    {
        byte[] buffer = (byte[])info.GetValue("Data", typeof(byte[]));
        try
        {
            SetNativeImage(InitializeFromStream(new MemoryStream(buffer)));
        }
        catch (ExternalException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (OutOfMemoryException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (FileNotFoundException)
        {
        }
    }

    /// <summary>
    /// Gets or sets the file name associated with this image. Setting the filename loads the image.
    /// </summary>
    public string FileName { get => _fileName; set { _fileName = value; Pixbuf = new Gdk.Pixbuf(value); } }

    /// <summary>
    /// Gets image-related flags. Implementation-specific.
    /// </summary>
    [Browsable(false)]
    public int Flags
    {
        get
        {
            int flags = 10;

            return flags;
        }
    }

    /// <summary>
    /// Gets the list of frame dimension GUIDs for this image.
    /// </summary>
    [Browsable(false)]
    public Guid[] FrameDimensionsList
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the image height in pixels.
    /// </summary>
    public int Height
    {
        get => Pixbuf == null ? _height : Pixbuf.Height;
        internal set { _height = value; }
    }

    /// <summary>
    /// Gets the horizontal resolution in pixels per inch.
    /// </summary>
    public float HorizontalResolution
    {
        get
        {
            float horzRes = 10;

            return horzRes;
        }
    }

    /// <summary>
    /// Gets the palette for indexed images, if applicable.
    /// </summary>
    [Browsable(false)]
    public ColorPalette Palette
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the physical dimensions of this image in the units of
    /// <see cref="T:System.Drawing.SizeF" />.
    /// </summary>
    public SizeF PhysicalDimension
    {
        get
        {
            float width=10;
            float height = 10;

            return new SizeF(width, height);
        }
    }

    /// <summary>
    /// Gets or sets the underlying Gdk
    /// <see cref="Gdk.Pixbuf" /> representation.
    /// Setting the property updates the internal pixbuf byte buffer.
    /// </summary>
    public Gdk.Pixbuf Pixbuf
    {
        get
        {
            if (_Pixbuf == null && _PixbufData != null) { _Pixbuf = new Gdk.Pixbuf(_PixbufData); }

            return _Pixbuf;
        }
        set { _Pixbuf = value; _PixbufData = value.SaveToBuffer("bmp"); }
    }

    /// <summary>
    /// Gets or sets the raw pixbuf byte buffer for this image.
    /// </summary>
    //“jpeg”, “tiff”, “png”, “ico” or “bmp”.
    public byte[] PixbufData
    {
        get
        {
            if (_PixbufData == null && _Pixbuf != null) { _PixbufData = _Pixbuf.SaveToBuffer("bmp"); }

            return _PixbufData;
        }
        set { _PixbufData = value; _Pixbuf = new Gdk.Pixbuf(value); }
    }

    /// <summary>
    /// Gets the pixel format for the image.
    /// </summary>
    public PixelFormat PixelFormat
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the list of property ids embedded in the image.
    /// </summary>
    [Browsable(false)]
    public int[] PropertyIdList
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the array of property items embedded in the image (EXIF / metadata).
    /// </summary>
    [Browsable(false)]
    public PropertyItem[] PropertyItems
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets the raw image format (for example Png, Jpeg).
    /// </summary>
    public ImageFormat RawFormat
    {
        get => _rawFormat ?? ImageFormat.Bmp;
        internal set => _rawFormat = value;
    }

    /// <summary>
    /// Gets the logical size of the image.
    /// </summary>
    public Size Size => new Size(Width, Height);

    /// <summary>
    /// Arbitrary user data slot associated with the image.
    /// </summary>
    [Localizable(false)]
    [DefaultValue(null)]
    public object Tag
    {
        get
        {
            return _userData;
        }
        set
        {
            _userData = value;
        }
    }

    /// <summary>
    /// Gets the vertical resolution in pixels per inch.
    /// </summary>
    public float VerticalResolution
    {
        get
        {
            float vertRes = 10;

            return vertRes;
        }
    }

    /// <summary>
    /// Gets the image width in pixels.
    /// </summary>
    public int Width
    {
        get => Pixbuf == null ? _width : Pixbuf.Width;
        internal set { _width = value; }
    }

    /// <summary>
    /// Creates a shallow clone of this image. Returned object is a
    /// <see cref="T:System.Drawing.Bitmap" /> instance.
    /// </summary>
    public object Clone()
    {
        return new Bitmap((byte[])this.PixbufData.Clone()) { Width = this.Width, Height = this.Height };
    }

    /// <summary>
    /// Disposes this image and releases resources.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Loads an image from the specified file.
    /// </summary>
    /// <param name="filename">Path to the image file.
    /// </param>
    public static Image FromFile(string filename)
    {
        return FromFile(filename, useEmbeddedColorManagement: false);
    }

    /// <summary>
    /// Loads an image from the specified file.
    /// </summary>
    /// <param name="filename">Path to the image file.
    /// </param>
    /// <param name="useEmbeddedColorManagement">Whether to use embedded color management (not used).
    /// </param>
    public static Image FromFile(string filename, bool useEmbeddedColorManagement)
    {
        if (!System.IO.File.Exists(filename))
        {
            filename = System.IO.Path.GetFullPath(filename);
            throw new FileNotFoundException(filename);
        }
        filename = System.IO.Path.GetFullPath(filename);
        string extension = IO.Path.GetExtension(filename)?.ToLower();
        byte[] filebytes =File.ReadAllBytes(filename);
        Bitmap bitmap = new Bitmap(filebytes);
        bitmap.GetImageFormat(extension);

        return bitmap;
    }

    /// <summary>
    /// Creates a Bitmap from a GDI HBITMAP handle.
    /// </summary>
    public static Bitmap FromHbitmap(IntPtr hbitmap)
    {
        return FromHbitmap(hbitmap, IntPtr.Zero);
    }

    /// <summary>
    /// Creates a Bitmap from a GDI HBITMAP handle and optional palette handle.
    /// </summary>
    public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
    {

        return new Bitmap(hbitmap);
    }

    /// <summary>
    /// Creates an image from a
    /// <see cref="T:System.IO.Stream" />.
    /// </summary>
    public static Image FromStream(Stream stream)
    {
        return FromStream(stream, useEmbeddedColorManagement: false);
    }

    /// <summary>
    /// Creates an image from a
    /// <see cref="T:System.IO.Stream" />.
    /// </summary>
    public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
    {
        return FromStream(stream, useEmbeddedColorManagement, validateImageData: true);
    }

    /// <summary>
    /// Creates an image from a
    /// <see cref="T:System.IO.Stream" />.
    /// </summary>
    public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
    {
        if (stream != null)
        {
            return new Bitmap(stream);
        }

        return null;
    }

    /// <summary>
    /// Returns the bounds of the image. The pageUnit is an input/output parameter.
    /// </summary>
    public RectangleF GetBounds(ref GraphicsUnit pageUnit)
    {
        return new RectangleF();
    }

    /// <summary>
    /// Returns encoder parameters supported by the encoder identified by the given GUID.
    /// </summary>
    public EncoderParameters GetEncoderParameterList(Guid encoder)
    {

        return null;

    }

    /// <summary>
    /// Returns the number of frames for the specified frame dimension.
    /// </summary>
    public int GetFrameCount(FrameDimension dimension)
    {

        return 1;
    }

    /// <summary>
    /// Returns the pixel format size in bits for the supplied pixel format.
    /// </summary>
    public static int GetPixelFormatSize(PixelFormat pixfmt)
    {
        return ((int)pixfmt >> 8) & 0xFF;
    }

    /// <summary>
    /// Retrieves a property item by id, or null if not present.
    /// </summary>
    public PropertyItem GetPropertyItem(int propid)
    {

        return null;

    }

    /// <summary>
    /// Returns a thumbnail image for this image. Not implemented.
    /// </summary>
    public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
    {
        return null;
    }

    /// <summary>
    /// Returns whether the supplied pixel format contains an alpha channel.
    /// </summary>
    public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
    {
        return (pixfmt & PixelFormat.Alpha) != 0;
    }

    /// <summary>
    /// Returns whether the supplied pixel format is canonical.
    /// </summary>
    public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
    {
        return (pixfmt & PixelFormat.Canonical) != 0;
    }

    /// <summary>
    /// Returns whether the supplied pixel format is extended.
    /// </summary>
    public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
    {
        return (pixfmt & PixelFormat.Extended) != 0;
    }

    /// <summary>
    /// Removes a property item by id.
    /// </summary>
    public void RemovePropertyItem(int propid)
    {

    }

    /// <summary>
    /// Rotates and/or flips the image.
    /// </summary>
    public void RotateFlip(RotateFlipType rotateFlipType)
    {

    }

    /// <summary>
    /// Saves the image to the specified filename. The image format is inferred from the extension.
    /// </summary>
    public void Save(string filename)
    {
        string extension = IO.Path.GetExtension(filename)?.ToLower();
        GetImageFormat(extension);
        Save(filename, RawFormat);
    }

    /// <summary>
    /// Saves the image to the specified filename using the given image format.
    /// </summary>
    public void Save(string filename, ImageFormat format)
    {
        Save(filename, format.FindEncoder(), null);
    }

    /// <summary>
    /// Saves the image to the specified filename using the provided encoder and parameters.
    /// </summary>
    public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
    {
        Pixbuf?.Save(filename, encoder.MimeType.Trim('.').ToLower());
    }

    /// <summary>
    /// Saves the image to the specified filename. The image format is inferred from the extension.
    /// </summary>
    public void Save(Stream stream, ImageFormat format)
    {
        Save(stream, null, null);
    }

    /// <summary>
    /// Saves the image to the specified filename. The image format is inferred from the extension.
    /// </summary>
    public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
    {
        if (PixbufData != null)
        {
            foreach (byte b in PixbufData)
                stream.WriteByte(b);
        }
    }

    /// <summary>
    /// Appends encoded data to a multi-frame image. Not implemented.
    /// </summary>
    public void SaveAdd(EncoderParameters encoderParams)
    {

    }

    /// <summary>
    /// Appends encoded image to a multi-frame image. Not implemented.
    /// </summary>
    public void SaveAdd(Image image, EncoderParameters encoderParams)
    {

    }

    /// <summary>
    /// Selects the active frame for animated images.
    /// </summary>
    public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
    {

        return 0;
    }

    /// <summary>
    /// Sets a property item on the image.
    /// </summary>
    public void SetPropertyItem(PropertyItem propitem)
    {

    }

    /// <summary>
    /// Factory for constructing an
    /// <see cref="T:System.Drawing.Image" /> wrapper from a native handle.
    /// </summary>
    internal static Image CreateImageObject(IntPtr nativeImage)
    {
        return new Bitmap(nativeImage);
    }

    /// <summary>
    /// Ensures image data is saved to disk. Platform-specific helper (no-op).
    /// </summary>
    internal static void EnsureSave(Image image, string filename, Stream dataStream)
    {

    }

    /// <summary>
    /// Sets the native image handle and updates internal pixbuf if the handle maps to a Gdk.Pixbuf.
    /// </summary>
    /// <param name="handle">Native image handle.
    /// </param>
    internal void SetNativeImage(IntPtr handle)
    {
        if (GLib.Object.TryGetObject(handle) is Gdk.Pixbuf pixbuf)
            _Pixbuf = pixbuf;
    }

    /// <summary>
    /// Validates a native image handle. No-op placeholder.
    /// </summary>
    internal static void ValidateImage(IntPtr image)
    {

    }

    /// <summary>
    /// Serializes this image into
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo" />.
    /// </summary>
    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        using MemoryStream memoryStream = new MemoryStream();
        Save(memoryStream);
        si.AddValue("Data", memoryStream.ToArray(), typeof(byte[]));
    }

    /// <summary>
    /// Releases resources used by this image.
    /// </summary>
    /// <param name="disposing">True when called from Dispose; false from finalizer.
    /// </param>
    protected new virtual void Dispose(bool disposing)
    {
        _Pixbuf?.Dispose();
        if (_PixbufData != null)
            _PixbufData = null;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Determines image format from file extension and returns the resolved ImageFormat.
    /// </summary>
    /// <param name="extension">File extension (including the dot), e.g. ".png".
    /// </param>
    private ImageFormat GetImageFormat(string extension)
    {
        if (extension == ".memorybmp")
        {
            RawFormat = ImageFormat.MemoryBmp;
        }
        else if (extension == ".bmp")
        {
            RawFormat = ImageFormat.Bmp;
        }
        else if (extension == ".emf")
        {
            RawFormat = ImageFormat.Emf;
        }
        else if (extension == ".wmf")
        {
            RawFormat = ImageFormat.Wmf;
        }
        else if (extension == ".gif")
        {
            RawFormat = ImageFormat.Gif;
        }
        else if (extension == ".jpeg")
        {
            RawFormat = ImageFormat.Jpeg;
        }
        else if (extension == ".png")
        {
            RawFormat = ImageFormat.Png;
        }
        else if (extension == ".tiff")
        {
            RawFormat = ImageFormat.Tiff;
        }
        else if (extension == ".exif")
        {
            RawFormat = ImageFormat.Exif;
        }
        else if (extension == ".icon")
        {
            RawFormat = ImageFormat.Icon;
        }
        else if (extension == ".heif")
        {
            RawFormat = ImageFormat.Heif;
        }
        else if (extension == ".webp")
        {
            RawFormat = ImageFormat.Webp;
        }

        return RawFormat;
    }

    /// <summary>
    /// Initializes the internal native image from a stream by creating a
    /// <see cref="Gdk.Pixbuf" />.
    /// </summary>
    /// <param name="stream">Stream containing image data.
    /// </param>
    /// <returns>Native handle for the created pixbuf.
    /// </returns>
    private IntPtr InitializeFromStream(Stream stream)
    {
        Pixbuf = new Gdk.Pixbuf(stream);

        return Pixbuf.Handle;
    }

    /// <summary>
    /// Saves the image to the specified filename. The image format is inferred from the extension.
    /// </summary>
    private void Save(MemoryStream stream)
    {
        Save(stream, null, null);
    }

    /// <summary>
    /// Throws if directory for the specified filename doesn't exist. Placeholder.
    /// </summary>
    private static void ThrowIfDirectoryDoesntExist(string filename)
    {

    }

    /// <summary>
    /// Delegate used as callback to abort thumbnail generation.
    /// </summary>
    public delegate bool GetThumbnailImageAbort();

}