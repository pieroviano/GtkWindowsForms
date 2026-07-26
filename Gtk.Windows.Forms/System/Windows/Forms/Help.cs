// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Encapsulates the HTML Help 1.0 engine. On the GTK layer displaying Help is a documented no-op
/// boundary; the methods exist so calling source compiles.</summary>
public static class Help
{
    /// <summary>Displays the contents of a Help file.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="url">The Help file path or URL.</param>
    public static void ShowHelp(Control parent, string url) { }

    /// <summary>Displays the contents of a Help file at the specified navigator location.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="url">The Help file path or URL.</param>
    /// <param name="navigator">The navigator command.</param>
    public static void ShowHelp(Control parent, string url, HelpNavigator navigator) { }

    /// <summary>Displays the contents of a Help file for a keyword.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="url">The Help file path or URL.</param>
    /// <param name="keyword">The keyword to look up.</param>
    public static void ShowHelp(Control parent, string url, string keyword) { }

    /// <summary>Displays the contents of a Help file at a navigator location with a parameter.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="url">The Help file path or URL.</param>
    /// <param name="command">The navigator command.</param>
    /// <param name="parameter">The topic parameter.</param>
    public static void ShowHelp(Control parent, string url, HelpNavigator command, object parameter) { }

    /// <summary>Displays the index of the specified Help file.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="url">The Help file path or URL.</param>
    public static void ShowHelpIndex(Control parent, string url) { }

    /// <summary>Displays a pop-up Help window.</summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="caption">The Help text.</param>
    /// <param name="location">The screen location.</param>
    public static void ShowPopup(Control parent, string caption, Point location) { }
}
