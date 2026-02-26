// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// For a given container control, determines whether the data in child controls
    /// will automatically be validated when the user attempts to change the focus.
    /// </summary>
    public enum AutoValidate
    {
        /// <summary>
        /// Implicit validation will not occur. Setting this value will not interfere with explicit calls to
        /// <see cref="System.Windows.Forms.ContainerControl.Validate" /> or
        /// <see cref="System.Windows.Forms.ContainerControl.ValidateChildren" />
        /// </summary>
        Disable = 0,

        /// <summary>
        /// Controls in this container will be validated when the focus changes.
        /// If a validation error occurs, the focus is forced to stay in the
        /// current control.
        /// </summary>
        EnablePreventFocusChange = 1,

        /// <summary>
        /// Implicit validation occurs, but if validation fails, focus will still change to the new control. If validation fails, the
        /// <see cref="System.Windows.Forms.Control.Validated" /> event will not fire.
        /// </summary>
        EnableAllowFocusChange = 2,

        /// <summary>
        /// The control inherits its
        /// <see cref="System.Windows.Forms.AutoValidate" /> behavior from its container (such as a form or another control). If there is no container control, it defaults to
        /// <see cref="System.Windows.Forms.AutoValidate.EnablePreventFocusChange" />
        /// </summary>
        Inherit = -1,
    }
}
