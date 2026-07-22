#nullable enable

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Gdk;
using Gtk;

namespace System.Drawing;

/// <summary>
/// An abstract base class that provides functionality for the
/// <see cref="T:System.Drawing.Bitmap" /> and
/// <see cref="T:System.Drawing.Imaging.Metafile" /> descended classes.
/// </summary>
/// <filterpriority>1
/// </filterpriority>
[Serializable]
public abstract class Image : MarshalByRefObject, IDisposable, ICloneable, ISerializable
{

    internal IntPtr nativeImage;

    private string? _fileName;

    private int _height;

    private Pixbuf? _Pixbuf;

    private byte[]? _PixbufData;

    private readonly byte[]? _rawData;

    private ImageFormat? _rawFormat;

    private object? _userData;

    private int _width;

    static Image()
    {
        Application.Init();
    }

    internal Image(byte[]? pixbuf)
    {
        PixbufData = pixbuf;
    }

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
        var buffer = (byte[])info.GetValue("Data", typeof(byte[]));
        try
        {
            SetNativeImage(InitializeFromStream(new MemoryStream(buffer)));
        }
        catch (ExternalException externalException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", externalException);
        }
        catch (ArgumentException argumentException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", argumentException);
        }
        catch (OutOfMemoryException outOfMemoryException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", outOfMemoryException);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", invalidOperationException);
        }
        catch (NotImplementedException notImplementedException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", notImplementedException);
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Image", fileNotFoundException);
        }
    }

    /// <summary>
    /// Gets or sets the file name associated with this image. Setting the filename loads the image.
    /// </summary>
    public string? FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    Pixbuf = new Gdk.Pixbuf(value);
                }
                catch (GLib.GException ex)
                {
                    System.Diagnostics.GtkLogger.Debug($"Failed to load Pixbuf from file '{value}'", ex);
                    Pixbuf = null;
                }
            }
            else
            {
                Pixbuf = null;
            }
        }
    }

    /// <summary>
    /// Gets image-related flags. Implementation-specific.
    /// </summary>
    [Browsable(false)]
    public int Flags
    {
        get
        {
            var flags = 10;

            return flags;
        }
    }

    /// <summary>
    /// Gets the list of frame dimension GUIDs for this image.
    /// </summary>
    [Browsable(false)]
    public Guid[]? FrameDimensionsList
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
    public ColorPalette? Palette
    {
        get;

        internal set;
    }

    /// <summary>
    /// Gets the physical dimensions of this image in the units of
    /// <see cref="T:System.Drawing.SizeF"/>.
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
    public Pixbuf? Pixbuf
    {
        get
        {
            if (_Pixbuf == null && _PixbufData != null)
            {
                try
                {
                    _Pixbuf = new Gdk.Pixbuf(_PixbufData);
                }
                catch (GLib.GException ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Failed to create Pixbuf from PixbufData", ex);
                    // If pixbuf creation fails, clear the underlying data to avoid repeated failures.
                    _PixbufData = null;
                    _Pixbuf = null;
                }
            }

            return _Pixbuf;
        }
        set
        {
            _Pixbuf = value;
            if (value != null)
            {
                try
                {
                    Width = value.Width;
                    Height = value.Height;
                    var type = RawFormat.GetMimeType().Substring(1);
                    if ("png,jpeg,gif,icon,tiff,webp".Contains(type))
                    {
                        _PixbufData = value.SaveToBuffer(type);
                    }
                    else if (value.HasAlpha)
                    {
                        _PixbufData = value.SaveToBuffer("png");
                        RawFormat = ImageFormat.Png;
                    }
                    else
                    {
                        _PixbufData = value.SaveToBuffer("jpeg");
                        RawFormat = ImageFormat.Jpeg;
                    }
                }
                catch (GLib.GException ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Pixbuf.SaveToBuffer failed in Image.Pixbuf setter", ex);
                    // keep Pixbuf but clear PixbufData to avoid using invalid buffer
                    _PixbufData = null;
                }
            }
            else
            {
                _PixbufData = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the raw pixbuf byte buffer for this image.
    /// </summary>
    //“jpeg”, “tiff”, “png”, “ico” or “bmp”.
    public byte[]? PixbufData
    {
        get
        {
            if (_PixbufData == null && _Pixbuf != null)
            {
                try
                {
                    var type = RawFormat.GetMimeType().Substring(1);
                    if ("png,jpeg,gif,icon,tiff,webp".Contains(type) == false)
                        type = "png";
                    _PixbufData = _Pixbuf.SaveToBuffer(type);
                }
                catch (GLib.GException ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Pixbuf.SaveToBuffer failed in Image.PixbufData getter", ex);
                    _PixbufData = null;
                }
            }

            return _PixbufData;
        }
        set
        {
            _PixbufData = value;
            if (value != null)
            {
                try
                {
                    RawFormat = DetectByFileHeader(value);
                    Pixbuf = new Gdk.Pixbuf(value);
                }
                catch (GLib.GException ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Failed to create Pixbuf from PixbufData in setter", ex);
                    _Pixbuf = null;
                    _PixbufData = null;
                }
            }
            else
            {
                _Pixbuf = null;
            }
        }
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
    public int[]? PropertyIdList
    {
        get;

        internal set;
    }

    /// <summary>
    /// Gets the array of property items embedded in the image (EXIF / metadata).
    /// </summary>
    [Browsable(false)]
    public PropertyItem[]? PropertyItems
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
    public object? Tag
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
        return new Bitmap((byte[]?)this.PixbufData?.Clone()) { Width = this.Width, Height = this.Height };
    }

    /// <summary>
    /// Disposes this image and releases resources.
    /// </summary>
    public void Dispose()
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
            throw new FileNotFoundException(filename);
        }
        var extension = IO.Path.GetExtension(filename)?.ToLower();
        var bitmap = new Bitmap(filename);
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
    public static Image? FromStream(Stream stream)
    {
        return FromStream(stream, useEmbeddedColorManagement: false);
    }

    /// <summary>
    /// Creates an image from a
    /// <see cref="T:System.IO.Stream" />.
    /// </summary>
    public static Image? FromStream(Stream stream, bool useEmbeddedColorManagement)
    {
        return FromStream(stream, useEmbeddedColorManagement, validateImageData: true);
    }

    /// <summary>
    /// Creates an image from a
    /// <see cref="T:System.IO.Stream" />.
    /// </summary>
    public static Image? FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
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
    public EncoderParameters? GetEncoderParameterList(Guid encoder)
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
    public PropertyItem? GetPropertyItem(int propid)
    {

        return null;

    }

    /// <summary>
    /// Returns a thumbnail image for this image. Not implemented.
    /// </summary>
    public Image? GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
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
        var extension = IO.Path.GetExtension(filename)?.ToLower();
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
    public void Save(string filename, ImageCodecInfo encoder, EncoderParameters? encoderParams)
    {
        Pixbuf?.Save(filename, encoder.MimeType?.Trim('.').ToLower());
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
    public void Save(Stream stream, ImageCodecInfo? encoder, EncoderParameters? encoderParams)
    {
        if (PixbufData != null)
        {
            foreach (var b in PixbufData)
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

    internal static Image CreateImageObject(IntPtr nativeImage)
    {
        return new Bitmap(nativeImage);
    }

    internal static void EnsureSave(Image image, string filename, Stream dataStream)
    {

    }

    internal void SetNativeImage(IntPtr handle)
    {
        if (GLib.Object.TryGetObject(handle) is Gdk.Pixbuf pixbuf)
            _Pixbuf = pixbuf;
    }

    internal static void ValidateImage(IntPtr image)
    {

    }

    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        using var memoryStream = new MemoryStream();
        Save(memoryStream);
        si.AddValue("Data", memoryStream.ToArray(), typeof(byte[]));
    }

    /// <summary>
    /// Releases resources used by this image.
    /// </summary>
    /// <param name="disposing">True when called from Dispose; false from finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        _Pixbuf?.Dispose();
        if (_PixbufData != null)
            _PixbufData = null;
    }

    private System.Drawing.Imaging.ImageFormat DetectByFileHeader(Stream stream)
    {
        try
        {
            using (var reader = new BinaryReader(stream))
            {
                byte[] header = reader.ReadBytes(16);
                return DetectByFileHeader(header);
            }
        }
        catch
        {
            return System.Drawing.Imaging.ImageFormat.MemoryBmp;
        }
    }

    private System.Drawing.Imaging.ImageFormat DetectByFileHeader(byte[] header)
    {
        try
        {
            if (header.Length < 15)
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;

            // JPEG: FF D8
            if (header[0] == 0xFF && header[1] == 0xD8)
                return System.Drawing.Imaging.ImageFormat.Jpeg;

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (header[0] == 0x89 && header[1] == 0x50 &&
                header[2] == 0x4E && header[3] == 0x47 &&
                header[4] == 0x0D && header[5] == 0x0A &&
                header[6] == 0x1A && header[7] == 0x0A)
                return System.Drawing.Imaging.ImageFormat.Png;

            // GIF: GIF87a or GIF89a
            if (header[0] == 0x47 && header[1] == 0x49 &&
                header[2] == 0x46 && header[3] == 0x38 &&
                (header[4] == 0x37 || header[4] == 0x39) &&
                header[5] == 0x61)
                return System.Drawing.Imaging.ImageFormat.Gif;

            // TIFF: II or MM
            if ((header[0] == 0x49 && header[1] == 0x49) ||
                (header[0] == 0x4D && header[1] == 0x4D))
                return System.Drawing.Imaging.ImageFormat.Tiff;

            // WebP: RIFF + WEBP
            if (header[0] == 0x52 && header[1] == 0x49 &&
                header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 &&
                header[10] == 0x42 && header[11] == 0x50)
                return System.Drawing.Imaging.ImageFormat.Webp;

            // BMP: BM
            if (header[0] == 0x42 && header[1] == 0x4D)
                return System.Drawing.Imaging.ImageFormat.Bmp;

            return System.Drawing.Imaging.ImageFormat.MemoryBmp;
        }
        catch
        {
            return System.Drawing.Imaging.ImageFormat.MemoryBmp;
        }
    }

    private ImageFormat GetImageFormat(string? extension)
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

    private IntPtr InitializeFromStream(Stream stream)
    {
        Pixbuf = new Gdk.Pixbuf(stream);

        return Pixbuf.Handle;
    }

    private void Save(MemoryStream stream)
    {
        Save(stream, null, null);
    }

    private static void ThrowIfDirectoryDoesntExist(string filename)
    {

    }

    /// <summary>
    /// Delegate used as callback to abort thumbnail generation.
    /// </summary>
    public delegate bool GetThumbnailImageAbort();

}