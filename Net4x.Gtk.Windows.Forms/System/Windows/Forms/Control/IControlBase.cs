// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Defines the minimal set of properties common to low-level control implementations.
    /// Implemented by control wrappers that expose core appearance and layout members.
    /// </summary>
    public interface IControlBase
    {
        /// <summary>
        /// Gets or sets the accessible default action description for accessibility clients.
        /// </summary>
        string AccessibleDefaultActionDescription { get; set; }

        /// <summary>
        /// Gets or sets the accessible description for the control.
        /// </summary>
        string AccessibleDescription { get; set; }

        /// <summary>
        /// Gets or sets the accessible name for the control.
        /// </summary>
        string AccessibleName { get; set; }

        /// <summary>
        /// Gets or sets the accessible role of the control.
        /// </summary>
        AccessibleRole AccessibleRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether drag-and-drop operations are allowed on the control.
        /// </summary>
        bool AllowDrop { get; set; }

        /// <summary>
        /// Gets or sets the anchoring of the control to its parent.
        /// </summary>
        AnchorStyles Anchor { get; set; }

        /// <summary>
        /// Gets or sets the auto-scroll offset for the control.
        /// </summary>
        Point AutoScrollOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control will automatically size to its contents.
        /// </summary>
        bool AutoSize { get; set; }

        /// <summary>
        /// Gets or sets the background color of the control.
        /// </summary>
        Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the background image for the control.
        /// </summary>
        Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the layout style for the background image.
        /// </summary>
        ImageLayout BackgroundImageLayout { get; set; }

        /// <summary>
        /// Gets or sets the binding context for data-binding operations.
        /// </summary>
        BindingContext BindingContext { get; set; }

        /// <summary>
        /// Gets the bottom position of the control relative to its container.
        /// </summary>
        int Bottom { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the control causes validation when it receives focus.
        /// </summary>
        bool CausesValidation { get; set; }

        /// <summary>
        /// Gets or sets how the control is docked within its parent.
        /// </summary>
        DockStyle Dock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets the location of the control relative to its parent.
        /// </summary>
        Point Location { get; set; }

        /// <summary>
        /// Gets or sets the margin for the control.
        /// </summary>
        Padding Margin { get; set; }

        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the padding within the control.
        /// </summary>
        Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the size of the control.
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Gets or sets the tab index of the control.
        /// </summary>
        int TabIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control is included in tab navigation.
        /// </summary>
        bool TabStop { get; set; }

        /// <summary>
        /// Gets or sets an object containing user-defined data about the control.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Gets or sets the text associated with the control.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control is visible.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        int Width { get; set; }

        //void SuspendLayout();
        //void ResumeLayout(bool resume);
        //void PerformLayout();
    }
}

