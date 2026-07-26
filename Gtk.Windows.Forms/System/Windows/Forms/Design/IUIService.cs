// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>Enables interaction with the user interface of the development environment that is hosting a designer.</summary>
public interface IUIService
{
    /// <summary>Gets the collection of styles that control the appearance of dialogs.</summary>
    IDictionary Styles { get; }

    /// <summary>Determines whether a component editor can be shown for the specified component.</summary>
    /// <param name="component">The component to test.</param>
    /// <returns><see langword="true" /> if a component editor can be shown; otherwise <see langword="false" />.</returns>
    bool CanShowComponentEditor(object component);

    /// <summary>Gets the window that should be the owner of dialogs shown by the service.</summary>
    /// <returns>The dialog owner window.</returns>
    IWin32Window GetDialogOwnerWindow();

    /// <summary>Marks the UI as dirty.</summary>
    void SetUIDirty();

    /// <summary>Shows the component editor for a component.</summary>
    /// <param name="component">The component to edit.</param>
    /// <param name="parent">The parent window.</param>
    /// <returns><see langword="true" /> if changes were made; otherwise <see langword="false" />.</returns>
    bool ShowComponentEditor(object component, IWin32Window parent);

    /// <summary>Shows the specified form.</summary>
    /// <param name="form">The form to show.</param>
    /// <returns>The dialog result.</returns>
    DialogResult ShowDialog(Form form);

    /// <summary>Shows the specified error message.</summary>
    /// <param name="message">The message.</param>
    void ShowError(string message);

    /// <summary>Shows the specified exception.</summary>
    /// <param name="ex">The exception.</param>
    void ShowError(Exception ex);

    /// <summary>Shows the specified exception with a message.</summary>
    /// <param name="ex">The exception.</param>
    /// <param name="message">The message.</param>
    void ShowError(Exception ex, string message);

    /// <summary>Shows the specified message.</summary>
    /// <param name="message">The message.</param>
    void ShowMessage(string message);

    /// <summary>Shows the specified message with a caption.</summary>
    /// <param name="message">The message.</param>
    /// <param name="caption">The caption.</param>
    void ShowMessage(string message, string caption);

    /// <summary>Shows the specified message with a caption and buttons.</summary>
    /// <param name="message">The message.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>The dialog result.</returns>
    DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

    /// <summary>Displays the specified tool window.</summary>
    /// <param name="toolWindow">The GUID of the tool window.</param>
    /// <returns><see langword="true" /> if the tool window was shown; otherwise <see langword="false" />.</returns>
    bool ShowToolWindow(Guid toolWindow);
}
