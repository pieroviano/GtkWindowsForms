using System.Collections.Generic;

namespace System.Drawing.Text;

/// <summary>
/// Provides a base class for installed and private font collections.
/// </summary>
public abstract class FontCollection : IDisposable
{
    /// <summary>
    /// The font families in this collection. Populated by derived collections.
    /// </summary>
    private protected readonly List<FontFamily> families = new();

    /// <summary>
    /// Gets the array of <see cref="T:System.Drawing.FontFamily" /> objects associated with this
    /// <see cref="T:System.Drawing.Text.FontCollection" />.
    /// </summary>
    /// <returns>
    /// An array of <see cref="T:System.Drawing.FontFamily" /> objects.
    /// </returns>
    public FontFamily[]? Families => families.ToArray();

    /// <summary>
    /// Releases all resources used by this <see cref="T:System.Drawing.Text.FontCollection" />.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.Drawing.Text.FontCollection" />
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            families.Clear();
        }
    }
}
