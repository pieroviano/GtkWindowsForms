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