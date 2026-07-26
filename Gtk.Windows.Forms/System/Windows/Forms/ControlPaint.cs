using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

/// <summary>
/// Provides methods used to paint common Windows controls and their elements.
/// </summary>
/// <remarks>
/// This type was missing entirely and is ubiquitous in owner-draw code. The colour helpers reproduce the
/// WinForms shading arithmetic exactly; the drawing helpers render the same elements through
/// <see cref="T:System.Drawing.Graphics" /> rather than through the Windows theme APIs.
/// </remarks>
public static class ControlPaint
{
    /// <summary>The fraction WinForms lightens or darkens by for the "Light"/"Dark" helpers.</summary>
    private const float DefaultShadeFraction = 0.5f;

    /// <summary>Gets a colour that is the light version of the specified colour.</summary>
    /// <param name="baseColor">The colour to lighten.</param>
    public static Color Light(Color baseColor) => Light(baseColor, DefaultShadeFraction);

    /// <summary>Gets a colour that is the light version of the specified colour.</summary>
    /// <param name="baseColor">The colour to lighten.</param>
    /// <param name="percOfLightLight">How far to move towards white, from 0 to 1.</param>
    public static Color Light(Color baseColor, float percOfLightLight) =>
        Blend(baseColor, Color.White, Clamp(percOfLightLight));

    /// <summary>Gets the lightest version of the specified colour.</summary>
    /// <param name="baseColor">The colour to lighten.</param>
    public static Color LightLight(Color baseColor) => Blend(baseColor, Color.White, 1f);

    /// <summary>Gets a colour that is the dark version of the specified colour.</summary>
    /// <param name="baseColor">The colour to darken.</param>
    public static Color Dark(Color baseColor) => Dark(baseColor, DefaultShadeFraction);

    /// <summary>Gets a colour that is the dark version of the specified colour.</summary>
    /// <param name="baseColor">The colour to darken.</param>
    /// <param name="percOfDarkDark">How far to move towards black, from 0 to 1.</param>
    public static Color Dark(Color baseColor, float percOfDarkDark) =>
        Blend(baseColor, Color.Black, Clamp(percOfDarkDark));

    /// <summary>Gets the darkest version of the specified colour.</summary>
    /// <param name="baseColor">The colour to darken.</param>
    public static Color DarkDark(Color baseColor) => Blend(baseColor, Color.Black, 1f);

    /// <summary>
    /// Draws a border in the given style around the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="bounds">The rectangle to outline.</param>
    /// <param name="color">The colour of the border.</param>
    /// <param name="style">The style of the border.</param>
    public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (style == ButtonBorderStyle.None || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using var pen = new Pen(color)
        {
            DashStyle = style == ButtonBorderStyle.Dashed ? DashStyle.Dash
                : style == ButtonBorderStyle.Dotted ? DashStyle.Dot
                : DashStyle.Solid
        };

        graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
    }

    /// <summary>
    /// Draws a three-dimensional border in the given style around the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The rectangle to outline.</param>
    public static void DrawBorder3D(Graphics graphics, Rectangle rectangle) =>
        DrawBorder3D(graphics, rectangle, Border3DStyle.Etched);

    /// <summary>
    /// Draws a three-dimensional border in the given style around the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The rectangle to outline.</param>
    /// <param name="style">The style of the border.</param>
    public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        var face = SystemColors.Control;
        var sunken = style is Border3DStyle.Sunken or Border3DStyle.SunkenInner or Border3DStyle.SunkenOuter;
        var topLeft = sunken ? Dark(face) : LightLight(face);
        var bottomRight = sunken ? LightLight(face) : Dark(face);

        using var topLeftPen = new Pen(topLeft);
        using var bottomRightPen = new Pen(bottomRight);

