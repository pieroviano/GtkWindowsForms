// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design;

/// <summary>Provides an interface for a <c>UITypeEditor</c> to display Windows Forms or drop-down controls.</summary>
public interface IWindowsFormsEditorService
{
    /// <summary>Closes any previously opened drop-down control area.</summary>
    void CloseDropDown();

    /// <summary>Displays the specified control in a drop-down area below a value field.</summary>
    /// <param name="control">The control to display.</param>
    void DropDownControl(Control control);

    /// <summary>Shows the specified dialog box.</summary>
    /// <param name="dialog">The form to display.</param>
    /// <returns>The dialog result.</returns>
    DialogResult ShowDialog(Form dialog);
}
