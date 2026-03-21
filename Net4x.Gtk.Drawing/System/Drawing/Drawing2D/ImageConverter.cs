using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace System.Drawing;

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
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="sourceType">
    /// A
    /// <see cref="T:System.Type">
    /// </see> that represents the type you want to convert from.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return true;
    }

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="destinationType">
    /// A
    /// <see cref="T:System.Type">
    /// </see> that represents the type you want to convert to.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
    {
        if (!(destinationType == typeof(byte[])))
        {
            return destinationType == typeof(string);
        }

        return true;
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// The
    /// <see cref="T:System.Globalization.CultureInfo">
    /// </see> to use as the current culture.
    /// </param>
    /// <param name="value">
    /// The
    /// <see cref="T:System.Object">
    /// </see> to convert.
    /// </param>
    /// <returns>
    /// An
    /// <see cref="T:System.Object">
    /// </see> that represents the converted value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is byte[] array)
        {
            var stream = GetBitmapStream(array) ?? new MemoryStream(array);
            return Image.FromStream(stream);
        }

        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// A
    /// <see cref="T:System.Globalization.CultureInfo">
    /// </see>. If null is passed, the current culture is assumed.
    /// </param>
    /// <param name="value">
    /// The
    /// <see cref="T:System.Object">
    /// </see> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The
    /// <see cref="T:System.Type">
    /// </see> to convert the value parameter to.
    /// </param>
    /// <returns>
    /// An
    /// <see cref="T:System.Object">
    /// </see> that represents the converted value.
    /// </returns>
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
            if (value is Image image)
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

    /// <summary>
    /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="value">
    /// An
    /// <see cref="T:System.Object">
    /// </see> that specifies the type of array for which to get properties.
    /// </param>
    /// <param name="attributes">
    /// An array of type
    /// <see cref="T:System.Attribute">
    /// </see> that is used as a filter.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.ComponentModel.PropertyDescriptorCollection">
    /// </see> with the properties that are exposed for this data type, or null if there are no properties.
    /// </returns>
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
        return TypeDescriptor.GetProperties(typeof(Image), attributes);
    }

    /// <summary>
    /// Returns whether this object supports properties, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <returns>
    /// true if
    /// <see cref="T:System.ComponentModel.TypeConverter.GetProperties(System.Object)">
    /// </see> should be called to find the properties of this object; otherwise, false.
    /// </returns>
    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
        return true;
    }

    private static ImageCodecInfo FindEncoder(ImageFormat imageformat)
    {
        ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
        var array = imageEncoders;
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
            var num = BinaryPrimitives.ReadInt16LittleEndian(rawData);
            if (num != 7189)
            {
                return null;
            }
            var num2 = BinaryPrimitives.ReadInt16LittleEndian(rawData.Slice(2, 2));
            if (rawData.Length <= num2 + 18 || !rawData.Slice(num2 + 12, 6).SequenceEqual(new byte[6] { 80, 66, 114, 117, 115, 104 }))
            {
                return null;
            }
            return new MemoryStream(rawData.Slice(78).ToArray());
        }
        catch (OutOfMemoryException outOfMemoryException)
        {
            System.Diagnostics.Trace.Write($"Unexpected exception in GetBitmapStream: {outOfMemoryException}");
        }
        catch (ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            System.Diagnostics.Trace.Write($"Unexpected exception in GetBitmapStream: {argumentOutOfRangeException}");
        }

        return null;
    }
}