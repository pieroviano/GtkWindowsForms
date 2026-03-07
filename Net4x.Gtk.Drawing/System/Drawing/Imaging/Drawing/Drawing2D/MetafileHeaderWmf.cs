using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal sealed class MetafileHeaderWmf
    {

        public float dpiX;

        public float dpiY;

        public int dummy1;

        public int dummy10;

        public int dummy11;

        public int dummy12;

        public int dummy13;

        public int dummy14;

        public int dummy15;

        public int dummy16;

        public int dummy2;

        public int dummy3;

        public int dummy4;

        public int dummy5;

        public int dummy6;

        public int dummy7;

        public int dummy8;

        public int dummy9;

        public EmfPlusFlags emfPlusFlags;

        public int EmfPlusHeaderSize;

        public int Height;

        public int LogicalDpiX;

        public int LogicalDpiY;

        public int size = Marshal.SizeOf<MetafileHeaderWmf>();

        public MetafileType type;

        public int version;

        public int Width;

        [MarshalAs(UnmanagedType.Struct)]
        public MetaHeader WmfHeader = new MetaHeader();

        public int X;

        public int Y;
    }
}
