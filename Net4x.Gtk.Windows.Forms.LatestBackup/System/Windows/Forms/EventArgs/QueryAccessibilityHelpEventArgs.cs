// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// The QueryAccessibilityHelpEventArgs is fired when AccessibleObject is providing help
/// to accessibility applications.
/// </summary>
[ComVisible(true)]
public class QueryAccessibilityHelpEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.QueryAccessibilityHelpEventArgs" /> class.
    /// </summary>
    public QueryAccessibilityHelpEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.QueryAccessibilityHelpEventArgs" /> class.
    /// </summary>
    /// <param name="helpNamespace">
    /// The help namespace.
    /// </param>
    /// <param name="helpString">
    /// The help string.
    /// </param>
    /// <param name="helpKeyword">
    /// The help keyword.
    /// </param>
    public QueryAccessibilityHelpEventArgs(string helpNamespace, string helpString, string helpKeyword)
    {
        HelpNamespace = helpNamespace;
        HelpString = helpString;
        HelpKeyword = helpKeyword;
    }

    /// <summary>
    /// Gets or sets the help keyword.
    /// </summary>
    /// <value>
    /// The help keyword.
    /// </value>
    public string HelpKeyword
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the help namespace.
    /// </summary>
    /// <value>
    /// The help namespace.
    /// </value>
    public string HelpNamespace
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the help string.
    /// </summary>
    /// <value>
    /// The help string.
    /// </value>
    public string HelpString
    {
        get;
        set;
    }

}