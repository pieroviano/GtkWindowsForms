using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    /// <summary>
    /// <see cref="System.Drawing.ImageConverter" />
    /// is a class that can be used to convert
    /// <see cref="System.Drawing.Image" />
    /// objects from one data type to another. Access this class through the
    /// <see cref="System.ComponentModel.TypeDescriptor" /> object.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ImageConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this
        /// <see cref="System.Drawing.ImageConverter" /> can convert an instance of a specified type to an
        /// <see cref="System.Drawing.Image" />
        /// , using the specified context.
        /// </summary>
        /// <returns>This method returns true if this
        /// <see cref="System.Drawing.ImageConverter" /> can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A
        /// <see cref="System.Type" /> that specifies the type you want to convert from.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        /// <summary>
        /// Determines whether this
        /// <see cref="System.Drawing.ImageConverter" /> can convert an
        /// <see cref="System.Drawing.Image" /> to an instance of a specified type, using the specified context.
        /// </summary>
        /// <returns>This method returns true if this
        /// <see cref="System.Drawing.ImageConverter" /> can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">A
        /// <see cref="System.Type" /> that specifies the type you want to convert to.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType)
        {
            if (!(destinationType == typeof(byte[])))
            {
                return destinationType == typeof(string);
            }
            return true;
        }
        /// <summary>
        /// Converts a specified object to an
        /// <see cref="System.Drawing.Image" />
        /// </summary>
        /// <returns>If this method succeeds, it returns the
        /// <see cref="System.Drawing.Image" /> that it created by converting the specified object. Otherwise, it throws an exception.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">A
        /// <see cref="System.Globalization.CultureInfo" /> that holds information about a specific culture.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Object" /> to be converted.
        /// </param>
        /// <exception cref="System.NotSupportedException">The conversion cannot be completed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        /// </PermissionSet>
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
        /// <summary>
        /// Converts an
        /// <see cref="System.Drawing.Image" /> (or an object that can be cast to an
        /// <see cref="System.Drawing.Image" />
        /// ) to the specified type.
        /// </summary>
        /// <returns>This method returns the converted object.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to get more information about the environment this converter is being called from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <param name="culture">A
        /// <see cref="System.Globalization.CultureInfo" /> object that specifies formatting conventions used by a particular culture.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Drawing.Image" /> to convert.
        /// </param>
        /// <param name="destinationType">
        /// The
        /// <see cref="System.Type" /> to convert the
        /// <see cref="System.Drawing.Image" /> to.
        /// </param>
        /// <exception cref="System.NotSupportedException">The conversion cannot be completed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Gets the set of properties for this type.
        /// </summary>
        /// <returns>The set of properties that should be exposed for this data type. If no properties should be exposed, this can return null. The default implementation always returns null.
        /// </returns>
        /// <param name="context">A type descriptor through which additional context can be provided.
        /// </param>
        /// <param name="value">The value of the object to get the properties for.
        /// </param>
        /// <param name="attributes">An array of
        /// <see cref="System.Attribute" /> objects that describe the properties.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(Image), attributes);
        }
        /// <summary>
        /// Indicates whether this object supports properties. By default, this is false.
        /// </summary>
        /// <returns>
        /// This method returns true if the
        /// <see cref="!:Overload:System.Drawing.ImageConverter.GetProperties" />
        /// method should be called to find the properties of this object.
        /// </returns>
        /// <param name="context">A type descriptor through which additional context can be provided.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
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
