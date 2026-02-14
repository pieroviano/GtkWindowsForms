using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Provides a type converter to convert opacity values to and from a string.
    public class OpacityConverter : TypeConverter
    {
        //
        // Summary:
        //     Initializes an instance of the System.Windows.Forms.OpacityConverter class.
        public OpacityConverter()
        {

        }

        //
        // Summary:
        //     Returns a value indicating whether this converter can convert an object of the
        //     specified source type to the native type of the converter that uses the specified
        //     context.
        //
        // Parameter:
        //   context:
        //     A System.ComponentModel.ITypeDescriptorContext that provides information about
        //     the context of a type converter.
        //
        //   sourceType:
        //     The System.Type that represents the type you want to convert from.
        //
        // Return result:
        //     true if this converter can perform the conversion; otherwise, false.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        //
        // Summary:
        //     Converts the specified object to the converter's native type.
        //
        // Parameter:
        //   context:
        //     A System.ComponentModel.ITypeDescriptorContext that provides information about
        //     the context of a type converter.
        //
        //   culture:
        //     The locale information for the conversion.
        //
        //   value:
        //     The object to convert.
        //
        // Return result:
        //     An System.Object that represents the converted value.
        //
        // Exception:
        //   T:System.Exception:
        //     The object was not a supported type for the conversion.
        //
        //   T:System.FormatException:
        //     value could not be properly converted to type System.Double. -or- The resulting
        //     converted value was less than zero percent or greater than one hundred percent.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) { return value; }
        //
        // Summary:
        //     Converts from the converter's native type to a value of the destination type.
        //
        // Parameter:
        //   context:
        //     A System.ComponentModel.ITypeDescriptorContext that provides information about
        //     the context of a type converter.
        //
        //   culture:
        //     The locale information for the conversion.
        //
        //   value:
        //     The value to convert.
        //
        //   destinationType:
        //     The type to convert the object to.
        //
        // Return result:
        //     An System.Object that represents the converted value.
        //
        // Exception:
        //   T:System.ArgumentNullException:
        //     destinationType is null.
        //
        //   T:System.NotSupportedException:
        //     value cannot be converted to the destinationType.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) { return value; }
    }
}

