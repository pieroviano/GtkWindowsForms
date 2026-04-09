using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

public class ColorConverter : TypeConverter
{

    private static Hashtable? colorConstants;

    private static readonly string ColorConstantsLock;

    private static Hashtable? systemColorConstants;

    private static readonly string SystemColorConstantsLock;

    private static StandardValuesCollection? values;

    private static readonly string ValuesLock;

    public ColorConverter()
    {
    }

    static ColorConverter()
    {
        ColorConverter.ColorConstantsLock = "colorConstants";
        ColorConverter.SystemColorConstantsLock = "systemColorConstants";
        ColorConverter.ValuesLock = "values";
    }

    private static Hashtable Colors
    {
        get
        {
            if (ColorConverter.colorConstants == null)
            {
                lock (ColorConverter.ColorConstantsLock)
                {
                    if (ColorConverter.colorConstants == null)
                    {
                        var hashtables = new Hashtable(StringComparer.OrdinalIgnoreCase);
                        ColorConverter.FillConstants(hashtables, typeof(Color));
                        ColorConverter.colorConstants = hashtables;
                    }
                }
            }

            return ColorConverter.colorConstants;
        }
    }

    private static Hashtable SystemColors
    {
        get
        {
            if (ColorConverter.systemColorConstants == null)
            {
                lock (ColorConverter.SystemColorConstantsLock)
                {
                    if (ColorConverter.systemColorConstants == null)
                    {
                        var hashtables = new Hashtable(StringComparer.OrdinalIgnoreCase);
                        ColorConverter.FillConstants(hashtables, typeof(SystemColors));
                        ColorConverter.systemColorConstants = hashtables;
                    }
                }
            }

            return ColorConverter.systemColorConstants;
        }
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }

        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string str)
        {
            return base.ConvertFrom(context, culture, value);
        }
        object? namedColor = null;
        var str1 = str.Trim();
        if (str1.Length != 0)
        {
            namedColor = ColorConverter.GetNamedColor(str1);
            if (namedColor == null)
            {
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }
                var listSeparator = culture.TextInfo.ListSeparator[0];
                var flag = true;
                var converter = TypeDescriptor.GetConverter(typeof(int));
                if (str1.IndexOf(listSeparator) == -1)
                {
                    if (str1.Length >= 2 && (str1[0] == '\'' || str1[0] == '\"') && str1[0] == str1[str1.Length - 1])
                    {
                        var str2 = str1.Substring(1, str1.Length - 2);
                        namedColor = Color.FromName(str2);
                        flag = false;
                    }
                    else if (str1.Length == 7 && str1[0] == '#' || str1.Length == 8 && (str1.StartsWith("0x") || str1.StartsWith("0X")) || str1.Length == 8 && (str1.StartsWith("&h") || str1.StartsWith("&H")))
                    {
                        namedColor = Color.FromArgb(-16777216 | (int)converter.ConvertFromString(context, culture, str1));
                    }
                }
                if (namedColor == null)
                {
                    var strArrays = str1.Split(new char[] { listSeparator });
                    var numArray = new int[(int)strArrays.Length];
                    for (var i = 0; i < (int)numArray.Length; i++)
                    {
                        numArray[i] = (int)converter.ConvertFromString(context, culture, strArrays[i]);
                    }
                    switch ((int)numArray.Length)
                    {
                        case 1:
                            {
                                namedColor = Color.FromArgb(numArray[0]);
                                goto case 2;
                            }
                        case 2:
                            {
                                flag = true;
                                break;
                            }
                        case 3:
                            {
                                namedColor = Color.FromArgb(numArray[0], numArray[1], numArray[2]);
                                goto case 2;
                            }
                        case 4:
                            {
                                namedColor = Color.FromArgb(numArray[0], numArray[1], numArray[2], numArray[3]);
                                goto case 2;
                            }
                        default:
                            {
                                goto case 2;
                            }
                    }
                }
                if (namedColor != null & flag)
                {
                    if (namedColor != null)
                    {
                        var argb = ((Color)namedColor).ToArgb();
                        foreach (Color color in ColorConverter.Colors.Values)
                        {
                            if (color.ToArgb() != argb)
                            {
                                continue;
                            }
                            namedColor = color;
                            break;
                        }
                    }
                }
            }
            if (namedColor == null)
            {
                throw new ArgumentException("InvalidColor");
            }
        }
        else
        {
            namedColor = Color.Empty;
        }

        return namedColor;
    }

    public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        string[] str;
        if (destinationType == null)
        {
            throw new ArgumentNullException("destinationType");
        }

        var colorValue = value as Color?;
        if (colorValue != null)
        {
            if (destinationType == typeof(string))
            {
                var color = colorValue.Value;
                if (color == Color.Empty)
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(color.Name))
                {
                    return color.Name;
                }
                if (color.IsNamedColor)
                {
                    return string.Concat("'", color.Name, "'");
                }
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }
                var str1 = string.Concat(culture.TextInfo.ListSeparator, " ");
                var converter = TypeDescriptor.GetConverter(typeof(int));
                var num = 0;
                if (color.A >= 255)
                {
                    str = new string[3];
                }
                else
                {
                    str = new string[4];
                    var num1 = num;
                    num = num1 + 1;
                    str[num1] = converter.ConvertToString(context, culture, color.A);
                }
                var num2 = num;
                num = num2 + 1;
                str[num2] = converter.ConvertToString(context, culture, color.R);
                var num3 = num;
                num = num3 + 1;
                str[num3] = converter.ConvertToString(context, culture, color.G);
                var num4 = num;
                num = num4 + 1;
                str[num4] = converter.ConvertToString(context, culture, color.B);
                return string.Join(str1, str);
            }
            if (destinationType == typeof(InstanceDescriptor))
            {
                MemberInfo? method = null;
                object[]? a = null;
                var color = colorValue.Value;
                if (color.IsEmpty)
                {
                    method = typeof(Color).GetField("Empty");
                }
                else if (!string.IsNullOrEmpty(color.Name))
                {
                    method = typeof(Color).GetProperty(color.Name);
                }
                else if (color.A != 255)
                {
                    method = typeof(Color).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                    a = new object[] { color.A, color.R, color.G, color.B };
                }
                else if (!color.IsNamedColor)
                {
                    method = typeof(Color).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int) });
                    a = new object[] { color.R, color.G, color.B };
                }
                else
                {
                    method = typeof(Color).GetMethod("FromName", new Type[] { typeof(string) });
                    a = new object[] { color.Name };
                }
                if (method == null)
                {
                    return null;
                }
                return new InstanceDescriptor(method, a);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (ColorConverter.values == null)
        {
            lock (ColorConverter.ValuesLock)
            {
                if (ColorConverter.values == null)
                {
                    var arrayLists = new ArrayList();
                    arrayLists.AddRange(ColorConverter.Colors.Values);
                    arrayLists.AddRange(ColorConverter.SystemColors.Values);
                    var count = arrayLists.Count;
                    for (var i = 0; i < count - 1; i++)
                    {
                        for (var j = i + 1; j < count; j++)
                        {
                            if (arrayLists[i].Equals(arrayLists[j]))
                            {
                                arrayLists.RemoveAt(j);
                                count--;
                                j--;
                            }
                        }
                    }
                    arrayLists.Sort(0, arrayLists.Count, new ColorConverter.ColorComparer());
                    ColorConverter.values = new TypeConverter.StandardValuesCollection(arrayLists.ToArray());
                }
            }
        }

        return ColorConverter.values;
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
        return true;
    }

    internal static object? GetNamedColor(string name)
    {
        object? item = null;
        item = ColorConverter.Colors[name];
        if (item != null)
        {
            return item;
        }
        item = ColorConverter.SystemColors[name];

        return item;
    }

    private static void FillConstants(Hashtable hash, Type enumType)
    {
        var methodAttribute = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Public | MethodAttributes.Static;
        var properties = enumType.GetProperties();
        for (var i = 0; i < (int)properties.Length; i++)
        {
            var value = properties[i];
            if (value.PropertyType == typeof(Color))
            {
                var getMethod = value.GetGetMethod();
                if (getMethod != null && (getMethod.Attributes & methodAttribute) == methodAttribute)
                {
                    object[]? objArray = null;
                    hash[value.Name] = value.GetValue(null, objArray);
                }
            }
        }
    }

    private class ColorComparer : IComparer
    {
        public ColorComparer()
        {
        }

        public int Compare(object left, object right)
        {
            var color = (Color)left;
            var color1 = (Color)right;

            return string.Compare(color.Name, color1.Name, false, CultureInfo.InvariantCulture);
        }
    }
}