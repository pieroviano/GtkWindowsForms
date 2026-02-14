namespace System.Drawing
{
    //
    // Summary:
    //     Defines methods for obtaining and releasing an existing handle to a Windows device
    //     context.
    public interface IDeviceContext : IDisposable
    {
        //
        // Summary:
        //     Returns the handle to a Windows device context.
        //
        // Return result:
        //     An System.IntPtr representing the handle of a device context.
        IntPtr GetHdc();
        //
        // Summary:
        //     Releases the handle of a Windows device context.
        void ReleaseHdc();
    }
}
