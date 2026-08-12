using System.Runtime.InteropServices;

namespace System.Drawing;

/// <summary>
/// A small, dependency-free BMP reader/writer. The Gtk4 runtime frequently ships without the gdk-pixbuf
/// BMP loader/saver (GTK3 had it), so <c>new Gdk.Pixbuf(bmpBytes)</c> and <c>Pixbuf.Save(path, "bmp")</c>
/// both fail. This decodes/encodes the common uncompressed 24/32-bit BMPs directly.
/// </summary>
internal static class BmpImageCodec
{
    /// <summary>Decodes an uncompressed 24/32-bit BMP into a pixbuf, or returns null if it cannot.</summary>
    public static Gdk.Pixbuf? TryDecode(byte[]? bmp)
    {
        try
        {
            if (bmp == null || bmp.Length < 54 || bmp[0] != (byte)'B' || bmp[1] != (byte)'M')
            {
                return null;
            }

            var dataOffset = (int)BitConverter.ToUInt32(bmp, 10);
            var width = BitConverter.ToInt32(bmp, 18);
            var storedHeight = BitConverter.ToInt32(bmp, 22);
            int bpp = BitConverter.ToUInt16(bmp, 28);
            var compression = (int)BitConverter.ToUInt32(bmp, 30);

            var topDown = storedHeight < 0;
            var height = Math.Abs(storedHeight);
            if (width <= 0 || height <= 0 || (bpp != 24 && bpp != 32) || (compression != 0 && compression != 3))
            {
                return null;
            }

            var srcStride = (width * bpp + 31) / 32 * 4; // rows padded to 4 bytes
            var bytesPerPixel = bpp / 8;
            var pixels = new byte[width * height * 4];
            for (var y = 0; y < height; y++)
            {
                var srcY = topDown ? y : height - 1 - y;
                var row = dataOffset + srcY * srcStride;
                if (row + width * bytesPerPixel > bmp.Length)
                {
                    return null;
                }

                for (var x = 0; x < width; x++)
                {
                    var di = (y * width + x) * 4;
                    var s = row + x * bytesPerPixel; // BGR(A)
                    pixels[di] = bmp[s + 2];
                    pixels[di + 1] = bmp[s + 1];
                    pixels[di + 2] = bmp[s];
                    pixels[di + 3] = bpp == 32 ? bmp[s + 3] : (byte)255;
                }
            }

            var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
            var stride = pixbuf.Rowstride;
            var dest = pixbuf.Pixels;
            for (var y = 0; y < height; y++)
            {
                Marshal.Copy(pixels, y * width * 4, dest + y * stride, width * 4);
            }

            return pixbuf;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Encodes a pixbuf as an uncompressed 32-bit (BGRA, bottom-up) BMP.</summary>
    public static byte[] Encode(Gdk.Pixbuf pixbuf)
    {
        var width = pixbuf.Width;
        var height = pixbuf.Height;
        var channels = pixbuf.NChannels; // 3 (RGB) or 4 (RGBA)
        var srcStride = pixbuf.Rowstride;

        var src = new byte[srcStride * height];
        Marshal.Copy(pixbuf.Pixels, src, 0, src.Length);

        const int headerSize = 54; // 14-byte file header + 40-byte info header
        var outStride = width * 4;
        var imageSize = outStride * height;
        var bmp = new byte[headerSize + imageSize];

        // BITMAPFILEHEADER
        bmp[0] = (byte)'B';
        bmp[1] = (byte)'M';
        WriteU32(bmp, 2, (uint)bmp.Length);
        WriteU32(bmp, 10, headerSize);

        // BITMAPINFOHEADER (32-bit, bottom-up)
        WriteU32(bmp, 14, 40);
        WriteI32(bmp, 18, width);
        WriteI32(bmp, 22, height);
        WriteU16(bmp, 26, 1);
        WriteU16(bmp, 28, 32);
        WriteU32(bmp, 34, (uint)imageSize);

        for (var y = 0; y < height; y++)
        {
            var srcRow = y * srcStride;
            var dstRow = headerSize + (height - 1 - y) * outStride; // bottom-up
            for (var x = 0; x < width; x++)
            {
                var s = srcRow + x * channels; // RGB(A)
                var d = dstRow + x * 4;
                bmp[d] = src[s + 2];     // B
                bmp[d + 1] = src[s + 1]; // G
                bmp[d + 2] = src[s];     // R
                bmp[d + 3] = channels == 4 ? src[s + 3] : (byte)255;
            }
        }

        return bmp;
    }

    private static void WriteU16(byte[] buffer, int offset, ushort value)
    {
        buffer[offset] = (byte)(value & 0xFF);
        buffer[offset + 1] = (byte)(value >> 8 & 0xFF);
    }

    private static void WriteU32(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)(value & 0xFF);
        buffer[offset + 1] = (byte)(value >> 8 & 0xFF);
        buffer[offset + 2] = (byte)(value >> 16 & 0xFF);
        buffer[offset + 3] = (byte)(value >> 24 & 0xFF);
    }

    private static void WriteI32(byte[] buffer, int offset, int value) => WriteU32(buffer, offset, (uint)value);
}
