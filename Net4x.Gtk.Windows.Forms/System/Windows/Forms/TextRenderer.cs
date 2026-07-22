using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides methods used to measure and render text.
/// </summary>
/// <remarks>
/// This type was missing entirely. <c>TextRenderer.MeasureText</c> / <c>DrawText</c> has been the
/// recommended WinForms text API since .NET 2.0 and appears in a large share of custom-drawn WinForms code,
/// so its absence was a frequent compile break. This implementation measures and draws through
/// <see cref="T:System.Drawing.Graphics" />, which is backed by Pango, and honours the alignment,
/// single-line and padding bits of <see cref="T:System.Windows.Forms.TextFormatFlags" />.
/// </remarks>
public static class TextRenderer
{
    /// <summary>The padding WinForms adds on each side unless <c>NoPadding</c> is requested.</summary>
    private const int GlyphPadding = 3;

    /// <summary>A scratch surface used when a caller measures without supplying a Graphics.</summary>
    private static Graphics CreateMeasurementSurface() => Graphics.FromImage(new Bitmap(1, 1));

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(string? text, Font? font) =>
        MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Default);

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font within the
    /// specified proposed size.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <param name="proposedSize">The size of the box the text is drawn into.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(string? text, Font? font, Size proposedSize) =>
        MeasureText(text, font, proposedSize, TextFormatFlags.Default);

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font and formatting.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <param name="proposedSize">The size of the box the text is drawn into.</param>
    /// <param name="flags">The formatting to apply to the measured text.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(string? text, Font? font, Size proposedSize, TextFormatFlags flags)
    {
        if (string.IsNullOrEmpty(text) || font == null)
        {
            return Size.Empty;
        }

        using var graphics = CreateMeasurementSurface();

        return MeasureCore(graphics, text!, font, proposedSize, flags);
    }

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font, measured on the
    /// specified device context.
    /// </summary>
    /// <param name="dc">The device context to measure on.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(IDeviceContext? dc, string? text, Font? font) =>
        MeasureText(dc, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Default);

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font within the
    /// specified proposed size, measured on the specified device context.
    /// </summary>
    /// <param name="dc">The device context to measure on.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <param name="proposedSize">The size of the box the text is drawn into.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(IDeviceContext? dc, string? text, Font? font, Size proposedSize) =>
        MeasureText(dc, text, font, proposedSize, TextFormatFlags.Default);

    /// <summary>
    /// Provides the size, in pixels, of the specified text drawn with the specified font and formatting,
    /// measured on the specified device context.
    /// </summary>
    /// <param name="dc">The device context to measure on.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font to apply to the measured text.</param>
    /// <param name="proposedSize">The size of the box the text is drawn into.</param>
    /// <param name="flags">The formatting to apply to the measured text.</param>
    /// <returns>The size of <paramref name="text" />, in pixels.</returns>
    public static Size MeasureText(IDeviceContext? dc, string? text, Font? font, Size proposedSize, TextFormatFlags flags)
    {
        if (string.IsNullOrEmpty(text) || font == null)
        {
            return Size.Empty;
        }

        if (dc is Graphics graphics)
        {
            return MeasureCore(graphics, text!, font, proposedSize, flags);
        }

        return MeasureText(text, font, proposedSize, flags);
    }

    /// <summary>
    /// Draws the specified text at the specified location using the specified font and colour.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="pt">The upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The colour of the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Point pt, Color foreColor) =>
        DrawText(dc, text, font, new Rectangle(pt, new Size(int.MaxValue, int.MaxValue)), foreColor,
            Color.Empty, TextFormatFlags.Default);

    /// <summary>
    /// Draws the specified text at the specified location using the specified font, colour and formatting.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="pt">The upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="flags">The formatting to apply to the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Point pt, Color foreColor, TextFormatFlags flags) =>
        DrawText(dc, text, font, new Rectangle(pt, new Size(int.MaxValue, int.MaxValue)), foreColor,
            Color.Empty, flags);

    /// <summary>
    /// Draws the specified text at the specified location using the specified font and colours.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="pt">The upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="backColor">The colour painted behind the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Point pt, Color foreColor, Color backColor) =>
        DrawText(dc, text, font, new Rectangle(pt, new Size(int.MaxValue, int.MaxValue)), foreColor,
            backColor, TextFormatFlags.Default);

    /// <summary>
    /// Draws the specified text at the specified location using the specified font, colours and formatting.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="pt">The upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="backColor">The colour painted behind the text.</param>
    /// <param name="flags">The formatting to apply to the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags) =>
        DrawText(dc, text, font, new Rectangle(pt, new Size(int.MaxValue, int.MaxValue)), foreColor,
            backColor, flags);

    /// <summary>
    /// Draws the specified text within the specified bounds using the specified font and colour.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="bounds">The rectangle the text is drawn within.</param>
    /// <param name="foreColor">The colour of the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Rectangle bounds, Color foreColor) =>
        DrawText(dc, text, font, bounds, foreColor, Color.Empty, TextFormatFlags.Default);

    /// <summary>
    /// Draws the specified text within the specified bounds using the specified font, colour and formatting.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="bounds">The rectangle the text is drawn within.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="flags">The formatting to apply to the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Rectangle bounds, Color foreColor, TextFormatFlags flags) =>
        DrawText(dc, text, font, bounds, foreColor, Color.Empty, flags);

    /// <summary>
    /// Draws the specified text within the specified bounds using the specified font and colours.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="bounds">The rectangle the text is drawn within.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="backColor">The colour painted behind the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Rectangle bounds, Color foreColor, Color backColor) =>
        DrawText(dc, text, font, bounds, foreColor, backColor, TextFormatFlags.Default);

    /// <summary>
    /// Draws the specified text within the specified bounds using the specified font, colours and
    /// formatting.
    /// </summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font to apply to the text.</param>
    /// <param name="bounds">The rectangle the text is drawn within.</param>
    /// <param name="foreColor">The colour of the text.</param>
    /// <param name="backColor">The colour painted behind the text; ignored when empty.</param>
    /// <param name="flags">The formatting to apply to the text.</param>
    public static void DrawText(IDeviceContext? dc, string? text, Font? font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags)
    {
        if (dc is not Graphics graphics || string.IsNullOrEmpty(text) || font == null)
        {
            return;
        }

        var padding = HasFlag(flags, TextFormatFlags.NoPadding) ? 0 : GlyphPadding;
        var layout = new Rectangle(
            bounds.X + padding,
            bounds.Y,
            bounds.Width == int.MaxValue ? int.MaxValue : Math.Max(0, bounds.Width - (2 * padding)),
            bounds.Height);

        if (!backColor.IsEmpty && backColor.A > 0 && bounds.Width != int.MaxValue && bounds.Height != int.MaxValue)
        {
            using var background = new SolidBrush(backColor);
            graphics.FillRectangle(background, bounds);
        }

        var measured = MeasureCore(graphics, text!, font, new Size(layout.Width, layout.Height), flags);
        var x = layout.X;
        var y = layout.Y;

        if (layout.Width != int.MaxValue && layout.Width > 0)
        {
            if (HasFlag(flags, TextFormatFlags.HorizontalCenter))
            {
                x += Math.Max(0, (layout.Width - measured.Width) / 2);
            }
            else if (HasFlag(flags, TextFormatFlags.Right))
            {
                x += Math.Max(0, layout.Width - measured.Width);
            }
        }

        if (layout.Height != int.MaxValue && layout.Height > 0)
        {
            if (HasFlag(flags, TextFormatFlags.VerticalCenter))
            {
                y += Math.Max(0, (layout.Height - measured.Height) / 2);
            }
            else if (HasFlag(flags, TextFormatFlags.Bottom))
            {
                y += Math.Max(0, layout.Height - measured.Height);
            }
        }

        using var brush = new SolidBrush(foreColor);
        graphics.DrawString(text!, font, brush, new PointF(x, y));
    }

    /// <summary>
    /// Measures <paramref name="text" /> on <paramref name="graphics" />, applying the padding and
    /// single-line rules of <paramref name="flags" />.
    /// </summary>
    private static Size MeasureCore(Graphics graphics, string text, Font font, Size proposedSize, TextFormatFlags flags)
    {
        var wrapWidth = proposedSize.Width;
        if (wrapWidth <= 0 || HasFlag(flags, TextFormatFlags.SingleLine))
        {
            wrapWidth = int.MaxValue;
        }

        var layout = new SizeF(
            wrapWidth == int.MaxValue ? float.MaxValue : wrapWidth,
            proposedSize.Height <= 0 ? float.MaxValue : proposedSize.Height);

        var measured = graphics.MeasureString(text, font, layout);
        var padding = HasFlag(flags, TextFormatFlags.NoPadding) ? 0 : 2 * GlyphPadding;

        return new Size(
            (int)Math.Ceiling(measured.Width) + padding,
            (int)Math.Ceiling(measured.Height));
    }

    /// <summary>
    /// Indicates whether every bit of <paramref name="flag" /> is present in <paramref name="flags" />.
    /// </summary>
    private static bool HasFlag(TextFormatFlags flags, TextFormatFlags flag) => (flags & flag) == flag;
}
