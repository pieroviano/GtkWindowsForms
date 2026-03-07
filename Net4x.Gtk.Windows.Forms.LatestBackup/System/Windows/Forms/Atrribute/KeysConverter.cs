// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a
    /// <see cref="System.ComponentModel.TypeConverter" /> to convert
    /// <see cref="System.Windows.Forms.Keys" />
    /// objects to and from other representations.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class KeysConverter : TypeConverter, IComparer
    {

        private List<string> displayOrder;

        private const Keys FirstAscii = System.Windows.Forms.Keys.A;

        private const Keys FirstDigit = System.Windows.Forms.Keys.D0;

        private const Keys FirstNumpadDigit = System.Windows.Forms.Keys.NumPad0;

        private IDictionary keyNames;

        private const Keys LastAscii = System.Windows.Forms.Keys.Z;

        private const Keys LastDigit = System.Windows.Forms.Keys.D9;

        private const Keys LastNumpadDigit = System.Windows.Forms.Keys.NumPad9;

        private StandardValuesCollection values;

        private List<string> DisplayOrder
        {
            get
            {
                if (displayOrder == null)
                {
                    Debug.Assert(keyNames == null);
                    Initialize();
                }

                return displayOrder;
            }
        }

        /// <summary>
        /// Access to a lookup table of name/value pairs for keys.  These are localized
        /// names.
        /// </summary>
        private IDictionary KeyNames
        {
            get
            {
                if (keyNames == null)
                {
                    Debug.Assert(displayOrder == null);
                    Initialize();
                }

                return keyNames;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this converter can convert an object in the specified source type to the native type of the converter using the specified context.
        /// </summary>
        /// <returns>true if the conversion can be performed; otherwise, false.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <param name="sourceType">The
        /// <see cref="System.Type" /> to convert from.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Enum[]))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns a value indicating whether this converter can convert an object in the specified source type to the native type of the converter using the specified context.
        /// </summary>
        /// <returns>true if the conversion can be performed; otherwise, false.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <param name="destinationType">The
        /// <see cref="System.Type" /> to convert to.
        /// </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Enum[]))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Compares two key values for equivalence.
        /// </summary>
        /// <returns>An integer indicating the relationship between the two parameters.Value Type Condition A negative integer.
        /// <paramref name="a" /> is less than
        /// <paramref name="b" /> zero
        /// <paramref name="a" /> equals
        /// <paramref name="b" /> A positive integer.
        /// <paramref name="a" /> is greater than
        /// <paramref name="b" />
        /// </returns>
        /// <param name="a">An
        /// <see cref="System.Object" /> that represents the first key to compare.
        /// </param>
        /// <param name="b">An
        /// <see cref="System.Object" /> that represents the second key to compare.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public int Compare(object a, object b)
        {
            return string.Compare(ConvertToString(a), ConvertToString(b), false, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified object to the converter's native type.
        /// </summary>
        /// <returns>An object that represents the converted
        /// <paramref name="value" />
        /// </returns>
        /// <param name="context">An ITypeDescriptorContext that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <param name="culture">A CultureInfo object to provide locale information.
        /// </param>
        /// <param name="value">The object to convert.
        /// </param>
        /// <exception cref="System.FormatException">An invalid key combination was supplied.-or- An invalid key name was supplied.
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
                    return null;
                }
                else
                {

                    // Parse an array of key tokens.
                    //
                    string[] tokens = text.Split(new char[] { '+' });
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        tokens[i] = tokens[i].Trim();
                    }

                    // Now lookup each key token in our key hashtable.
                    //
                    Keys key = (Keys)0;
                    bool foundKeyCode = false;

                    for (int i = 0; i < tokens.Length; i++)
                    {
                        object obj = KeyNames[tokens[i]];

                        if (obj == null)
                        {

                            // Key was not found in our table.  See if it is a valid value in
                            // the Keys enum.
                            //
                            obj = Enum.Parse(typeof(Keys), tokens[i]);
                        }

                        if (obj != null)
                        {
                            Keys currentKey = (Keys)obj;

                            if ((currentKey & Keys.KeyCode) != 0)
                            {

                                // We found a match.  If we have previously found a
                                // key code, then check to see that this guy
                                // isn't a key code (it is illegal to have, say,
                                // "A + B"
                                //
                                if (foundKeyCode)
                                {
                                    throw new FormatException("SR.KeysConverterInvalidKeyCombination");
                                }
                                foundKeyCode = true;
                            }

                            // Now OR the key into our current key
                            //
                            key |= currentKey;
                        }
                        else
                        {

                            // We did not match this key.  Report this as an error too.
                            //
                            throw new FormatException(string.Format("SR.KeysConverterInvalidKeyName", tokens[i]));

                        }
                    }

                    return (object)key;
                }
            }
            else if (value is Enum[])
            {
                long finalValue = 0;
                foreach (Enum e in (Enum[])value)
                {
                    finalValue |= Convert.ToInt64(e, CultureInfo.InvariantCulture);
                }
                return Enum.ToObject(typeof(Keys), finalValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the specified object to the specified destination type.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that represents the converted
        /// <paramref name="value" />
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <param name="culture">A
        /// <see cref="System.Globalization.CultureInfo" /> to provide locale information.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Object" /> to convert.
        /// </param>
        /// <param name="destinationType">The
        /// <see cref="System.Type" /> to convert the object to.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="destinationType" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is Keys || value is int)
            {
                bool asString = destinationType == typeof(string);
                bool asEnum = false;
                if (!asString)
                {
                    asEnum = destinationType == typeof(Enum[]);
                }

                if (asString || asEnum)
                {
                    Keys key = (Keys)value;
                    bool added = false;
                    ArrayList terms = new ArrayList();
                    Keys modifiers = (key & Keys.Modifiers);

                    // First, iterate through and do the modifiers. These are
                    // additive, so we support things like Ctrl + Alt
                    //
                    for (int i = 0; i < DisplayOrder.Count; i++)
                    {
                        string keyString = (string)DisplayOrder[i];
                        Keys keyValue = (Keys)keyNames[keyString];
                        if (((int)(keyValue) & (int)modifiers) != 0)
                        {

                            if (asString)
                            {
                                if (added)
                                {
                                    terms.Add("+");
                                }

                                terms.Add((string)keyString);
                            }
                            else
                            {
                                terms.Add(keyValue);
                            }

                            added = true;
                        }
                    }

                    // Now reset and do the key values.  Here, we quit if
                    // we find a match.
                    //
                    Keys keyOnly = (key & Keys.KeyCode);
                    bool foundKey = false;

                    if (added && asString)
                    {
                        terms.Add("+");
                    }

                    for (int i = 0; i < DisplayOrder.Count; i++)
                    {
                        string keyString = (string)DisplayOrder[i];
                        Keys keyValue = (Keys)keyNames[keyString];
                        if (keyValue.Equals(keyOnly))
                        {

                            if (asString)
                            {
                                terms.Add((string)keyString);
                            }
                            else
                            {
                                terms.Add(keyValue);
                            }
                            added = true;
                            foundKey = true;
                            break;
                        }
                    }

                    // Finally, if the key wasn't in our list, add it to
                    // the end anyway.  Here we just pull the key value out
                    // of the enum.
                    //
                    if (!foundKey && Enum.IsDefined(typeof(Keys), (int)keyOnly))
                    {
                        if (asString)
                        {
                            terms.Add(((Enum)keyOnly).ToString());
                        }
                        else
                        {
                            terms.Add((Enum)keyOnly);
                        }
                    }

                    if (asString)
                    {
                        StringBuilder b = new StringBuilder(32);
                        foreach (string t in terms)
                        {
                            b.Append(t);
                        }
                        return b.ToString();
                    }
                    else
                    {
                        return (Enum[])terms.ToArray(typeof(Enum));
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type that this type converter is designed for when provided with a format context.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.TypeConverter.StandardValuesCollection" /> that holds a standard set of valid values, which can be empty if the data type does not support a standard set of values.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                ArrayList list = new ArrayList();

                ICollection keys = KeyNames.Values;

                foreach (object o in keys)
                {
                    list.Add(o);
                }

                list.Sort(this);

                values = new StandardValuesCollection(list.ToArray());
            }

            return values;
        }

        /// <summary>
        /// Determines if the list of standard values returned from GetStandardValues is an exclusive list using the specified
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" />
        /// </summary>
        /// <returns>true if the collection returned from
        /// <see cref="!:Overload:System.Windows.Forms.KeysConverter.GetStandardValues" /> is an exhaustive list of possible values; otherwise, false if other values are possible. The default implementation for this method always returns false.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to extract additional information about the environment this converter is being invoked from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this object supports a standard set of values that can be picked from a list.
        /// </summary>
        /// <returns>Always returns true.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context, which can be used to extract additional information about the environment this converter is being invoked from. This parameter or properties of this parameter can be null.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private void AddKey(string key, Keys value)
        {
            keyNames[key] = value;
            displayOrder.Add(key);
        }

        private void Initialize()
        {
            keyNames = new Hashtable(34);
            displayOrder = new List<string>(34);

            //new whidbey keys follow here...
            // Add string mappings for these values (numbers 0-9) so that the keyboard shortcuts
            // will be displayed properly in menus.
            AddKey("0", Keys.D0);
            AddKey("1", Keys.D1);
            AddKey("2", Keys.D2);
            AddKey("3", Keys.D3);
            AddKey("4", Keys.D4);
            AddKey("5", Keys.D5);
            AddKey("6", Keys.D6);
            AddKey("7", Keys.D7);
            AddKey("8", Keys.D8);
            AddKey("9", Keys.D9);
        }

    }
}
