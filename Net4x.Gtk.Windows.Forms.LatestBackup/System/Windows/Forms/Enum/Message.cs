// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Implements a Windows message.
/// </summary>
public struct Message
{
#if DEBUG
    private static readonly TraceSwitch s_allWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

    public IntPtr HWnd
    {
        get;
        set;
    }

    /// <summary>
    /// Specifies the
    /// <see cref="!:Message.lparam" /> of the message.
    /// </summary>
    /// <value>
    /// The l parameter.
    /// </value>
    public IntPtr LParam
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the MSG.
    /// </summary>
    /// <value>
    /// The MSG.
    /// </value>
    public int Msg
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    /// <value>
    /// The result.
    /// </value>
    public IntPtr Result
    {
        get;
        set;
    }

    /// <summary>
    /// Specifies the
    /// <see cref="!:Message.wparam" /> of the message.
    /// </summary>
    /// <value>
    /// The w parameter.
    /// </value>
    public IntPtr WParam
    {
        get;
        set;
    }

    /// <summary>
    /// Creates a new
    /// <see cref="T:System.Windows.Forms.Message" /> object.
    /// </summary>
    /// <param name="hWnd">
    /// The h WND.
    /// </param>
    /// <param name="msg">
    /// The MSG.
    /// </param>
    /// <param name="wparam">
    /// The wparam.
    /// </param>
    /// <param name="lparam">
    /// The lparam.
    /// </param>
    /// <returns>
    /// Message.
    /// </returns>
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
    /// Determines whether the specified
    /// <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="o">
    /// The
    /// <see cref="T:System.Object" /> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the specified
    /// <see cref="T:System.Object" /> is equal to this instance; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
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

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

    /// <summary>
    /// Gets the
    /// <see cref="!:Message.lparam" /> value, and converts the value to an object.
    /// </summary>
    /// <param name="cls">
    /// The CLS.
    /// </param>
    /// <returns>
    /// System.Object.
    /// </returns>
    public object GetLParam(Type cls) => Marshal.PtrToStructure(LParam, cls);

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    //public override string ToString() => MessageDecoder.ToString(this);
    public override string ToString() => this.ToString();

}