// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the reason that a form was closed.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum CloseReason
    {
        /// <summary>
        /// No reason for closure of the Form.
        /// </summary>
        None = 0,

        /// <summary>
        /// In the process of shutting down, Windows has closed the application.
        /// </summary>
        WindowsShutDown = 1,

        /// <summary>
        /// The parent form of this MDI form is closing.
        /// </summary>
        MdiFormClosing = 2,

        /// <summary>
        /// The user is closing the form through the user interface (UI), for example by clicking the Close button on the form window, selecting Close from the window's control menu, or pressing ALT+F4.
        /// </summary>
        UserClosing = 3,

        /// <summary>
        /// The Microsoft Windows Task Manager is closing the application.
        /// </summary>
        TaskManagerClosing = 4,

        /// <summary>
        /// A form is closing because its owner is closing.
        /// </summary>
        FormOwnerClosing = 5,

        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.Application.Exit" /> method of the
        /// <see cref="System.Windows.Forms.Application" /> class was invoked.
        /// </summary>
        ApplicationExitCall = 6
    }
}
