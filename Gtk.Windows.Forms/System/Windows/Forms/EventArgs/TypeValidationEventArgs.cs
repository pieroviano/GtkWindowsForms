// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.MaskedTextBox.TypeValidationCompleted" /> event.
/// </summary>
public class TypeValidationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TypeValidationEventArgs" /> class.
    /// </summary>
    /// <param name="validatingType">
    /// Type of the validating.
    /// </param>
    /// <param name="isValidInput">
    /// if set to
    /// <c>
    /// true
    /// </c>is valid input.
    /// </param>
    /// <param name="returnValue">
    /// The return value.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
    {
        ValidatingType = validatingType;
        IsValidInput = isValidInput;
        ReturnValue = returnValue;
        Message = message;
    }

    /// <summary>
    /// Specifies whether focus should be allowed to be shifted from the control.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if cancel; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Cancel
    {
        get;
        set;
    }

    /// <summary>
    /// The exception thrown by the validating object while performing the data validation.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is valid input; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsValidInput { get; }

    /// <summary>
    /// A message about the validation operation. Intended to be populated with
    /// an exception information if any thrown.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; }

    /// <summary>
    /// The value returned from the Parse method.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public object ReturnValue { get; }

    /// <summary>
    /// The position where the test failed the mask constraint.
    /// </summary>
    /// <value>
    /// The type of the validating.
    /// </value>
    public Type ValidatingType { get; }
}