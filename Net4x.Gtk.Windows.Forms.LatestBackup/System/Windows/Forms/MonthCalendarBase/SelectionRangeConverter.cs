// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    /// SelectionRangeConverter is a class that can be used to convert
    /// SelectionRange objects from one data type to another.  Access this
    /// class through the TypeDescriptor.
    /// </summary>
    public class SelectionRangeConverter : TypeConverter
    {
        /// <summary>
        /// Determines if this converter can convert an object of the specified source type to the native type of the converter by querying the supplied type descriptor context.
        /// </summary>
        /// <returns>true if the converter can perform the specified conversion; otherwise, false.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">The source
        /// <see cref="System.Type" /> to be converted.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(DateTime))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the specified destination type by using the specified context.
        /// </summary>
        /// <returns>true if this converter can perform the specified conversion; otherwise, false.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">The destination
        /// <see cref="System.Type" /> to convert into.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(DateTime))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the specified value to the converter's native type by using the specified locale.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that represents the converted
        /// <paramref name="value" />
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The locale information for the conversion.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Object" /> to convert.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value" /> is of type
        /// <see cref="System.String" /> but could not be parsed into two strings representing dates.
        /// </exception>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is of type
        /// <see cref="System.String" /> that was parsed into two strings, but the conversion of one or both into the
        /// <see cref="System.DateTime" /> type did not succeed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string text = ((string)value).Trim();
                if (text.Length == 0)
                {
                    return new SelectionRange(DateTime.Now.Date, DateTime.Now.Date);
                }

                // Separate the string into the two dates, and parse each one
                //
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }
                char separator = culture.TextInfo.ListSeparator[0];
                string[] tokens = text.Split(new char[] { separator });

                if (tokens.Length == 2)
                {
                    TypeConverter dateTimeConverter = TypeDescriptor.GetConverter(typeof(DateTime));
                    DateTime start = (DateTime)dateTimeConverter.ConvertFromString(context, culture, tokens[0]);
                    DateTime end = (DateTime)dateTimeConverter.ConvertFromString(context, culture, tokens[1]);
                    return new SelectionRange(start, end);
                }
                else
                {
                    throw new ArgumentException(string.Format("Text Parse Failed Format:{0},{1}",
                                                              text,
                                                              "Start" + separator + " End"));
                }
            }
            else if (value is DateTime dt)
            {
                return new SelectionRange(dt, dt);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the specified
        /// <see cref="System.Windows.Forms.SelectionRangeConverter" /> object to another type by using the specified culture.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that represents the converted
        /// <paramref name="value" />
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The locale information for the conversion.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Object" /> to convert.
        /// </param>
        /// <param name="destinationType">The destination
        /// <see cref="System.Type" /> to convert into.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="destinationType" /> is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="value" /> cannot be converted to the
        /// <paramref name="destinationType" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is SelectionRange range)
            {
                if (destinationType == typeof(string))
                {
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string sep = culture.TextInfo.ListSeparator + " ";
                    PropertyDescriptorCollection props = GetProperties(value);
                    string[] args = new string[props.Count];

                    for (int i = 0; i < props.Count; i++)
                    {
                        object propValue = props[i].GetValue(value);
                        args[i] = TypeDescriptor.GetConverter(propValue).ConvertToString(context, culture, propValue);
                    }

                    return string.Join(sep, args);
                }
                if (destinationType == typeof(DateTime))
                {
                    return range.Start;
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo ctor = typeof(SelectionRange).GetConstructor(new Type[] {
                        typeof(DateTime), typeof(DateTime)});
                    if (ctor != null)
                    {
                        return new InstanceDescriptor(ctor, new object[] { range.Start, range.End });
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Creates a
        /// <see cref="System.Windows.Forms.SelectionRange" /> object by using the specified type descriptor and set of property values for that object.
        /// </summary>
        /// <returns>If successful, the newly created
        /// <see cref="System.Windows.Forms.SelectionRange" />; otherwise, this method throws an exception.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="propertyValues">An
        /// <see cref="System.Collections.IDictionary" /> that contains the new property values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="propertyValues" /> is null or its Start and End elements could not be converted into a
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            try
            {
                return new SelectionRange((DateTime)propertyValues["Start"],
                                          (DateTime)propertyValues["End"]);
            }
            catch (InvalidCastException invalidCast)
            {
                throw new ArgumentException("Property Value Invalid Entry", invalidCast);
            }
            catch (NullReferenceException nullRef)
            {
                throw new ArgumentException("Property Value Invalid Entry", nullRef);
            }
        }

        /// <summary>
        /// Determines if changing a value on an instance should require a call to
        /// <see cref="!:Overload:System.Windows.Forms.SelectionRangeConverter.CreateInstance" /> to create a new value.
        /// </summary>
        /// <returns>true if
        /// <see cref="!:Overload:System.Windows.Forms.SelectionRangeConverter.CreateInstance" /> must be called to make a change to one or more properties; otherwise false.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns the set of filtered properties for the
        /// <see cref="System.Windows.Forms.SelectionRange" /> type
        /// </summary>
        /// <returns>If successful, the set of properties that should be exposed for the
        /// <see cref="System.Windows.Forms.SelectionRange" /> type; otherwise, null.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="value">An
        /// <see cref="System.Object" /> that specifies the type of array for which to get properties.
        /// </param>
        /// <param name="attributes">An array of type
        /// <see cref="System.Attribute" /> that is used as a filter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(SelectionRange), attributes);
            return props.Sort(new string[] { "Start", "End" });
        }

        /// <summary>
        /// Determines whether the current object supports properties that use the specified type descriptor context.
        /// </summary>
        /// <returns>true if
        /// <see cref="!:Overload:System.Windows.Forms.SelectionRangeConverter.GetProperties" /> can be called to find the properties of a
        /// <see cref="System.Windows.Forms.SelectionRange" /> object; otherwise, false.
        /// </returns>
        /// <param name="context">A
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
