using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
/// Provides a type converter to convert
/// <see cref="T:System.Windows.Forms.ListViewItem" />
/// objects to and from various other representations.
/// </summary>
public class ListViewItemConverter : ExpandableObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItemConverter" /> class.
    /// </summary>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public ListViewItemConverter()
    {
        throw new System.NotImplementedException();
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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        throw new System.NotImplementedException();
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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        throw new System.NotImplementedException();
    }

}