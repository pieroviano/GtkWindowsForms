using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
internal sealed class MetafileHeaderEmf
{

    public float dpiX;

    public float dpiY;

    public SafeNativeMethods.ENHMETAHEADER EmfHeader;

    public EmfPlusFlags emfPlusFlags;

    public int EmfPlusHeaderSize;

    public int Height;

    public int LogicalDpiX;

    public int LogicalDpiY;

    public int size;

    public MetafileType type;

    public int version;

    public int Width;

    public int X;

    public int Y;

    internal ref byte GetPinnableReference()
    {
        return ref Unsafe.As<MetafileType, byte>(ref type);
    }
}

internal static class SafeNativeMethods
{
    public struct ENHMETAHEADER
    {

        public int bOpenGL;

        public int cbPixelFormat;

        public int dSignature;

        public int iType;

        public int nBytes;

        public int nDescription;

        public short nHandles;

        public int nPalEntries;

        public int nRecords;

        public int nSize;

        public int nVersion;

        public int offDescription;

        public int offPixelFormat;

        public int rclBounds_bottom;

        public int rclBounds_left;

        public int rclBounds_right;

        public int rclBounds_top;

        public int rclFrame_bottom;

        public int rclFrame_left;

        public int rclFrame_right;

        public int rclFrame_top;

        public short sReserved;

        public int szlDevice_cx;

        public int szlDevice_cy;

        public int szlMillimeters_cx;

        public int szlMillimeters_cy;

    }
}