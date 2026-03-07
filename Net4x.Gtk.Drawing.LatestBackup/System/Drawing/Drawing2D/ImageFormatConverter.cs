using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;

namespace System.Drawing
{
    /// <summary>
    /// <see cref="System.Drawing.ImageFormatConverter" />
    /// is a class that can be used to convert
    /// <see cref="System.Drawing.Imaging.ImageFormat" />
    /// objects from one data type to another. Access this class through the
    /// <see cref="System.ComponentModel.TypeDescriptor" /> object.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ImageFormatConverter : TypeConverter
    {
        /// <summary>
        /// Indicates whether this converter can convert an object in the specified source type to the native type of the converter.
        /// </summary>
        /// <returns>This method returns true if this object can perform the conversion.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to get more information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <param name="sourceType">The type you want to convert from.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (!(sourceType == typeof(string)))
            {
                return base.CanConvertFrom(context, sourceType);
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the specified destination type using the context.
        /// </summary>
        /// <returns>This method returns true if this object can perform the conversion.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that specifies the context for this type conversion.
        /// </param>
        /// <param name="destinationType">
        /// The
        /// <see cref="System.Type" /> that represents the type to which you want to convert this
        /// <see cref="System.Drawing.Imaging.ImageFormat" /> object.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the specified object to an
        /// <see cref="System.Drawing.Imaging.ImageFormat" /> object.
        /// </summary>
        /// <returns>The converted object.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to get more information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <param name="culture">A
        /// <see cref="System.Globalization.CultureInfo" /> object that specifies formatting conventions for a particular culture.
        /// </param>
        /// <param name="value">The object to convert.
        /// </param>
        /// <exception cref="System.NotSupportedException">The conversion cannot be completed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text == null)
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
        /// Converts the specified object to the specified type.
        /// </summary>
        /// <returns>The converted object.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to get more information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <param name="culture">A
        /// <see cref="System.Globalization.CultureInfo" /> object that specifies formatting conventions for a particular culture.
        /// </param>
        /// <param name="value">The object to convert.
        /// </param>
        /// <param name="destinationType">The type to convert the object to.
        /// </param>
        /// <exception cref="System.NotSupportedException">The conversion cannot be completed.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="destinationType" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ImageFormat imageFormat = value as ImageFormat;
            if (imageFormat != null)
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
                    ConstructorInfo constructor = typeof(ImageFormat).GetConstructor(new Type[1] { typeof(Guid) });
                    return new InstanceDescriptor(constructor, new object[1] { imageFormat.Guid });
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Gets a collection that contains a set of standard values for the data type this validator is designed for. Returns null if the data type does not support a standard set of values.
        /// </summary>
        /// <returns>A collection that contains a standard set of valid values, or null. The default implementation always returns null.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to get more information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Indicates whether this object supports a standard set of values that can be picked from a list.
        /// </summary>
        /// <returns>
        /// This method returns true if the
        /// <see cref="!:Overload:System.Drawing.ImageFormatConverter.GetStandardValues" /> method should be called to find a common set of values the object supports.
        /// </returns>
        /// <param name="context">A type descriptor through which additional context can be provided.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
