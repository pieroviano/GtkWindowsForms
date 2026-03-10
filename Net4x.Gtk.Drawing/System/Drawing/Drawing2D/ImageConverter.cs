using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// <see cref="T:System.Drawing.ImageConverter" />
    /// is a class that can be used to convert
    /// <see cref="T:System.Drawing.Image" />
    /// objects from one data type to another. Access this class through the
    /// <see cref="T:System.ComponentModel.TypeDescriptor" /> object.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ImageConverter : TypeConverter
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
            byte[] array = value as byte[];
            if (array != null)
            {
                Stream stream = GetBitmapStream(array) ?? new MemoryStream(array);
                return Image.FromStream(stream);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return "none";
                }
                if (value is Image)
                {
                    return value!.ToString();
                }
            }
            else if (destinationType == typeof(byte[]))
            {
                if (value == null)
                {
                    return Array.Empty<byte>();
                }
                Image image = value as Image;
                if (image != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        ImageFormat imageFormat = image.RawFormat;
                        if (imageFormat == ImageFormat.Jpeg)
                        {
                            imageFormat = ImageFormat.Png;
                        }
                        ImageCodecInfo encoder = FindEncoder(imageFormat) ?? FindEncoder(ImageFormat.Png);
                        image.Save(memoryStream, encoder, null);
                        return memoryStream.ToArray();
                    }
                }
            }
            throw GetConvertFromException(value);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Image), attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private static ImageCodecInfo FindEncoder(ImageFormat imageformat)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo[] array = imageEncoders;
            foreach (ImageCodecInfo imageCodecInfo in array)
            {
                if (imageCodecInfo.FormatID.Equals(imageformat.Guid))
                {
                    return imageCodecInfo;
                }
            }

            return null;
        }

        private static Stream GetBitmapStream(ReadOnlySpan<byte> rawData)
        {
            try
            {
                short num = BinaryPrimitives.ReadInt16LittleEndian(rawData);
                if (num != 7189)
                {
                    return null;
                }
                short num2 = BinaryPrimitives.ReadInt16LittleEndian(rawData.Slice(2, 2));
                if (rawData.Length <= num2 + 18 || !rawData.Slice(num2 + 12, 6).SequenceEqual(new byte[6] { 80, 66, 114, 117, 115, 104 }))
                {
                    return null;
                }
                return new MemoryStream(rawData.Slice(78).ToArray());
            }
            catch (OutOfMemoryException)
            {
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            return null;
        }

    }
}
