namespace System.Drawing
{

    /// <summary>
    /// Defines methods for obtaining and releasing an existing handle to a Windows device
    /// context.
    /// </summary>
    public interface IDeviceContext : IDisposable
    {

        /// <summary>
        /// Returns the handle to a Windows device context.
        /// Return result:
        /// An System.IntPtr representing the handle of a device context.
        /// </summary>
        IntPtr GetHdc();

        /// <summary>
        /// Releases the handle of a Windows device context.
        /// </summary>
        void ReleaseHdc();
    }
}