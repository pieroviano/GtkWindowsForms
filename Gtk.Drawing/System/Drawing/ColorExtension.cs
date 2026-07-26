namespace System.Drawing;

internal static class ColorExtension
{
    public static Color FromKnownColor(KnownColor color)
    {
        // Construct the color directly from KnownColor to avoid recursion
        return Color.FromName(color.ToString()); ;
    }

    public static Color FromName(string name)
    {
        // Recursion guard: if name matches a SystemColors property, return it directly
        var sysColorsType = typeof(SystemColors);
        var prop = sysColorsType.GetProperty(name);
        if (prop != null && prop.PropertyType == typeof(Color))
        {
            return (Color)prop.GetValue(null);
        }
        var namedColor = ColorConverter.GetNamedColor(name);
        if (namedColor != null)
        {
            return (Color)namedColor;
        }
        return Color.Transparent;
    }

}