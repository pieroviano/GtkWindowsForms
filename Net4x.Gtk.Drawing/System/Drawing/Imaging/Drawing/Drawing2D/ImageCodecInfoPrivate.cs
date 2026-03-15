using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ImageCodecInfoPrivate
{
    public Guid Clsid;

    public IntPtr CodecName;

    public IntPtr DllName;

    public IntPtr FilenameExtension;

    public int Flags;

    public IntPtr FormatDescription;

    public Guid FormatID;

    public IntPtr MimeType;

    public int SigCount;

    public IntPtr SigMask;

    public IntPtr SigPattern;

    public int SigSize;

    public int Version;

}