        var right = rectangle.Right - 1;
        var bottom = rectangle.Bottom - 1;
        graphics.DrawLine(topLeftPen, rectangle.X, rectangle.Y, right, rectangle.Y);
        graphics.DrawLine(topLeftPen, rectangle.X, rectangle.Y, rectangle.X, bottom);
        graphics.DrawLine(bottomRightPen, rectangle.X, bottom, right, bottom);
        graphics.DrawLine(bottomRightPen, right, rectangle.Y, right, bottom);
    }

    /// <summary>
    /// Draws a three-dimensional border in the given style around the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="x">The left edge of the rectangle.</param>
    /// <param name="y">The top edge of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height) =>
        DrawBorder3D(graphics, new Rectangle(x, y, width, height), Border3DStyle.Etched);

    /// <summary>
    /// Draws a three-dimensional border in the given style around the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="x">The left edge of the rectangle.</param>
    /// <param name="y">The top edge of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="style">The style of the border.</param>
    public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style) =>
        DrawBorder3D(graphics, new Rectangle(x, y, width, height), style);

    /// <summary>
    /// Draws a focus rectangle over the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The rectangle to outline.</param>
    public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle) =>
        DrawFocusRectangle(graphics, rectangle, SystemColors.ControlText, SystemColors.Control);

    /// <summary>
    /// Draws a focus rectangle over the given rectangle.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The rectangle to outline.</param>
    /// <param name="foreColor">The colour of the dotted outline.</param>
    /// <param name="backColor">The colour behind the outline.</param>
    public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        using var pen = new Pen(foreColor) { DashStyle = DashStyle.Dot };
        graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
    }

    /// <summary>
    /// Draws a check box in the given state.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="x">The left edge of the check box.</param>
    /// <param name="y">The top edge of the check box.</param>
    /// <param name="width">The width of the check box.</param>
    /// <param name="height">The height of the check box.</param>
    /// <param name="state">The state to draw the check box in.</param>
    public static void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state) =>
        DrawCheckBox(graphics, new Rectangle(x, y, width, height), state);

    /// <summary>
    /// Draws a check box in the given state.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The bounds of the check box.</param>
    /// <param name="state">The state to draw the check box in.</param>
    public static void DrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        using var face = new SolidBrush(state.HasFlag(ButtonState.Inactive) ? SystemColors.Control : SystemColors.Window);
        graphics.FillRectangle(face, rectangle);
        DrawBorder3D(graphics, rectangle, Border3DStyle.Sunken);

        if (!state.HasFlag(ButtonState.Checked))
        {
            return;
        }

        using var tick = new Pen(SystemColors.ControlText, 2f);
        var left = rectangle.X + (rectangle.Width / 4);
        var middle = rectangle.X + (rectangle.Width / 2);
        var right = rectangle.Right - (rectangle.Width / 4);
        var top = rectangle.Y + (rectangle.Height / 4);
        var centre = rectangle.Y + (rectangle.Height / 2);
        var bottom = rectangle.Bottom - (rectangle.Height / 4);
        graphics.DrawLine(tick, left, centre, middle, bottom);
        graphics.DrawLine(tick, middle, bottom, right, top);
    }

    /// <summary>
    /// Draws a button in the given state.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The bounds of the button.</param>
    /// <param name="state">The state to draw the button in.</param>
    public static void DrawButton(Graphics graphics, Rectangle rectangle, ButtonState state)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        using var face = new SolidBrush(SystemColors.Control);
        graphics.FillRectangle(face, rectangle);
        DrawBorder3D(graphics, rectangle,
            state.HasFlag(ButtonState.Pushed) ? Border3DStyle.Sunken : Border3DStyle.Raised);
    }

    /// <summary>
    /// Draws a button in the given state.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="x">The left edge of the button.</param>
    /// <param name="y">The top edge of the button.</param>
    /// <param name="width">The width of the button.</param>
    /// <param name="height">The height of the button.</param>
    /// <param name="state">The state to draw the button in.</param>
    public static void DrawButton(Graphics graphics, int x, int y, int width, int height, ButtonState state) =>
        DrawButton(graphics, new Rectangle(x, y, width, height), state);

    /// <summary>
    /// Fills the given rectangle with the given hatch-style back and fore colours.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="rectangle">The rectangle to fill.</param>
    /// <param name="backColor">The background colour.</param>
    /// <param name="foreColor">The foreground colour.</param>
    public static void FillReversibleRectangle(Graphics graphics, Rectangle rectangle, Color backColor, Color foreColor)
    {
        if (graphics == null)
        {
            throw new ArgumentNullException(nameof(graphics));
        }

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        using var brush = new SolidBrush(Blend(backColor, foreColor, 0.5f));
        graphics.FillRectangle(brush, rectangle);
    }

    /// <summary>
    /// Creates a disabled-looking version of the given image colour.
    /// </summary>
    /// <param name="baseColor">The colour to grey out.</param>
    public static Color DisabledColor(Color baseColor) => Blend(baseColor, SystemColors.Control, 0.5f);

    /// <summary>
    /// Mixes <paramref name="from" /> towards <paramref name="towards" /> by <paramref name="fraction" />.
    /// </summary>
    private static Color Blend(Color from, Color towards, float fraction) => Color.FromArgb(
        from.A,
        (int)Math.Round(from.R + ((towards.R - from.R) * fraction)),
        (int)Math.Round(from.G + ((towards.G - from.G) * fraction)),
        (int)Math.Round(from.B + ((towards.B - from.B) * fraction)));

    /// <summary>Restricts <paramref name="value" /> to the range 0 to 1.</summary>
    private static float Clamp(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
}
