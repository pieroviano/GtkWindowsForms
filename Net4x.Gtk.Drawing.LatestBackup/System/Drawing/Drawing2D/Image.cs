using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{

    public class GtkImageConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
        {
            if (!(destinationType == typeof(byte[])))
            {
                return destinationType == typeof(string);
            }

            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Image), attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    /// <summary>
    /// An abstract base class that provides functionality for the
    /// <see cref="System.Drawing.Bitmap" /> and
    /// <see cref="System.Drawing.Imaging.Metafile" /> descended classes.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [Serializable]
    public abstract class Image : Gtk.Widget, IDisposable, ICloneable, ISerializable//,MarshalByRefObject
    {

        internal IntPtr nativeImage;

        private string _fileName;

        private int _height;

        private Gdk.Pixbuf _Pixbuf;

        private byte[] _PixbufData;

        private byte[] _rawData;

        private ImageFormat _rawFormat;

        private object _userData;

        private int _width;
        internal Image(byte[] pixbuf)
        {
            PixbufData = pixbuf;
        }

        internal Image(IntPtr nativeImage)
        {
            SetNativeImage(nativeImage);
        }

        protected Image()
        {
        }

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
        public string FileName { get => _fileName; set { _fileName = value; Pixbuf = new Gdk.Pixbuf(value); } }

        [Browsable(false)]
        public int Flags
        {
            get
            {
                int flags = 10;

                return flags;
            }
        }

        [Browsable(false)]
        public Guid[] FrameDimensionsList
        {
            get;
            internal set;
        }

        public int Height
        {
            get => Pixbuf == null ? _height : Pixbuf.Height;
            internal set { _height = value; }
        }

        public float HorizontalResolution
        {
            get
            {
                float horzRes = 10;

                return horzRes;
            }
        }

        [Browsable(false)]
        public ColorPalette Palette
        {
            get;
            internal set;
        }

        public SizeF PhysicalDimension
        {
            get
            {
                float width = 10;
                float height = 10;

                return new SizeF(width, height);
            }
        }

        public Gdk.Pixbuf Pixbuf
        {
            get
            {
                if (_Pixbuf == null && _PixbufData != null) { _Pixbuf = new Gdk.Pixbuf(_PixbufData); }

                return _Pixbuf;
            }
            set { _Pixbuf = value; _PixbufData = value.SaveToBuffer("bmp"); }
        }

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

        public PixelFormat PixelFormat
        {
            get;
            internal set;
        }

        [Browsable(false)]
        public int[] PropertyIdList
        {
            get;
            internal set;
        }

        [Browsable(false)]
        public PropertyItem[] PropertyItems
        {
            get;
            internal set;
        }

        public ImageFormat RawFormat
        {
            get => _rawFormat ?? ImageFormat.Bmp;
            internal set => _rawFormat = value;
        }

        public Size Size => new Size(Width, Height);

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

        public float VerticalResolution
        {
            get
            {
                float vertRes = 10;

                return vertRes;
            }
        }

        public int Width
        {
            get => Pixbuf == null ? _width : Pixbuf.Width;
            internal set { _width = value; }
        }

        public object Clone()
        {
            return new Bitmap((byte[])this.PixbufData.Clone()) { Width = this.Width, Height = this.Height };
        }

        public new void Dispose()
        {
            Dispose(true);
        }

        public static Image FromFile(string filename)
        {
            return FromFile(filename, useEmbeddedColorManagement: false);
        }

        public static Image FromFile(string filename, bool useEmbeddedColorManagement)
        {
            if (!System.IO.File.Exists(filename))
            {
                filename = System.IO.Path.GetFullPath(filename);
                throw new FileNotFoundException(filename);
            }
            filename = System.IO.Path.GetFullPath(filename);
            string extension = IO.Path.GetExtension(filename)?.ToLower();
            byte[] filebytes = File.ReadAllBytes(filename);
            Bitmap bitmap = new Bitmap(filebytes);
            bitmap.GetImageFormat(extension);

            return bitmap;
        }

        public static Bitmap FromHbitmap(IntPtr hbitmap)
        {
            return FromHbitmap(hbitmap, IntPtr.Zero);
        }

        public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
        {

            return new Bitmap(hbitmap);
        }

        public static Image FromStream(Stream stream)
        {
            return FromStream(stream, useEmbeddedColorManagement: false);
        }

        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
        {
            return FromStream(stream, useEmbeddedColorManagement, validateImageData: true);
        }

        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            if (stream != null)
            {
                return new Bitmap(stream);
            }

            return null;
        }

        public RectangleF GetBounds(ref GraphicsUnit pageUnit)
        {
            return new RectangleF();
        }

        public EncoderParameters GetEncoderParameterList(Guid encoder)
        {

            return null;

        }

        public int GetFrameCount(FrameDimension dimension)
        {

            return 1;
        }

        public static int GetPixelFormatSize(PixelFormat pixfmt)
        {
            return ((int)pixfmt >> 8) & 0xFF;
        }

        public PropertyItem GetPropertyItem(int propid)
        {

            return null;

        }

        public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
        {
            return null;
        }

        public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
        {
            return (pixfmt & PixelFormat.Alpha) != 0;
        }

        public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
        {
            return (pixfmt & PixelFormat.Canonical) != 0;
        }

        public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
        {
            return (pixfmt & PixelFormat.Extended) != 0;
        }

        public void RemovePropertyItem(int propid)
        {

        }

        public void RotateFlip(RotateFlipType rotateFlipType)
        {

        }

        public void Save(string filename)
        {
            string extension = IO.Path.GetExtension(filename)?.ToLower();
            GetImageFormat(extension);
            Save(filename, RawFormat);
        }

        public void Save(string filename, ImageFormat format)
        {
            Save(filename, format.FindEncoder(), null);
        }

        public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            if (Pixbuf != null)
                Pixbuf.Save(filename, encoder.MimeType.Trim('.').ToLower());
        }

        public void Save(Stream stream, ImageFormat format)
        {
            Save(stream, null, null);
        }

        public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            if (PixbufData != null)
            {
                foreach (byte b in PixbufData)
                    stream.WriteByte(b);
            }
        }

        public void SaveAdd(EncoderParameters encoderParams)
        {

        }

        public void SaveAdd(Image image, EncoderParameters encoderParams)
        {

        }

        public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
        {

            return 0;
        }

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
            using MemoryStream memoryStream = new MemoryStream();
            Save(memoryStream);
            si.AddValue("Data", memoryStream.ToArray(), typeof(byte[]));
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (_Pixbuf != null)
                _Pixbuf.Dispose();
            if (_PixbufData != null)
                _PixbufData = null;
            base.Dispose(disposing);
        }
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

        public delegate bool GetThumbnailImageAbort();
    }
}
