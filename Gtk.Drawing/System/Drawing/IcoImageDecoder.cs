using System.IO;

namespace System.Drawing;

/// <summary>
/// A small, dependency-free decoder for Windows <c>.ico</c> / <c>.cur</c> images.
/// </summary>
/// <remarks>
/// The GTK 4 runtime does not always ship the gdk-pixbuf ICO loader (GTK 3 did), so
/// <c>new Gdk.Pixbuf(icoBytes)</c> fails with "Unrecognized image file format" - which crashed every
/// form/cursor/icon that loads a <c>.ico</c>. This turns the icon's best image directly into a
/// <see cref="Gdk.Pixbuf" /> instead, covering the common encodings: an embedded PNG (handed to the
/// built-in PNG loader) and 32/24-bit BMP/DIB frames (decoded here, with the 1-bpp AND mask applied when a
/// frame has no alpha of its own).
/// </remarks>
internal static class IcoImageDecoder
{
    /// <summary>
    /// Attempts to decode ICO/CUR bytes into a <see cref="Gdk.Pixbuf" /> (the largest / deepest frame).
    /// Returns <see langword="null" /> when the data is not an icon or cannot be decoded here.
    /// </summary>
    public static Gdk.Pixbuf? TryDecode(byte[]? ico)
    {
        try
        {
            if (ico == null || ico.Length < 6)
            {
                return null;
            }

            // ICONDIR: reserved(0), type(1=icon,2=cursor), count.
            if (BitConverter.ToUInt16(ico, 0) != 0)
            {
                return null;
            }

            var type = BitConverter.ToUInt16(ico, 2);
            if (type != 1 && type != 2)
            {
                return null;
            }

            int count = BitConverter.ToUInt16(ico, 4);
            if (count < 1 || ico.Length < 6 + count * 16)
            {
                return null;
            }

            // Pick the frame with the most pixels, breaking ties by colour depth.
            var bestOffset = -1;
            var bestSize = -1;
            long bestScore = -1;
            for (var i = 0; i < count; i++)
            {
                var e = 6 + i * 16;
                var w = ico[e] == 0 ? 256 : ico[e];
                var h = ico[e + 1] == 0 ? 256 : ico[e + 1];
                int bpp = BitConverter.ToUInt16(ico, e + 6);
                var bytes = (int)BitConverter.ToUInt32(ico, e + 8);
                var offset = (int)BitConverter.ToUInt32(ico, e + 12);
                if (offset < 0 || bytes <= 0 || (long)offset + bytes > ico.Length)
                {
                    continue;
                }

                var score = (long)w * h * Math.Max(bpp, 1);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestOffset = offset;
                    bestSize = bytes;
                }
            }

            if (bestOffset < 0)
            {
                return null;
            }

            var frame = new byte[bestSize];
            Array.Copy(ico, bestOffset, frame, 0, bestSize);

            // PNG-compressed frame (Vista+): the PNG loader is built in, so hand it over.
            if (bestSize > 8 && frame[0] == 0x89 && frame[1] == 0x50 && frame[2] == 0x4E && frame[3] == 0x47)
            {
                return new Gdk.Pixbuf(frame);
            }

            return DecodeDib(frame);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Decodes a BMP/DIB icon frame (BITMAPINFOHEADER, no file header) into a pixbuf.</summary>
    private static Gdk.Pixbuf? DecodeDib(byte[] dib)
    {
        if (dib.Length < 40)
        {
            return null;
        }

        var headerSize = (int)BitConverter.ToUInt32(dib, 0);
        var width = BitConverter.ToInt32(dib, 4);
        var storedHeight = BitConverter.ToInt32(dib, 8);
        int bpp = BitConverter.ToUInt16(dib, 14);

        // In an icon DIB the stored height spans the XOR colour rows plus the 1-bpp AND mask, so the real
        // image is half as tall. (A .cur frame can already be top-down with a negative height.)
        var topDown = storedHeight < 0;
        var absHeight = Math.Abs(storedHeight);
        var height = absHeight / 2;
        if (width <= 0 || height <= 0)
        {
            return null;
        }

        // Only the direct-colour depths are handled; palette formats (<=8 bpp) are rare for shipped icons.
        if (bpp != 32 && bpp != 24)
        {
            return null;
        }

        var xorStride = (width * bpp + 31) / 32 * 4; // rows padded to 4 bytes
        var xorStart = headerSize;
        var xorLength = xorStride * height;
        if (xorStart + xorLength > dib.Length)
        {
            return null;
        }

        // 1-bpp AND (transparency) mask, immediately after the colour rows.
        var maskStride = (width + 31) / 32 * 4;
        var maskStart = xorStart + xorLength;
        var hasMask = maskStart + maskStride * height <= dib.Length;

        var pixels = new byte[width * height * 4];
        for (var y = 0; y < height; y++)
        {
            // BMP rows are bottom-up unless the height was negative.
            var srcY = topDown ? y : height - 1 - y;
            var xorRow = xorStart + srcY * xorStride;
            var maskRow = maskStart + srcY * maskStride;
            for (var x = 0; x < width; x++)
            {
                var di = (y * width + x) * 4;
                if (bpp == 32)
                {
                    var s = xorRow + x * 4; // BGRA
                    pixels[di] = dib[s + 2];
                    pixels[di + 1] = dib[s + 1];
                    pixels[di + 2] = dib[s];
                    pixels[di + 3] = dib[s + 3];
                }
                else
                {
                    var s = xorRow + x * 3; // BGR
                    pixels[di] = dib[s + 2];
                    pixels[di + 1] = dib[s + 1];
                    pixels[di + 2] = dib[s];
                    pixels[di + 3] = 255;
                }

                // Apply the AND mask: a set bit means the pixel is transparent.
                if (hasMask)
                {
                    var bit = dib[maskRow + (x >> 3)] >> (7 - (x & 7)) & 1;
                    if (bit == 1)
                    {
                        pixels[di + 3] = 0;
                    }
                }
            }
        }

        return new Gdk.Pixbuf(new GLib.Bytes(pixels), Gdk.Colorspace.Rgb, true, 8, width, height, width * 4);
    }
}
