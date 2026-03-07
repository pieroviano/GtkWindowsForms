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
#if DEBUG
        private static readonly TraceSwitch s_allWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

        public IntPtr HWnd { get; set; }

        /// <summary>
        /// Specifies the
        /// <see cref="Message.lparam" /> of the message.
        /// </summary>
        public IntPtr LParam { get; set; }

        public int Msg { get; set; }

        public IntPtr Result { get; set; }

        /// <summary>
        /// Specifies the
        /// <see cref="Message.wparam" /> of the message.
        /// </summary>
        public IntPtr WParam { get; set; }

        /// <summary>
        /// Creates a new
        /// <see cref="Message" /> object.
        /// </summary>
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

        public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

        /// <summary>
        /// Gets the
        /// <see cref="Message.lparam" /> value, and converts the value to an object.
        /// </summary>
        public object GetLParam(Type cls) => Marshal.PtrToStructure(LParam, cls);

        //public override string ToString() => MessageDecoder.ToString(this);
        public override string ToString() => this.ToString();
    }
}
