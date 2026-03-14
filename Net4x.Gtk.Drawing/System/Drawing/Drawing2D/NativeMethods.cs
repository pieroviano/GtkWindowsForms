using System.Runtime.InteropServices;

namespace System.Drawing;

internal static class NativeMethods
{

    public const int MAX_PATH = 260;

    internal const int BI_BITFIELDS = 3;

    internal const int BI_RGB = 0;

    internal const int DIB_RGB_COLORS = 0;

    internal const int SM_REMOTESESSION = 4096;

    internal static HandleRef NullHandleRef => new HandleRef(null, IntPtr.Zero);

    internal struct BITMAPINFOHEADER
    {

        public short biBitCount;

        public int biClrImportant;

        public int biClrUsed;

        public int biCompression;

        public int biHeight;

        public short biPlanes;

        public int biSize;

        public int biSizeImage;

        public int biWidth;

        public int biXPelsPerMeter;

        public int biYPelsPerMeter;

    }

    internal struct PALETTEENTRY
    {

        public byte peBlue;

        public byte peFlags;

        public byte peGreen;

        public byte peRed;

    }

    internal struct RGBQUAD
    {
        public byte rgbBlue;

        public byte rgbGreen;

        public byte rgbRed;

        public byte rgbReserved;

    }
}