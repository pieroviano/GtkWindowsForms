// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
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
        /// The file containing Help for the
        /// <see cref="T:System.Windows.Forms.AccessibleObject" />
        /// </param>
        /// <param name="helpString">
        /// The string defining what Help to get for the
        /// <see cref="T:System.Windows.Forms.AccessibleObject" />
        /// </param>
        /// <param name="helpKeyword">
        /// The keyword to associate with the Help request for the
        /// <see cref="T:System.Windows.Forms.AccessibleObject" />
        /// </param>
        public QueryAccessibilityHelpEventArgs(string helpNamespace, string helpString, string helpKeyword)
        {
            HelpNamespace = helpNamespace;
            HelpString = helpString;
            HelpKeyword = helpKeyword;
        }

        /// <summary>
        /// Gets or sets the Help keyword for the specified control.
        /// </summary>
        /// <returns>
        /// The Help topic associated with the
        /// <see cref="T:System.Windows.Forms.AccessibleObject" /> that was queried.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public string HelpKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the name of the Help file.
        /// </summary>
        /// <returns>
        /// The name of the Help file. This name can be in the form C:\path\sample.chm or /folder/file.htm.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public string HelpNamespace { get; set; }

        /// <summary>
        /// Gets or sets the string defining what Help to get for the
        /// <see cref="T:System.Windows.Forms.AccessibleObject" />
        /// </summary>
        /// <returns>
        /// The Help to retrieve for the accessible object.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public string HelpString { get; set; }

    }
}
