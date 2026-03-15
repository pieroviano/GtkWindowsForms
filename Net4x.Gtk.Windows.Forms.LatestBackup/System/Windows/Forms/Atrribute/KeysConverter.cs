// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// Provides a
/// <see cref="T:System.ComponentModel.TypeConverter" /> to convert
/// <see cref="T:System.Windows.Forms.Keys" />
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
    /// Determines if this converter can convert an object in the given source
    /// type to the native type of the converter.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string) || sourceType == typeof(Enum[]))
        {
            return true;
        }

        return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Gets a value indicating whether this converter can
    /// convert an object to the given destination type using the context.
    /// </summary>
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
    public int Compare(object a, object b)
    {
        return string.Compare(ConvertToString(a), ConvertToString(b), false, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the given object to the converter's native type.
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string)
        {

            var text = ((string)value).Trim();

            if (text.Length == 0)
            {
                return null;
            }
            else
            {

                // Parse an array of key tokens.
                //
                var tokens = text.Split(new char[] { '+' });
                for (int i = 0; i < tokens.Length; i++)
                {
                    tokens[i] = tokens[i].Trim();
                }

                // Now lookup each key token in our key hashtable.
                //
                var key = (Keys)0;
                var foundKeyCode = false;

                for (int i = 0; i < tokens.Length; i++)
                {
                    object obj = KeyNames[tokens[i]] ?? Enum.Parse(typeof(Keys), tokens[i]);
                    if (obj != null)
                    {
                        var currentKey = (Keys)obj;

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
    /// Converts the given object to another type.  The most common types to convert
    /// are to and from a string object.  The default implementation will make a call
    /// to ToString on the object if the object is valid and if the destination
    /// type is string.  If this cannot convert to the desitnation type, this will
    /// throw a NotSupportedException.
    /// </summary>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (value is Keys || value is int)
        {
            var asString = destinationType == typeof(string);
            var asEnum = false;
            if (!asString)
            {
                asEnum = destinationType == typeof(Enum[]);
            }

            if (asString || asEnum)
            {
                var key = (Keys)value;
                var added = false;
                var terms = new ArrayList();
                Keys modifiers = (key & Keys.Modifiers);

                // First, iterate through and do the modifiers. These are
                // additive, so we support things like Ctrl + Alt
                //
                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    var keyString = (string)DisplayOrder[i];
                    var keyValue = (Keys)keyNames[keyString];
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
                var foundKey = false;

                if (added && asString)
                {
                    terms.Add("+");
                }

                for (int i = 0; i < DisplayOrder.Count; i++)
                {
                    var keyString = (string)DisplayOrder[i];
                    var keyValue = (Keys)keyNames[keyString];
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
                    var b = new StringBuilder(32);
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
    /// Retrieves a collection containing a set of standard values
    /// for the data type this validator is designed for.  This
    /// will return null if the data type does not support a standard set of values.
    /// </summary>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (values == null)
        {
            var list = new ArrayList();

            var keys = KeyNames.Values;

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
    /// Determines if the list of standard values returned from
    /// GetStandardValues is an exclusive list.  If the list
    /// is exclusive, then no other values are valid, such as
    /// in an enum data type.  If the list is not exclusive,
    /// then there are other valid values besides the list of
    /// standard values GetStandardValues provides.
    /// </summary>
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
        return false;
    }

    /// <summary>
    /// Determines if this object supports a standard set of values
    /// that can be picked from a list.
    /// </summary>
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