using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Drawing;

/// <summary>
/// Converts <see cref="Font" /> objects to and from their string representation (for example
/// <c>"Microsoft Sans Serif, 8.25pt, style=Bold"</c>), so fonts stored in <c>.resx</c>/designer
/// resources round-trip. Gtk.Drawing reimplements <see cref="Font" />; without this converter
/// <see cref="TypeDescriptor.GetConverter(Type)" /> fell back to the base converter, which cannot
/// convert from a string, and resource deserialization threw <see cref="NotSupportedException" />.
/// </summary>
public class FontConverter : TypeConverter
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }

        return base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc />
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string s)
        {
            return base.ConvertFrom(context, culture, value);
        }

        var font = s.Trim();
        if (font.Length == 0)
        {
            return base.ConvertFrom(context, culture, value);
        }

        culture ??= CultureInfo.CurrentCulture;

        // Format is: FamilyName, size[units][, style=Bold, Italic, ...]
        var separator = culture.TextInfo.ListSeparator[0];
        var fields = font.Split(separator);

        var familyName = fields[0].Trim();
        var size = 8f;
        var unit = GraphicsUnit.Point;

        if (fields.Length > 1)
        {
            var sizeField = fields[1].Trim();
            var units = string.Empty;
            for (var i = 0; i < sizeField.Length; i++)
            {
                if (char.IsLetter(sizeField[i]))
                {
                    size = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(context, culture, sizeField.Substring(0, i))!;
                    units = sizeField.Substring(i);
                    break;
                }
            }

            if (units.Length == 0)
            {
                size = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(context, culture, sizeField)!;
            }

            unit = units switch
            {
                "display" => GraphicsUnit.Display,
                "doc" => GraphicsUnit.Document,
                "pt" => GraphicsUnit.Point,
                "in" => GraphicsUnit.Inch,
                "mm" => GraphicsUnit.Millimeter,
                "px" => GraphicsUnit.Pixel,
                "world" => GraphicsUnit.World,
                _ => GraphicsUnit.Point,
            };
        }

        var style = FontStyle.Regular;
        for (var i = 2; i < fields.Length; i++)
        {
            var field = fields[i];
            if (field.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) != -1) style |= FontStyle.Bold;
            if (field.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) != -1) style |= FontStyle.Italic;
            if (field.IndexOf("Strikeout", StringComparison.OrdinalIgnoreCase) != -1) style |= FontStyle.Strikeout;
            if (field.IndexOf("Underline", StringComparison.OrdinalIgnoreCase) != -1) style |= FontStyle.Underline;
        }

        return new Font(familyName, size, style, unit);
    }

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (value is Font font)
        {
            culture ??= CultureInfo.CurrentCulture;
            if (destinationType == typeof(string))
            {
                var separator = culture.TextInfo.ListSeparator + " ";
                var sb = new StringBuilder();
                sb.Append(font.Name).Append(separator);
                sb.Append(font.Size.ToString(culture)).Append(UnitSuffix(font.Unit));
                if (font.Style != FontStyle.Regular)
                {
                    sb.Append(separator).Append("style=").Append(font.Style.ToString());
                }

                return sb.ToString();
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                var ctor = typeof(Font).GetConstructor(new[] { typeof(string), typeof(float), typeof(FontStyle), typeof(GraphicsUnit) });
                if (ctor != null)
                {
                    return new InstanceDescriptor(ctor, new object?[] { font.Name, font.Size, font.Style, font.Unit });
                }
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    private static string UnitSuffix(GraphicsUnit unit) => unit switch
    {
        GraphicsUnit.Display => "display",
        GraphicsUnit.Document => "doc",
        GraphicsUnit.Point => "pt",
        GraphicsUnit.Inch => "in",
        GraphicsUnit.Millimeter => "mm",
        GraphicsUnit.Pixel => "px",
        GraphicsUnit.World => "world",
        _ => "pt",
    };

    /// <inheritdoc />
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

    /// <inheritdoc />
    public override object? CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
    {
        if (propertyValues == null)
        {
            throw new ArgumentNullException(nameof(propertyValues));
        }

        var name = propertyValues["Name"] as string ?? "Tahoma";
        var size = propertyValues["Size"] is float f ? f : 8f;
        var unit = propertyValues["Unit"] is GraphicsUnit u ? u : GraphicsUnit.Point;
        var charSet = propertyValues["GdiCharSet"] is byte b ? b : (byte)1;
        var vertical = propertyValues["GdiVerticalFont"] is true;

        var style = FontStyle.Regular;
        if (propertyValues["Bold"] is true) style |= FontStyle.Bold;
        if (propertyValues["Italic"] is true) style |= FontStyle.Italic;
        if (propertyValues["Strikeout"] is true) style |= FontStyle.Strikeout;
        if (propertyValues["Underline"] is true) style |= FontStyle.Underline;

        return new Font(name, size, style, unit, charSet, vertical);
    }

    /// <inheritdoc />
    public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

    /// <inheritdoc />
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
        if (value is Font)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        return base.GetProperties(context, value, attributes);
    }
}
