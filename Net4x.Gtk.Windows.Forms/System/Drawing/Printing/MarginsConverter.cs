// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Drawing.Printing;

/// <summary>
/// Provides a type converter to convert
/// <see cref="T:System.Drawing.Printing.Margins" /> to and from various other representations, such as a string.
/// </summary>
public class MarginsConverter : ExpandableObjectConverter
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
                => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

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
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                => destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);

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
    /// <exception cref="T:System.ArgumentException">
    /// TextParseFailedFormat
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo? culture, object value)
    {
        if (value is string strValue)
        {
        }
        else
        {
            return base.ConvertFrom(context, culture, value);
        }

        var text = strValue.Trim();

        if (text.Length == 0)
        {
            return null;
        }
        else
        {
            // Parse 4 integer values.
            culture ??= CultureInfo.CurrentCulture;
            var sep = culture.TextInfo.ListSeparator[0];
            var tokens = text.Split(sep);
            var values = new int[tokens.Length];
            var intConverter = GetIntConverter();

            for (var i = 0; i < values.Length; i++)
            {
                // Note: ConvertFromString will raise exception if value cannot be converted.
                values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i])!;
            }

            if (values.Length != 4)
            {
                throw new ArgumentException("TextParseFailedFormat");
            }

            return new Margins(values[0], values[1], values[2], values[3]);
        }
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
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo? culture, object value, Type destinationType)
    {
        if (value is Margins margins)
        {
            if (destinationType == typeof(string))
            {
                culture ??= CultureInfo.CurrentCulture;
                var sep = culture.TextInfo.ListSeparator + " ";
                var intConverter = GetIntConverter();
                var args = new string[4];
                var nArg = 0;

                // Note: ConvertToString will raise exception if value cannot be converted.
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Left);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Right);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Top);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Bottom);

                return string.Join(sep, args);
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                if (typeof(Margins).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }) is { } constructor)
                {
                    return new InstanceDescriptor(
                        constructor,
                        new object[] { margins.Left, margins.Right, margins.Top, margins.Bottom });
                }
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Creates an instance of the type that this
    /// <see cref="T:System.ComponentModel.TypeConverter">
    /// </see> is associated with, using the specified context, given a set of property values for the object.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <param name="propertyValues">
    /// An
    /// <see cref="T:System.Collections.IDictionary">
    /// </see> of new property values.
    /// </param>
    /// <returns>
    /// An
    /// <see cref="T:System.Object">
    /// </see> representing the given
    /// <see cref="T:System.Collections.IDictionary">
    /// </see>, or null if the object cannot be created. This method always returns null.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// PropertyValueInvalidEntry
    /// </exception>
    public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
    {
        var left = propertyValues["Left"];
        var right = propertyValues["Right"];
        var top = propertyValues["Top"];
        var bottom = propertyValues["Bottom"];

        return left is int && right is int && bottom is int && top is int ? (object)new Margins((int)left, (int)right, (int)top, (int)bottom) : throw new ArgumentException("PropertyValueInvalidEntry");
    }

    /// <summary>
    /// Returns whether changing a value on this object requires a call to
    /// <see cref="T:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)">
    /// </see> to create a new value, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An
    /// <see cref="T:System.ComponentModel.ITypeDescriptorContext">
    /// </see> that provides a format context.
    /// </param>
    /// <returns>
    /// true if changing a property on this object requires a call to
    /// <see cref="T:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)">
    /// </see> to create a new value; otherwise, false.
    /// </returns>
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

    private static TypeConverter GetIntConverter() => TypeDescriptor.GetConverter(typeof(int));

}