// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies constants that define the state of the link.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum LinkState
    {
        /// <summary>
        /// The state of a link in its normal state (none of the other states apply).
        /// </summary>
        Normal = 0x00,
        /// <summary>
        /// The state of a link over which a mouse pointer is resting.
        /// </summary>
        Hover = 0x01,
        /// <summary>
        /// The state of a link that has been clicked.
        /// </summary>
        Active = 0x02,
        /// <summary>
        /// The state of a link that has been visited.
        /// </summary>
        Visited = 0x04
    }
}
