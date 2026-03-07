// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for controls that derive from the
    /// <see cref="!:System.Windows.Forms.UpDownBase" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class UpDownEventArgs : EventArgs
    {
        public UpDownEventArgs(int buttonPushed)
        {
            ButtonID = buttonPushed;
        }

        public int ButtonID { get; }
    }
}
