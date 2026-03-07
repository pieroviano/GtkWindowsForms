// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements a Windows message.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public struct Message
    {
        /// <summary>
        /// Gets or sets the window handle of the message.
        /// </summary>
        /// <returns>The window handle of the message.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
#if DEBUG
        private static readonly TraceSwitch s_allWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

        public IntPtr HWnd { get; set; }

        /// <summary>
        /// Specifies the
        /// <see cref="System.Windows.Forms.Message.LParam" /> field of the message.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Message.LParam" /> field of the message.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public IntPtr LParam { get; set; }

        /// <summary>
        /// Gets or sets the ID number for the message.
        /// </summary>
        /// <returns>The ID number for the message.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public int Msg { get; set; }

        /// <summary>
        /// Specifies the value that is returned to Windows in response to handling the message.
        /// </summary>
        /// <returns>The return value of the message.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public IntPtr Result { get; set; }

        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.Message.WParam" /> field of the message.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Message.WParam" /> field of the message.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public IntPtr WParam { get; set; }

        /// <summary>
        /// Creates a new
        /// <see cref="System.Windows.Forms.Message" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Message" /> that represents the message that was created.
        /// </returns>
        /// <param name="hWnd">The window handle that the message is for.
        /// </param>
        /// <param name="msg">The message ID.
        /// </param>
        /// <param name="wparam">The message
        /// <paramref name="wparam" /> field.
        /// </param>
        /// <param name="lparam">The message
        /// <paramref name="lparam" /> field.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var m = new Message
            {
                HWnd = hWnd,
                Msg = msg,
                WParam = wparam,
                LParam = lparam,
                Result = IntPtr.Zero
            };

#if DEBUG
            if (s_allWinMessages.TraceVerbose)
            {
                Debug.WriteLine(m.ToString());
            }
#endif
            return m;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="o">The object to compare with the current object.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public override bool Equals(object o)
        {
            if (!(o is Message m))
            {
                return false;
            }

            return HWnd == m.HWnd &&
                   Msg == m.Msg &&
                   WParam == m.WParam &&
                   LParam == m.LParam &&
                   Result == m.Result;
        }

        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.Message.LParam" /> value and converts the value to an object.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that represents an instance of the class specified by the
        /// <paramref name="cls" /> parameter, with the data from the
        /// <see cref="System.Windows.Forms.Message.LParam" /> field of the message.
        /// </returns>
        /// <param name="cls">The type to use to create an instance. This type must be declared as a structure type.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public object GetLParam(Type cls) => Marshal.PtrToStructure(LParam, cls);

        /// <summary>
        /// Returns a
        /// <see cref="System.String" /> that represents the current
        /// <see cref="System.Windows.Forms.Message" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that represents the current
        /// <see cref="System.Windows.Forms.Message" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        //public override string ToString() => MessageDecoder.ToString(this);
        public override string ToString() => this.ToString();

    }
}
