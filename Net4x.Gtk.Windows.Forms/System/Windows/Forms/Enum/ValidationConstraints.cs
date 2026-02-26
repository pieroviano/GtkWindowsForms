// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Defines constants that inform
    /// <see cref="!:System.Windows.Forms.ContainerControl.ValidateChildren(System.Windows.Forms.ValidationConstraints)" />
    /// about how it should validate a container's child controls.
    /// </summary>
    [Flags]
    public enum ValidationConstraints
    {
        /// <summary>
        /// Validates all child controls, and all children of these child controls, regardless of their property settings.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Child control must be selectable to be validated.
        ///
        /// Note: This flag allows validation of a control that has the
        /// ControlStyles.Selectable style, ie. has the ability to be selected,
        /// even when that control is currently unselectable because it has been
        /// disabled or hidden. To prevent validation of selectable controls that
        /// are currently disabled or hidden,
        /// </summary>
        Selectable = 0x01,

        /// <summary>
        /// Validates child controls whose
        /// <see cref="System.Windows.Forms.Control.Enabled" /> property is set to true.
        /// </summary>
        Enabled = 0x02,

        /// <summary>
        /// Validates child controls whose
        /// <see cref="System.Windows.Forms.Control.Visible" /> property is set to true.
        /// </summary>
        Visible = 0x04,

        /// <summary>
        /// Validates child controls that have a
        /// <see cref="System.Windows.Forms.Control.TabStop" /> value set, which means that the user can navigate to the control using the TAB key.
        /// </summary>
        TabStop = 0x08,

        /// <summary>
        /// Validates child controls that are directly hosted within the container. Does not validate any of the children of these children. For example, if you have a
        /// <see cref="System.Windows.Forms.Form" /> that contains a custom
        /// <see cref="System.Windows.Forms.UserControl" />, and the
        /// <see cref="System.Windows.Forms.UserControl" /> contains a
        /// <see cref="System.Windows.Forms.Button" />, using
        /// <see cref="System.Windows.Forms.ValidationConstraints.ImmediateChildren" /> will cause the
        /// <see cref="System.Windows.Forms.Control.Validating" /> event of the
        /// <see cref="System.Windows.Forms.UserControl" /> to occur, but not the
        /// <see cref="System.Windows.Forms.Control.Validating" /> event of the
        /// <see cref="System.Windows.Forms.Button" />
        /// </summary>
        ImmediateChildren = 0x10,
    }
}
