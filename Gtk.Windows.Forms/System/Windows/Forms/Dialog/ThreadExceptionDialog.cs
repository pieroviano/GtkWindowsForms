// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Implements a dialog box that is displayed when an unhandled exception occurs on the UI thread.
/// </summary>
public class ThreadExceptionDialog : Form
{
    private readonly Exception _exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.ThreadExceptionDialog" /> class for
    /// the specified exception.
    /// </summary>
    /// <param name="t">The <see cref="T:System.Exception" /> to display.</param>
    public ThreadExceptionDialog(Exception t)
    {
        _exception = t;
        Text = "Unhandled exception";

        var message = new Label
        {
            AutoSize = true,
            Text = BuildMessage(t),
        };
        Controls.Add(message);
    }

    private static string BuildMessage(Exception t)
    {
        if (t == null)
        {
            return string.Empty;
        }

        return "Unhandled exception has occurred in your application. " +
               t.GetType().FullName + Environment.NewLine + Environment.NewLine + t.Message;
    }

    /// <summary>Returns a string that represents the dialog and its exception.</summary>
    /// <returns>A string.</returns>
    public override string ToString() => base.ToString() + ", Exception: " + _exception?.GetType().FullName;
}
