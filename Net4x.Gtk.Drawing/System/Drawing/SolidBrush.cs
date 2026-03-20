
namespace System.Drawing;

/// <summary>
/// Defines a brush of a single color. Brushes are used to fill graphics shapes, such as rectangles, ellipses, pies, polygons, and paths. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class SolidBrush : Brush
{

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.SolidBrush" /> object of the specified color.
    /// </summary>
    /// <param name="color">
    /// A
    /// <see cref="T:System.Drawing.Color" /> structure that represents the color of this brush.
    /// </param>
    public SolidBrush(Color color)
    {
        this.Color = color;
    }

    /// <summary>
    /// Gets or sets the color of this
    /// <see cref="T:System.Drawing.SolidBrush" /> object.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.Color" /> structure that represents the color of this brush.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// The
    /// <see cref="T:System.Drawing.SolidBrush.Color" /> property is set on an immutable
    /// <see cref="T:System.Drawing.SolidBrush" />.
    /// </exception>
    public Color Color
    {
        get;

        set;
    }

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.SolidBrush" /> object.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.SolidBrush" /> object that this method creates.
    /// </returns>
    public override object Clone()
    {
        return null;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.Drawing.Brush" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true" /> to release both managed and unmanaged resources;
    /// <see langword="false" /> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {

    }

}