using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct WmfMetaHeader
    {

        internal short _headerSize;

        internal int _maxRecord;

        internal short _noObjects;

        internal short _noParameters;

        internal int _size;

        internal short _type;

        internal short _version;
    }
}
