// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.MaskedTextBox.TypeValidationCompleted" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TypeValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TypeValidationEventArgs" /> class.
        /// </summary>
        /// <param name="validatingType">The
        /// <see cref="System.Type" /> that the formatted input string was being validated against.
        /// </param>
        /// <param name="isValidInput">A
        /// <see cref="System.Boolean" /> value indicating whether the formatted string was successfully converted to the validating type.
        /// </param>
        /// <param name="returnValue">An
        /// <see cref="System.Object" /> that is the result of the formatted string being converted to the target type.
        /// </param>
        /// <param name="message">A
        /// <see cref="System.String" /> containing a description of the conversion process.
        /// </param>
        public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
        {
            ValidatingType = validatingType;
            IsValidInput = isValidInput;
            ReturnValue = returnValue;
            Message = message;
        }

        /// <summary>
        /// Gets the type that the formatted input string is being validated against.
        /// </summary>
        /// <returns>The target
        /// <see cref="System.Type" /> of the conversion process. This should never be null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Type ValidatingType { get; }

        /// <summary>
        /// Gets a value indicating whether the formatted input string was successfully converted to the validating type.
        /// </summary>
        /// <returns>true if the formatted input string can be converted into the type specified by the
        /// <see cref="System.Windows.Forms.TypeValidationEventArgs.ValidatingType" /> property; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsValidInput { get; }

        /// <summary>
        /// Gets a text message describing the conversion process.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> containing a description of the conversion process.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string Message { get; }

        /// <summary>
        /// Gets the object that results from the conversion of the formatted input string.
        /// </summary>
        /// <returns>If the validation is successful, an
        /// <see cref="System.Object" /> that represents the converted type; otherwise, null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object ReturnValue { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <returns>true if the event should be canceled and focus retained by the
        /// <see cref="System.Windows.Forms.MaskedTextBox" /> control; otherwise, false to continue validation processing.
        /// </returns>
        public bool Cancel { get; set; }
    }
}
