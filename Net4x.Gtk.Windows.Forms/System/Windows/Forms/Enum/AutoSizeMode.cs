// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how a control will behave when its
    /// <see cref="System.Windows.Forms.Control.AutoSize" /> property is enabled.
    /// </summary>
    public enum AutoSizeMode
    {
        /// <summary>
        /// The same behavior as you get for controls with AutoSize and no AutoSizeMode
        /// property. The control will grow or shrink to encompass the contents (e.g.
        /// text for a Button, child controls for a container). The MinimumSize and
        /// MaximumSize are followed, but the current value of the Size property is
        /// ignored.
        /// </summary>
        GrowAndShrink,

        /// <summary>
        /// The control grows as much as necessary to fit its contents but does not shrink smaller than the value of its
        /// <see cref="System.Windows.Forms.Control.Size" />   property. The form can be resized, but cannot be made so small that any of its contained controls are hidden.
        /// </summary>
        GrowOnly
    }
}
