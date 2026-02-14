namespace System.Drawing.Text
{
    //
    // Summary:
    //     Provides a base class for installed and private font collections.
    public abstract class FontCollection : IDisposable
    {
        //
        // Summary:
        //     Gets the array of System.Drawing.FontFamily objects associated with this System.Drawing.Text.FontCollection.
        //
        // Return result:
        //     An array of System.Drawing.FontFamily objects.
        public FontFamily[] Families { get; }

        //
        // Summary:
        //     Releases all resources used by this System.Drawing.Text.FontCollection.
        public void Dispose() { }
        //
        // Summary:
        //     Releases the unmanaged resources used by the System.Drawing.Text.FontCollection
        //     and optionally releases the managed resources.
        //
        // Parameter:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only unmanaged
        //     resources.
        protected virtual void Dispose(bool disposing) { }
    }
}
