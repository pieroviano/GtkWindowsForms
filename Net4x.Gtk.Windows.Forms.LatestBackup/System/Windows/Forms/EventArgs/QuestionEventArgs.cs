// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for events that need a true or false answer to a question.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class QuestionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.QuestionEventArgs" /> class using a default
        /// <see cref="System.Windows.Forms.QuestionEventArgs.Response" /> property value of false.
        /// </summary>
        public QuestionEventArgs()
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.QuestionEventArgs" /> class using the specified default value for the
        /// <see cref="System.Windows.Forms.QuestionEventArgs.Response" /> property.
        /// </summary>
        /// <param name="response">The default value of the
        /// <see cref="System.Windows.Forms.QuestionEventArgs.Response" /> property.
        /// </param>
        public QuestionEventArgs(bool response)
        {
            Response = response;
        }
        /// <summary>
        /// Gets or sets a value indicating the response to a question represented by the event.
        /// </summary>
        /// <returns>true for an affirmative response; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Response { get; set; }
    }
}
