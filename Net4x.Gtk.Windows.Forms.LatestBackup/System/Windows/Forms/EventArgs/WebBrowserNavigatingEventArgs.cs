// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.WebBrowser.Navigating" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class WebBrowserNavigatingEventArgs : CancelEventArgs
    {

        private readonly Uri _url;
        private readonly string _targetFrameName;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs" /> class.
        /// </summary>
        /// <param name="url">A
        /// <see cref="System.Uri" /> representing the location of the document to which the
        /// <see cref="System.Windows.Forms.WebBrowser" /> control is navigating.
        /// </param>
        /// <param name="targetFrameName">The name of the Web page frame in which the new document will be loaded.
        /// </param>
        public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
        {
            _url = url;
            _targetFrameName = targetFrameName;
        }

        /// <summary>
        /// Gets the location of the document to which the
        /// <see cref="System.Windows.Forms.WebBrowser" /> control is navigating.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Uri" /> representing the location of the document to which the
        /// <see cref="System.Windows.Forms.WebBrowser" /> control is navigating.
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

        /// <summary>
        /// Gets the name of the Web page frame in which the new document will be loaded.
        /// </summary>
        /// <returns>The name of the frame in which the new document will be loaded.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string TargetFrameName
        {
            get
            {
                return _targetFrameName;
            }
        }
    }
}
