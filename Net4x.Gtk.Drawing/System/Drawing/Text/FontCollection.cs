namespace System.Drawing.Text
{

    /// <summary>
    /// Provides a base class for installed and private font collections.
    /// </summary>
    public abstract class FontCollection : IDisposable
    {

        /// <summary>
        /// Gets the array of
        /// <see cref="T:System.Drawing.FontFamily" /> objects associated with this
        /// <see cref="T:System.Drawing.Text.FontCollection" />
        /// </summary>
        /// <returns>
        /// An array of
        /// <see cref="T:System.Drawing.FontFamily" /> objects.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public FontFamily[] Families { get; }

        /// <summary>
        /// Releases all resources used by this System.Drawing.Text.FontCollection.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Releases the unmanaged resources used by the System.Drawing.Text.FontCollection
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) { }

    }
}
