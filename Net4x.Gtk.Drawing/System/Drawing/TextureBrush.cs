using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing;

/// <summary>
/// Each property of the <see cref="T:System.Drawing.TextureBrush" /> class is a
/// <see cref="T:System.Drawing.Brush" /> object that uses an image to fill the interior of a shape.
/// </summary>
/// <remarks>
/// This was the one core GDI+ brush type the shim did not have — <see cref="T:System.Drawing.SolidBrush" />,
/// <see cref="T:System.Drawing.Drawing2D.HatchBrush" />,
/// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> and
/// <see cref="T:System.Drawing.Drawing2D.PathGradientBrush" /> were all present. Cairo supports surface
/// patterns with repeat modes natively, which is what backs this class.
/// </remarks>
public sealed class TextureBrush : Brush
{
    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image.
    /// </summary>
    /// <param name="bitmap">The image to fill with.</param>
    public TextureBrush(Image bitmap) : this(bitmap, WrapMode.Tile)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image and
    /// wrap mode.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="wrapMode">How the image is tiled when it is smaller than the area being filled.</param>
    public TextureBrush(Image image, WrapMode wrapMode)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
        WrapMode = wrapMode;
        ImageRectangle = new RectangleF(0f, 0f, image.Width, image.Height);
        Transform = new Matrix();
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified region of the
    /// specified image.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    public TextureBrush(Image image, Rectangle dstRect) : this(image, WrapMode.Tile)
    {
        ImageRectangle = new RectangleF(dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height);
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified region of the
    /// specified image.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    public TextureBrush(Image image, RectangleF dstRect) : this(image, WrapMode.Tile)
    {
        ImageRectangle = dstRect;
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image, wrap
    /// mode and region.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="wrapMode">How the image is tiled.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect) : this(image, wrapMode)
    {
        ImageRectangle = new RectangleF(dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height);
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image, wrap
    /// mode and region.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="wrapMode">How the image is tiled.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect) : this(image, wrapMode)
    {
        ImageRectangle = dstRect;
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image and
    /// image attributes.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    /// <param name="imageAttr">Additional image attributes; kept for compatibility.</param>
    public TextureBrush(Image image, RectangleF dstRect, ImageAttributes? imageAttr) : this(image, dstRect)
    {
        ImageAttributes = imageAttr;
    }

    /// <summary>
    /// Initializes a new <see cref="T:System.Drawing.TextureBrush" /> that uses the specified image and
    /// image attributes.
    /// </summary>
    /// <param name="image">The image to fill with.</param>
    /// <param name="dstRect">The region of <paramref name="image" /> to use.</param>
    /// <param name="imageAttr">Additional image attributes; kept for compatibility.</param>
    public TextureBrush(Image image, Rectangle dstRect, ImageAttributes? imageAttr) : this(image, dstRect)
    {
        ImageAttributes = imageAttr;
    }

    /// <summary>
    /// Gets the image this brush fills with.
    /// </summary>
    public Image Image { get; }

    /// <summary>
    /// Gets the region of <see cref="P:System.Drawing.TextureBrush.Image" /> this brush draws.
    /// </summary>
    public RectangleF ImageRectangle { get; private set; }

    /// <summary>
    /// Gets or sets the wrap mode used when the fill area is larger than the image.
    /// </summary>
    public WrapMode WrapMode { get; set; }

    /// <summary>
    /// Gets or sets the transform applied to the image before it is drawn.
    /// </summary>
    public Matrix Transform { get; set; }

    /// <summary>
    /// Gets the image attributes supplied when the brush was created, if any.
    /// </summary>
    public ImageAttributes? ImageAttributes { get; private set; }

    /// <summary>
    /// Creates an exact copy of this brush.
    /// </summary>
    /// <returns>The copy.</returns>
    public override object Clone() => new TextureBrush(Image, WrapMode, ImageRectangle)
    {
        Transform = Transform,
        ImageAttributes = ImageAttributes
    };

    /// <summary>
    /// Resets the transform of this brush to the identity matrix.
    /// </summary>
    public void ResetTransform() => Transform = new Matrix();

    /// <summary>
    /// Multiplies the transform of this brush by the specified matrix.
    /// </summary>
    /// <param name="matrix">The matrix to multiply by.</param>
    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    /// <summary>
    /// Multiplies the transform of this brush by the specified matrix in the specified order.
    /// </summary>
    /// <param name="matrix">The matrix to multiply by.</param>
    /// <param name="order">The order in which to apply the multiplication.</param>
    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        if (matrix == null)
        {
            return;
        }

        Transform.Multiply(matrix, order);
    }

    /// <summary>
    /// Translates the transform of this brush by the specified amounts.
    /// </summary>
    /// <param name="dx">The horizontal offset.</param>
    /// <param name="dy">The vertical offset.</param>
    public void TranslateTransform(float dx, float dy) => Transform.Translate(dx, dy);

    /// <summary>
    /// Translates the transform of this brush by the specified amounts in the specified order.
    /// </summary>
    /// <param name="dx">The horizontal offset.</param>
    /// <param name="dy">The vertical offset.</param>
    /// <param name="order">The order in which to apply the translation.</param>
    public void TranslateTransform(float dx, float dy, MatrixOrder order) => Transform.Translate(dx, dy, order);

    /// <summary>
    /// Scales the transform of this brush by the specified amounts.
    /// </summary>
    /// <param name="sx">The horizontal scale factor.</param>
    /// <param name="sy">The vertical scale factor.</param>
    public void ScaleTransform(float sx, float sy) => Transform.Scale(sx, sy);

    /// <summary>
    /// Scales the transform of this brush by the specified amounts in the specified order.
    /// </summary>
    /// <param name="sx">The horizontal scale factor.</param>
    /// <param name="sy">The vertical scale factor.</param>
    /// <param name="order">The order in which to apply the scaling.</param>
    public void ScaleTransform(float sx, float sy, MatrixOrder order) => Transform.Scale(sx, sy, order);

    /// <summary>
    /// Rotates the transform of this brush by the specified angle.
    /// </summary>
    /// <param name="angle">The angle of rotation, in degrees.</param>
    public void RotateTransform(float angle) => Transform.Rotate(angle);

    /// <summary>
    /// Rotates the transform of this brush by the specified angle in the specified order.
    /// </summary>
    /// <param name="angle">The angle of rotation, in degrees.</param>
    /// <param name="order">The order in which to apply the rotation.</param>
    public void RotateTransform(float angle, MatrixOrder order) => Transform.Rotate(angle, order);

    /// <summary>
    /// Builds the Cairo pattern that paints this texture, honouring the wrap mode.
    /// </summary>
    /// <returns>The pattern, or <see langword="null" /> when the image has no pixels.</returns>
    internal Cairo.Pattern? CreatePattern()
    {
        var pixbuf = Image.Pixbuf;
        if (pixbuf == null)
        {
            return null;
        }

        var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, pixbuf.Width, pixbuf.Height);
        using (var context = new Cairo.Context(surface))
        {
            Gdk.CairoHelper.SetSourcePixbuf(context, pixbuf, 0, 0);
            context.Paint();
        }

        return new Cairo.SurfacePattern(surface)
        {
            Extend = WrapMode == WrapMode.Clamp ? Cairo.Extend.None : Cairo.Extend.Repeat
        };
    }
}
