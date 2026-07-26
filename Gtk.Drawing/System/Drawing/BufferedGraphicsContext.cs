namespace System.Drawing;

/// <summary>
/// Provides methods for creating graphics buffers that can be used for double buffering.
/// </summary>
/// <remarks>
/// Part of the buffered-graphics family, which was missing entirely.
/// </remarks>
public sealed class BufferedGraphicsContext : IDisposable
{
    private Size _maximumBuffer = new(225, 96);

    /// <summary>
    /// Gets or sets the maximum size of a buffer to keep in memory.
    /// </summary>
    /// <exception cref="T:System.ArgumentException">
    /// The width or height is not positive.
    /// </exception>
    public Size MaximumBuffer
    {
        get => _maximumBuffer;
        set
        {
            if (value.Width <= 0 || value.Height <= 0)
            {
                throw new ArgumentException("The maximum buffer size must be positive.", nameof(value));
            }

            _maximumBuffer = value;
        }
    }

    /// <summary>
    /// Creates a buffer of the given size drawing onto the given surface.
    /// </summary>
    /// <param name="targetGraphics">The surface the buffer will be rendered onto.</param>
    /// <param name="targetRectangle">The area the buffer covers.</param>
    /// <returns>The allocated buffer.</returns>
    public BufferedGraphics Allocate(Graphics? targetGraphics, Rectangle targetRectangle) =>
        new(targetGraphics, targetRectangle);

    /// <summary>
    /// Creates a buffer of the given size for the given device context. Not available on this platform.
    /// </summary>
    /// <param name="targetDC">A handle to a device context.</param>
    /// <param name="targetRectangle">The area the buffer covers.</param>
    public BufferedGraphics Allocate(IntPtr targetDC, Rectangle targetRectangle) =>
        throw new PlatformNotSupportedException(
            "GDI device contexts are not available on this platform; allocate against a Graphics instead.");

    /// <summary>
    /// Releases any cached buffer this context is holding.
    /// </summary>
    public void Invalidate()
    {
    }

    /// <summary>
    /// Releases the resources used by this context.
    /// </summary>
    public void Dispose()
    {
    }
}
