// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.WebBrowser.Navigating" /> event.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class WebBrowserNavigatingEventArgs : CancelEventArgs
{
    private readonly string _targetFrameName;

    private readonly Uri _url;

    /// <summary>
    /// Creates an instance of the
    /// <see cref="T:System.Windows.Forms.WebBrowserNavigatingEventArgs" /> class.
    /// </summary>
    public WebBrowserNavigatingEventArgs(Uri url, string targetFrameName)
    {
        _url = url;
        _targetFrameName = targetFrameName;
    }

    /// <summary>
    /// In case an individual frame is about to be navigated, this contains the frame name.
    /// </summary>
    public string TargetFrameName
    {
        get
        {
            return _targetFrameName;
        }
    }

    /// <summary>
    /// Url the browser is navigating to.
    /// </summary>
    public Uri Url
    {
        get
        {
            return _url;
        }
    }
}