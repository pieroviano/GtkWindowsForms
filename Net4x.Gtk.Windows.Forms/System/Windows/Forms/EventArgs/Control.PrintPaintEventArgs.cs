// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Partial definition of the
/// <see cref="T:System.Windows.Forms.Control" /> class that contains the nested
/// <see cref="T:System.Windows.Forms.Control.ControlCollection" />.
/// The collection manages child controls/widgets for a parent
/// <see cref="T:System.Windows.Forms.Control" /> and bridges WinForms semantics
/// to the underlying GTK container.
/// </summary>
/// Represents a Windows Forms control that is backed by a GTK widget.
/// This class provides a compatibility layer so WinForms-style APIs can be used on GTK platforms.
/// </summary>
public partial class Control
{
    private sealed class PrintPaintEventArgs : PaintEventArgs
    {
        private Message _m;

        internal PrintPaintEventArgs(Message m, IntPtr dc, Rectangle clipRect)
            : base(dc, clipRect)
        {
            _m = m;
        }

        internal Message Message
        {
            get
            {
                return _m;
            }
        }

    }
}