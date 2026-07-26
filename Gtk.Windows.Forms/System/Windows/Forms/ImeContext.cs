// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides methods to manipulate the Input Method Editor (IME) context of a window. On the GTK
/// layer IME state is managed by the toolkit, so these are safe no-ops / default queries.</summary>
public static class ImeContext
{
    /// <summary>Disables the IME for the specified window handle.</summary>
    /// <param name="handle">The window handle.</param>
    public static void Disable(IntPtr handle) { }

    /// <summary>Enables the IME for the specified window handle.</summary>
    /// <param name="handle">The window handle.</param>
    public static void Enable(IntPtr handle) { }

    /// <summary>Gets the current IME mode of the specified window handle.</summary>
    /// <param name="handle">The window handle.</param>
    /// <returns>The current <see cref="T:System.Windows.Forms.ImeMode" />.</returns>
    public static ImeMode GetImeMode(IntPtr handle) => ImeMode.NoControl;

    /// <summary>Gets a value indicating whether the IME is open for the specified window handle.</summary>
    /// <param name="handle">The window handle.</param>
    /// <returns><see langword="false" />.</returns>
    public static bool IsOpen(IntPtr handle) => false;

    /// <summary>Sets the IME status for the specified window handle.</summary>
    /// <param name="imeMode">The IME mode to set.</param>
    /// <param name="handle">The window handle.</param>
    public static void SetImeStatus(ImeMode imeMode, IntPtr handle) { }

    /// <summary>Sets the open status of the IME for the specified window handle.</summary>
    /// <param name="open">Whether the IME should be open.</param>
    /// <param name="handle">The window handle.</param>
    public static void SetOpenStatus(bool open, IntPtr handle) { }
}
