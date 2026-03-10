// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// IWin32Window is the interface implemented by objects that can return a handle to a Win32 window. This is used by various methods in Windows Forms that require a window handle, such as ShowDialog and IWin32Window.Handle.
    /// </summary>
    [Guid("458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IWin32Window
    {
        /// <summary>
        /// Gets the handle to the window represented by the implementer.
        /// </summary>
        /// <returns>
        /// A handle to the window represented by the implementer.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        IntPtr Handle { get; }

    }
}
