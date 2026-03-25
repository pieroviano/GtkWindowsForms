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