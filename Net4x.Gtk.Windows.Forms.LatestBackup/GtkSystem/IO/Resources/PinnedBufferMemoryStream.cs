using System.Runtime.InteropServices;

namespace GtkSystem.IO.Resources;

internal sealed class PinnedBufferMemoryStream : UnmanagedMemoryStream
{
    private readonly byte[] _array;

    private GCHandle _pinningHandle;

    internal unsafe PinnedBufferMemoryStream(byte[] array)
    {
        _array = array;
        _pinningHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
        var num = array.Length;
        fixed (byte* pointer = &MemoryMarshal.GetReference<byte>(array))
        {
            Initialize(pointer, num, num, FileAccess.Read);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_pinningHandle.IsAllocated)
        {
            _pinningHandle.Free();
        }
        base.Dispose(disposing);
    }
}