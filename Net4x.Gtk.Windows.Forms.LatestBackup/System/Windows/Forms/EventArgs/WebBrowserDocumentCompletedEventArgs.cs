// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.WebBrowser.DocumentCompleted" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class WebBrowserDocumentCompletedEventArgs : EventArgs
    {
        private readonly Uri _url;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.WebBrowserDocumentCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="url">A
        /// <see cref="System.Uri" /> representing the location of the document that was loaded.
        /// </param>
        public WebBrowserDocumentCompletedEventArgs(Uri url)
        {
            _url = url;
        }

        /// <summary>
        /// Gets the location of the document to which the
        /// <see cref="System.Windows.Forms.WebBrowser" /> control has navigated.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Uri" /> representing the location of the document that was loaded.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Uri Url
        {
            get
            {
                return _url;
            }
        }
    }
}
