using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

/// <summary>
/// <see cref="T:System.Drawing.ImageFormatConverter" />
/// is a class that can be used to convert
/// <see cref="T:System.Drawing.Imaging.ImageFormat" />
/// objects from one data type to another. Access this class through the
/// <see cref="T:System.ComponentModel.TypeDescriptor" /> object.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public class ImageFormatConverter : TypeConverter
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
        if (!(sourceType == typeof(string)))
        {
            return base.CanConvertFrom(context, sourceType);
        }

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
        if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
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
    /// <exception cref="T:System.FormatException">ImageFormat err
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string text)
        {
            return base.ConvertFrom(context, culture, value);
        }
        if (text[0] == '[' && text.Length >= 50 && Guid.TryParse(text.Substring(14, 36), out var result))
        {
            return new ImageFormat(result);
        }
        if (text.Equals("Bmp", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Bmp;
        }
        if (text.Equals("Emf", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Emf;
        }
        if (text.Equals("Exif", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Exif;
        }
        if (text.Equals("Gif", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Gif;
        }
        if (text.Equals("Icon", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Icon;
        }
        if (text.Equals("Jpeg", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Jpeg;
        }
        if (text.Equals("MemoryBmp", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.MemoryBmp;
        }
        if (text.Equals("Png", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Png;
        }
        if (text.Equals("Tiff", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Tiff;
        }
        if (text.Equals("Wmf", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Wmf;
        }
        if (text.Equals("Heif", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Heif;
        }
        if (text.Equals("Webp", StringComparison.OrdinalIgnoreCase))
        {
            return ImageFormat.Webp;
        }
        throw new FormatException("ImageFormat err");
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
        if (value is ImageFormat imageFormat)
        {
            if (destinationType == typeof(string))
            {
                return imageFormat.ToString();
            }
            if (destinationType == typeof(InstanceDescriptor))
            {
                string text = null;
                if (imageFormat.Guid.Equals(ImageFormat.Bmp.Guid))
                {
                    text = "Bmp";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Emf.Guid))
                {
                    text = "Emf";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Exif.Guid))
                {
                    text = "Exif";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Gif.Guid))
                {
                    text = "Gif";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Icon.Guid))
                {
                    text = "Icon";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
                {
                    text = "Jpeg";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.MemoryBmp.Guid))
                {
                    text = "MemoryBmp";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Png.Guid))
                {
                    text = "Png";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Tiff.Guid))
                {
                    text = "Tiff";
                }
                else if (imageFormat.Guid.Equals(ImageFormat.Wmf.Guid))
                {
                    text = "Wmf";
                }
                if (text != null)
                {
                    return new InstanceDescriptor(typeof(ImageFormat).GetProperty(text), null);
                }
                var constructor = typeof(ImageFormat).GetConstructor(new Type[1] { typeof(Guid) });
                return new InstanceDescriptor(constructor, new object[1] { imageFormat.Guid });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection">
    /// </see> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
    /// </returns>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(new ImageFormat[12]
        {
            ImageFormat.MemoryBmp,
            ImageFormat.Bmp,
            ImageFormat.Emf,
            ImageFormat.Wmf,
            ImageFormat.Gif,
            ImageFormat.Jpeg,
            ImageFormat.Png,
            ImageFormat.Tiff,
            ImageFormat.Exif,
            ImageFormat.Icon,
            ImageFormat.Heif,
            ImageFormat.Webp
        });
    }

    /// <summary>
    /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <returns>
    /// true if
    /// <see cref="T:System.ComponentModel.TypeConverter.GetStandardValues">
    /// </see> should be called to find a common set of values the object supports; otherwise, false.
    /// </returns>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
        return true;
    }

}