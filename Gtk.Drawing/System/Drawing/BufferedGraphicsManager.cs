namespace System.Drawing;

/// <summary>
/// Provides access to the default <see cref="T:System.Drawing.BufferedGraphicsContext" /> for the
/// application domain.
/// </summary>
/// <remarks>
/// Part of the buffered-graphics family, which was missing entirely. This is the entry point of the
/// standard flicker-free painting recipe:
/// <code>
/// using var buffer = BufferedGraphicsManager.Current.Allocate(e.Graphics, ClientRectangle);
/// buffer.Graphics.Clear(BackColor);
/// // ... draw ...
/// buffer.Render();
/// </code>
/// </remarks>
public static class BufferedGraphicsManager
{
    /// <summary>
    /// Gets the default context for the current application domain.
    /// </summary>
    public static BufferedGraphicsContext Current { get; } = new();
}
