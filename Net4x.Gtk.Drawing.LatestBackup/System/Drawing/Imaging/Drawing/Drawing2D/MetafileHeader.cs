using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    /// <summary>
    /// Contains attributes of an associated
    /// <see cref="T:System.Drawing.Imaging.Metafile" />. Not inheritable.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MetafileHeader
    {

        internal MetafileHeaderEmf emf;

        internal MetafileHeaderWmf wmf;

        internal MetafileHeader()
        {
        }

        /// <summary>
        /// Gets a
        /// <see cref="T:System.Drawing.Rectangle" /> that bounds the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.Drawing.Rectangle" /> that bounds the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public Rectangle Bounds
        {
            get
            {
                if (!IsWmf())
                {
                    return new Rectangle(emf.X, emf.Y, emf.Width, emf.Height);
                }

                return new Rectangle(wmf.X, wmf.Y, wmf.Width, wmf.Height);
            }
        }

        /// <summary>
        /// Gets the horizontal resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The horizontal resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public float DpiX
        {
            get
            {
                if (!IsWmf())
                {
                    return emf.dpiX;
                }

                return wmf.dpiX;
            }
        }

        /// <summary>
        /// Gets the vertical resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The vertical resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public float DpiY
        {
            get
            {
                if (!IsWmf())
                {
                    return emf.dpiY;
                }

                return wmf.dpiY;
            }
        }

        /// <summary>
        /// Gets the size, in bytes, of the enhanced metafile plus header file.
        /// </summary>
        /// <returns>
        /// The size, in bytes, of the enhanced metafile plus header file.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int EmfPlusHeaderSize
        {
            get
            {

                return wmf.EmfPlusHeaderSize;
            }
        }

        /// <summary>
        /// Gets the logical horizontal resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The logical horizontal resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int LogicalDpiX
        {
            get
            {

                return wmf.LogicalDpiX;
            }
        }

        /// <summary>
        /// Gets the logical vertical resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The logical vertical resolution, in dots per inch, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int LogicalDpiY
        {
            get
            {

                return wmf.LogicalDpiY;
            }
        }

        /// <summary>
        /// Gets the size, in bytes, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The size, in bytes, of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int MetafileSize
        {
            get
            {
                if (!IsWmf())
                {
                    return emf.size;
                }

                return wmf.size;
            }
        }

        /// <summary>
        /// Gets the type of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.Drawing.Imaging.MetafileType" /> enumeration that represents the type of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public MetafileType Type
        {
            get
            {
                if (!IsWmf())
                {
                    return emf.type;
                }

                return wmf.type;
            }
        }

        /// <summary>
        /// Gets the version number of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// The version number of the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int Version
        {
            get
            {
                if (!IsWmf())
                {
                    return emf.version;
                }

                return wmf.version;
            }
        }

        /// <summary>
        /// Gets the Windows metafile (WMF) header file for the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.Drawing.Imaging.MetaHeader" /> that contains the WMF header file for the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public MetaHeader WmfHeader
        {
            get
            {

                return wmf.WmfHeader;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is device dependent.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is device dependent; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsDisplay()
        {
            if (IsEmfPlus())
            {
                return (emf.emfPlusFlags & EmfPlusFlags.Display) != 0;
            }

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmf()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format or the Windows enhanced metafile plus format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile format or the Windows enhanced metafile plus format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmfOrEmfPlus()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile plus format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows enhanced metafile plus format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmfPlus()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Dual enhanced metafile format. This format supports both the enhanced and the enhanced plus format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Dual enhanced metafile format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmfPlusDual()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" />
        /// supports only the Windows enhanced metafile plus format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> supports only the Windows enhanced metafile plus format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsEmfPlusOnly()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows metafile format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows metafile format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsWmf()
        {

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows placeable metafile format.
        /// </summary>
        /// <returns>
        /// true if the associated
        /// <see cref="T:System.Drawing.Imaging.Metafile" /> is in the Windows placeable metafile format; otherwise, false.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsWmfPlaceable()
        {

            return false;
        }

    }
}
