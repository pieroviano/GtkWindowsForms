namespace System.Drawing;

/// <summary>
/// Provides a graphics buffer for double buffering.
/// </summary>
/// <remarks>
/// The buffered-graphics family was missing entirely, so the standard flicker-free custom-painting
/// recipe — allocate a buffer, draw into it, render it in one go — did not compile. The buffer here is an
/// off-screen <see cref="T:System.Drawing.Bitmap" />; <see cref="M:System.Drawing.BufferedGraphics.Render" />
/// blits it onto the target surface.
/// </remarks>
public sealed class BufferedGraphics : IDisposable
{
    private readonly Rectangle _targetRectangle;

    private Bitmap? _buffer;

    private Graphics? _bufferGraphics;

    private Graphics? _targetGraphics;

    /// <summary>
    /// Initializes a new buffer of the given size drawing onto the given target.
    /// </summary>
    /// <param name="targetGraphics">The surface the buffer is rendered onto, if any.</param>
    /// <param name="targetRectangle">The area the buffer covers.</param>
    internal BufferedGraphics(Graphics? targetGraphics, Rectangle targetRectangle)
    {
        _targetGraphics = targetGraphics;
        _targetRectangle = targetRectangle;
        _buffer = new Bitmap(Math.Max(1, targetRectangle.Width), Math.Max(1, targetRectangle.Height));
        _bufferGraphics = Graphics.FromImage(_buffer);
    }

    /// <summary>
    /// Gets the surface to draw the buffered content onto.
    /// </summary>
    public Graphics Graphics => _bufferGraphics
                                ?? throw new ObjectDisposedException(nameof(BufferedGraphics));

    /// <summary>
    /// Writes the buffer onto the surface it was allocated against.
    /// </summary>
    public void Render() => Render(_targetGraphics);

    /// <summary>
    /// Writes the buffer onto the given surface.
    /// </summary>
    /// <param name="target">The surface to write onto. Nothing happens when it is null.</param>
    public void Render(Graphics? target)
    {
        if (target == null || _buffer == null || _bufferGraphics == null)
        {
            return;
        }

        // Flush the buffer's own drawing before blitting it, otherwise the last operations are still
        // sitting in the Cairo context.
        _bufferGraphics.Flush();
        target.DrawImage(_buffer, _targetRectangle.X, _targetRectangle.Y);
    }

    /// <summary>
    /// Writes the buffer onto the given device context. Not available on this platform.
    /// </summary>
    /// <param name="targetDC">A handle to a device context.</param>
    public void Render(IntPtr targetDC) =>
        throw new PlatformNotSupportedException(
            "GDI device contexts are not available on this platform; render onto a Graphics instead.");

    /// <summary>
    /// Releases the buffer.
    /// </summary>
    public void Dispose()
    {
        _bufferGraphics?.Dispose();
        _bufferGraphics = null;
        _buffer?.Dispose();
        _buffer = null;
        _targetGraphics = null;
    }
}
