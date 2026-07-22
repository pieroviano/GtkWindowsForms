namespace System.Drawing;

/// <summary>
/// Each property of the <see cref="T:System.Drawing.SystemIcons" /> class is an
/// <see cref="T:System.Drawing.Icon" /> for a Windows system-wide icon.
/// </summary>
/// <remarks>
/// This type was missing entirely, so custom message dialogs using <c>SystemIcons.Error</c> and friends did
/// not compile. The icons are rendered from the GTK icon theme where it supplies a match, and drawn
/// programmatically otherwise, so no icon assets need to be shipped with the library.
/// </remarks>
public static class SystemIcons
{
    /// <summary>The edge length of a generated icon, in pixels.</summary>
    private const int IconSize = 32;

    /// <summary>Gets an icon for the application.</summary>
    public static Icon Application => FromTheme("application-x-executable", Color.FromArgb(255, 90, 90, 90), Shape.Square);

    /// <summary>Gets an icon containing an asterisk, used for information messages.</summary>
    public static Icon Asterisk => Information;

    /// <summary>Gets an icon containing an exclamation mark.</summary>
    public static Icon Exclamation => Warning;

    /// <summary>Gets an icon containing a red circle with a white cross, used for error messages.</summary>
    public static Icon Error => FromTheme("dialog-error", Color.FromArgb(255, 200, 40, 40), Shape.Circle);

    /// <summary>Gets an icon containing a red circle with a white cross.</summary>
    public static Icon Hand => Error;

    /// <summary>Gets an icon containing a blue circle with an "i", used for information messages.</summary>
    public static Icon Information => FromTheme("dialog-information", Color.FromArgb(255, 40, 110, 200), Shape.Circle);

    /// <summary>Gets an icon containing a question mark.</summary>
    public static Icon Question => FromTheme("dialog-question", Color.FromArgb(255, 40, 110, 200), Shape.Circle);

    /// <summary>Gets an icon containing a yellow triangle with an exclamation mark.</summary>
    public static Icon Warning => FromTheme("dialog-warning", Color.FromArgb(255, 220, 170, 20), Shape.Triangle);

    /// <summary>Gets an icon containing a shield.</summary>
    public static Icon Shield => FromTheme("security-high", Color.FromArgb(255, 60, 130, 190), Shape.Shield);

    /// <summary>Gets an icon for a window.</summary>
    public static Icon WinLogo => Application;

    /// <summary>The fallback outline drawn when the icon theme has no match.</summary>
    private enum Shape
    {
        Circle,
        Triangle,
        Square,
        Shield
    }

    /// <summary>
    /// Loads <paramref name="themeName" /> from the GTK icon theme, falling back to a drawn placeholder.
    /// </summary>
    /// <param name="themeName">The freedesktop icon name to look for.</param>
    /// <param name="fallbackColor">The colour of the drawn placeholder.</param>
    /// <param name="shape">The outline of the drawn placeholder.</param>
    private static Icon FromTheme(string themeName, Color fallbackColor, Shape shape)
    {
        try
        {
            var theme = Gtk.IconTheme.Default;
            if (theme != null && theme.HasIcon(themeName))
            {
                var pixbuf = theme.LoadIcon(themeName, IconSize, Gtk.IconLookupFlags.UseBuiltin);
                if (pixbuf != null)
                {
                    return IconFromPixbuf(pixbuf);
                }
            }
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug($"SystemIcons could not load '{themeName}' from the icon theme: {ex.Message}");
        }

        return Draw(fallbackColor, shape);
    }

    /// <summary>
    /// Draws a simple placeholder icon in the given colour and shape.
    /// </summary>
    private static Icon Draw(Color color, Shape shape)
    {
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, IconSize, IconSize);
        using (var context = new Cairo.Context(surface))
        {
            context.SetSourceRGBA(color.R / 255d, color.G / 255d, color.B / 255d, 1d);
            switch (shape)
            {
                case Shape.Circle:
                    context.Arc(IconSize / 2d, IconSize / 2d, (IconSize / 2d) - 2d, 0d, 2d * Math.PI);
                    break;

                case Shape.Triangle:
                    context.MoveTo(IconSize / 2d, 2d);
                    context.LineTo(IconSize - 2d, IconSize - 3d);
                    context.LineTo(2d, IconSize - 3d);
                    context.ClosePath();
                    break;

                case Shape.Shield:
                    context.MoveTo(IconSize / 2d, 2d);
                    context.LineTo(IconSize - 4d, 8d);
                    context.LineTo(IconSize / 2d, IconSize - 2d);
                    context.LineTo(4d, 8d);
                    context.ClosePath();
                    break;

                default:
                    context.Rectangle(3d, 3d, IconSize - 6d, IconSize - 6d);
                    break;
            }

            context.Fill();
        }

        surface.Flush();

        return IconFromPixbuf(new Gdk.Pixbuf(surface, 0, 0, IconSize, IconSize));
    }

    /// <summary>
    /// Wraps a pixbuf in an <see cref="T:System.Drawing.Icon" />.
    /// </summary>
    private static Icon IconFromPixbuf(Gdk.Pixbuf pixbuf)
    {
        var bitmap = new Bitmap(pixbuf.Width, pixbuf.Height) { Pixbuf = pixbuf };
        using var stream = new IO.MemoryStream(pixbuf.SaveToBuffer("png"));

        return new Icon(stream);
    }
}
