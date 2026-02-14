using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{

    /// <summary>
    /// Provides a type converter to convert opacity values to and from a string.
    /// </summary>
    public class OpacityConverter : TypeConverter
    {

        /// <summary>
        /// Initializes an instance of the System.Windows.Forms.OpacityConverter class.
        /// </summary>
        public OpacityConverter()
        {

        }


        /// <summary>
        /// Returns a value indicating whether this converter can convert an object of the
        /// specified source type to the native type of the converter that uses the specified
        /// context.
        /// Parameter:
        /// context:
        /// A System.ComponentModel.ITypeDescriptorContext that provides information about
        /// the context of a type converter.
        /// sourceType:
        /// The System.Type that represents the type you want to convert from.
        /// Return result:
        /// true if this converter can perform the conversion; otherwise, false.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Converts the specified object to the converter's native type.
        /// Parameter:
        /// context:
        /// A System.ComponentModel.ITypeDescriptorContext that provides information about
        /// the context of a type converter.
        /// culture:
        /// The locale information for the conversion.
        /// value:
        /// The object to convert.
        /// Return result:
        /// An System.Object that represents the converted value.
        /// Exception:
        /// T:System.Exception:
        /// The object was not a supported type for the conversion.
        /// T:System.FormatException:
        /// value could not be properly converted to type System.Double. -or- The resulting
        /// converted value was less than zero percent or greater than one hundred percent.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) { return value; }

        /// <summary>
        /// Converts from the converter's native type to a value of the destination type.
        /// Parameter:
        /// context:
        /// A System.ComponentModel.ITypeDescriptorContext that provides information about
        /// the context of a type converter.
        /// culture:
        /// The locale information for the conversion.
        /// value:
        /// The value to convert.
        /// destinationType:
        /// The type to convert the object to.
        /// Return result:
        /// An System.Object that represents the converted value.
        /// Exception:
        /// T:System.ArgumentNullException:
        /// destinationType is null.
        /// T:System.NotSupportedException:
        /// value cannot be converted to the destinationType.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) { return value; }
    }
}

