namespace System.Drawing.Drawing2D;

/// <summary>
/// Encapsulates a
/// <see cref="T:System.Drawing.Brush" />
/// with a linear gradient. This class cannot be inherited.
/// </summary>
public sealed class LinearGradientBrush : Brush
{
    private Matrix _transform = new Matrix();
    private float _focus;
    private float _scale = 1f;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class with the specified points and colors.
    /// </summary>
    public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
    {
        this.Rectangle = CreateRectangle(point1.X, point1.Y, point2.X, point2.Y);
        this.LinearColors = new[] { color1, color2 };
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class with the specified points and colors.
    /// </summary>
    public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
    {
        this.Rectangle = CreateRectangle(point1.X, point1.Y, point2.X, point2.Y);
        this.LinearColors = new[] { color1, color2 };
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class based on a rectangle, starting and ending colors, and orientation.
    /// </summary>
    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
    {
        this.Rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        this.LinearColors = new[] { color1, color2 };
        this.LinearGradientMode = linearGradientMode;
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class based on a rectangle, starting and ending colors, and an orientation angle.
    /// </summary>
    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
    {
        this.Rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        this.LinearColors = new[] { color1, color2 };
        this.Angle = angle;
        this.WrapMode = WrapMode.Clamp;
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class based on a rectangle, starting and ending colors, and an orientation angle.
    /// </summary>
    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable)
    {
        this.Rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        this.LinearColors = new[] { color1, color2 };
        this.Angle = angle;
        this.WrapMode = WrapMode.Clamp;
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> based on a rectangle, starting and ending colors, and an orientation mode.
    /// </summary>
    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
    {
        this.Rectangle = rect;
        this.LinearColors = new[] { color1, color2 };
        this.LinearGradientMode = linearGradientMode;
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class based on a rectangle, starting and ending colors, and an orientation angle.
    /// </summary>
    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
    {
        this.Rectangle = rect;
        this.LinearColors = new[] { color1, color2 };
        this.Angle = angle;
        this.WrapMode = WrapMode.Clamp;
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" /> class based on a rectangle, starting and ending colors, and an orientation angle.
    /// </summary>
    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable)
    {
        this.Rectangle = rect;
        this.LinearColors = new[] { color1, color2 };
        this.Angle = angle;
        this.WrapMode = WrapMode.Clamp;
    }

    private static RectangleF CreateRectangle(float x1, float y1, float x2, float y2)
    {
        return new RectangleF(x1, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
    }

    /// <summary>
    /// Gets or sets the angle.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    /// Gets or sets a
    /// <see cref="T:System.Drawing.Drawing2D.Blend" /> that specifies positions and factors that define a custom falloff for the gradient.
    /// </summary>
    public Blend? Blend { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gamma correction is enabled for this
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" />.
    /// </summary>
    public bool GammaCorrection { get; set; }

    /// <summary>
    /// Gets or sets a
    /// <see cref="T:System.Drawing.Drawing2D.ColorBlend" /> that defines a multicolor linear gradient.
    /// </summary>
    public ColorBlend? InterpolationColors { get; set; }

    /// <summary>
    /// Gets or sets the starting and ending colors of the gradient.
    /// </summary>
    public Color[] LinearColors { get; set; }

    /// <summary>
    /// Gets or sets the linear gradient mode.
    /// </summary>
    public LinearGradientMode LinearGradientMode { get; set; }

    /// <summary>
    /// Gets a rectangular region that defines the starting and ending points of the gradient.
    /// </summary>
    public RectangleF Rectangle { get; private set; }

    /// <summary>
    /// Gets or sets a copy
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that defines a local geometric transform for this
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" />.
    /// </summary>
    public Matrix? Transform
    {
        get => _transform;
        set => _transform = value ?? new Matrix();
    }

    /// <summary>
    /// Gets or sets a
    /// <see cref="T:System.Drawing.Drawing2D.WrapMode" /> enumeration that indicates the wrap mode for this
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" />.
    /// </summary>
    public WrapMode WrapMode { get; set; } = WrapMode.Tile;

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.Drawing2D.LinearGradientBrush" />.
    /// </summary>
    public override object Clone()
    {
        return new LinearGradientBrush(Rectangle, LinearColors[0], LinearColors[1], Angle)
        {
            Blend = Blend,
            GammaCorrection = GammaCorrection,
            InterpolationColors = InterpolationColors,
            LinearGradientMode = LinearGradientMode,
            Transform = Transform,
            WrapMode = WrapMode
        };
    }

    /// <summary>
    /// Multiplies the transform.
    /// </summary>
    public void MultiplyTransform(Matrix matrix)
    {
        _transform.Multiply(matrix);
    }

    /// <summary>
    /// Multiplies the transform.
    /// </summary>
    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        _transform.Multiply(matrix, order);
    }

    /// <summary>
    /// Resets the transform.
    /// </summary>
    public void ResetTransform()
    {
        _transform = new Matrix();
    }

    /// <summary>
    /// Rotates the transform.
    /// </summary>
    public void RotateTransform(float angle)
    {
        _transform.Rotate(angle);
    }

    /// <summary>
    /// Rotates the transform.
    /// </summary>
    public void RotateTransform(float angle, MatrixOrder order)
    {
        _transform.Rotate(angle, order);
    }

    /// <summary>
    /// Scales the transform.
    /// </summary>
    public void ScaleTransform(float sx, float sy)
    {
        _transform.Scale(sx, sy);
    }

    /// <summary>
    /// Scales the transform.
    /// </summary>
    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        _transform.Scale(sx, sy, order);
    }

    /// <summary>
    /// Creates triangular blend.
    /// </summary>
    public void SetBlendTriangularShape(float focus)
    {
        SetBlendTriangularShape(focus, 1f);
    }

    /// <summary>
    /// Creates triangular blend.
    /// </summary>
    public void SetBlendTriangularShape(float focus, float scale)
    {
        _focus = focus;
        _scale = scale;

        Blend = new Blend
        {
            Factors = new[] { 0f, scale, 0f },
            Positions = new[] { 0f, focus, 1f }
        };
    }

    /// <summary>
    /// Creates sigma bell blend.
    /// </summary>
    public void SetSigmaBellShape(float focus)
    {
        SetSigmaBellShape(focus, 1f);
    }

    /// <summary>
    /// Creates sigma bell blend.
    /// </summary>
    public void SetSigmaBellShape(float focus, float scale)
    {
        _focus = focus;
        _scale = scale;

        Blend = new Blend
        {
            Factors = new[] { 0f, scale, scale, 0f },
            Positions = new[] { 0f, focus * 0.5f, focus, 1f }
        };
    }

    /// <summary>
    /// Translates the transform.
    /// </summary>
    public void TranslateTransform(float dx, float dy)
    {
        _transform.Translate(dx, dy);
    }

    /// <summary>
    /// Translates the transform.
    /// </summary>
    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        _transform.Translate(dx, dy, order);
    }
}