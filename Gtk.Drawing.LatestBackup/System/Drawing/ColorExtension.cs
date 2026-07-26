namespace System.Drawing;

internal static class ColorExtension
{
    public static Color FromKnownColor(KnownColor color) =>
        FromName(color.ToString());

    public static Color FromName(string name)
    {
        object namedColor = ColorConverter.GetNamedColor(name);
        if (namedColor != null)
        {
            return (Color)namedColor;
        }
        return Color.Transparent;
    }

}