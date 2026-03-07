// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.DataGrid.Navigate" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class NavigateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.NavigateEventArgs" /> class.
        /// </summary>
        /// <param name="isForward">true to navigate in a forward direction; otherwise, false.
        /// </param>
        public NavigateEventArgs(bool isForward)
        {
            Forward = isForward;
        }
        /// <summary>
        /// Gets a value indicating whether to navigate in a forward direction.
        /// </summary>
        /// <returns>true if the navigation is in a forward direction; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Forward { get; }
    }
}
