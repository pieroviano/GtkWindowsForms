// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.VisualStyles;

/// <summary>
/// Provides methods for drawing and obtaining information about a <see cref="T:System.Windows.Forms.VisualStyles.VisualStyleElement" />.
/// </summary>
/// <remarks>
/// On this GTK layer there is no Windows theme service to query, so the drawing methods fall back to classic
/// rendering (via <see cref="T:System.Windows.Forms.ControlPaint" /> and
/// <see cref="T:System.Windows.Forms.TextRenderer" />) and the <c>Get*</c> query methods return sensible
/// defaults. The class/part/state triple is tracked so source that inspects it behaves.
/// </remarks>
public sealed class VisualStyleRenderer
{
    /// <summary>Initializes a new instance from the specified element.</summary>
    /// <param name="element">The visual style element.</param>
    public VisualStyleRenderer(VisualStyleElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        SetParameters(element);
    }

    /// <summary>Initializes a new instance from a class name, part and state.</summary>
    /// <param name="className">The class name.</param>
    /// <param name="part">The part identifier.</param>
    /// <param name="state">The state identifier.</param>
    public VisualStyleRenderer(string className, int part, int state)
    {
        SetParameters(className, part, state);
    }

    /// <summary>Gets a value indicating whether visual styles are supported. Always <see langword="true" /> (GTK theme).</summary>
    public static bool IsSupported => true;

    /// <summary>Gets the class name of the current element.</summary>
    public string Class { get; private set; } = string.Empty;

    /// <summary>Gets the part of the current element.</summary>
    public int Part { get; private set; }

    /// <summary>Gets the state of the current element.</summary>
    public int State { get; private set; }

    /// <summary>Gets the handle of the theme data. Always <see cref="F:System.IntPtr.Zero" /> on GTK.</summary>
    public IntPtr Handle => IntPtr.Zero;

    /// <summary>Gets the last HRESULT returned by a theming operation. Always 0.</summary>
    public int LastHResult => 0;

    /// <summary>Determines whether the specified element is defined by the current visual style.</summary>
    /// <param name="element">The element to test.</param>
    /// <returns><see langword="true" />.</returns>
    public static bool IsElementDefined(VisualStyleElement element) => element != null;

    /// <summary>Sets the parameters of this renderer to those of the specified element.</summary>
    /// <param name="element">The visual style element.</param>
    public void SetParameters(VisualStyleElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        SetParameters(element.ClassName, element.Part, element.State);
    }

    /// <summary>Sets the parameters of this renderer to the specified class, part and state.</summary>
    /// <param name="className">The class name.</param>
    /// <param name="part">The part identifier.</param>
    /// <param name="state">The state identifier.</param>
    public void SetParameters(string className, int part, int state)
    {
        Class = className ?? string.Empty;
        Part = part;
        State = state;
    }

    /// <summary>Draws the background image of the current element within the specified bounds.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    public void DrawBackground(IDeviceContext dc, Rectangle bounds)
    {
        if (dc is Graphics g)
        {
            using var brush = new SolidBrush(SystemColors.Control);
            g.FillRectangle(brush, bounds);
        }
    }

    /// <summary>Draws the background image of the current element within the bounds, clipped to a region.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="clipRectangle">The rectangle to clip to.</param>
    public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
        => DrawBackground(dc, Rectangle.Intersect(bounds, clipRectangle));

    /// <summary>Draws an edge of the specified rectangle.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The rectangle whose edges are drawn.</param>
    /// <param name="edges">The edges to draw.</param>
    /// <param name="style">The edge style.</param>
    /// <param name="effects">The edge effects.</param>
    /// <returns>The interior rectangle.</returns>
    public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
    {
        if (dc is Graphics g)
        {
            ControlPaint.DrawBorder3D(g, bounds);
        }

        return Rectangle.Inflate(bounds, -2, -2);
    }

