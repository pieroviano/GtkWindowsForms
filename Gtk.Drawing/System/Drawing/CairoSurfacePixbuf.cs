using System.Runtime.InteropServices;

namespace System.Drawing;

/// <summary>
/// Gtk 4 removed <c>gdk_pixbuf_get_from_surface</c> — the Gtk 3 <c>new Gdk.Pixbuf(surface, srcX, srcY,
/// width, height)</c> path this shim used to snapshot a Cairo surface into a <see cref="Gdk.Pixbuf" />.
/// This helper reproduces it by hand: it paints the requested region into a tightly-controlled ARGB32
/// image surface, then converts Cairo's <b>premultiplied, native-endian</b> ARGB (byte order B,G,R,A on
/// little-endian) into the pixbuf's <b>straight</b> RGBA.
/// </summary>
internal static class CairoSurfacePixbuf
{
    /// <summary>Snapshots a region of a Cairo surface into a straight-alpha RGBA <see cref="Gdk.Pixbuf" />.</summary>
    /// <param name="surface">The source Cairo surface.</param>
    /// <param name="srcX">Left of the region to copy, in <paramref name="surface" /> coordinates.</param>
    /// <param name="srcY">Top of the region to copy.</param>
    /// <param name="width">Region width in pixels.</param>
    /// <param name="height">Region height in pixels.</param>
    /// <returns>A newly allocated 8-bit-per-channel RGBA pixbuf of the requested size.</returns>
    public static Gdk.Pixbuf ToPixbuf(this Cairo.Surface surface, int srcX, int srcY, int width, int height)
    {
        using var img = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
        using (var cr = new Cairo.Context(img))
        {
            cr.SetSourceSurface(surface, -srcX, -srcY);
            cr.Paint();
        }

        img.Flush();

        var stride = img.Stride;
        var src = img.Data;                        // premultiplied B,G,R,A
        var rgba = new byte[width * height * 4];    // straight R,G,B,A

        for (var y = 0; y < height; y++)
        {
            var s = y * stride;
            var d = y * width * 4;
            for (var x = 0; x < width; x++, s += 4, d += 4)
            {
                byte b = src[s + 0], g = src[s + 1], r = src[s + 2], a = src[s + 3];
                if (a != 0 && a != 255)
                {
                    r = (byte)(r * 255 / a);
                    g = (byte)(g * 255 / a);
                    b = (byte)(b * 255 / a);
                }

                rgba[d + 0] = r;
                rgba[d + 1] = g;
                rgba[d + 2] = b;
                rgba[d + 3] = a;
            }
        }

        var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
        var rowstride = pixbuf.Rowstride;
        var dest = pixbuf.Pixels;
        for (var y = 0; y < height; y++)
        {
            Marshal.Copy(rgba, y * width * 4, dest + (y * rowstride), width * 4);
        }

        return pixbuf;
    }
}
