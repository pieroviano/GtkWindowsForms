// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.WebBrowser.ProgressChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class WebBrowserProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.WebBrowserProgressChangedEventArgs" /> class.
        /// </summary>
        /// <param name="currentProgress">The number of bytes that are loaded already.
        /// </param>
        /// <param name="maximumProgress">The total number of bytes to be loaded.
        /// </param>
        public WebBrowserProgressChangedEventArgs(long currentProgress, long maximumProgress)
        {
            CurrentProgress = currentProgress;
            MaximumProgress = maximumProgress;
        }

        /// <summary>
        /// Gets the number of bytes that have been downloaded.
        /// </summary>
        /// <returns>The number of bytes that have been loaded or -1 to indicate that the download has completed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public long CurrentProgress { get; }

        /// <summary>
        /// Specifies total number of bytes of item being downloaded.
        /// CurrentProgress/MaximumProgress*100 = progress percentage.
        /// </summary>
        public long MaximumProgress { get; }
    }
}