    /// <summary>Draws the specified image within the bounds.</summary>
    /// <param name="g">The graphics to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="image">The image to draw.</param>
    public void DrawImage(Graphics g, Rectangle bounds, Image image)
    {
        if (image != null)
        {
            g.DrawImage(image, bounds);
        }
    }

    /// <summary>Draws the image from the specified image list within the bounds.</summary>
    /// <param name="g">The graphics to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="imageList">The image list.</param>
    /// <param name="imageIndex">The index of the image to draw.</param>
    public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex)
    {
        if (imageList != null && imageIndex >= 0 && imageIndex < imageList.Images.Count)
        {
            g.DrawImage(imageList.Images[imageIndex], bounds);
        }
    }

    /// <summary>Draws the background of a control's parent within the specified bounds.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="childControl">The control whose parent background is drawn.</param>
    public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
    {
        if (dc is Graphics g)
        {
            using var brush = new SolidBrush(childControl?.BackColor ?? SystemColors.Control);
            g.FillRectangle(brush, bounds);
        }
    }

    /// <summary>Draws text within the specified bounds.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="textToDraw">The text to draw.</param>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw)
        => DrawText(dc, bounds, textToDraw, false, TextFormatFlags.Default);

    /// <summary>Draws text within the specified bounds, optionally grayed.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="textToDraw">The text to draw.</param>
    /// <param name="drawDisabled">Whether to draw the text as disabled.</param>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled)
        => DrawText(dc, bounds, textToDraw, drawDisabled, TextFormatFlags.Default);

    /// <summary>Draws text within the specified bounds with formatting.</summary>
    /// <param name="dc">The device context to draw on.</param>
    /// <param name="bounds">The bounds to draw within.</param>
    /// <param name="textToDraw">The text to draw.</param>
    /// <param name="drawDisabled">Whether to draw the text as disabled.</param>
    /// <param name="flags">The text formatting.</param>
    public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags)
    {
        var color = drawDisabled ? SystemColors.GrayText : SystemColors.ControlText;
        TextRenderer.DrawText(dc, textToDraw, SystemFonts.DefaultFont, bounds, color, flags);
    }

    /// <summary>Returns the content rectangle of the background.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="bounds">The background bounds.</param>
    /// <returns>The content rectangle.</returns>
    public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds) => bounds;

    /// <summary>Returns the total extent of the background.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="contentBounds">The content bounds.</param>
    /// <returns>The background extent.</returns>
    public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds) => contentBounds;

    /// <summary>Returns the region of the background.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="bounds">The background bounds.</param>
    /// <returns>A region covering the bounds.</returns>
    public Region GetBackgroundRegion(IDeviceContext dc, Rectangle bounds) => new Region(bounds);

    /// <summary>Returns the value of the specified boolean property. Defaults to <see langword="false" />.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns><see langword="false" />.</returns>
    public bool GetBoolean(BooleanProperty prop) => false;

    /// <summary>Returns the value of the specified color property.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns>A system color appropriate for the property.</returns>
    public Color GetColor(ColorProperty prop) => prop switch
    {
        ColorProperty.TextColor => SystemColors.ControlText,
        ColorProperty.BorderColor => SystemColors.ControlDark,
        ColorProperty.FillColor => SystemColors.Control,
        _ => SystemColors.Control,
    };

    /// <summary>Returns the value of the specified enumerated property. Defaults to 0.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns>0.</returns>
    public int GetEnumValue(EnumProperty prop) => 0;

    /// <summary>Returns the value of the specified filename property. Defaults to the empty string.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns>The empty string.</returns>
    public string GetFilename(FilenameProperty prop) => string.Empty;

    /// <summary>Returns the value of the specified font property.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="prop">The property to query.</param>
    /// <returns>The default font.</returns>
    public Font GetFont(IDeviceContext dc, FontProperty prop) => SystemFonts.DefaultFont;

    /// <summary>Returns the value of the specified integer property. Defaults to 0.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns>0.</returns>
    public int GetInteger(IntegerProperty prop) => 0;

    /// <summary>Returns the size of the part. Defaults to <see cref="F:System.Drawing.Size.Empty" />.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="type">The size type.</param>
    /// <returns><see cref="F:System.Drawing.Size.Empty" />.</returns>
    public Size GetPartSize(IDeviceContext dc, ThemeSizeType type) => Size.Empty;

    /// <summary>Returns the size of the part within bounds. Defaults to the bounds size.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="type">The size type.</param>
    /// <returns>The bounds size.</returns>
    public Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type) => bounds.Size;

    /// <summary>Returns the value of the specified point property. Defaults to <see cref="F:System.Drawing.Point.Empty" />.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns><see cref="F:System.Drawing.Point.Empty" />.</returns>
    public Point GetPoint(PointProperty prop) => Point.Empty;

    /// <summary>Returns the value of the specified margins property. Defaults to <see cref="F:System.Windows.Forms.Padding.Empty" />.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="prop">The property to query.</param>
    /// <returns><see cref="F:System.Windows.Forms.Padding.Empty" />.</returns>
    public Padding GetMargins(IDeviceContext dc, MarginProperty prop) => Padding.Empty;

    /// <summary>Returns the value of the specified string property. Defaults to the empty string.</summary>
    /// <param name="prop">The property to query.</param>
    /// <returns>The empty string.</returns>
    public string GetString(StringProperty prop) => string.Empty;

    /// <summary>Returns the extent of the specified text.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="textToDraw">The text.</param>
    /// <param name="flags">The formatting.</param>
    /// <returns>The text bounds.</returns>
    public Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
    {
        var size = TextRenderer.MeasureText(dc, textToDraw, SystemFonts.DefaultFont);
        return new Rectangle(Point.Empty, size);
    }

    /// <summary>Returns the extent of the specified text within bounds.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="textToDraw">The text.</param>
    /// <param name="flags">The formatting.</param>
    /// <returns>The text bounds.</returns>
    public Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
    {
        var size = TextRenderer.MeasureText(dc, textToDraw, SystemFonts.DefaultFont, bounds.Size, flags);
        return new Rectangle(bounds.Location, size);
    }

    /// <summary>Returns the text metrics of the current font.</summary>
    /// <param name="dc">The device context.</param>
    /// <returns>A default <see cref="T:System.Windows.Forms.VisualStyles.TextMetrics" />.</returns>
    public TextMetrics GetTextMetrics(IDeviceContext dc) => default;

    /// <summary>Performs a hit test on the background.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="backgroundRectangle">The background rectangle.</param>
    /// <param name="pt">The point to test.</param>
    /// <param name="options">The hit test options.</param>
    /// <returns>A hit test code.</returns>
    public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
        => backgroundRectangle.Contains(pt) ? HitTestCode.Nowhere : HitTestCode.Nowhere;

    /// <summary>Performs a hit test on the background clipped to a region.</summary>
    /// <param name="g">The graphics.</param>
    /// <param name="backgroundRectangle">The background rectangle.</param>
    /// <param name="region">The clip region.</param>
    /// <param name="pt">The point to test.</param>
    /// <param name="options">The hit test options.</param>
    /// <returns>A hit test code.</returns>
    public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
        => HitTestCode.Nowhere;

    /// <summary>Performs a hit test on the background using a native region handle.</summary>
    /// <param name="dc">The device context.</param>
    /// <param name="backgroundRectangle">The background rectangle.</param>
    /// <param name="hRgn">The native region handle.</param>
    /// <param name="pt">The point to test.</param>
    /// <param name="options">The hit test options.</param>
    /// <returns>A hit test code.</returns>
    public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
        => HitTestCode.Nowhere;

    /// <summary>Indicates whether the background of the current element has transparent parts.</summary>
    /// <returns><see langword="false" />.</returns>
    public bool IsBackgroundPartiallyTransparent() => false;
}
